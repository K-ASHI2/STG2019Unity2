using Assets.Script.Common;
using Assets.Script.Enemy;
using Assets.Script.Effect;
using Assets.Script.GameSceneControllers;
using UnityEngine;

namespace Assets.Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        // 2Dスプライトの物理演算
        [SerializeField] private Rigidbody2D rb;
        // 自機の弾のオブジェクトプール
        [SerializeField] private PlayerBulletPools playerBulletPools;
        // 敵の弾のオブジェクトプール
        [SerializeField] private EnemyBulletPools enemyBulletPools;
        // エフェクトのオブジェクトプール
        [SerializeField] private EffectAnimationPools effectAnimationPools;
        // 自機の判定の円のスプライトレンダラー
        [SerializeField] private SpriteRenderer judgeCircleSpriteRenderer;
        // 自機の判定の円のコライダー
        [SerializeField] private CircleCollider2D judgeCircleCollider;
        // 自機のグレイズ円のコライダー
        [SerializeField] private CircleCollider2D grazeCircleCollider;
        // ゲームシーンの状態操作クラス
        [SerializeField] private GameSceneController gameSceneController;
        // ボスの制御クラス
        [SerializeField] private BossController bossController;
        // Sun Magicianのスプライト
        [SerializeField] private Sprite PlayerSpriteSM;
        [SerializeField] private Sprite judgeCircleSpriteSM;

        #region 移動速度の定義
        // DXライブラリ版と速度を統一する
        // verocityは1秒=60フレームで1Unit=100pixel動くが、DXライブラリでは1フレームあたりのpixel数で動かしている
        // DXライブラリ版ではバグで1フレームに2回更新してしまっているので注意する
        // 遅いキャラの高速移動の速度(6.75*60/100=4.05)
        private const float PMOVE_HIGH1 = 4.05f;
        // 速いキャラの高速移動の速度(7.5*60/100=4.5)
        private const float PMOVE_HIGH2 = 4.5f;
        // 低速移動の速度(3.0*60/100=1.8)
        private const float PMOVE_SLOW = 1.8f;
        #endregion

        // グレイズ時の属性ゲージ上昇量
        private const int GRAZE_UP_GAUGE = 30;
        // 弾消し時の属性ゲージ上昇量」
        private const int VANISH_UP_GAUGE_E = 60;
        private const int VANISH_UP_GAUGE_N = 30;
        private const int VANISH_UP_GAUGE_H = 10;
        // 属性ゲージの最大値
        private const int MAX_ELEMENT = 960;
        // 被弾後の無敵時間
        private const float DAMEGED_INVINCIBLE_TIME = 3.75f;

        // 弾生成クラス
        private PlayerBulletGenerater bulletGenerater;
        // 自機の種類
        public PlayerCharacter PlayerCharacter { private get; set; } = PlayerCharacter.LunaMagician_A;
        // 自機の種類
        public int GrazeCount { get; internal set; }
        // 氷属性ゲージ
        public float IceGauge { get; internal set; }
        // 雷属性ゲージ
        public float ThunderGauge { get; internal set; }
        // 炎属性ゲージ
        public float FireGauge { get; internal set; }
        // 残機
        public int Life { get; private set; }
        // ボムの残数
        public int Bomb { get; private set; }
        // Renderer(被弾時の透過処理に使う)
        private SpriteRenderer spriteRenderer;

        // ScriptableObjectはstaticで保持しておく
        // 自機の弾の設定
        private static PlayerBulletSetting playerBulletSetting;
        // 自機のレーザーの設定
        private static PlayerLaserSetting playerLaserSetting;
        // Luna Magicianのスプライト(デフォルトのスプライト)
        private Sprite PlayerSpriteLM;
        private Sprite judgeCircleSpriteLM;
        // ボムの初期値
        private int firstBomb;
        // ボム使用時間のインターバル
        private float bombIntervalCount;
        // 無敵時間のカウント(0より大きいなら無敵)
        private float invincibleCount;
        // 弾削除時のゲージ上昇量
        private int vanishedUpGauge = 0;
        // デフォルトの自機の位置
        private Vector2 defaultPosition;

        private void Awake()
        {
            // プレイヤーのスプライトを取得しておく
            spriteRenderer = GetComponent<SpriteRenderer>();

            // デフォルトのスプライトをLuna Magicianのスプライトとして保持しておく
            PlayerSpriteLM = spriteRenderer.sprite;
            judgeCircleSpriteLM = judgeCircleSpriteRenderer.sprite;

            // プレイヤーのデフォルト位置を覚えておき、リトライ時にも使用できるようにする
            defaultPosition = transform.localPosition;
        }

        // Start is called before the first frame update
        void Start()
        {
            // 初期化関連の処理はGameSceneControllerから呼び出す
        }

        // Update is called once per frame
        void Update()
        {
            // ポーズ中以外の時更新処理を行う
            if (!gameSceneController.IsPauseMode)
            {
                // プレイヤーの移動
                MovePlayer();

                // 無敵時間減少
                if(invincibleCount > 0)
                {
                    invincibleCount -= Time.deltaTime;
                    if(invincibleCount <= 0)
                    {
                        invincibleCount = 0;
                        // 遅延した場合に半透明のまま無敵時間終了する可能性があるので点滅描画を元に戻す
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                    }

                    // 無敵時間の点滅描写
                    // 無敵時間終了直前以外なら1/12秒毎に点滅
                    if (invincibleCount > 0.33f && (int)(invincibleCount * 12) % 2 == 1)
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.25f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                    }
                }

                // 各弾源の更新(ショット生成やインターバルの更新)
                bulletGenerater.GeneraterUpDate();

                // 属性ゲージ関連の更新処理
                ElementGaugeUpDate();
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(bool isPractice, Difficulty difficulty)
        {
            // 自機の種類に合わせて自機の画像と判定の大きさを設定
            if(PlayerCharacter == PlayerCharacter.LunaMagician_A || PlayerCharacter == PlayerCharacter.LunaMagician_B)
            {
                spriteRenderer.sprite = PlayerSpriteLM;
                judgeCircleSpriteRenderer.sprite = judgeCircleSpriteLM;
                judgeCircleCollider.radius = 0.035f;
                grazeCircleCollider.radius = 0.435f;
            }
            else
            {
                spriteRenderer.sprite = PlayerSpriteSM;
                judgeCircleSpriteRenderer.sprite = judgeCircleSpriteSM;
                judgeCircleCollider.radius = 0.040f;
                grazeCircleCollider.radius = 0.440f;
            }

            // 弾生成関係クラスの初期化
            playerBulletPools.Initialize();
            playerBulletSetting = SettingDataLoader.PlayerBulletSetting;
            playerLaserSetting = SettingDataLoader.PlayerLaserSetting;
            bulletGenerater = new PlayerBulletGenerater(playerBulletPools, transform, PlayerCharacter, playerBulletSetting, playerLaserSetting);

            // プレイヤーの位置を初期位置に戻す
            transform.localPosition = defaultPosition;

            // 残機とボムを初期値に設定
            if (isPractice)
            {
                Life = 0;
                firstBomb = 0;
                Bomb = 0;
            }
            else
            {
                // TODO 弾幕数増やしたら残機を増やす
                Life = 2;
                switch(PlayerCharacter)
                {
                    case PlayerCharacter.LunaMagician_A:
                        firstBomb = 3;
                        break;
                    case PlayerCharacter.LunaMagician_B:
                        firstBomb = 4;
                        break;
                    case PlayerCharacter.SunMagician_A:
                        firstBomb = 2;
                        break;
                    case PlayerCharacter.SunMagician_B:
                        firstBomb = 5;
                        break;
                }
                Bomb = firstBomb;
            }

            // 弾消滅時のゲージ上昇量を難易度に応じた値に設定する
            switch(difficulty)
            {
                case Difficulty.Easy:
                    vanishedUpGauge = VANISH_UP_GAUGE_E;
                    break;
                case Difficulty.Normal:
                    vanishedUpGauge = VANISH_UP_GAUGE_N;
                    break;
                case Difficulty.Hard:
                    vanishedUpGauge = VANISH_UP_GAUGE_H;
                    break;
            }

            GrazeCount = 0;
            IceGauge = 0;
            ThunderGauge = 0;
            FireGauge = 0;
            invincibleCount = 0;
            bombIntervalCount = 0;

            effectAnimationPools.Initialize();
        }

        /// <summary>
        /// プレイヤーを動かす
        /// </summary>
        private void MovePlayer()
        {
            // 左なら-1、右なら1、入力なければ0
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            // 移動する向きを求める
            // 斜めに入力した場合はXYそれぞれルート2の速度になる
            Vector2 direction = new Vector2(inputX, inputY).normalized;

            // プレイヤーの種類と低速ボタンの押下有無によって速度の倍率を変える
            float speedRate;
            // 左のShiftキー押している間なら低速移動
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speedRate = PMOVE_SLOW;

                // 低速時は判定の円を表示する
                judgeCircleSpriteRenderer.enabled = true;
            }
            // 高速移動
            else
            {
                if (PlayerCharacter == PlayerCharacter.LunaMagician_A || PlayerCharacter == PlayerCharacter.LunaMagician_B)
                {
                    speedRate = PMOVE_HIGH1;
                }
                else
                {
                    speedRate = PMOVE_HIGH2;
                }

                // 高速時は判定の円を消す
                judgeCircleSpriteRenderer.enabled = false;
            }

            // 移動速度を計算し、移動させる
            // 物理演算するのでvelocityが一番良さそう
            rb.velocity = direction * speedRate;

            // 画面外に出ないようにする
            var clampedX = Mathf.Clamp(transform.localPosition.x, PlayArea.Left + judgeCircleCollider.radius * 2, PlayArea.Right - judgeCircleCollider.radius * 2);
            var clampedY = Mathf.Clamp(transform.localPosition.y, PlayArea.Lower + judgeCircleCollider.radius, PlayArea.Upper - judgeCircleCollider.radius * 2);
            transform.localPosition = new Vector3(clampedX, clampedY);

            // ボム
            bombIntervalCount -= Time.deltaTime;
            if (Input.GetKey(KeyCode.X) && bombIntervalCount < 0 && Bomb > 0)
            {
                // エフェクト生成
                var effectAnimation = effectAnimationPools.GetBulletPool(EffectAnimationPools.EffectType.StarBomb).Launch();
                var createPos = new Vector2(0, 0);
                effectAnimation.transform.localPosition = createPos;
                bombIntervalCount = 1.5f;

                // ボムのダメージと弾消し、ボム数減少
                bossController.Dameged(300.0f);
                enemyBulletPools.VanishAllBullets();
                Bomb--;

                // TODO キャラ毎に演出変えるようにしたら時間や位置も変える、ダメージは持続ダメにして弾消滅も一定時間持続するように直す
                // LA 1.5秒　画面中心基準　全画面判定
                // LB 2.33秒　自機基準　自機の周辺に円判定
                // SA 2秒　自機前方基準　自機より上側の一定の幅のみ
                // SB 0.75秒　自機基準　全画面判定
            }
        }

        /// <summary>
        /// 属性ゲージの更新
        /// </summary>
        private void ElementGaugeUpDate()
        {
            // 属性モード中は自然にゲージが減る
            // ゲージが空になったら属性モード終了
            if (bulletGenerater.iceMode)
            {
                IceGauge -= Time.deltaTime * 120.0f;
                if (IceGauge <= 0)
                {
                    bulletGenerater.iceMode = false;
                }
            }
            if (bulletGenerater.thunderMode)
            {
                ThunderGauge -= Time.deltaTime * 120.0f;
                if (ThunderGauge <= 0)
                {
                    bulletGenerater.thunderMode = false;
                }
            }
            if (bulletGenerater.fireMode)
            {
                FireGauge -= Time.deltaTime * 120.0f;
                if (FireGauge <= 0)
                {
                    bulletGenerater.fireMode = false;
                    FireGauge = 0;
                }
            }

            //ゲージ満タンになっていたらモードを変える;
            if (IceGauge >= MAX_ELEMENT)
            {
                IceGauge = MAX_ELEMENT;
                bulletGenerater.iceMode = true;
            }

            if (ThunderGauge >= MAX_ELEMENT)
            {
                ThunderGauge = MAX_ELEMENT;
                bulletGenerater.thunderMode = true;
            }

            if (FireGauge >= MAX_ELEMENT)
            {
                FireGauge = MAX_ELEMENT;
                bulletGenerater.fireMode = true;
            }
        }

        /// <summary>
        /// 被弾処理(無敵時にも呼び出し自体は行われる)
        /// </summary>
        public void BulletHit()
        {
            // 無敵以外の時にダメージ処理を行う
            if(invincibleCount <= 0)
            {
                if (Life == 0)
                {
                    // ゲームオーバー画面表示
                    gameSceneController.StartClearWindow(false);
                }
                else
                {
                    // ライフを減らしてボム数を戻す
                    Life--;
                    Bomb = firstBomb;

                    // 無敵時間設定
                    invincibleCount = DAMEGED_INVINCIBLE_TIME;

                    // 弾消し(TODO 少しタイミングを遅らせる)
                    enemyBulletPools.VanishAllBullets();

                    // TODO 被弾エフェクト追加
                    //var effectAnimation = effectAnimationPools.GetBulletPool(EffectAnimationPools.EffectType.BlueBulletVanish).Launch();
                    //effectAnimation.transform.localPosition = transform.localPosition;

                    // TODO 被弾後に画面下部から出現
                }
            }
        }

        /// <summary>
        /// グレイズ処理
        /// </summary>
        public void Grazed(EnemyBullet.Element element)
        {
            GrazeCount++;

            // 属性に応じたゲージを増やす
            switch (element)
            {
                case EnemyBullet.Element.Ice:
                    IceGauge += GRAZE_UP_GAUGE;
                    break;
                case EnemyBullet.Element.Thunder:
                    ThunderGauge += GRAZE_UP_GAUGE;
                    break;
                case EnemyBullet.Element.Fire:
                    FireGauge += GRAZE_UP_GAUGE;
                    break;
            }
        }

        /// <summary>
        /// 弾消滅時のゲージ上昇処理
        /// </summary>
        public void VanishedGaugeUp(EnemyBullet.Element element)
        {
            // 属性に応じたゲージを増やす
            switch (element)
            {
                case EnemyBullet.Element.Ice:
                    IceGauge += vanishedUpGauge;
                    break;
                case EnemyBullet.Element.Thunder:
                    ThunderGauge += vanishedUpGauge;
                    break;
                case EnemyBullet.Element.Fire:
                    FireGauge += vanishedUpGauge;
                    break;
            }
        }
    }
}
