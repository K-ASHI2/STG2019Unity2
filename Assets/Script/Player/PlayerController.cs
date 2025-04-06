using Assets.Script.Common;
using Assets.Script.Enemy;
using Assets.Script.Effect;
using Assets.Script.GameSceneControllers;
using UnityEngine;

namespace Assets.Script.Player
{
    public class PlayerController : MonoBehaviour
    {
        // 2D�X�v���C�g�̕������Z
        [SerializeField] private Rigidbody2D rb;
        // ���@�̒e�̃I�u�W�F�N�g�v�[��
        [SerializeField] private PlayerBulletPools playerBulletPools;
        // �G�̒e�̃I�u�W�F�N�g�v�[��
        [SerializeField] private EnemyBulletPools enemyBulletPools;
        // �G�t�F�N�g�̃I�u�W�F�N�g�v�[��
        [SerializeField] private EffectAnimationPools effectAnimationPools;
        // ���@�̔���̉~�̃X�v���C�g�����_���[
        [SerializeField] private SpriteRenderer judgeCircleSpriteRenderer;
        // ���@�̔���̉~�̃R���C�_�[
        [SerializeField] private CircleCollider2D judgeCircleCollider;
        // ���@�̃O���C�Y�~�̃R���C�_�[
        [SerializeField] private CircleCollider2D grazeCircleCollider;
        // �Q�[���V�[���̏�ԑ���N���X
        [SerializeField] private GameSceneController gameSceneController;
        // �{�X�̐���N���X
        [SerializeField] private BossController bossController;
        // Sun Magician�̃X�v���C�g
        [SerializeField] private Sprite PlayerSpriteSM;
        [SerializeField] private Sprite judgeCircleSpriteSM;

        #region �ړ����x�̒�`
        // DX���C�u�����łƑ��x�𓝈ꂷ��
        // verocity��1�b=60�t���[����1Unit=100pixel�������ADX���C�u�����ł�1�t���[���������pixel���œ������Ă���
        // DX���C�u�����łł̓o�O��1�t���[����2��X�V���Ă��܂��Ă���̂Œ��ӂ���
        // �x���L�����̍����ړ��̑��x(6.75*60/100=4.05)
        private const float PMOVE_HIGH1 = 4.05f;
        // �����L�����̍����ړ��̑��x(7.5*60/100=4.5)
        private const float PMOVE_HIGH2 = 4.5f;
        // �ᑬ�ړ��̑��x(3.0*60/100=1.8)
        private const float PMOVE_SLOW = 1.8f;
        #endregion

        // �O���C�Y���̑����Q�[�W�㏸��
        private const int GRAZE_UP_GAUGE = 30;
        // �e�������̑����Q�[�W�㏸�ʁv
        private const int VANISH_UP_GAUGE_E = 60;
        private const int VANISH_UP_GAUGE_N = 30;
        private const int VANISH_UP_GAUGE_H = 10;
        // �����Q�[�W�̍ő�l
        private const int MAX_ELEMENT = 960;
        // ��e��̖��G����
        private const float DAMEGED_INVINCIBLE_TIME = 3.75f;

        // �e�����N���X
        private PlayerBulletGenerater bulletGenerater;
        // ���@�̎��
        public PlayerCharacter PlayerCharacter { private get; set; } = PlayerCharacter.LunaMagician_A;
        // ���@�̎��
        public int GrazeCount { get; internal set; }
        // �X�����Q�[�W
        public float IceGauge { get; internal set; }
        // �������Q�[�W
        public float ThunderGauge { get; internal set; }
        // �������Q�[�W
        public float FireGauge { get; internal set; }
        // �c�@
        public int Life { get; private set; }
        // �{���̎c��
        public int Bomb { get; private set; }
        // Renderer(��e���̓��ߏ����Ɏg��)
        private SpriteRenderer spriteRenderer;

