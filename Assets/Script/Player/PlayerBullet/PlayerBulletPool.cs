using Assets.Script.GameSceneControllers;
using UnityEngine;

namespace Assets.Script.Player
{
    /// <summary>
    /// プレイヤーの弾のオブジェクトプール
    /// </summary>
    public class PlayerBulletPool : BulletPoolBase
    {
        // 敵にダメージを与える処理のクラス
        public EnemyDamager EnemyDamager { private get; set; }

        private void Awake()
        {
        }

        /// <summary>
        /// 弾をアクティブにして取り出す処理
        /// </summary>
        public PlayerBullet Launch()
        {
            PlayerBullet tmpBullet;
            // Queueが空なら弾を生成する
            if (nonActiveBulletQueue.Count <= 0)
            {
                // 生成
                tmpBullet = (PlayerBullet)Instantiate(bullet, new Vector2(0, 0), Quaternion.identity, transform);
                // 弾オブジェクト側にどのオブジェクトプールに含まれるかセットする
                tmpBullet.SetBulletPool(this);
                // 弾に敵にダメージを与えるクラスを設定する
                tmpBullet.EnemyDamager = EnemyDamager;
            }
            else
            {
                //Queueから弾を一つ取り出す
                tmpBullet = (PlayerBullet)nonActiveBulletQueue.Dequeue();
                // 弾をアクティブ状態にする
                tmpBullet.gameObject.SetActive(true);
            }
            // アクティブ弾のリストに追加
            activeBulletList.Add(tmpBullet);

            // 弾の座標や種類は呼び出し元のメソッドで設定する
            return tmpBullet;
        }
    }
}