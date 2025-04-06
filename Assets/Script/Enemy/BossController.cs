using Assets.Script.Common;
using Assets.Script.GameSceneControllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Enemy
{
    /// <summary>
    /// 各チャプター固有のパラメータ設定
    /// </summary>
    [Serializable]
    public class BossChapterSetting
    {
        // ボスのHP
        public float bossHp;
        // 制限時間(画面表示上の時間、実際には約1秒多い)
        public float maxTime;
    }

    public class BossController : MonoBehaviour
    {
        // ボスのHP(チャプター番号と配列の添え字を揃えるため、チャプター0は使わない)
        [SerializeField] private List<BossChapterSetting> chapterSettings;
        // ゲームシーン全体のコントローラ
        [SerializeField] private GameSceneController gameSceneController;
        // ボスの弾幕生成クラス
        [SerializeField] private BossBurrageCreater bossBurrageCreater;
        // チャプター間のボスの行動のインターバルの初期値
        private const float CHAPTER_INTERVAL = 0.67f;

        // 無敵時間のカウント(0より大きいなら無敵)
        private float invincible;
        // ボス出現時のカウント
        public float AppearCount { private get; set; }
        // HP
        public float Hp { get; private set; }
        // 最大HP
        public float MaxHp { get; private set; }
        // トータルHP
        public float TotalHp { get; private set; }
        // 最大トータルHP
        public float MaxTotalHp { get; private set; }
        // 制限時間
        public float TimeCount { get; private set; }
        // 制限時間減少中かどうかのフラグ
        public bool IsTimeCounting { get; private set; }
        // ボスの行動モード
        public int BossMode { private get; set; }
        // チャプター間のインターバルのカウント
        private float chapterIntervalCount;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            IsTimeCounting = false;

            // ボスの出現処理
            if (AppearCount > 0)
            {
                // 画面外から下にスライドさせる
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - 1.55f * Time.deltaTime);
                // カウント減少
                AppearCount -= Time.deltaTime;

                // インターバル経過後にチャプターのボスの行動を読み込む
                if (AppearCount <= 0)
                {
                    ChangeBurrageMode(BossMode, gameSceneController.StartChapterNum != 0);
                }
            }
            // チャプター移行処理
            else if (chapterIntervalCount > 0)
            {
                // 敵の弾を消す処理を行い続けるをDXライブラリ版では入れていたが、なくてもよさそう
                // カウント減少
                chapterIntervalCount -= Time.deltaTime;

                // インターバル経過後にチャプターを一つ進める
                if (chapterIntervalCount <= 0)
                {
                    ChangeBurrageMode(BossMode + 1, false);
                    gameSceneController.ChapterNum++;
                }
            }
            else
            {
                // 無敵時間のカウント減少(ボス出現中とチャプター移行中は除く、現状は無敵自体必要なタイミングないかも)
                if (invincible > 0)
                {
                    invincible -= Time.deltaTime;
                }
                // 制限時間減少(ボス出現中とチャプター移行中は除く)
                if (TimeCount > 0 && AppearCount <= 0 && chapterIntervalCount <= 0)
                {
                    TimeCount -= Time.deltaTime;
                    IsTimeCounting = true;

                    // 制限時間超過したらボスのHPを0にする
                    if(TimeCount <= 0)
                    {
                        // HP分ダメージを与える
                        // TODO 時間切れの時はアイテム出さないようにする(フラグ増やす？)
                        Dameged(Hp);
                    }
                }
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Difficulty difficulty)
        {
            bossBurrageCreater.Initialize(difficulty);
            invincible = 0;
            // 敵の位置を画面中央の画面外にする
            transform.localPosition = new Vector2(0, 3.3f);
            AppearCount = 0;
            Hp = 0;
            MaxHp = 0;
            TotalHp = 0;
            MaxTotalHp = 0;
            chapterIntervalCount = 0;
            TimeCount = 0;
            IsTimeCounting = false;
        }

        /// <summary>
        /// 指定したチャプターに応じた弾幕のステータスに切り替える
        /// </summary>
        public void ChangeBurrageMode(int chapter, bool isBurragePractice)
        {
            // チャプター設定数を超過していたら終了
            if(chapterSettings.Count <= chapter)
            {
                return;
            }

            // ボスのモードはチャプター番号と同一にする(イベントの関係上、チャプターの管理とボスの状態移行が少しずれることはある)
            BossMode = chapter;

            // チャプターにあったHPと制限時間を設定する
            MaxHp = chapterSettings[chapter].bossHp;
            Hp = MaxHp;
            // 制限時間は約1秒足した状態にする(0秒になった1秒後にタイムアップなので、1秒ぐらい多くしないと画面上の表示開始時間と一致しない)
            TimeCount = chapterSettings[chapter].maxTime + 0.999f;

            // BurragePraciticeか最初の弾幕ならトータルのHP量を設定する
            if (isBurragePractice)
            {
                MaxTotalHp = MaxHp;
                TotalHp = MaxTotalHp;
            }
            else if (chapter == 1)
            {
                // 全チャプターのHPを加算する
                MaxTotalHp = 0;
                foreach(var chapterSetting in chapterSettings)
                {
                    MaxTotalHp += chapterSetting.bossHp;
                }
                TotalHp = MaxTotalHp;
            }

            // ボスの弾生成
            bossBurrageCreater.CreateBurrage(BossMode);
        }

        /// <summary>
        /// 被ダメージ処理
        /// </summary>
        public void Dameged(float damage)
        {
            // 無敵以外かつチャプター移行中以外の時にダメージを与える
            if(invincible <= 0 && AppearCount <= 0 && chapterIntervalCount <= 0)
            {
                Hp -= damage;
                TotalHp -= damage;

                // トータルHPが0になったらクリア
                if (TotalHp <= 0)
                {
                    gameSceneController.StartClearWindow(true);
                }

                // HPが0になったらチャプター変更
                if (Hp <= 0)
                {
                    // オーバーキルした分トータルHPを戻す
                    TotalHp -= Hp;

                    // 弾生成処理停止+弾消滅
                    bossBurrageCreater.StopVarrage();

                    // インターバルを設定
                    chapterIntervalCount = CHAPTER_INTERVAL;
                }
            }
        }
    }
}