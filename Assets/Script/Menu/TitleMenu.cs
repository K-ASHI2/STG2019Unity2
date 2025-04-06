using Assets.Script.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Script.Menu.TitleSceneMenuChanger;

namespace Assets.Script.Menu
{
    public class TitleMenu : MenuSelectBase
    {
        [SerializeField] private Canvas titleCanvas;
        [SerializeField] private TitleSceneMenuChanger titleSceneMenuChanger;
        private const TitleSceneMenu THIS_SCENE_MENU = TitleSceneMenu.Title;
        public bool isFirstFrame;

        private enum TitleMenuItem
        {
            GameStart = 1,
            BurragePractice = 2,
            //KeyConfig = 3,
            Exit = 3
        }

        private void Start()
        {
            isFirstFrame = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (titleCanvas.enabled)
            {
                if (StartLock)
                {
                    // 最初のフレーム以外で決定ボタンとキャンセルボタンが離されたらロック解除
                    // 最初のフレームはボタンの入力が取得できず必ずfalseになってしまう
                    if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X) && !isFirstFrame)
                    {
                        StartLock = false;
                    }
                }

                if (isFirstFrame)
                {
                    isFirstFrame = false;
                }

                if (!StartLock)
                {
                    BaseUpdate();

                    if (Input.GetKey(KeyCode.Z))
                    {
                        // キャンセルボタンの戻る先の設定を追加
                        var backMenuSetting = new BackMenuSetting()
                        {
                            backMenu = THIS_SCENE_MENU,
                            selectedItemNumH = selectedItemNumH,
                            selectedItemNumV = selectedItemNumV,
                        };
                        titleSceneMenuChanger.backMenuSettingStack.Push(backMenuSetting);

                        // メニュー画面の選択項目によって画面遷移・ゲーム終了
                        switch ((TitleMenuItem)selectedItemNumV)
                        {
                            case TitleMenuItem.GameStart:
                                titleSceneMenuChanger.ChangeMenu(THIS_SCENE_MENU, TitleSceneMenu.CharacterSelect);
                                break;
                            case TitleMenuItem.BurragePractice:
                                titleSceneMenuChanger.ChangeMenu(THIS_SCENE_MENU, TitleSceneMenu.BurragePractice);
                                break;
                            //case TitleMenuItem.KeyConfig:
                            //    titleSceneMenuChanger.ChangeMenu(THIS_SCENE_MENU, TitleSceneMenu.KeyConfig);
                            //    break;
                            case TitleMenuItem.Exit:
                                // ゲーム終了
                                EndGame();
                                break;
                            default:
                                // 通常失敗しないはずだが、一応エラー出しておく
                                Debug.LogWarning($"Invalid TitleMenuItem error");
                                break;
                        }
                    }
                    else if (Input.GetKey(KeyCode.X))
                    {
                        // メニュー画面のEXITを選択状態にする
                        selectedItemNumV = (int)TitleMenuItem.Exit;
                        ItemSelectionChanged();
                    }
                }
            }
        }

        /// <summary>
        /// ゲーム終了
        /// </summary>
        private void EndGame()
        {
            // Editorから起動時にも終了できるようにしておく
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}