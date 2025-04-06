using UnityEngine;
using static Assets.Script.Menu.TitleSceneMenuChanger;

namespace Assets.Script.Menu
{
    public class KeyConfigMenu : MenuSelectBase
    {
        [SerializeField] private Canvas keyConfigCanvas;
        [SerializeField] private TitleSceneMenuChanger titleSceneMenuChanger;

        private const TitleSceneMenu THIS_SCENE_MENU = TitleSceneMenu.KeyConfig;
        private enum KeyConfigMenuItem
        {
            Shot = 1,
            Bomb = 2,
            Slow = 3,
            Pause = 4,
            Reset = 5,
            Quit = 6
        }

        // Update is called once per frame
        private void Update()
        {
            if (keyConfigCanvas.enabled)
            {
                BaseUpdate();

                if (StartLock)
                {
                    // 決定ボタンが離されたらロック解除
                    if (!Input.GetKey(KeyCode.Z))
                    {
                        StartLock = false;
                    }
                }

                if (!StartLock)
                {
                    if (Input.GetKey(KeyCode.Z))
                    {
                        // メニュー画面の選択項目によって画面遷移・ゲーム終了
                        switch ((KeyConfigMenuItem)selectedItemNumV)
                        {
                            case KeyConfigMenuItem.Shot:

                                break;
                            case KeyConfigMenuItem.Bomb:
                                break;
                            case KeyConfigMenuItem.Slow:
                                break;
                            case KeyConfigMenuItem.Pause:
                                break;
                            case KeyConfigMenuItem.Reset:
                                break;
                            case KeyConfigMenuItem.Quit:
                                // タイトル画面に戻る
                                titleSceneMenuChanger.BackMenu(THIS_SCENE_MENU);
                                break;
                            default:
                                // 通常失敗しないはずだが、一応エラー出しておく
                                Debug.LogWarning($"Invalid KeyConfigMenuItem error");
                                break;
                        }
                    }
                    // キーコンフィグ中はキャンセルボタン押しても特に処理はしない
                }
            }
        }
    }
}