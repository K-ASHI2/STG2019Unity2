using UnityEngine;
using Assets.Script.Common;

namespace Assets.Script.GameSceneControllers
{
    public class PlayArea : MonoBehaviour
    {
        public static float Left { get; private set; }
        public static float Right { get; private set; }
        public static float Upper { get; private set; }
        public static float Lower { get; private set; }
        // 画面外の弾消滅位置の余白(弾が完全に画面外に出る位置にする)
        public static float VanishBulletMargin { get; private set; } = 1.0f;

        /// <summary>
        /// プレイエリアの起動時にプレイエリアの大きさを計算して覚えておく
        /// </summary>
        void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            SpriteRenderer playAreaSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var sizeX = playAreaSpriteRenderer.bounds.size.x;
            Right = sizeX / 2;
            Left = sizeX / 2 * -1;
            var sizeY = playAreaSpriteRenderer.bounds.size.y;
            Upper = sizeY / 2;
            Lower = sizeY / 2 * -1;
        }

        /// <summary>
        /// 物体がプレイエリア外かどうか判定して返す
        /// </summary>
        /// <param name="vector">プレイエリア外か判定する座標</param>
        /// <param name="overMoveX">画面外に出たX軸方向の距離</param>
        /// <param name="overMoveY">画面外に出たY軸方向の距離</param>
        public static int OutOfPlayAreaCheck(Vector3 vector, out float overMoveX, out float overMoveY)
        {
            int direction = (int)Direction.None;
            overMoveX = 0.0f;
            overMoveY = 0.0f;
            if (vector.x < Left)
            {
                direction |= (int)Direction.Left;
                overMoveX = vector.x - Left;
            }
            else if (vector.x > Right)
            {
                direction |= (int)Direction.Right;
                overMoveX = vector.x - Right;
            }
            if (vector.y < Lower)
            {
                direction |= (int)Direction.Lower;
                overMoveY = vector.y - Lower;
            }
            else if (vector.y > Upper)
            {
                direction |= (int)Direction.Upper;
                overMoveY = vector.y - Upper;
            }

            return direction;
        }


        /// <summary>
        /// 物体全体が弾を消す範囲に入ったかどうか判定して返す
        /// </summary>
        public static bool IsVanishBulletArea(Vector3 vector, float xSize, float ySize)
        {
            bool result;
            if (vector.y - ySize < Lower - VanishBulletMargin
                || vector.y + ySize > Upper + VanishBulletMargin
                || vector.x - xSize < Left - VanishBulletMargin
                || vector.x + xSize > Right + VanishBulletMargin)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}