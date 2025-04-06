using UnityEngine;
using static Assets.Script.Menu.TitleSceneMenuChanger;

namespace Assets.Script.Menu
{
    public class BurragePracticeMenu : MenuSelectBase
    {
        [SerializeField] private Canvas burragePracticeCanvas;
        [SerializeField] private TitleSceneMenuChanger titleSceneMenuChanger;
        private const TitleSceneMenu THIS_SCENE_MENU = TitleSceneMenu.BurragePractice;

        // Update is called once per frame
        private void Update()
        {
            if (burragePracticeCanvas.enabled)
            {
                if (StartLock)
                {
                    // 決定ボタンとキャンセルボタンが離されたらロック解除
                    if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X))
                    {
                        StartLock = false;
                    }
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

                        // メニュー画面の番号はそのまま弾幕番号になっているので、値をセットしておく
                        titleSceneMenuChanger.ChapterNum = selectedItemNumV;
                        // キャラ選択に移行
                        titleSceneMenuChanger.ChangeMenu(THIS_SCENE_MENU, TitleSceneMenu.CharacterSelect);
                    }
                    else if (Input.GetKey(KeyCode.X))
                    {
                        // タイトル画面に戻る
                        titleSceneMenuChanger.BackMenu(THIS_SCENE_MENU);
                    }
                }
            }
        }
    }
}