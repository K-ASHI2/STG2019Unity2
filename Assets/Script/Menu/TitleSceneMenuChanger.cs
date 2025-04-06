using Assets.Script.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Script.Menu.TitleSceneMenuChanger;

namespace Assets.Script.Menu
{
    /// <summary>
    /// 戻るボタンで戻る先のメニューとその時の選択位置
    /// </summary>
    public class BackMenuSetting
    {
        public TitleSceneMenu backMenu;
        public int selectedItemNumH;
        public int selectedItemNumV;
    }

    public class TitleSceneMenuChanger : MonoBehaviour
    {
        [SerializeField] private Canvas titleCanvas;
        [SerializeField] private Canvas characterSelectCanvas;
        [SerializeField] private Canvas difficultySelectCanvas;
        [SerializeField] private Canvas burragePracticeCanvas;
        [SerializeField] private Canvas keyConfigCanvas;

        [SerializeField] private TitleMenu titleMenu;
        [SerializeField] private CharacterSelectMenu characterSelectMenu;
        [SerializeField] private DifficultySelectMenu difficultySelectMenu;
        [SerializeField] private BurragePracticeMenu burragePracticeMenu;
        [SerializeField] private KeyConfigMenu keyConfigMenu;

        // 画面遷移元のメニュー
        private TitleSceneMenu changeFromMenu;
        // 画面遷移先のメニュー
        private TitleSceneMenu changeToMenu;
        // ゲーム開始後にどの弾幕から開始するか
        // 0なら通常プレイ、1～9なら対応した弾幕のPracticeモード
        public int ChapterNum { private get; set; }
        public Difficulty Difficulty { private get; set; }
        // プレイヤーキャラクター
        public PlayerCharacter PlayerCharacter { private get; set; }
        // キャンセルボタンを押して戻った時の遷移先メニューを入れたスタック
        public Stack<BackMenuSetting> backMenuSettingStack;

        public enum TitleSceneMenu
        {
            NoSelected,
            Title,
            CharacterSelect,
            DifficultySelect,
            BurragePractice,
            KeyConfig
        }

        // Use this for initialization
        void Start()
        {
            changeFromMenu = TitleSceneMenu.NoSelected;
            changeToMenu = TitleSceneMenu.NoSelected;

            // 各キャンバスの初期表示状態設定
            titleCanvas.enabled = true;
            characterSelectCanvas.enabled = false;
            difficultySelectCanvas.enabled = false;
            burragePracticeCanvas.enabled = false;
            keyConfigCanvas.enabled = false;

            backMenuSettingStack = new();
        }

        // Update is called once per frame
        void Update()
        {
            // 遷移元のメニューを非表示にする
            switch(changeFromMenu)
            {
                case TitleSceneMenu.Title:
                    titleCanvas.enabled = false;
                    break;
                case TitleSceneMenu.CharacterSelect:
                    characterSelectCanvas.enabled = false;
                    break;
                case TitleSceneMenu.DifficultySelect:
                    difficultySelectCanvas.enabled = false;
                    break;
                case TitleSceneMenu.BurragePractice:
                    burragePracticeCanvas.enabled = false;
                    break;
                case TitleSceneMenu.KeyConfig:
                    keyConfigCanvas.enabled = false;
                    break;
            }

            // 遷移先のメニューを表示状態にする
            switch (changeToMenu)
            {
                case TitleSceneMenu.Title:
                    titleCanvas.enabled = true;
                    titleMenu.StartLock = true;
                    break;
                case TitleSceneMenu.CharacterSelect:
                    characterSelectCanvas.enabled = true;
                    characterSelectMenu.StartLock = true;
                    break;
                case TitleSceneMenu.DifficultySelect:
                    difficultySelectCanvas.enabled = true;
                    difficultySelectMenu.StartLock = true;
                    // 難易度選択画面はNormalをデフォルトで選択状態にする
                    difficultySelectMenu.SetSelectedItemNum(1, 2);
                    break;
                case TitleSceneMenu.BurragePractice:
                    burragePracticeCanvas.enabled = true;
                    burragePracticeMenu.StartLock = true;
                    break;
                case TitleSceneMenu.KeyConfig:
                    keyConfigCanvas.enabled = true;
                    keyConfigMenu.StartLock = true;
                    break;
            }

            // メニュー遷移指示状態をリセットする
            if (changeFromMenu != TitleSceneMenu.NoSelected)
            {
                changeFromMenu = TitleSceneMenu.NoSelected;
                changeToMenu = TitleSceneMenu.NoSelected;
            }
        }

        /// <summary>
        /// メニュー画面の遷移指示
        /// </summary>
        /// <param name="changeFromMenu"></param>
        /// <param name="changeToMenu"></param>
        internal void ChangeMenu(TitleSceneMenu changeFromMenu, TitleSceneMenu changeToMenu)
        {
            this.changeFromMenu = changeFromMenu;
            this.changeToMenu = changeToMenu;
        }

        /// <summary>
        /// ゲームシーンに移行
        /// </summary>
        internal void StartGameScene()
        {
            // ゲームシーンロード後のイベントを登録
            SceneManager.sceneLoaded += GameSceneLoaded;
            // 設定データの初期化+ゲームシーン読み込み
            StartCoroutine(SettingDataLoader.GameSceneLoad());
        }

        /// <summary>
        /// ゲームシーンロード後のイベント処理
        /// </summary>
        private void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // シーン切り替え後のゲームシーン管理スクリプトを取得
            var gameSceneController = GameObject.FindWithTag("GameController").GetComponent<GameSceneControllers.GameSceneController>();

            // キャラクターと難易度とchapterを渡す
            gameSceneController.SetPlayerCharacter(PlayerCharacter);
            gameSceneController.Difficulty = Difficulty;
            gameSceneController.StartChapterNum = ChapterNum;

            // 初期化を実施する
            gameSceneController.GameSceneInitialize();

            // イベントから削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }

        /// <summary>
        /// 前の画面に戻る
        /// </summary>
        internal void BackMenu(TitleSceneMenu changeFromMenu)
        {
            var backMenuSetting = backMenuSettingStack.Pop();
            this.changeFromMenu = changeFromMenu;
            changeToMenu = backMenuSetting.backMenu;
        }
    }
}