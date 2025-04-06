using Assets.Script.GameSceneControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Player
{
    public class PlayerBullet : BulletBase
    {
        // 敵へのダメージ管理スクリプト
        public EnemyDamager EnemyDamager { private get; set; }

        // 攻撃力
        public float Atk { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayerBullet()
        {
            // Startではなくコンストラクタで初期値を入れないと他メソッドのStartで初期化した値が上書きされてしまう場合がある
            Atk = 0;
        }

        // Use this for initialization
        void Start()
        {
            BaseStart();
        }

        // Update is called once per frame
        void Update()
        {
            // レーザー以外ならプレイエリア外に出た弾を消す(オブジェクトプールに戻す)
            if (!IsLaser && PlayArea.IsVanishBulletArea(transform.localPosition, sizeX, sizeY))
            {
                BulletPool.Collect(this);
            }
        }

        /// <summary>
        /// 敵への弾の命中判定
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 敵に命中した場合のみ処理(弾同士や自機とぶつかった場合にもイベント走るはず)
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // レーザー以外ならダメージを与えて弾を消す
                if (!IsLaser)
                {
                    EnemyDamager.EnemyDameged(collision.gameObject, Atk);
                    BulletPool.Collect(this);
                }
            }
        }

        /// <summary>
        /// 敵へのレーザーの命中判定
        /// </summary>
        private void OnTriggerStay2D(Collider2D collision)
        {
            // 敵に命中した場合のみ処理
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // レーザーなら当たった時間に応じたダメージを与える(秒単位で攻撃力は設定しておく)
                if (IsLaser)
                {
                    EnemyDamager.EnemyDameged(collision.gameObject, Atk * Time.deltaTime);
                }
            }
        }
    }
}