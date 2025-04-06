using UnityEngine;

namespace Assets.Script.Effect
{
    /// <summary>
    /// エフェクトのオブジェクト制御クラス
    /// </summary>
    public class EffectController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        // 対象のエフェクトを保持しているオブジェクトプールクラス
        public EffectAnimationPool EffectAnimationPool { private get; set; }
        
        /// <summary>
        /// アニメーションが終了したときに呼ばれるメソッド
        /// </summary>
        public void OnAnimationCompleted()
        {
            // 非アクティブ状態にする
            // オブジェクトプールこのクラスから参照するのが難しそうなので、定期的にオブジェクトプール側でチェックして回収する
            animator.gameObject.SetActive(false);
        }
    }
}