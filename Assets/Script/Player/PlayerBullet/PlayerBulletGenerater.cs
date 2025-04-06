using UnityEngine;
using Assets.Script.Common;
using static Assets.Script.Player.PlayerBulletPools;
using System.Collections.Generic;

namespace Assets.Script.Player
{
    /// <summary>
    /// �e�������N���X
    /// </summary>
    internal class PlayerBulletGenerater
    {
        // ���C���V���b�g�̃C���^�[�o��(5/60�b)
        private const float mainShotInterval = 0.08333334f;

        // ���@�̒e�̃I�u�W�F�N�g�v�[��
        private PlayerBulletPools playerBulletPools;
        // �v���C���[��TransForm�I�u�W�F�N�g
        private readonly Transform playerTransform;
        // ���@�̒e�̐ݒ�
        PlayerBulletSetting playerBulletSetting;
        // ���@�̃��[�U�[�̐ݒ�
        PlayerLaserSetting playerLaserSetting;
        // ���@�̃p���[
        // TODO �Q�Ɠn���ɂ��āA�v���C���[�̃p���[�̕ω��ɍ��킹�ĕϓ�����悤�ɂ���AP�A�C�e����������܂łƂ肠����3�Œ�ɂ���
        private float playerPower = 3.0f;
        // �������[�h
        internal bool iceMode;
        internal bool thunderMode;
        internal bool fireMode;

        // ���[�U�[�͏��������ɐ���������I�u�W�F�N�g��ێ����Ă���
        private List<PlayerBullet> sunLaserList;
        private List<PlayerBullet> iceLaserList;
        private List<PlayerBullet> thunderLaserList;
        private List<PlayerBullet> fireLaserList;

        // ���[�U�[�̉摜�̍���(���[�U�[�͍��{���n�_�ɐ�������)
        private Vector3 laserHalfHeight;

        // ���x�d����double�ɂ��Ă���
        // �T�u�V���b�g�̓��C���V���b�g�ƕʎ����ŃC���^�[�o����p�ӂ���
        private double mainShotIntervalCount = 0;
        private double subShotIntervalCount = 0;
        private double subShotIntervalCount2 = 0;
        // �����V���b�g��3�F���Ԃɏo��ꍇ�̏��ԊǗ�
        private BulletPhase elementShotPhase;
        private PlayerCharacter playerCharacter = PlayerCharacter.LunaMagician_A;

        /// <summary>
        /// �ǂ̑����V���b�g���ˏo���鏇�Ԃ��\��
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

            // ���[�U�[�̓X�e�[�W�J�n���ɐ������ĕێ����Ă���
            // Initialize�Ŏ��{����Ə������̂��тɐ�������Ă��܂��̂ŃR���X�g���N�^�Ŏ��{
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

            // ������
            Initialize();
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Initialize()
        {
            // �~�T�C���͒ʏ�e�Ƒ����V���b�g�̎������ʂȂ̂ōŏ��̑�����ς��Ă����K�v����
            elementShotPhase = BulletPhase.Ice;

            iceMode = false;
            thunderMode = false;
            fireMode = false;
        }

        /// <summary>
        /// �e���[�U�[�̃I�u�W�F�N�g����ꂽ���X�g�����
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

            // ����̂݃��[�U�[�̍��������߂Ċo���Ă���
            if (laserHalfHeight.y == 0.0f)
            {
                laserHalfHeight = new Vector3
                {
                    y = playerLaserList[0].GetComponent<SpriteRenderer>().bounds.size.y / 2
                };
            }
        }

