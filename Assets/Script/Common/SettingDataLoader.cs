using Assets.Script.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Assets.Script.Common
{
    public static class SettingDataLoader
    {
        private static AsyncOperationHandle<PlayerBulletSetting> asyncLoadSettingPB;
        private static AsyncOperationHandle<PlayerLaserSetting> asyncLoadSettingPL;

        // 初期化完了したかどうか
        public static bool IsInitialized { get; private set; }

        // 弾とレーザーのデータ
        public static PlayerBulletSetting PlayerBulletSetting { get; private set; }
        public static PlayerLaserSetting PlayerLaserSetting { get; private set; }

        /// <summary>
        /// ロード処理開始
        /// どのシーンから起動しても最初に開始される
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void LoadingStart()
        {
#if UNITY_EDITOR
            PlayerBulletSetting = Addressables.LoadAssetAsync<PlayerBulletSetting>("Assets/PlayerData/PlayerBulletSetting.asset").WaitForCompletion(); ;
            PlayerLaserSetting = Addressables.LoadAssetAsync<PlayerLaserSetting>("Assets/PlayerData/PlayerLaserSetting.asset").WaitForCompletion(); ;
            // Editorから動かす場合は、ゲームシーンから起動する場合もあるので初期化まで実施
            //          SettingDataInitializeForEditor();

            PlayerBulletSetting.Initialize();
            PlayerLaserSetting.Initialize();
            // 初期化済みにする
            IsInitialized = true;
#else
            // 単独起動の場合はゲームシーンに移動するタイミングで初期化実施(タイトル画面からゲームシーンに移動するまでの間でロード処理実施できる)
            asyncLoadSettingPB = Addressables.LoadAssetAsync<PlayerBulletSetting>("Assets/PlayerData/PlayerBulletSetting.asset");
            asyncLoadSettingPL = Addressables.LoadAssetAsync<PlayerLaserSetting>("Assets/PlayerData/PlayerLaserSetting.asset");
            // 未初期化状態にする
            IsInitialized = false;
#endif
        }

        /// <summary>
        /// 読み込んだ設定の初期化+ゲームシーン読み込み(Editor外からの起動時用)
        /// </summary>
        public static IEnumerator GameSceneLoad()
        {
            // 未初期化時のみ実施
            if (!IsInitialized)
            {
                // ScriptableObjectのロードが終わるまで待機してから初期化実施
                yield return asyncLoadSettingPB;
                if (asyncLoadSettingPB.Status == AsyncOperationStatus.Succeeded)
                {
                    PlayerBulletSetting = asyncLoadSettingPB.Result;
                    PlayerBulletSetting.Initialize();
                }
                else
                {
                    // 通常失敗しないはずだが、一応エラー出しておく
                    Debug.LogWarning($"playerBulletSetting loading failed");
                }

                // レーザー設定も同様
                yield return asyncLoadSettingPL;
                if (asyncLoadSettingPL.Status == AsyncOperationStatus.Succeeded)
                {
                    PlayerLaserSetting = asyncLoadSettingPL.Result;
                    PlayerLaserSetting.Initialize();
                }
                else
                {
                    Debug.LogWarning($"playerLaserSetting loading failed");
                }

                IsInitialized = true;
            }

            // ロード完了状態になったらシーン移動
            SceneManager.LoadScene("Game");
        }
    }
}