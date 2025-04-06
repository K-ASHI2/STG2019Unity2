using UnityEngine;
using static Assets.Script.Enemy.EnemyBulletPools;

namespace Assets.Script.Enemy
{
    /// <summary>
    /// 弾幕の設定
    /// </summary>
    internal class ClockwiseBurrageSetting
    {
        // 敵の弾生成クラス
        internal EnemyBulletGenerater enemyBulletGenerater;
        // 弾を射出する基準となる座標
        internal Transform basePoint;
        // 反復実行間隔
        internal float repeatRate;
        // 弾速
        internal float speed;
        // 開始時の射出角度
        internal float startAngle;
        // 回転角
        internal float rotaAngle;
        // 進行方向を向けて弾を発射するかどうか
        internal bool isImageRotat;
        // 弾の種類
        internal EnemyBulletImageType enemyBulletImageType;
    }

    // TODO 弾幕の設定と状態をリストにして持ち、複数種類の弾幕を同時に管理できるようにする(弾幕7で3つ同時に動かすときに必要)
    /// <summary>
    /// 時計回りまたは半時計回りに弾を射出する弾幕生成クラス(マイナスなら時計回り、プラスなら反時計回り)
    /// </summary>
    internal class ClockwiseBurrageCreater : MonoBehaviour
    {
        // 敵の弾生成クラス
        private EnemyBulletGenerater enemyBulletGenerater;
        // 弾を射出する基準となる座標
        private Transform basePoint;
        // 反復実行間隔
        private float repeatRate;
        // 弾速
        private float speed;
        // 現在の射出角度
        private float angle;
        // 回転角
        private float rotaAngle;
        // 弾の発射間隔のカウント
        private float repeatTimeCount;
        // 有効かどうか
        public bool IsUse { private get; set; } = false;
        // 進行方向を向けて弾を発射するかどうか
        bool isImageRotat;
        // 弾の種類
        EnemyBulletImageType enemyBulletImageType;

        private void Update()
        {
            if (IsUse)
            {
                repeatTimeCount += Time.deltaTime;

                // 発射間隔の時間を超過したら弾を出す
                // 遅延対策で、deltaTimeの余り分は移動させる
                // 2個以上の弾がdeltaTimeの更新時間内に生成される可能性もあるのでwhileループする
                while (repeatTimeCount >= repeatRate)
                {
                    // 生成間隔を超過していた時間を計算しておき、その分だけ生成時に移動させる
                    var overCount = repeatTimeCount - repeatRate;
                    // 弾を生成し、一回分回転角を進める
                    var enemyBullet = enemyBulletGenerater.CreateBullet(enemyBulletImageType, basePoint.localPosition, speed, angle, overCount, isImageRotat);
                    angle += rotaAngle;

                    // 生成間隔1回分減少させて再確認
                    repeatTimeCount -= repeatRate;
                }
            }
        }

        /// <summary>
        /// 弾幕生成開始(初期化)
        /// </summary>
        internal void StartCreateBurrage(ClockwiseBurrageSetting clockwiseBurrageSetting)
        {
            enemyBulletGenerater = clockwiseBurrageSetting.enemyBulletGenerater;
            basePoint = clockwiseBurrageSetting.basePoint;
            repeatRate = clockwiseBurrageSetting.repeatRate;
            speed = clockwiseBurrageSetting.speed;
            angle = clockwiseBurrageSetting.startAngle;
            rotaAngle = clockwiseBurrageSetting.rotaAngle;
            isImageRotat = clockwiseBurrageSetting.isImageRotat;
            enemyBulletImageType = clockwiseBurrageSetting.enemyBulletImageType;
            repeatTimeCount = 0;
            IsUse = true;
        }
    }
}