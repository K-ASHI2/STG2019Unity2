using UnityEngine;
using Assets.Script.Common;
using static Assets.Script.Player.PlayerBulletPools;
using System.Collections.Generic;

namespace Assets.Script.Player
{
    /// <summary>
    /// 弾源生成クラス
    /// </summary>
    internal class PlayerBulletGenerater
    {
        // メインショットのインターバル(5/60秒)
        private const float mainShotInterval = 0.08333334f;

        // 自機の弾のオブジェクトプール
        private PlayerBulletPools playerBulletPools;
        // プレイヤーのTransFormオブジェクト
        private readonly Transform playerTransform;
        // 自機の弾の設定
        PlayerBulletSetting playerBulletSetting;
        // 自機のレーザーの設定
        PlayerLaserSetting playerLaserSetting;
        // 自機のパワー
        // TODO 参照渡しにして、プレイヤーのパワーの変化に合わせて変動するようにする、Pアイテム実装するまでとりあえず3固定にする
        private float playerPower = 3.0f;
        // 属性モード
        internal bool iceMode;
        internal bool thunderMode;
        internal bool fireMode;

        // レーザーは初期化時に生成した後オブジェクトを保持しておく
        private List<PlayerBullet> sunLaserList;
        private List<PlayerBullet> iceLaserList;
        private List<PlayerBullet> thunderLaserList;
        private List<PlayerBullet> fireLaserList;

        // レーザーの画像の高さ(レーザーは根本を始点に生成する)
        private Vector3 laserHalfHeight;

        // 精度重視でdoubleにしておく
        // サブショットはメインショットと別周期でインターバルを用意する
        private double mainShotIntervalCount = 0;
        private double subShotIntervalCount = 0;
        private double subShotIntervalCount2 = 0;
        // 属性ショットが3色順番に出る場合の順番管理
        private BulletPhase elementShotPhase;
        private PlayerCharacter playerCharacter = PlayerCharacter.LunaMagician_A;

        /// <summary>
        /// どの属性ショットを射出する順番か表す
        /// </summary>
        private enum BulletPhase
        {
            NonElement,
            Ice,
            Thunder,
            Fire,
        }

