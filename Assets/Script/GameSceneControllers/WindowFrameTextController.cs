using Assets.Script.Player;
using Assets.Script.Enemy;
using TMPro;
using UnityEngine;

namespace Assets.Script.GameSceneControllers
{
    /// <summary>
    /// WindowFrameのテキストの更新(難易度のみプレイ中に変化しないのでGameSceneCongtroller側で管理)
    /// </summary>
    public class WindowFrameTextController : MonoBehaviour
    {
        // シーンのコントローラー
        [SerializeField] private GameSceneController gameSceneController;
        // プレイヤーのコントローラー
        [SerializeField] private PlayerController playerController;
        // ボスのコントローラー
        [SerializeField] private BossController bossController;
        // ハイスコア
        [SerializeField] private TextMeshProUGUI HiScoreNumText;
        // スコア
        [SerializeField] private TextMeshProUGUI ScoreNumText;
        // ライフ
        [SerializeField] private TextMeshProUGUI LifeNumText;
        // ボム数
        [SerializeField] private TextMeshProUGUI BombNumText;
        // パワー
        //[SerializeField] private TextMeshProUGUI PowerNumText;
        // アイテムのスコア
        //[SerializeField] private TextMeshProUGUI ItemScoreText;
        // グレイズ数
        [SerializeField] private TextMeshProUGUI GrazeText;
        // ボスの攻撃の残り時間
        [SerializeField] private TextMeshProUGUI TimeText;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // スコア表示が画面外に出ないようにする(ついでにオーバーフロー対策)
            // 1フレームにlongの上限超えたらオーバーフローするが、現状はそういう状況にはならないはず
            if (gameSceneController.Score > 10000000000)
            {
                gameSceneController.Score = 9999999999;
            }

            // ハイスコアの更新チェック
            if (gameSceneController.Score > gameSceneController.HiScore)
            {
                gameSceneController.HiScore = gameSceneController.Score;
            }

            //HiScoreNumText.text = gameSceneController.HiScore.ToString();
            //ScoreNumText.text = gameSceneController.Score.ToString();                                                                                                                
            LifeNumText.text = playerController.Life.ToString();
            BombNumText.text = playerController.Bomb.ToString();
            //PowerNumText.text = playerController.Power.ToString();
            //ItemScoreText.text = gameSceneController.ItemScore.ToString();
            GrazeText.text = playerController.GrazeCount.ToString();

            // 残り時間をカウント中のみ描画
            if(bossController.IsTimeCounting)
            {
                TimeText.text = ((int)bossController.TimeCount).ToString();
            }
            else
            {
                TimeText.text = "";
            }
        }
    }
}