        // ScriptableObject��static�ŕێ����Ă���
        // ���@�̒e�̐ݒ�
        private static PlayerBulletSetting playerBulletSetting;
        // ���@�̃��[�U�[�̐ݒ�
        private static PlayerLaserSetting playerLaserSetting;
        // Luna Magician�̃X�v���C�g(�f�t�H���g�̃X�v���C�g)
        private Sprite PlayerSpriteLM;
        private Sprite judgeCircleSpriteLM;
        // �{���̏����l
        private int firstBomb;
        // �{���g�p���Ԃ̃C���^�[�o��
        private float bombIntervalCount;
        // ���G���Ԃ̃J�E���g(0���傫���Ȃ疳�G)
        private float invincibleCount;
        // �e�폜���̃Q�[�W�㏸��
        private int vanishedUpGauge = 0;
        // �f�t�H���g�̎��@�̈ʒu
        private Vector2 defaultPosition;

        private void Awake()
        {
            // �v���C���[�̃X�v���C�g���擾���Ă���
            spriteRenderer = GetComponent<SpriteRenderer>();

            // �f�t�H���g�̃X�v���C�g��Luna Magician�̃X�v���C�g�Ƃ��ĕێ����Ă���
            PlayerSpriteLM = spriteRenderer.sprite;
            judgeCircleSpriteLM = judgeCircleSpriteRenderer.sprite;

            // �v���C���[�̃f�t�H���g�ʒu���o���Ă����A���g���C���ɂ��g�p�ł���悤�ɂ���
            defaultPosition = transform.localPosition;
        }

        // Start is called before the first frame update
        void Start()
        {
            // �������֘A�̏�����GameSceneController����Ăяo��
        }