        internal PlayerBulletGenerater(PlayerBulletPools playerBulletPools, Transform playerTransform, PlayerCharacter playerCharacter,
            PlayerBulletSetting playerBulletSetting, PlayerLaserSetting playerLaserSetting)
        {
            this.playerBulletPools = playerBulletPools;
            this.playerTransform = playerTransform;
            this.playerCharacter = playerCharacter;
            this.playerBulletSetting = playerBulletSetting;
            this.playerLaserSetting = playerLaserSetting;

            // レーザーはステージ開始時に生成して保持しておく
            // Initializeで実施すると初期化のたびに生成されてしまうのでコンストラクタで実施
            if (playerCharacter == PlayerCharacter.SunMagician_A)
            {
                PlayerBulletPool bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.SunLaser);
                CreateLaserObjectList(bulletPool, playerLaserSetting.PlayerLaserDataSet[LaserType.SunLaser].Count, out sunLaserList);
                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.IceLaser);
                CreateLaserObjectList(bulletPool, playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser].Count, out iceLaserList);
                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.ThunderLaser);
                CreateLaserObjectList(bulletPool, playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser].Count, out thunderLaserList);
                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FireLaser);
                CreateLaserObjectList(bulletPool, playerLaserSetting.PlayerLaserDataSet[LaserType.FireLaser].Count, out fireLaserList);
            }

            // 初期化
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            // ミサイルは通常弾と属性ショットの周期が別なので最初の属性を変えておく必要あり
            elementShotPhase = BulletPhase.Ice;

            iceMode = false;
            thunderMode = false;
            fireMode = false;
        }

        /// <summary>
        /// 各レーザーのオブジェクトを入れたリストを作る
        /// </summary>
        private void CreateLaserObjectList(PlayerBulletPool bulletPool, int createNum, out List<PlayerBullet> playerLaserList)
        {
            playerLaserList = new List<PlayerBullet>();
            for (int i = 0; i < createNum; i++)
            {
                var laserObject = bulletPool.Launch();
                laserObject.gameObject.SetActive(false);
                playerLaserList.Add(laserObject);
            }

            // 初回のみレーザーの高さを求めて覚えておく
            if (laserHalfHeight.y == 0.0f)
            {
                laserHalfHeight = new Vector3
                {
                    y = playerLaserList[0].GetComponent<SpriteRenderer>().bounds.size.y / 2
                };
            }
        }

        /// <summary>
        /// プレイヤーのUpdateに合わせて行う更新処理
        /// 弾を生成しないタイミングでも呼ぶ
        /// </summary>
        internal void GeneraterUpDate()
        {
            // ショットボタンが押されていたらショット生成
            //if (Input.GetButtonDown("Fire1"))
            if (Input.GetKey(KeyCode.Z))
            {
                PlayerBulletPool bulletPool;
                // プレイヤーの種類で分岐
                // メインショット
                if (mainShotIntervalCount == 0)
                {
                    if (playerCharacter == PlayerCharacter.LunaMagician_A || playerCharacter == PlayerCharacter.LunaMagician_B)
                    {
                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.MainShotBlue);
                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.MainShot]);
                    }
                    else
                    {
                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.MainShotRed);
                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.MainShot]);
                    }
                    mainShotIntervalCount = mainShotInterval;
                }

                // サブショット
                if (subShotIntervalCount == 0)
                {
                    switch (playerCharacter)
                    {
                        // ワイドショット
                        case PlayerCharacter.LunaMagician_A:
                            switch (elementShotPhase)
                            {
                                case BulletPhase.NonElement:
                                    // 通常弾
                                    bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.CrescentCutter);
                                    GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.CrescentCutter]);

                                    // 次の弾を氷弾に設定する
                                    elementShotPhase = BulletPhase.Ice;
                                    break;
                                case BulletPhase.Ice:
                                    // 氷属性ゲージ溜まっている時だけ発射する
                                    if (iceMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.IceCutter);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.IceCutter]);
                                    }
                                    // 発射有無にかかわらず次の弾を雷弾に設定する
                                    elementShotPhase = BulletPhase.Thunder;
                                    break;
                                case BulletPhase.Thunder:
                                    // 雷属性ゲージ溜まっている時だけ発射する
                                    if (thunderMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.ThunderCutter);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.ThunderCutter]);
                                    }
                                    // 発射有無にかかわらず次の弾を炎弾に設定する
                                    elementShotPhase = BulletPhase.Fire;
                                    break;
                                case BulletPhase.Fire:
                                    // 炎属性ゲージ溜まっている時だけ発射する
                                    if (fireMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FireCutter);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FireCutter]);
                                    }
                                    // 発射有無にかかわらず次の弾を通常弾に設定する
                                    elementShotPhase = BulletPhase.NonElement;
                                    break;
                                default:
                                    Debug.LogWarning($"BulletPhase is invalid type.");
                                    break;
                            }
                            // 発射有無にかかわらずインターバルを5/60秒に設定
                            subShotIntervalCount = 0.08333334;
                            break;
                        // ミサイル
                        case PlayerCharacter.LunaMagician_B:
                            // 通常弾
                            bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.BlackMissile);
                            GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.BlackMissile]);
                            // インターバルを15/60秒に設定
                            subShotIntervalCount = 0.25;
                            break;
                        //レーザーはインターバルを使わない
                        case PlayerCharacter.SunMagician_A:
                            break;
                        // 全方位ショット
                        case PlayerCharacter.SunMagician_B:
                            // 属性ゲージが溜まっている時は属性弾、溜まってない時は通常弾を出す
                            // 右上、右、右下のショット(氷属性)
                            if (iceMode)
                            {
                                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.IceFrushBullet);
                                GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.IceFrushBullet]);
                            }
                            else
                            {
                                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FrushBullet);
                                GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FrushBulletRight]);
                            }
                            // 左上、左、左下のショット(雷属性)
                            if (thunderMode)
                            {
                                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.ThunderFrushBullet);
                                GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.ThunderFrushBullet]);
                            }
                            else
                            {
                                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FrushBullet);
                                GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FrushBulletLeft]);
                            }
                            // 上下のショット(炎属性)
                            if (fireMode)
                            {
                                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FireFrushBullet);
                                GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FireFrushBullet]);
                            }
                            else
                            {
                                bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FrushBullet);
                                GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FrushBulletUpAndDown]);
                            }
                            // インターバルを3/60秒に設定
                            subShotIntervalCount = 0.05;
                            break;
                        default:
                            Debug.LogWarning($"playerCharacter is invalid type.");
                            break;
                    }
                }
                // 2種類目以降の周期のサブショット
                if (subShotIntervalCount2 == 0)
                {
                    switch (playerCharacter)
                    {
                        case PlayerCharacter.LunaMagician_B:    // ミサイル
                            switch (elementShotPhase)
                            {
                                case BulletPhase.Ice:
                                    // 氷属性ゲージ溜まっている時だけ発射する
                                    if (iceMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.IceMissile);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.IceMissile]);
                                    }
                                    // 発射有無にかかわらず次の弾を雷弾に設定する
                                    elementShotPhase = BulletPhase.Thunder;
                                    break;
                                case BulletPhase.Thunder:
                                    // 雷属性ゲージ溜まっている時だけ発射する
                                    if (thunderMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.ThunderMissile);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.ThunderMissile]);
                                    }
                                    // 発射有無にかかわらず次の弾を炎弾に設定する
                                    elementShotPhase = BulletPhase.Fire;
                                    break;
                                case BulletPhase.Fire:
                                    // 炎属性ゲージ溜まっている時だけ発射する
                                    if (fireMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FireMissile);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FireMissile]);
                                    }
                                    // 発射有無にかかわらず次の弾を氷弾に設定する
                                    elementShotPhase = BulletPhase.Ice;
                                    break;
                                default:
                                    Debug.LogWarning($"BulletPhase is invalid type.");
                                    break;
                            }
                            // 発射有無にかかわらずインターバルを5/60秒に設定
                            subShotIntervalCount2 = 0.08333334;
                            break;
                        default:    // その他の弾では2つ目の周期は使用しない
                            break;
                    }
                }

                // レーザーのみ別処理
                if (playerCharacter == PlayerCharacter.SunMagician_A)
                {
                    // 通常弾
                    EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.SunLaser], sunLaserList);

                    // 氷属性ゲージ溜まっている時だけ発射する
                    if (iceMode)
                    {
                        EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser], iceLaserList);
                    }
                    else
                    {
                        DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser], iceLaserList);
                    }
                    // 雷属性ゲージ溜まっている時だけ発射する
                    if (thunderMode)
                    {
                        EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser], thunderLaserList);
                    }
                    else
                    {
                        DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser], thunderLaserList);
                    }
                    // 炎属性ゲージ溜まっている時だけ発射する
                    if (fireMode)
                    {
                        EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.FireLaser], fireLaserList);
                    }
                    else
                    {
                        DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.FireLaser], fireLaserList);
                    }
                }
            }
            else
            {
                // ショットボタンが離されたらレーザーを消す
                if (playerCharacter == PlayerCharacter.SunMagician_A)
                {
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.SunLaser], sunLaserList);
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser], iceLaserList);
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser], thunderLaserList);
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.FireLaser], fireLaserList);
                }
            }

            // インターバルの時間減少
            // インターバルはdeltaTimeなので単位は秒
            // 読みやすさ重視で配列にしていないが、配列に入れるかクラスにしたほうがよさそう？
            if (mainShotIntervalCount > 0)
            {
                mainShotIntervalCount -= Time.deltaTime;
                if (mainShotIntervalCount < 0)
                {
                    mainShotIntervalCount = 0;
                }
            }
            if (subShotIntervalCount > 0)
            {
                subShotIntervalCount -= Time.deltaTime;
                if (subShotIntervalCount < 0)
                {
                    subShotIntervalCount = 0;
                }
            }
            if (subShotIntervalCount2 > 0)
            {
                subShotIntervalCount2 -= Time.deltaTime;
                if (subShotIntervalCount2 < 0)
                {
                    subShotIntervalCount2 = 0;
                }
            }

            //Debug.Log($"Time.deltaTime = {Time.deltaTime}, mainShotIntervalCount = {mainShotIntervalCount}, subShotIntervalCount = {subShotIntervalCount}");
        }



        /// <summary>
        /// NWayのプレイヤーの弾オブジェクト生成
        /// </summary>
        private void GeneratePlayerBurrageObject(PlayerBulletPool bulletPool, List<PlayerSingleBulletData> playerSingleBulletDataList)
        {
            foreach (var data in playerSingleBulletDataList)
            {
                var createPos = playerTransform.localPosition + (Vector3)data.ShotPos;
                GeneratePlayerBulletObject(bulletPool, createPos, (float)(data.BaseAtk + (int)playerPower * data.PowerAtkRate), data.Vx, data.Vy);
            }
        }

        /// <summary>
        /// プレイヤーの弾オブジェクト生成(オブジェクトプールから取り出して値を設定)
        /// </summary>
        private void GeneratePlayerBulletObject(PlayerBulletPool bulletPool, Vector2 createPos, float atk, float vx, float vy)
        {
            var playerBullet = bulletPool.Launch();
            // 弾の生成に失敗したら終了
            if(playerBullet == null)
            {
                return;
            }
            playerBullet.transform.localPosition = createPos;
            playerBullet.Atk = atk;
            playerBullet.SetVelocity(new Vector2(vx, vy));
        }

        /// <summary>
        /// レーザーを有効にする
        /// </summary>
        private void EnablePlayerMultipleLaser(List<PlayerSingleLaserData> playerSingleLaserDataList, List<PlayerBullet> laserList)
        {
            for (int i = 0; i < laserList.Count; i++)
            {
                var laser = laserList[i];
                // 非アクティブなレーザーのみアクティブにする
                if (!laser.gameObject.activeSelf)
                {
                    laser.gameObject.SetActive(true);
                    // 念のため速度も0に戻す
                    laser.SetVelocity(new Vector2(0, 0));
                }
                var laserData = playerSingleLaserDataList[i];
                // 攻撃力と位置は常時更新する
                laser.Atk = (float)(laserData.BaseAtk + (int)playerPower * laserData.PowerAtkRate);
                laser.transform.localPosition = playerTransform.localPosition + (Vector3)laserData.ShotPos + laserHalfHeight;
            }
        }

        /// <summary>
        /// レーザーを無効にする
        /// </summary>
        private void DisablePlayerMultipleLaser(List<PlayerSingleLaserData> playerSingleLaserDataList, List<PlayerBullet> laserList)
        {
            for (int i = 0; i < laserList.Count; i++)
            {
                var laser = laserList[i];
                // アクティブなレーザーのみ非アクティブにする
                if (laser.gameObject.activeSelf)
                {
                    laser.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// ショット発射間隔のリセット(被弾後など)
        /// </summary>
        //internal void ResetInterval()
        //{
        //    mainShotIntervalCount = 0;
        //    subShotIntervalCount = 0;
        //}
    }
}