using Assets.Script.GameSceneControllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.Menu
{
    public class PauseMenu : MenuSelectBase
    {
        [SerializeField] private GameSceneController gameSceneController;

        private enum PauseMenuItem
        {
            Countinue = 1,
            Restart = 2,
            Title = 3
        }

        // Update is called once per frame
        private void Update()
        {
            // �|�[�Y�{�^��������ă|�[�Y�˓���͂��̃{�^�����������܂ŉ�ʑJ�ڏ������s��Ȃ�
            if (StartLock)
            {
                // �L�[���͂������ꂽ�烍�b�N����
                if (!Input.GetKey(KeyCode.Escape) && !Input.GetKey(KeyCode.Z))
                {
                    StartLock = false;
                }
            }

            if (!StartLock)
            {
                BaseUpdate();
                if (Input.GetKey(KeyCode.Z))
                {
                    // ���j���[��ʂ̑I�����ڂɂ���ĉ�ʑJ�ځE�Q�[���I��
                    switch ((PauseMenuItem)selectedItemNumV)
                    {
                        case PauseMenuItem.Countinue:
                            // �Q�[���v���C��ʂɖ߂�
                            gameSceneController.EndPause();
                            break;
                        case PauseMenuItem.Restart:
                            // �ŏ�������Ȃ���
                            gameSceneController.EndPause();
                            gameSceneController.GameSceneInitialize();
                            break;
                        case PauseMenuItem.Title:
                            // �|�[�Y���I�������Ă���A�^�C�g����ʂɖ߂�
                            gameSceneController.EndPause();
                            // �Q�[���V�[�����[�h��̃C�x���g��o�^
                            SceneManager.sceneLoaded += TitleSceneLoaded;
                            SceneManager.LoadScene("Title");
                            break;
                        default: break;
                    }

                }
                else if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Escape))
                {
                    // �Q�[���v���C��ʂɖ߂�
                    gameSceneController.EndPause();
                }
            }
        }

        /// <summary>
        /// �|�[�Y��ʂ��\�������^�C�~���O�̃C�x���g
        /// </summary>
        public new void OnEnable()
        {
            // Start�C�x���g�̑O�ɂ���uEnable�ɂȂ��Ă��܂��Ă΂��̂ŁA���̃^�C�~���O�ł̓X�L�b�v
            if (defaultColorList != null)
            {
                // �I�����ڂ̐F���f�t�H���g��Ԃɖ߂�
                SetItemDefaultColor();
                // �I�����ڂ�1�Ԗڂ̍��ڂɖ߂�
                selectedItemNumV = 1;
                ItemSelectionChanged();
                StartLock = true;
            }
            // ���ʏ�����OnEnable���Ă�
            base.OnEnable();
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
    }
}