        /// <summary>
        /// �v���C���[��Update�ɍ��킹�čs���X�V����
        /// �e�𐶐����Ȃ��^�C�~���O�ł��Ă�
        /// </summary>
        internal void GeneraterUpDate()
        {
            // �V���b�g�{�^����������Ă�����V���b�g����
            //if (Input.GetButtonDown("Fire1"))
            if (Input.GetKey(KeyCode.Z))
            {
                PlayerBulletPool bulletPool;
                // �v���C���[�̎�ނŕ���
                // ���C���V���b�g
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

                // �T�u�V���b�g
                if (subShotIntervalCount == 0)
                {
                    switch (playerCharacter)
                    {
                        // ���C�h�V���b�g
                        case PlayerCharacter.LunaMagician_A:
                            switch (elementShotPhase)
                            {
                                case BulletPhase.NonElement:
                                    // �ʏ�e
                                    bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.CrescentCutter);
                                    GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.CrescentCutter]);

                                    // ���̒e��X�e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.Ice;
                                    break;
                                case BulletPhase.Ice:
                                    // �X�����Q�[�W���܂��Ă��鎞�������˂���
                                    if (iceMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.IceCutter);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.IceCutter]);
                                    }
                                    // ���˗L���ɂ�����炸���̒e�𗋒e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.Thunder;
                                    break;
                                case BulletPhase.Thunder:
                                    // �������Q�[�W���܂��Ă��鎞�������˂���
                                    if (thunderMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.ThunderCutter);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.ThunderCutter]);
                                    }
                                    // ���˗L���ɂ�����炸���̒e�����e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.Fire;
                                    break;
                                case BulletPhase.Fire:
                                    // �������Q�[�W���܂��Ă��鎞�������˂���
                                    if (fireMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FireCutter);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FireCutter]);
                                    }
                                    // ���˗L���ɂ�����炸���̒e��ʏ�e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.NonElement;
                                    break;
                                default:
                                    Debug.LogWarning($"BulletPhase is invalid type.");
                                    break;
                            }
                            // ���˗L���ɂ�����炸�C���^�[�o����5/60�b�ɐݒ�
                            subShotIntervalCount = 0.08333334;
                            break;
                        // �~�T�C��
                        case PlayerCharacter.LunaMagician_B:
                            // �ʏ�e
                            bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.BlackMissile);
                            GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.BlackMissile]);
                            // �C���^�[�o����15/60�b�ɐݒ�
                            subShotIntervalCount = 0.25;
                            break;
                        //���[�U�[�̓C���^�[�o�����g��Ȃ�
                        case PlayerCharacter.SunMagician_A:
                            break;
                        // �S���ʃV���b�g
                        case PlayerCharacter.SunMagician_B:
                            // �����Q�[�W�����܂��Ă��鎞�͑����e�A���܂��ĂȂ����͒ʏ�e���o��
                            // �E��A�E�A�E���̃V���b�g(�X����)
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
                            // ����A���A�����̃V���b�g(������)
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
                            // �㉺�̃V���b�g(������)
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
                            // �C���^�[�o����3/60�b�ɐݒ�
                            subShotIntervalCount = 0.05;
                            break;
                        default:
                            Debug.LogWarning($"playerCharacter is invalid type.");
                            break;
                    }
                }
                // 2��ޖڈȍ~�̎����̃T�u�V���b�g
                if (subShotIntervalCount2 == 0)
                {
                    switch (playerCharacter)
                    {
                        case PlayerCharacter.LunaMagician_B:    // �~�T�C��
                            switch (elementShotPhase)
                            {
                                case BulletPhase.Ice:
                                    // �X�����Q�[�W���܂��Ă��鎞�������˂���
                                    if (iceMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.IceMissile);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.IceMissile]);
                                    }
                                    // ���˗L���ɂ�����炸���̒e�𗋒e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.Thunder;
                                    break;
                                case BulletPhase.Thunder:
                                    // �������Q�[�W���܂��Ă��鎞�������˂���
                                    if (thunderMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.ThunderMissile);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.ThunderMissile]);
                                    }
                                    // ���˗L���ɂ�����炸���̒e�����e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.Fire;
                                    break;
                                case BulletPhase.Fire:
                                    // �������Q�[�W���܂��Ă��鎞�������˂���
                                    if (fireMode)
                                    {
                                        bulletPool = playerBulletPools.GetBulletPool(PlayerBulletImageType.FireMissile);
                                        GeneratePlayerBurrageObject(bulletPool, playerBulletSetting.PlayerBarrageDataSet[BulletType.FireMissile]);
                                    }
                                    // ���˗L���ɂ�����炸���̒e��X�e�ɐݒ肷��
                                    elementShotPhase = BulletPhase.Ice;
                                    break;
                                default:
                                    Debug.LogWarning($"BulletPhase is invalid type.");
                                    break;
                            }
                            // ���˗L���ɂ�����炸�C���^�[�o����5/60�b�ɐݒ�
                            subShotIntervalCount2 = 0.08333334;
                            break;
                        default:    // ���̑��̒e�ł�2�ڂ̎����͎g�p���Ȃ�
                            break;
                    }
                }

                // ���[�U�[�̂ݕʏ���
                if (playerCharacter == PlayerCharacter.SunMagician_A)
                {
                    // �ʏ�e
                    EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.SunLaser], sunLaserList);

                    // �X�����Q�[�W���܂��Ă��鎞�������˂���
                    if (iceMode)
                    {
                        EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser], iceLaserList);
                    }
                    else
                    {
                        DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser], iceLaserList);
                    }
                    // �������Q�[�W���܂��Ă��鎞�������˂���
                    if (thunderMode)
                    {
                        EnablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser], thunderLaserList);
                    }
                    else
                    {
                        DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser], thunderLaserList);
                    }
                    // �������Q�[�W���܂��Ă��鎞�������˂���
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
                // �V���b�g�{�^���������ꂽ�烌�[�U�[������
                if (playerCharacter == PlayerCharacter.SunMagician_A)
                {
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.SunLaser], sunLaserList);
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.IceLaser], iceLaserList);
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.ThunderLaser], thunderLaserList);
                    DisablePlayerMultipleLaser(playerLaserSetting.PlayerLaserDataSet[LaserType.FireLaser], fireLaserList);
                }
            }

            // �C���^�[�o���̎��Ԍ���
            // �C���^�[�o����deltaTime�Ȃ̂ŒP�ʂ͕b
            // �ǂ݂₷���d���Ŕz��ɂ��Ă��Ȃ����A�z��ɓ���邩�N���X�ɂ����ق����悳�����H
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
        /// NWay�̃v���C���[�̒e�I�u�W�F�N�g����
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
        /// �v���C���[�̒e�I�u�W�F�N�g����(�I�u�W�F�N�g�v�[��������o���Ēl��ݒ�)
        /// </summary>
        private void GeneratePlayerBulletObject(PlayerBulletPool bulletPool, Vector2 createPos, float atk, float vx, float vy)
        {
            var playerBullet = bulletPool.Launch();
            // �e�̐����Ɏ��s������I��
            if(playerBullet == null)
            {
                return;
            }
            playerBullet.transform.localPosition = createPos;
            playerBullet.Atk = atk;
            playerBullet.SetVelocity(new Vector2(vx, vy));
        }

        /// <summary>
        /// ���[�U�[��L���ɂ���
        /// </summary>
        private void EnablePlayerMultipleLaser(List<PlayerSingleLaserData> playerSingleLaserDataList, List<PlayerBullet> laserList)
        {
            for (int i = 0; i < laserList.Count; i++)
            {
                var laser = laserList[i];
                // ��A�N�e�B�u�ȃ��[�U�[�̂݃A�N�e�B�u�ɂ���
                if (!laser.gameObject.activeSelf)
                {
                    laser.gameObject.SetActive(true);
                    // �O�̂��ߑ��x��0�ɖ߂�
                    laser.SetVelocity(new Vector2(0, 0));
                }
                var laserData = playerSingleLaserDataList[i];
                // �U���͂ƈʒu�͏펞�X�V����
                laser.Atk = (float)(laserData.BaseAtk + (int)playerPower * laserData.PowerAtkRate);
                laser.transform.localPosition = playerTransform.localPosition + (Vector3)laserData.ShotPos + laserHalfHeight;
            }
        }

        /// <summary>
        /// ���[�U�[�𖳌��ɂ���
        /// </summary>
        private void DisablePlayerMultipleLaser(List<PlayerSingleLaserData> playerSingleLaserDataList, List<PlayerBullet> laserList)
        {
            for (int i = 0; i < laserList.Count; i++)
            {
                var laser = laserList[i];
                // �A�N�e�B�u�ȃ��[�U�[�̂ݔ�A�N�e�B�u�ɂ���
                if (laser.gameObject.activeSelf)
                {
                    laser.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// �V���b�g���ˊԊu�̃��Z�b�g(��e��Ȃ�)
        /// </summary>
        //internal void ResetInterval()
        //{
        //    mainShotIntervalCount = 0;
        //    subShotIntervalCount = 0;
        //}
    }
}