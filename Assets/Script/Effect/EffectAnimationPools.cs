using Assets.Script.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Effect
{
    /// <summary>
    /// 敵の弾のオブジェクトプールを持つクラス
    /// Playerクラスに全部持たせると分かりづらいので分けて持たせる
    /// 座標系はPlayAreaにしたいのでHierarchyではPlayerの下ではなくPlayAreaの下に配置する
    /// 配列やDictionaryに入れたほうが良いかも
    /// </summary>
    public class EffectAnimationPools : MonoBehaviour
    {
        // 敵の弾のオブジェクトプールのリスト
        [SerializeField] private List<EffectAnimationPool> effectAnimationPoolList;

        /// <summary>
        /// エフェクトの種類
        /// </summary>
        public enum EffectType
        {
            BlueBulletVanish = 0,
            YellowBulletVanish = 1,
            RedBulletVanish = 2,
            StarBomb = 3
        }

        private void Awake()
        {
        }

        /// <summary>
        /// エフェクトのオブジェクトプールを取得するメソッド
        /// </summary>
        internal EffectAnimationPool GetBulletPool(EffectType bulletImageType)
        {
            return effectAnimationPoolList[(int)bulletImageType];
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            foreach (var effectAnimationPool in effectAnimationPoolList)
            {
                effectAnimationPool.Initialize();
            }
        }
    }
}