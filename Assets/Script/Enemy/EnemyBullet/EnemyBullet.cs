using Assets.Script.Player;
using Assets.Script.GameSceneControllers;
using UnityEngine;
using Assets.Script.Effect;
using Assets.Script.Common;
using UnityEngine.UIElements;

namespace Assets.Script.Enemy
{
    public class EnemyBullet : BulletBase
    {
        // 属性
        [SerializeField] private Element element;
        // 弾の種類
        [SerializeField] private Type type;

        // プレイヤー制御クラス(被弾処理用)
        public PlayerController PlayerController { private get; set; }
        // エフェクトのオブジェクトプール
        public EffectAnimationPools EffectAnimationPools { private get; set; }
        // レーザーかどうか
        internal bool Grazed { private get; set; } = false;

        // 敵の弾の属性
        public enum Element
        {
            Ice,
            Thunder,
            Fire
        }

        // 敵の弾の種類(弾の画像毎の反射時の動き用)
        public enum Type
        {
            Normal,
            SnowBulletS,
            SnowBulletM,
            ThunderBullet,
            FireBulletS
        }

        // Use this for initialization
        void Start()
        {
            BaseStart();
        }

        // Update is called once per frame
        void Update()
        {
            // レーザー以外ならプレイエリアより一定範囲以上外に出た弾を消す(オブジェクトプールに戻す)
            if (!IsLaser && PlayArea.IsVanishBulletArea(transform.localPosition, sizeX, sizeY))
            {
                BulletPool.Collect(this);
            }
        }

        /// <summary>
        /// プレイヤーへの弾の命中判定
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // レーザー以外の判定処理を行う
            if (!IsLaser)
            {
                // プレイヤーに命中した場合
                if (collision.gameObject.CompareTag("Player"))
                {
                    // 被弾時の弾消しと重複しないように先に命中した弾を消す
                    VanishBullet();
                    PlayerController.BulletHit();
                }
                // 未グレイズの弾がグレイズ範囲に入った場合
                if (collision.gameObject.CompareTag("GrazeCircle") && !Grazed)
                {
                    // グレイズ数を増加させ、グレイズ済みにする
                    PlayerController.Grazed(element);
                    Grazed = true;
                }
            }
        }

        /// <summary>
        /// プレイヤーへのレーザーの命中判定(敵は現状レーザー出さないので未使用)
        /// </summary>
        private void OnTriggerStay2D(Collider2D collision)
        {
            // レーザーの判定処理を行う
            if (IsLaser)
            {
                // レーザーがプレイヤーに命中した場合、被弾処理を行った後弾は消さない
                if (collision.gameObject.CompareTag("Player"))
                {
                    PlayerController.BulletHit();
                }
                // 未グレイズの弾がグレイズ範囲に入った場合
                if (collision.gameObject.CompareTag("GrazeCircle") && !Grazed)
                {
                    // グレイズ数を増加させ、グレイズ済みにする
                    // レーザーの場合連続グレイズできるように一定時間で戻したほうがいいかも
                    PlayerController.Grazed(element);
                    Grazed = true;
                }
            }
        }

        /// <summary>
        /// ボムや攻撃段階切り替わり時、プレイヤーに命中時などの弾消滅時の処理(画面外に出て消えるときは呼ばない)
        /// </summary>
        private void VanishBullet()
        {
            PlayerController.VanishedGaugeUp(element);

            // 消滅エフェクト生成
            CreateVanishEffectAndElementGaugeUp();

            // 弾を回収する
            BulletPool.Collect(this);
        }

        /// <summary>
        /// 消滅エフェクト生成+弾消滅時のゲージ上昇
        /// </summary>
        internal void CreateVanishEffectAndElementGaugeUp()
        {
            EffectAnimationPools.EffectType effectType;
            switch (element)
            {
                case Element.Ice:
                    effectType = EffectAnimationPools.EffectType.BlueBulletVanish;
                    break;
                case Element.Thunder:
                    effectType = EffectAnimationPools.EffectType.YellowBulletVanish;
                    break;
                case Element.Fire:
                    effectType = EffectAnimationPools.EffectType.RedBulletVanish;
                    break;
                default:
                    effectType = EffectAnimationPools.EffectType.BlueBulletVanish;
                    break;
            }
            var effectAnimation = EffectAnimationPools.GetBulletPool(effectType).Launch();
            effectAnimation.transform.localPosition = transform.localPosition;

            // 弾消滅時の属性ゲージ上昇
            PlayerController.VanishedGaugeUp(element);
        }

