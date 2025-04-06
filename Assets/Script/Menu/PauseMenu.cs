using Assets.Script.GameSceneControllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.Menu
{
    public class PauseMenu : MenuSelectBase
    {
        [SerializeField] private GameSceneController gameSceneController;

        private enum PauseMenuItem
        {
            Countinue = 1,
            Restart = 2,
            Title = 3
        }

        // Update is called once per frame
        private void Update()
        {
            // ポーズボタン押されてポーズ突入後はそのボタンが離されるまで画面遷移処理を行わない
            if (StartLock)
            {
                // キー入力が離されたらロック解除
                if (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Z))
                {
                    StartLock = false;
                }
            }

            if (!StartLock)
            {
                BaseUpdate();
                if (Input.GetKey(KeyCode.Z))
                {
                    // メニュー画面の選択項目によって画面遷移・ゲーム終了
                    switch ((PauseMenuItem)selectedItemNumV)
                    {
                        case PauseMenuItem.Countinue:
                            // ゲームプレイ画面に戻る
                            gameSceneController.EndPause();
                            break;
                        case PauseMenuItem.Restart:
                            // 最初からやりなおす
                            gameSceneController.EndPause();
                            gameSceneController.GameSceneInitialize();
                            break;
                        case PauseMenuItem.Title:
                            // ポーズを終了させてから、タイトル画面に戻る
                            gameSceneController.EndPause();
                            // ゲームシーンロード後のイベントを登録
                            SceneManager.sceneLoaded += TitleSceneLoaded;
                            SceneManager.LoadScene("Title");
                            break;
                        default: break;
                    }

                }
                else if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Escape))
                {
                    // ゲームプレイ画面に戻る
                    gameSceneController.EndPause();
                }
            }
        }

        /// <summary>
        /// ポーズ画面が表示されるタイミングのイベント
        /// </summary>
        public new void OnEnable()
        {
            // Startイベントの前にも一瞬Enableになってしまい呼ばれるので、そのタイミングではスキップ
            if (defaultColorList != null)
            {
                // 選択項目の色をデフォルト状態に戻す
                SetItemDefaultColor();
                // 選択項目を1番目の項目に戻す
                selectedItemNumV = 1;
                ItemSelectionChanged();
                StartLock = true;
            }
            // 共通処理のOnEnableも呼ぶ
            base.OnEnable();
        }


        /// <summary>
        /// ゲームシーンロード後のイベント処理
        /// </summary>
        private void TitleSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // シーン切り替え後のゲームシーン管理スクリプトを取得
            var titleMenu = GameObject.FindWithTag("TitleMenu").GetComponent<TitleMenu>();

            // 連打防止
            titleMenu.StartLock = true;

            // イベントから削除
            SceneManager.sceneLoaded -= TitleSceneLoaded;
        }
    }
}