        // Update is called once per frame
        void Update()
        {
            // �|�[�Y���ȊO�̎��X�V�������s��
            if (!gameSceneController.IsPauseMode)
            {
                // �v���C���[�̈ړ�
                MovePlayer();

                // ���G���Ԍ���
                if(invincibleCount > 0)
                {
                    invincibleCount -= Time.deltaTime;
                    if(invincibleCount <= 0)
                    {
                        invincibleCount = 0;
                        // �x�������ꍇ�ɔ������̂܂ܖ��G���ԏI������\��������̂œ_�ŕ`������ɖ߂�
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                    }

                    // ���G���Ԃ̓_�ŕ`��
                    // ���G���ԏI�����O�ȊO�Ȃ�1/12�b���ɓ_��
                    if (invincibleCount > 0.33f && (int)(invincibleCount * 12) % 2 == 1)
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.25f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                    }
                }

                // �e�e���̍X�V(�V���b�g������C���^�[�o���̍X�V)
                bulletGenerater.GeneraterUpDate();

                // �����Q�[�W�֘A�̍X�V����
                ElementGaugeUpDate();
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        public void Initialize(bool isPractice, Difficulty difficulty)
        {
            // ���@�̎�ނɍ��킹�Ď��@�̉摜�Ɣ���̑傫����ݒ�
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

            // �e�����֌W�N���X�̏�����
            playerBulletPools.Initialize();
            playerBulletSetting = SettingDataLoader.PlayerBulletSetting;
            playerLaserSetting = SettingDataLoader.PlayerLaserSetting;
            bulletGenerater = new PlayerBulletGenerater(playerBulletPools, transform, PlayerCharacter, playerBulletSetting, playerLaserSetting);

            // �v���C���[�̈ʒu�������ʒu�ɖ߂�
            transform.localPosition = defaultPosition;

            // �c�@�ƃ{���������l�ɐݒ�
            if (isPractice)
            {
                Life = 0;
                firstBomb = 0;
                Bomb = 0;
            }
            else
            {
                // TODO �e�������₵����c�@�𑝂₷
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

            // �e���Ŏ��̃Q�[�W�㏸�ʂ��Փx�ɉ������l�ɐݒ肷��
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
        /// �v���C���[�𓮂���
        /// </summary>
        private void MovePlayer()
        {
            // ���Ȃ�-1�A�E�Ȃ�1�A���͂Ȃ����0
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputY = Input.GetAxisRaw("Vertical");

            // �ړ�������������߂�
            // �΂߂ɓ��͂����ꍇ��XY���ꂼ�ꃋ�[�g2�̑��x�ɂȂ�
            Vector2 direction = new Vector2(inputX, inputY).normalized;

            // �v���C���[�̎�ނƒᑬ�{�^���̉����L���ɂ���đ��x�̔{����ς���
            float speedRate;
            // ����Shift�L�[�����Ă���ԂȂ�ᑬ�ړ�
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speedRate = PMOVE_SLOW;

                // �ᑬ���͔���̉~��\������
                judgeCircleSpriteRenderer.enabled = true;
            }
            // �����ړ�
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

                // �������͔���̉~������
                judgeCircleSpriteRenderer.enabled = false;
            }

            // �ړ����x���v�Z���A�ړ�������
            // �������Z����̂�velocity����ԗǂ�����
            rb.velocity = direction * speedRate;

            // ��ʊO�ɏo�Ȃ��悤�ɂ���
            var clampedX = Mathf.Clamp(transform.localPosition.x, PlayArea.Left + judgeCircleCollider.radius * 2, PlayArea.Right - judgeCircleCollider.radius * 2);
            var clampedY = Mathf.Clamp(transform.localPosition.y, PlayArea.Lower + judgeCircleCollider.radius, PlayArea.Upper - judgeCircleCollider.radius * 2);
            transform.localPosition = new Vector3(clampedX, clampedY);

            // �{��
            bombIntervalCount -= Time.deltaTime;
            if (Input.GetKey(KeyCode.X) && bombIntervalCount < 0 && Bomb > 0)
            {
                // �G�t�F�N�g����
                var effectAnimation = effectAnimationPools.GetBulletPool(EffectAnimationPools.EffectType.StarBomb).Launch();
                var createPos = new Vector2(0, 0);
                effectAnimation.transform.localPosition = createPos;
                bombIntervalCount = 1.5f;

                // �{���̃_���[�W�ƒe�����A�{��������
                bossController.Dameged(300.0f);
                enemyBulletPools.VanishAllBullets();
                Bomb--;

                // TODO �L�������ɉ��o�ς���悤�ɂ����玞�Ԃ�ʒu���ς���A�_���[�W�͎����_���ɂ��Ēe���ł���莞�Ԏ�������悤�ɒ���
                // LA 1.5�b�@��ʒ��S��@�S��ʔ���
                // LB 2.33�b�@���@��@���@�̎��ӂɉ~����
                // SA 2�b�@���@�O����@���@���㑤�̈��̕��̂�
                // SB 0.75�b�@���@��@�S��ʔ���
            }
        }

        /// <summary>
        /// �����Q�[�W�̍X�V
        /// </summary>
        private void ElementGaugeUpDate()
        {
            // �������[�h���͎��R�ɃQ�[�W������
            // �Q�[�W����ɂȂ����瑮�����[�h�I��
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

            //�Q�[�W���^���ɂȂ��Ă����烂�[�h��ς���;
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
        /// ��e����(���G���ɂ��Ăяo�����͍̂s����)
        /// </summary>
        public void BulletHit()
        {
            // ���G�ȊO�̎��Ƀ_���[�W�������s��
            if(invincibleCount <= 0)
            {
                if (Life == 0)
                {
                    // �Q�[���I�[�o�[��ʕ\��
                    gameSceneController.StartClearWindow(false);
                }
                else
                {
                    // ���C�t�����炵�ă{������߂�
                    Life--;
                    Bomb = firstBomb;

                    // ���G���Ԑݒ�
                    invincibleCount = DAMEGED_INVINCIBLE_TIME;

                    // �e����(TODO �����^�C�~���O��x�点��)
                    enemyBulletPools.VanishAllBullets();

                    // TODO ��e�G�t�F�N�g�ǉ�
                    //var effectAnimation = effectAnimationPools.GetBulletPool(EffectAnimationPools.EffectType.BlueBulletVanish).Launch();
                    //effectAnimation.transform.localPosition = transform.localPosition;

                    // TODO ��e��ɉ�ʉ�������o��
                }
            }
        }

        /// <summary>
        /// �O���C�Y����
        /// </summary>
        public void Grazed(EnemyBullet.Element element)
        {
            GrazeCount++;

            // �����ɉ������Q�[�W�𑝂₷
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
        /// �e���Ŏ��̃Q�[�W�㏸����
        /// </summary>
        public void VanishedGaugeUp(EnemyBullet.Element element)
        {
            // �����ɉ������Q�[�W�𑝂₷
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
