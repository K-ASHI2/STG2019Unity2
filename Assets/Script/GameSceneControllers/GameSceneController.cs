using Assets.Script.Menu;
using Assets.Script.Player;
using Assets.Script.Common;
using Assets.Script.Enemy;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Script.GameSceneControllers
{
    public class GameSceneController : MonoBehaviour
    {
        // ポーズ画面のウィンドウ
        [SerializeField] private GameObject pauseWindow;
        // ポーズメニュー
        [SerializeField] private PauseMenu pauseMenu;
        // クリア画面のオブジェクト
        [SerializeField] private GameObject clearWindowObject;
        // クリア画面の制御クラス
        [SerializeField] private ClearWindowController clearWindowController;
        // ゲームプレイ画面を照射するライト
        [SerializeField] private Light2D light2D;
        // 難易度のテキスト
        [SerializeField] private TextMeshProUGUI difficultyText;
        // プレイヤー
        [SerializeField] public PlayerController playerController;
        // ボス
        [SerializeField] public BossController bossController;
        // 自機の弾のオブジェクトプール
        [SerializeField] private PlayerBulletPools playerBulletPools;
        // 敵の弾のオブジェクトプール
        [SerializeField] private EnemyBulletPools enemyBulletPools;

        // ポーズ状態かどうか
        public bool IsPauseMode { get; private set; }
        // ポーズ画面から戻った時にEscapeボタンで戻らないようにする
        public bool StartLock { protected get; set; } = false;
        // チャプター番号(0なら通常モード、1以上ならチャプター番号)
        public int StartChapterNum { get; set; }
        // 弾幕の段階(ステージの進捗度なので、ボス側ではなくシーン側に持つ)
        public int ChapterNum { get; set; }
        // 難易度
        public Difficulty Difficulty { private get; set; } = Difficulty.Normal;
        // ハイスコア
        public long HiScore { internal get; set; }
        // スコア　C++だとlong longだったので、longだと桁落ちで計算ずれるかも
        public long Score { internal get; set; }

        private bool isiInitialized = false;

        // Use this for initialization
        void Start()
        {
            IsPauseMode = false;
            pauseWindow.SetActive(false);
            difficultyText.text = Difficulty.ToString();

            // 未初期化状態なら初期化処理を呼び出す(デバッグ時にGameシーンから起動した場合のみ該当)
            if (!isiInitialized)
            {
                GameSceneInitialize();
            }
        }

        // Update is called once per frame
        void Update()
        {
            // ポーズ解除後はポーズボタンが離されるまで画面遷移処理を行わない
            if (StartLock)
            {
                // キー入力が離されたらロック解除
                if (!Input.GetKey(KeyCode.Escape))
                {
                    StartLock = false;
                }
            }

            // ポーズ画面への遷移チェック
            if (!StartLock && !IsPauseMode && Input.GetKey(KeyCode.Escape))
            {
                Time.timeScale = 0;
                IsPauseMode = true;
                light2D.intensity = 0.5f;
                pauseWindow.SetActive(true);
            }
        }
        /// <summary>
        /// ポーズ画面またはクリア画面の終了処理
        /// </summary>
        public void EndPause()
        {
            Time.timeScale = 1.0f;
            IsPauseMode = false;
            light2D.intensity = 1.0f;
            pauseWindow.SetActive(false);
            // ゲームシーンに戻った後に再度ポーズ画面に突入しないようにする
            StartLock = true;
        }


        /// <summary>
        /// クリア画面への遷移処理
        /// </summary>
        public void StartClearWindow(bool isClear)
        {
            Time.timeScale = 0;
            // ポーズのフラグを流用
            IsPauseMode = true;
            light2D.intensity = 0.5f;

            clearWindowController.SetTitleText(isClear);
            clearWindowObject.SetActive(true);
        }


        /// <summary>
        /// クリア画面の終了処理
        /// </summary>
        public void EndClearWindow()
        {
            Time.timeScale = 1.0f;
            IsPauseMode = false;
            light2D.intensity = 1.0f;
            clearWindowObject.SetActive(false);
            // ゲームシーンに戻った後に再度ポーズ画面に突入しないようにする
            StartLock = true;
        }

        /// <summary>
        /// ゲームシーンの初期化
        /// </summary>
        public void GameSceneInitialize()
        {
            // プレイヤーを初期化
            playerController.Initialize(StartChapterNum != 0, Difficulty);

            // スコアを0に戻す
            Score = 0;

            // TODO アイテムをすべて消す

            // TODO BurragePractice設定でなければ、ステージの音楽を最初から鳴らす
            //StageMusic();

            bossController.Initialize(Difficulty);

            // チャプターの初期化(通常プレイなら最初、Practiceなら指定した場面)
            if (StartChapterNum == 0)
            {
                ChapterNum = 1;
            }
            else
            {
                ChapterNum = StartChapterNum;
            }
            bossController.BossMode = ChapterNum;

            // ボス生成し、画面外から画面内に向けて登場させる
            bossController.AppearCount = 0.7f;

            // クリア画面を非表示にする
            clearWindowObject.SetActive(false);

            // 初期化完了にしておく
            isiInitialized = true;
        }

        private void RemoveAllBullet()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("EnemyBullet");
            foreach (GameObject ball in objects)
            {
                Destroy(ball);
            }
        }

        public void SetPlayerCharacter(PlayerCharacter playerCharacter)
        {
            playerController.PlayerCharacter = playerCharacter;
        }
    }
}