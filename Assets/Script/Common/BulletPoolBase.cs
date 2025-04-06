using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    /// <summary>
    /// 弾のオブジェクトプールのベースクラス
    /// </summary>
    public abstract class BulletPoolBase : MonoBehaviour
    {
        // プレハブのオブジェクト(プレハブのスクリプトクラス)
        [SerializeField] protected BulletBase bullet;
        // 非アクティブ状態の弾を格納するキュー
        [NonSerialized] protected Queue<BulletBase> nonActiveBulletQueue;
        // アクティブ状態の弾を格納するリスト
        [NonSerialized] protected List<BulletBase> activeBulletList;

        /// <summary>
        /// 初期化処理(シーン開始時の状態にする時のみ呼ぶ)
        /// </summary>
        public void Initialize()
        {
            // 未初期化状態ならキューとリストの初期化
            if (nonActiveBulletQueue == null)
            {
                nonActiveBulletQueue = new Queue<BulletBase>();
                activeBulletList = new List<BulletBase>();
            }
            // 初期化済みなら生成済みの弾を全部破棄してリストとキューを空にする
            else
            {
                // アクティブ弾
                if (activeBulletList != null)
                {
                    foreach (var activeBullet in activeBulletList)
                    {
                        Destroy(activeBullet.gameObject);
                    }
                    activeBulletList.Clear();
                }
                // 非アクティブ弾
                if (nonActiveBulletQueue != null)
                {
                    foreach (var nonActiveBullet in nonActiveBulletQueue)
                    {
                        Destroy(nonActiveBullet.gameObject);
                    }
                    nonActiveBulletQueue.Clear();
                }
            }
        }

        // 弾取り出し処理と弾消滅処理は継承先のクラスで継承先のクラスに応じた型指定で作る 
        // abstractで書くと戻り値の型がBulletBaseにしかできないので、呼び出し元で使いづらそう

        /// <summary>
        /// 弾の回収処理
        /// </summary>
        public void Collect(BulletBase _bullet)
        {
            // 弾のゲームオブジェクトを非アクティブ状態にする
            _bullet.gameObject.SetActive(false);
            // 非アクティブ弾のキューに格納
            nonActiveBulletQueue.Enqueue(_bullet);
            // アクティブ弾のリストから削除
            activeBulletList.Remove(_bullet);
        }

        /// <summary>
        /// このクラス内で持つ画面表示中の弾回収処理
        /// </summary>
        public void CollectAllBullets()
        {
            if (activeBulletList != null)
            {
                foreach (var activeBullet in activeBulletList)
                {
                    // 弾のゲームオブジェクトを非アクティブ状態にする
                    activeBullet.gameObject.SetActive(false);
                    // 非アクティブ弾のキューに格納
                    nonActiveBulletQueue.Enqueue(activeBullet);
                }
                // アクティブ弾のリストを空にする
                activeBulletList.Clear();
            }
        }
    }
}