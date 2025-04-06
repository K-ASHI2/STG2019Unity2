using Assets.Script.Enemy;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.GameSceneControllers
{
    /// <summary>
    /// 敵にダメージを与える処理のクラス
    /// </summary>
    public class EnemyDamager : MonoBehaviour
    {
        // ボスのコントローラリスト(今は一体だが一応リストにしている)
        [SerializeField] private List<BossController> bossControllers;
        // 雑魚敵も作るならリストを追加する

        public void EnemyDameged(GameObject gameObject, float damage)
        {
            // ボスと一致するgameObjectを探し出し、そのボスのHPを減らす
            // GetComponentで探すより早いはず
            foreach (var bossController in bossControllers)
            {
                if(bossController.gameObject == gameObject)
                {
                    bossController.Dameged(damage);
                    break;
                }
            }
        }
    }
}