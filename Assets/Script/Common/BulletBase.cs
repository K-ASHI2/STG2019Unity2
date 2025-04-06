using UnityEngine;

namespace Assets.Script
{
    /// <summary>
    /// 弾のベースクラス
    /// 生成したフレームの弾が動かないように、他のクラスより先に処理する
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public abstract class BulletBase : MonoBehaviour
    {
        /// <summary>
        /// 2Dスプライトの物理演算クラス
        /// </summary>
        [SerializeField] protected Rigidbody2D rb;
        // コライダー
        [SerializeField] private Collider2D bulletCollider;
        // レーザーかどうか
        [SerializeField] private bool isLaser;
        // 弾の縦横方向の大きさ
        protected float sizeX;
        protected float sizeY;

        // 弾のオブジェクトプール
        public BulletPoolBase BulletPool { get; private set; }
        // レーザーかどうか(他クラスから取得するとき用)
        public bool IsLaser { get; private set; }

        /// <summary>
        /// 継承先のクラスのStartで呼ぶ想定の初期化メソッド
        /// </summary>
        protected void BaseStart()
        {
            IsLaser = isLaser;

            // レーザーまたは通常弾の弾の種類に応じた判定サイズを設定する
            if (isLaser)
            {
                sizeX = ((BoxCollider2D)bulletCollider).size.x;
                sizeY = ((BoxCollider2D)bulletCollider).size.y;
            }
            else
            {
                sizeX = ((CircleCollider2D)bulletCollider).radius;
                sizeY = ((CircleCollider2D)bulletCollider).radius;
            }
        }

        /// <summary>
        /// 弾の速度設定
        /// </summary>
        public void SetVelocity(Vector2 velocity)
        {
            // xy方向に移動させる
            rb.velocity = velocity;
        }

        /// <summary>
        /// 弾を指定した位置に移動する
        /// </summary>
        public void MovePosition(Vector3 position)
        {
            // Rigidbody2Dの座標を変更しようとしても適用されないので、transformの位置を変更する
            transform.localPosition += position;
        }

        /// <summary>
        /// 弾のオブジェクトプールを上位層からセットする
        /// </summary>
        public void SetBulletPool(BulletPoolBase bulletPool)
        {
            BulletPool = bulletPool;
        }
    }
}