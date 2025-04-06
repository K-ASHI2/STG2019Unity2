using Assets.Script.GameSceneControllers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.Menu
{
    public class ClearWindowController : MonoBehaviour
    {
        [SerializeField] private GameSceneController gameSceneController;
        // Clear文字テキストのオブジェクト
        [SerializeField] private GameObject ClearTextObject;
        // GameOver文字テキストのオブジェクト
        [SerializeField] private GameObject GameOverTextObject;

        // クリア画面表示後の画面遷移可能になるまでの待ち時間
        private const float WAIT_TIME = 1.0f;
        // 画面遷移後、特定のボタンが離されるまでボタン入力を無効化する
        public bool StartLock { private get; set; } = true;
        // Wait中かどうか
        private bool isWait;

        // Update is called once per frame
        private void Update()
        {
            // 画面遷移後は特定のボタンが離されるまで画面遷移処理を行わない
            if (StartLock)
            {
                // ショット・ボム、十字キーボタンが離されたらロック解除
                if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X))
                {
                    StartLock = false;
                }
            }

            if (!StartLock &&  !isWait)
            {
                //選択項目がないのでBaseUpdateは不要
                if (Input.anyKey)
                {
                    // 画面の設定を元に戻してからタイトル画面に戻る
                    gameSceneController.EndClearWindow();
                    // ゲームシーンロード後のイベントを登録
                    SceneManager.sceneLoaded += TitleSceneLoaded;
                    SceneManager.LoadScene("Title");
                }
            }
        }

        /// <summary>
        /// クリア画面が表示されるタイミングのイベント
        /// </summary>
        public void OnEnable()
        {
            StartLock = true;
            // タイトル画面にすぐに遷移してしまわないようにコルーチンでWaitを入れる
            StartCoroutine("CoroutineWait");
        }

        /// <summary>
        /// コルーチンを使用したWait処理
        /// クリア画面ではTimeScaleを0にしているので、DeltaTimeやWaitForSecondsでは時間が進行しなくなってしまう
        /// </summary>
        /// <returns></returns>
        public IEnumerator CoroutineWait()
        {
            // Wait時間の待機中だけWait状態にする
            isWait = true;
            yield return new WaitForSecondsRealtime(WAIT_TIME);
            isWait = false;
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

        /// <summary>
        /// ゲームクリア/ゲームオーバー画面のタイトル文字の設定
        /// </summary>
        public void SetTitleText(bool isClear)
        {
            if (isClear)
            {
                ClearTextObject.SetActive(true);
                GameOverTextObject.SetActive(false);
            }
            else
            {
                ClearTextObject.SetActive(false);
                GameOverTextObject.SetActive(true);
            }
        }
    }
}