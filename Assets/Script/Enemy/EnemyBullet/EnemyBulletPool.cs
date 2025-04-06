using Assets.Script.Effect;
using Assets.Script.GameSceneControllers;
using Assets.Script.Player;
using UnityEngine;

namespace Assets.Script.Enemy
{
    /// <summary>
    /// 敵の弾のオブジェクトプール
    /// </summary>
    public class EnemyBulletPool : BulletPoolBase
    {
        // 敵にダメージを与える処理のクラス
        public PlayerController PlayerController { private get; set; }
        // エフェクトのオブジェクトプール
        public EffectAnimationPools EffectAnimationPools { private get; set; }

        private void Awake()
        {
        }

        /// <summary>
        /// 弾をアクティブにして取り出す処理
        /// </summary>
        public EnemyBullet Launch()
        {
            EnemyBullet tmpBullet;
            // 非アクティブな弾を入れているキューが空なら弾を生成する
            if (nonActiveBulletQueue.Count <= 0)
            {
                // 生成
                tmpBullet = (EnemyBullet)Instantiate(bullet, new Vector2(0, 0), Quaternion.identity, transform);
                // 弾オブジェクト側にどのオブジェクトプールに含まれるかセットする
                tmpBullet.SetBulletPool(this);
                // 被弾処理用にプレイヤーのコントローラを設定する
                tmpBullet.PlayerController = PlayerController;
                // 消滅演出用にエフェクトのオブジェクトプールクラスも設定する
                tmpBullet.EffectAnimationPools = EffectAnimationPools;
            }
            else
            {
                //Queueから弾を一つ取り出す
                tmpBullet = (EnemyBullet)nonActiveBulletQueue.Dequeue();
                // 弾をアクティブ状態にする
                tmpBullet.gameObject.SetActive(true);
            }
            // 未グレイズ状態に戻す
            tmpBullet.Grazed = false;
            // アクティブ弾のリストに追加
            activeBulletList.Add(tmpBullet);

            // 弾の座標や種類は呼び出し元のメソッドで設定する
            return tmpBullet;
        }

        /// <summary>
        /// このクラス内で持つ画面表示中の弾消滅処理(消滅演出なしの場合はCollectAllBulletsを使う)
        /// </summary>
        public void VanishAllBullets()
        {
            foreach (var activeBullet in activeBulletList)
            {
                // 弾の消滅演出開始
                ((EnemyBullet)activeBullet).CreateVanishEffectAndElementGaugeUp();
                // 弾のゲームオブジェクトを非アクティブ状態にする
                activeBullet.gameObject.SetActive(false);
                // 非アクティブ弾のキューに格納
                nonActiveBulletQueue.Enqueue(activeBullet);
            }
            // アクティブ弾のリストを空にする
            activeBulletList.Clear();
        }

        /// <summary>
        /// 反射弾の反射チェック
        /// </summary>
        internal void ReflectionBulletCheck(EnemyBulletGenerater enemyBulletGenerater)
        {
            if (activeBulletList != null)
            {
                // 削除した場合にリストの順序が狂わないように後ろからチェックする
                for(int i = activeBulletList.Count - 1; i >= 0; i--)
                {
                    var activeBullet = activeBulletList[i];
                    var isReflectionBullet = ((EnemyBullet)activeBullet).ReflectionBulletCheck(enemyBulletGenerater);
                    if (isReflectionBullet)
                    {
                        // 現在の弾を回収する
                        Collect(activeBullet);
                    }
                }
            }
        }
    }
}