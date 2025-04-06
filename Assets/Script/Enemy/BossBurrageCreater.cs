using Assets.Script.Common;
using UnityEngine;
using static Assets.Script.Enemy.EnemyBulletPools;

namespace Assets.Script.Enemy
{
    /// <summary>
    /// ボスの弾幕を生成するクラス
    /// </summary>
    internal class BossBurrageCreater : MonoBehaviour
    {
        // ボスの座標
        [SerializeField] private Transform bossTransform;
        // 敵の弾のオブジェクトプール
        [SerializeField] private EnemyBulletPools enemyBulletPools;
        // 時計回りに弾を射出する弾幕生成クラス
        [SerializeField] private ClockwiseBurrageCreater clockwiseBurrageCreater;

        // 右
        public const float RIGHT_ANGLE = 0.0f;
        // 上
        public const float UPPER_ANGLE = 90.0f;
        // 左
        public const float LEFT_ANGLE = 180.0f;
        // 下
        public const float UNDER_ANGLE = 270.0f;

        // 敵の弾生成クラス
        private EnemyBulletGenerater enemyBulletGenerater;
        // 難易度
        private Difficulty difficulty;

        // Use this for initialization
        void Start()
        {
            enemyBulletGenerater = new EnemyBulletGenerater(enemyBulletPools);
        }

        private void Update()
        {
            // 反射弾の反射チェック
            enemyBulletPools.ReflectionBulletCheck(enemyBulletGenerater);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        internal void Initialize(Difficulty difficulty)
        {
            this.difficulty = difficulty;
            enemyBulletPools.Initialize();

            // 各弾幕生成クラスの弾生成フラグをfalseに戻す
            clockwiseBurrageCreater.IsUse = false;
        }

        /// <summary>
        /// 弾幕生成処理停止+弾消滅
        /// </summary>
        internal void StopVarrage()
        {
            clockwiseBurrageCreater.IsUse = false;
            enemyBulletPools.VanishAllBullets();
        }

        /// <summary>
        /// ボスの弾幕生成
        /// モード変更時に一度だけ呼び出して、予約する
        /// </summary>
        internal void CreateBurrage(int bossMode)
        {
            // ボスの段階に応じた弾幕を生成する
            // TODO 各弾幕生成処理を難易度分けする
            switch (bossMode)
            {
                case 1:
                    CreateBurrage1();
                    break;
                case 2:
                    CreateBurrage2();
                    break;
                case 3:
                    CreateBurrage3();
                    break;
                case 4:
                    CreateBurrage4();
                    break;
                // TODO 2,4,6,7,8番目の弾幕追加 
                //case 5:
                //    CreateBurrage5();
                //    break;
                //case 6:
                //    CreateBurrage6();
                //    break;
                //case 7:
                //    CreateBurrage7();
                //    break;
                //case 8:
                //    CreateBurrage8();
                //    break;
                //case 9:
                //    CreateBurrage9();
                //    break;
            }
        }

        private void CreateBurrage1()
        {
            var clockwiseBurrageSetting = new ClockwiseBurrageSetting()
            {
                enemyBulletGenerater = enemyBulletGenerater,
                basePoint = bossTransform,
                speed = 3.0f,
                repeatRate = 0.083f,
                startAngle = RIGHT_ANGLE - 6.0f,
                rotaAngle = -6.0f,
                isImageRotat = false,
                enemyBulletImageType = EnemyBulletImageType.SnowBulletS
            };
            clockwiseBurrageCreater.StartCreateBurrage(clockwiseBurrageSetting);
        }

        private void CreateBurrage2()
        {
            var clockwiseBurrageSetting = new ClockwiseBurrageSetting()
            {
                enemyBulletGenerater = enemyBulletGenerater,
                basePoint = bossTransform,
                speed = 3.0f,
                repeatRate = 0.083f,
                startAngle = RIGHT_ANGLE - 6.0f,
                rotaAngle = -6.0f,
                isImageRotat = true,
                enemyBulletImageType = EnemyBulletImageType.ThunderBullet
            };
            clockwiseBurrageCreater.StartCreateBurrage(clockwiseBurrageSetting);
        }

        private void CreateBurrage3()
        {
            var clockwiseBurrageSetting = new ClockwiseBurrageSetting()
            {
                enemyBulletGenerater = enemyBulletGenerater,
                basePoint = bossTransform,
                speed = 3.0f,
                repeatRate = 0.083f,
                startAngle = RIGHT_ANGLE - 6.0f,
                rotaAngle = -6.0f,
                isImageRotat = true,
                enemyBulletImageType = EnemyBulletImageType.FireBulletS
            };
            clockwiseBurrageCreater.StartCreateBurrage(clockwiseBurrageSetting);
        }

        private void CreateBurrage4()
        {

        }
    }
}
