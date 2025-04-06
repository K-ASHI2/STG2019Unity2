using UnityEngine;
using Assets.Script.Common;
using static Assets.Script.Menu.TitleSceneMenuChanger;

namespace Assets.Script.Menu
{
    public class CharacterSelectMenu : MenuSelectBase
    {
        [SerializeField] private Canvas characterSelectMenuCanvas;
        [SerializeField] private TitleSceneMenuChanger titleSceneMenuChanger;
        private const TitleSceneMenu THIS_SCENE_MENU = TitleSceneMenu.CharacterSelect;

        // Update is called once per frame
        private void Update()
        {
            if (characterSelectMenuCanvas.enabled)
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

                        // メニュー画面の番号からキャラクター番号を求めて与える
                        if (selectedItemNumV == 1)
                        {
                            if (selectedItemNumH == 1)
                            {
                                titleSceneMenuChanger.PlayerCharacter = PlayerCharacter.LunaMagician_A;
                            }
                            else
                            {
                                titleSceneMenuChanger.PlayerCharacter = PlayerCharacter.LunaMagician_B;
                            }
                        }
                        else
                        {
                            if (selectedItemNumH == 1)
                            {
                                titleSceneMenuChanger.PlayerCharacter = PlayerCharacter.SunMagician_A;
                            }
                            else
                            {
                                titleSceneMenuChanger.PlayerCharacter = PlayerCharacter.SunMagician_B;
                            }
                        }

                        // 難易度選択に移行
                        titleSceneMenuChanger.ChangeMenu(THIS_SCENE_MENU, TitleSceneMenu.DifficultySelect);
                    }
                    else if (Input.GetKey(KeyCode.X))
                    {
                        // 直前の画面に戻る
                        titleSceneMenuChanger.BackMenu(THIS_SCENE_MENU);
                    }
                }
            }
        }
    }
}