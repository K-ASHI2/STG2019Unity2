using Assets.Script.Effect;
using Assets.Script.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Enemy
{
    /// <summary>
    /// 敵の弾のオブジェクトプールを持つクラス
    /// </summary>
    public class EnemyBulletPools : MonoBehaviour
    {
        // プレイヤーのコントローラ
        [SerializeField] private PlayerController playerController;
        // 敵の弾のオブジェクトプールのリスト
        [SerializeField] private List<EnemyBulletPool> enemyBulletPoolList;
        // エフェクトのオブジェクトプール
        [SerializeField] private EffectAnimationPools effectAnimationPools;

        /// <summary>
        /// 弾の画像(画像の種類毎にオブジェクトプールを用意しておく、定義した番号順にリストに入れておく)
        /// </summary>
        internal enum EnemyBulletImageType
        {
            BlueBullet = 0,
            YellowBullet = 1,
            RedBullet = 2,
            SnowBulletS = 3,
            SnowBulletM = 4,
            ThunderBullet = 5,
            FireBulletS = 6,
            FireBulletM = 7,
        }

        private void Awake()
        {
            foreach(var enemyBulletPool in enemyBulletPoolList)
            {
                enemyBulletPool.PlayerController = playerController;
                enemyBulletPool.EffectAnimationPools = effectAnimationPools;
            }
        }

        /// <summary>
        /// 弾のオブジェクトプールを取得するメソッド
        /// </summary>
        internal EnemyBulletPool GetBulletPool(EnemyBulletImageType bulletImageType)
        {
            return enemyBulletPoolList[(int)bulletImageType];
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            foreach (var enemyBulletPool in enemyBulletPoolList)
            {
                enemyBulletPool.Initialize();
            }
        }

        // <summary>
        /// 画面表示中の弾消滅処理(消滅演出なしの場合はCollectAllBulletsを使う)
        /// </summary>
        public void VanishAllBullets()
        {
            if (enemyBulletPoolList != null)
            {
                foreach (var enemyBulletPool in enemyBulletPoolList)
                {
                    enemyBulletPool.VanishAllBullets();
                }
            }
        }

        // <summary>
        /// 画面表示中の全弾回収処理
        /// </summary>
        public void CollectAllBullets()
        {
            if (enemyBulletPoolList != null)
            {
                foreach (var enemyBulletPool in enemyBulletPoolList)
                {
                    enemyBulletPool.CollectAllBullets();
                }
            }
        }

        /// <summary>
        /// 反射弾の反射チェック
        /// </summary>
        internal void ReflectionBulletCheck(EnemyBulletGenerater enemyBulletGenerater)
        {
            if (enemyBulletPoolList != null)
            {
                foreach (var enemyBulletPool in enemyBulletPoolList)
                {
                    enemyBulletPool.ReflectionBulletCheck(enemyBulletGenerater);
                }
            }
        }
    }
}