        /// <summary>
        /// 反射弾の反射チェック
        /// </summary>
        internal bool ReflectionBulletCheck(EnemyBulletGenerater enemyBulletGenerater)
        {
            bool isReflectionBullet = false;

            // 弾の中心が画面外に出た時、特定の種類の弾なら別の弾に変化させる
            int direction = PlayArea.OutOfPlayAreaCheck(transform.localPosition, out var overMoveX, out var overMoveY);

            var vx = rb.velocity.x;
            var vy = rb.velocity.y;

            // 画面外に出てから画面更新までに動いた時間
            float overMoveTimeX = 0;
            float overMoveTimeY = 0;
            if (overMoveX > 0)
            {
                overMoveTimeX = overMoveX / rb.velocity.x;
            }
            if (overMoveY > 0)
            {
                overMoveTimeY = overMoveY / rb.velocity.y;
            }
            // XY方向のうち、長いほうの時間が最初に画面枠に衝突した時間になる
            float overMoveLongerTime = Mathf.Min(overMoveTimeX, overMoveTimeY);
            // 画面端に衝突後に移動しすぎた分戻す
            transform.localPosition -= new Vector3(vx * overMoveLongerTime, vy * overMoveLongerTime);

            if (direction != (int)Direction.None)
            {
                EnemyBulletPools.EnemyBulletImageType enemyBulletImageType = (EnemyBulletPools.EnemyBulletImageType)(-1);
                switch (type)
                {
                    case Type.SnowBulletS:
                        // 下以外に出た場合のみランダムな速度で画面下方向基準で30度の2Way弾
                        if ((direction & (int)Direction.Lower) == 0)
                        {
                            enemyBulletImageType = EnemyBulletPools.EnemyBulletImageType.BlueBullet;
                            var newSpeed = 0.5f + Random.Range(0.0f, 1.0f);
                            enemyBulletGenerater.CreateBullet(enemyBulletImageType, transform.localPosition, newSpeed, -120.0f, overMoveLongerTime, false);
                            enemyBulletGenerater.CreateBullet(enemyBulletImageType, transform.localPosition, newSpeed, -60.0f, overMoveLongerTime, false);
                        }
                        break;
                    case Type.SnowBulletM:
                        // 左右の場合はX方向反転、上下の場合はY方向反転
                        enemyBulletImageType = EnemyBulletPools.EnemyBulletImageType.SnowBulletS;
                        if ((direction & (int)Direction.Left) != 0 || (direction & (int)Direction.Right) != 0)
                        {
                            vx *= -1;
                        }
                        if ((direction & (int)Direction.Lower) != 0 || (direction & (int)Direction.Upper) != 0)
                        {
                            vy *= -1;
                        }
                        enemyBulletGenerater.CreateBulletByVxVy(enemyBulletImageType, transform.localPosition, vx, vy, overMoveLongerTime, false);
                        break;
                    case Type.ThunderBullet:
                        // 自機狙いに変化
                        var playerX = PlayerController.transform.localPosition.x;
                        var playerY = PlayerController.transform.localPosition.y;
                        // 弾に対する自機の角度(rad)を求める
                        float rad = Mathf.Atan2(playerY - transform.localPosition.y, playerX - transform.localPosition.x);
                        enemyBulletImageType = EnemyBulletPools.EnemyBulletImageType.YellowBullet;
                        enemyBulletGenerater.CreateBulletByRad(enemyBulletImageType, transform.localPosition, 1.0f, rad, overMoveLongerTime, false);
                        break;
                    case Type.FireBulletS:
                        // 左右の場合はX方向反転、上下の場合はY方向反転
                        enemyBulletImageType = EnemyBulletPools.EnemyBulletImageType.RedBullet;
                        if ((direction & (int)Direction.Left) != 0 || (direction & (int)Direction.Right) != 0)
                        {
                            vx *= -1;
                        }
                        if ((direction & (int)Direction.Lower) != 0 || (direction & (int)Direction.Upper) != 0)
                        {
                            vy *= -1;
                        }
                        enemyBulletGenerater.CreateBulletByVxVy(enemyBulletImageType, transform.localPosition, vx, vy, overMoveLongerTime, false);
                        break;
                    default:
                        // その他の弾の場合は何もしない
                        break;
                }

                // 反射弾の場合、呼び出し元クラスで弾を回収する
                if ((int)enemyBulletImageType != -1)
                {
                    isReflectionBullet = true;
                }
            }

            return isReflectionBullet;
        }
    }
}