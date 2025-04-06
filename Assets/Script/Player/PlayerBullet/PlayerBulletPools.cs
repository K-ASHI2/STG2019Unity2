using Assets.Script.Enemy;
using Assets.Script.GameSceneControllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Player
{
    /// <summary>
    /// 自機の弾のオブジェクトプールを持つクラス
    /// Playerクラスに全部持たせると分かりづらいので分けて持たせる
    /// 座標系はPlayAreaにしたいのでHierarchyではPlayerの下ではなくPlayAreaの下に配置する
    /// </summary>
    public class PlayerBulletPools : MonoBehaviour
    {
        // 敵にダメージを与える処理のクラス
        [SerializeField] private EnemyDamager enemyDamager;
        // 自機の弾のオブジェクトプールのリスト
        [SerializeField] private List<PlayerBulletPool> playerBulletPoolList;

        /// <summary>
        /// 弾の画像(画像の種類毎にオブジェクトプールを用意しておく)
        /// </summary>
        internal enum PlayerBulletImageType
        {
            MainShotBlue = 0,
            MainShotRed = 1,
            CrescentCutter = 2,
            IceCutter = 3,
            ThunderCutter = 4,
            FireCutter = 5,
            BlackMissile = 6,
            IceMissile = 7,
            ThunderMissile = 8,
            FireMissile = 9,
            FrushBullet = 10,
            IceFrushBullet = 11,
            ThunderFrushBullet = 12,
            FireFrushBullet = 13,
            SunLaser = 14,
            IceLaser = 15,
            ThunderLaser = 16,
            FireLaser = 17,
        }

        private void Awake()
        {
            foreach (var playerBulletPool in playerBulletPoolList)
            {
                playerBulletPool.EnemyDamager = enemyDamager;
            }
        }

        /// <summary>
        /// 弾のオブジェクトプールを取得するメソッド
        /// </summary>
        /// <param name="bulletImageType"></param>
        /// <returns></returns>
        internal PlayerBulletPool GetBulletPool(PlayerBulletImageType bulletImageType)
        {
            return playerBulletPoolList[(int)bulletImageType];
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            foreach (var playerBulletPool in playerBulletPoolList)
            {
                playerBulletPool.Initialize();
            }
        }

        /// <summary>
        /// 画面表示中の自機の全弾回収処理
        /// </summary>
        public void CollectAllBullets()
        {
            if (playerBulletPoolList != null)
            {
                foreach (var playerBulletPool in playerBulletPoolList)
                {
                    playerBulletPool.CollectAllBullets();
                }
            }
        }
    }
}