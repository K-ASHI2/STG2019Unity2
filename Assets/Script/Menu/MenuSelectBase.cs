using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Script.Menu
{
    /// <summary>
    /// メニュー画面の横方向の選択項目テキストオブジェクトを入れるクラス
    /// </summary>
    [System.Serializable]
    public class HorizontalMenuTextItems
    {
        [SerializeField] public List<TextMeshProUGUI> MenuTextItemsH;
    }

    public abstract class MenuSelectBase : MonoBehaviour
    {
        // 選択可能なテキストオブジェクト
        [SerializeField] private List<HorizontalMenuTextItems> menuTextItems;
        // 選択項目変更時の待ち時間
        private const float WAIT_TIME = 0.25f;
        // 選択中の項目の縦方向の番号
        protected int selectedItemNumV = 1;
        // 選択中の項目の横方向の番号
        protected int selectedItemNumH = 1;
        // メニュー画面遷移後、特定のボタンが離されるまでボタン入力を無効化する
        // OnEnableでは上手くイベントが発生しないのでシーンの状態変更処理の中でセットする
        public bool StartLock { protected get; set; } = true;
        // UnityEditorで設定した色
        protected List<List<Color>> defaultColorList;
        // 直前に選択されていた縦方向の項目
        protected int beforeSelectedItemNumV = 1;
        // 直前に選択されていた横方向の項目
        protected int beforeSelectedItemNumH = 1;
        // Wait中かどうか
        public bool IsWait { protected get; set; } = false;

        // Use this for initialization
        protected void Awake()
        {
            // UnityEditorで設定した初期状態の色を入れておく
            defaultColorList = new List<List<Color>>();
            foreach (var menuTextItems in menuTextItems)
            {
                var defaultColorListH = new List<Color>();
                foreach (var horizontalMenuTextItem in menuTextItems.MenuTextItemsH)
                {
                    defaultColorListH.Add(horizontalMenuTextItem.color);
                }
                defaultColorList.Add(defaultColorListH);
            }

            // 先頭行の先頭列の項目を仮の初期選択状態とする
            menuTextItems[0].MenuTextItemsH[0].color = Color.red;
        }

        /// <summary>
        /// フレーム毎の共通更新処理(Wait時間更新、上下移動)
        /// 継承先のメソッドにも必ずUpdate処理を書くので別途呼び出す
        /// </summary>
        protected void BaseUpdate()
        {
            if (!IsWait && !StartLock)
            {
                // 直前に選択されていた項目を更新
                beforeSelectedItemNumV = selectedItemNumV;
                beforeSelectedItemNumH = selectedItemNumH;

                // 上下キーが押されたら選択中の項目を変更
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (selectedItemNumV > 1)
                    {
                        selectedItemNumV--;
                    }
                    // 一番上の項目選択中なら一番下の項目を選択状態にする
                    else
                    {
                        selectedItemNumV = menuTextItems.Count;
                    }
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (selectedItemNumV < menuTextItems.Count)
                    {
                        selectedItemNumV++;
                    }
                    // 一番下の項目選択中なら一番上の項目を選択状態にする
                    else
                    {
                        selectedItemNumV = 1;
                    }
                }

                // 横方向の選択項目が2個以上ある状態で、左右キーが押されたら選択中の項目を変更
                if (menuTextItems[selectedItemNumV - 1].MenuTextItemsH.Count > 1)
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        if (selectedItemNumH > 1)
                        {
                            selectedItemNumH--;
                        }
                        // 一番左の項目選択中なら一番右の項目を選択状態にする
                        else
                        {
                            selectedItemNumH = menuTextItems[selectedItemNumV - 1].MenuTextItemsH.Count;
                        }
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        if (selectedItemNumH < menuTextItems[selectedItemNumV - 1].MenuTextItemsH.Count)
                        {
                            selectedItemNumH++;
                        }
                        // 一番右の項目選択中なら一番左の項目を選択状態にする
                        else
                        {
                            selectedItemNumH = 1;
                        }
                    }
                }

                // 選択項目が変更された場合、色を変えて一定時間上下操作を無効化する
                if (beforeSelectedItemNumV != selectedItemNumV || beforeSelectedItemNumH != selectedItemNumH)
                {
                    ItemSelectionChanged();
                    // 連続でカーソルが移動しないようにWaitを入れる
                    StartCoroutine("CoroutineWait");
                }
            }

            // 決定ボタンやキャンセルボタンの処理は継承先のクラスで実装する
        }

        /// <summary>
        /// メニューの選択項目変更時の処理
        /// </summary>
        protected void ItemSelectionChanged()
        {
            // 列数が多い行で上下を押した場合、移動先の行の右側には列が存在しない場合があるため、この場合は右端の列を選んだことにする
            if(selectedItemNumH > menuTextItems[beforeSelectedItemNumV - 1].MenuTextItemsH.Count)
            {
                selectedItemNumH = menuTextItems[beforeSelectedItemNumV - 1].MenuTextItemsH.Count;
            }

            menuTextItems[beforeSelectedItemNumV - 1].MenuTextItemsH[beforeSelectedItemNumH - 1].color = defaultColorList[beforeSelectedItemNumV - 1][beforeSelectedItemNumH - 1];
            menuTextItems[selectedItemNumV - 1].MenuTextItemsH[selectedItemNumH - 1].color = Color.red;
        }

        /// <summary>
        /// メニュー選択項目を設定する
        /// </summary>
        public void SetSelectedItemNum(int selectedItemNumH, int selectedItemNumV)
        {
            this.selectedItemNumH = selectedItemNumH;
            this.selectedItemNumV = selectedItemNumV;
            ItemSelectionChanged();
        }

        /// <summary>
        /// メニューのテキストをデフォルトの色に戻す
        /// </summary>
        protected void SetItemDefaultColor()
        {
            for (int i = 0; i < menuTextItems.Count; i++)
            {
                for (int j = 0; j < menuTextItems[i].MenuTextItemsH.Count; j++)
                {
                    menuTextItems[i].MenuTextItemsH[j].color = defaultColorList[i][j];
                }
            }
        }

        /// <summary>
        /// コルーチンを使用したWait処理
        /// ポーズ画面ではTimeScaleを0にしているので、DeltaTimeやWaitForSecondsでは時間が進行しなくなってしまう
        /// </summary>
        /// <returns></returns>
        public IEnumerator CoroutineWait()
        {
            // Wait時間の待機中だけWait状態にする
            IsWait = true;
            yield return new WaitForSecondsRealtime(WAIT_TIME);
            IsWait = false;
        }


        public void OnEnable()
        {
            // SetActiveでfalseになるとコルーチンが中断されfalseにならないため、画面が表示されるタイミングで戻す
            IsWait = false;
        }

        /// <summary>
        /// 選択中の項目の指定(画面切り替え時用)
        /// </summary>
        public void SetSelectedItemNum(int selectedItemNum)
        {
            this.selectedItemNumV = selectedItemNum;
            ItemSelectionChanged();
        }
    }
}