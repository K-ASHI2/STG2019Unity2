using UnityEngine;
using Assets.Script.Common;
using static Assets.Script.Menu.TitleSceneMenuChanger;

namespace Assets.Script.Menu
{
    public class DifficultySelectMenu : MenuSelectBase
    {
        [SerializeField] private Canvas difficultySelectCanvas;
        [SerializeField] private TitleSceneMenuChanger titleSceneMenuChanger;
        private const TitleSceneMenu THIS_SCENE_MENU = TitleSceneMenu.DifficultySelect;

        // Update is called once per frame
        private void Update()
        {
            if (difficultySelectCanvas.enabled)
            {
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
                    BaseUpdate();

                    if (Input.GetKey(KeyCode.Z))
                    {
                        // メニュー画面の番号はそのまま難易度に対応しているので、値をセットしておく
                        titleSceneMenuChanger.Difficulty = (Difficulty)selectedItemNumV;

                        // ゲームシーンに移行
                        titleSceneMenuChanger.StartGameScene();
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