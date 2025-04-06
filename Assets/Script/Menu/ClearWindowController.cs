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
        // Clear�����e�L�X�g�̃I�u�W�F�N�g
        [SerializeField] private GameObject ClearTextObject;
        // GameOver�����e�L�X�g�̃I�u�W�F�N�g
        [SerializeField] private GameObject GameOverTextObject;

        // �N���A��ʕ\����̉�ʑJ�ډ\�ɂȂ�܂ł̑҂�����
        private const float WAIT_TIME = 1.0f;
        // ��ʑJ�ڌ�A����̃{�^�����������܂Ń{�^�����͂𖳌�������
        public bool StartLock { private get; set; } = true;
        // Wait�����ǂ���
        private bool isWait;

        // Update is called once per frame
        private void Update()
        {
            // ��ʑJ�ڌ�͓���̃{�^�����������܂ŉ�ʑJ�ڏ������s��Ȃ�
            if (StartLock)
            {
                // �V���b�g�E�{���A�\���L�[�{�^���������ꂽ�烍�b�N����
                if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X))
                {
                    StartLock = false;
                }
            }

            if (!StartLock &&  !isWait)
            {
                //�I�����ڂ��Ȃ��̂�BaseUpdate�͕s�v
                if (Input.anyKey)
                {
                    // ��ʂ̐ݒ�����ɖ߂��Ă���^�C�g����ʂɖ߂�
                    gameSceneController.EndClearWindow();
                    // �Q�[���V�[�����[�h��̃C�x���g��o�^
                    SceneManager.sceneLoaded += TitleSceneLoaded;
                    SceneManager.LoadScene("Title");
                }
            }
        }

        /// <summary>
        /// �N���A��ʂ��\�������^�C�~���O�̃C�x���g
        /// </summary>
        public void OnEnable()
        {
            StartLock = true;
            // �^�C�g����ʂɂ����ɑJ�ڂ��Ă��܂�Ȃ��悤�ɃR���[�`����Wait������
            StartCoroutine("CoroutineWait");
        }

        /// <summary>
        /// �R���[�`�����g�p����Wait����
        /// �N���A��ʂł�TimeScale��0�ɂ��Ă���̂ŁADeltaTime��WaitForSeconds�ł͎��Ԃ��i�s���Ȃ��Ȃ��Ă��܂�
        /// </summary>
        /// <returns></returns>
        public IEnumerator CoroutineWait()
        {
            // Wait���Ԃ̑ҋ@������Wait��Ԃɂ���
            isWait = true;
            yield return new WaitForSecondsRealtime(WAIT_TIME);
            isWait = false;
        }

        /// <summary>
        /// �Q�[���V�[�����[�h��̃C�x���g����
        /// </summary>
        private void TitleSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // �V�[���؂�ւ���̃Q�[���V�[���Ǘ��X�N���v�g���擾
            var titleMenu = GameObject.FindWithTag("TitleMenu").GetComponent<TitleMenu>();

            // �A�Ŗh�~
            titleMenu.StartLock = true;

            // �C�x���g����폜
            SceneManager.sceneLoaded -= TitleSceneLoaded;
        }

        /// <summary>
        /// �Q�[���N���A/�Q�[���I�[�o�[��ʂ̃^�C�g�������̐ݒ�
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