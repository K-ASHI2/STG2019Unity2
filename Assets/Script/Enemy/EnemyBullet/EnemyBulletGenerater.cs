using UnityEngine;
using static Assets.Script.Enemy.EnemyBulletPools;

namespace Assets.Script.Enemy
{
    internal class EnemyBulletGenerater
    {
        // 敵の弾のオブジェクトプール
        private EnemyBulletPools enemyBulletPools;

        public EnemyBulletGenerater(EnemyBulletPools enemyBulletPools)
        {
            this.enemyBulletPools = enemyBulletPools;
        }

        /// <summary>
        /// 弾生成(角度と速度で指定、XY共通で追加で移動すべき時間を与える)
        /// </summary>
        internal EnemyBullet CreateBullet(EnemyBulletImageType enemyBulletImageType, Vector3 createPos, float speed, float angle, float overCount, bool isImageRotat)
        {
            // 弧度法からラジアンに変換する
            var rad = angle * Mathf.Deg2Rad;

            return CreateBulletByRad(enemyBulletImageType, createPos, speed, rad, overCount, isImageRotat);
        }

        /// <summary>
        /// 弾生成(ラジアンの角度と速度で指定)
        /// </summary>
        internal EnemyBullet CreateBulletByRad(EnemyBulletImageType enemyBulletImageType, Vector3 createPos, float speed, float rad, float overCount, bool isImageRotat)
        {
            // XY軸方向の移動速度を計算する
            var vx = speed * Mathf.Cos(rad);
            var vy = speed * Mathf.Sin(rad);

            // 弾生成
            var enemyBullet = CreateBulletByVxVy(enemyBulletImageType, createPos, vx, vy, overCount, isImageRotat);

            return enemyBullet;
        }

        /// <summary>
        /// 弾生成(XY軸方向の速度で指定)
        /// </summary>
        internal EnemyBullet CreateBulletByVxVy(EnemyBulletImageType enemyBulletImageType, Vector3 createPos, float vx, float vy, float overCount, bool isImageRotat)
        {
            var bulletPool = enemyBulletPools.GetBulletPool(enemyBulletImageType);
            var enemyBullet = bulletPool.Launch();
            enemyBullet.transform.localPosition = createPos;

            // 移動速度を設定する
            enemyBullet.SetVelocity(new Vector2(vx, vy));

            // 弾の向きを進行方向が前になるように変更
            if (isImageRotat)
            {
                float zAngle = Mathf.Atan2(vy, vx) * Mathf.Rad2Deg - 90.0f;
                enemyBullet.transform.rotation = Quaternion.Euler(0, 0, zAngle);
            }

            // 生成間隔を超過していた時間分だけ移動させる
            var movePositon = new Vector2(vx * overCount, vy * overCount);
            enemyBullet.MovePosition(movePositon);

            return enemyBullet;
        }
    }
}