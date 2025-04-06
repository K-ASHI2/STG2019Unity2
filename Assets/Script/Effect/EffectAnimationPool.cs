using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Script.Effect
{
    /// <summary>
    /// エフェクトアニメーションのオブジェクトプール制御クラス
    /// </summary>
    public class EffectAnimationPool : MonoBehaviour
    {
        // 生成するエフェクトのUnity標準アニメーションクラス
        [SerializeField] private Animator animator;
        // 非アクティブ状態のエフェクトを格納するキュー
        [NonSerialized] protected Queue<Animator> nonActiveEffectQueue;
        // アクティブ状態のエフェクトを格納するリスト
        [NonSerialized] protected List<Animator> activeEffectList;

        // エフェクトをキューに回収する処理の間隔
        private const float COLLECT_INTERVAL_TIME = 1.0f;
        // エフェクトをキューに回収するまでの時間のカウント
        private float collectIntevalCount;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // 未初期化状態ならキューとリストの初期化
            if (nonActiveEffectQueue == null)
            {
                nonActiveEffectQueue = new Queue<Animator>();
                activeEffectList = new List<Animator>();
            }
            // 初期化済みなら生成済みのエフェクトを全部破棄してリストとキューを空にする
            else
            {
                // アクティブエフェクト
                if (activeEffectList != null)
                {
                    foreach (var activeBullet in activeEffectList)
                    {
                        Destroy(activeBullet.gameObject);
                    }
                    activeEffectList.Clear();
                }
                // 非アクティブエフェクト
                if (nonActiveEffectQueue != null)
                {
                    foreach (var nonActiveBullet in nonActiveEffectQueue)
                    {
                        Destroy(nonActiveBullet.gameObject);
                    }
                    nonActiveEffectQueue.Clear();
                }
            }

            collectIntevalCount = COLLECT_INTERVAL_TIME;
        }

        private void Update()
        {
            // 一定時間おきに非アクティブなアニメーションを回収する
            collectIntevalCount -= Time.deltaTime;
            if (collectIntevalCount <= 0 && activeEffectList != null && nonActiveEffectQueue != null)
            {
                // foreachでは削除できないので、for文を後ろから回して削除
                for (int i = activeEffectList.Count - 1; i >= 0; i--)
                {
                    var effect = activeEffectList[i];
                    if (!effect.gameObject.activeSelf)
                    {
                        // 非アクティブエフェクトのキューに格納
                        nonActiveEffectQueue.Enqueue(effect);
                        // アクティブエフェクトのリストから削除
                        activeEffectList.Remove(effect);
                    }
                }
                collectIntevalCount = COLLECT_INTERVAL_TIME;
            }
        }

        /// <summary>
        /// 弾をアクティブにして取り出す処理
        /// </summary>
        public Animator Launch()
        {
            Animator tmpEffect;
            // 非アクティブなエフェクトを入れているキューが空ならエフェクトを生成する
            if (nonActiveEffectQueue.Count <= 0)
            {
                // アニメーションを生成
                tmpEffect = Instantiate(animator, new Vector2(0, 0), Quaternion.identity, transform);
            }
            else
            {
                //Queueからエフェクトを一つ取り出す
                tmpEffect = nonActiveEffectQueue.Dequeue();
                // エフェクトをアクティブ状態にする
                tmpEffect.gameObject.SetActive(true);
                // リスタートフラグを有効にし、最初からアニメーションを再生しなおす
                tmpEffect.SetBool("isRestart", true);
            }
            // アクティブ中のエフェクトのリストに追加
            activeEffectList.Add(tmpEffect);

            // エフェクトの座標や種類は呼び出し元のメソッドで設定する
            return tmpEffect;
        }
    }
}