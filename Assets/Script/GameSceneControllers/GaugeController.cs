using Assets.Script.Common;
using Assets.Script.Enemy;
using Assets.Script.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.GameSceneControllers
{
    /// <summary>
    /// HPバーの更新(難易度のみプレイ中に変化しないのでGameSceneCongtroller側で管理)
    /// </summary>
    public class GaugeController : MonoBehaviour
    {
        // プレイヤーのコントローラー
        [SerializeField] private PlayerController playerController;
        // ボスのコントローラー
        [SerializeField] private BossController bossController;
        // モードHPバーのゲージ画像
        [SerializeField] private Image modeHpGauge;
        // トータルHPバーのゲージ画像
        [SerializeField] private Image totalHpGauge;
        // 氷属性バーのゲージ画像
        [SerializeField] private Image iceGauge;
        // 雷属性バーのゲージ画像
        [SerializeField] private Image thunderGauge;
        // 炎属性バーのゲージ画像
        [SerializeField] private Image fireGauge;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            // ボスのHPバーの色更新
            UpdateHPColor(bossController.Hp, bossController.MaxHp);

            // 各ゲージの現在値更新
            GaugeUpdate(modeHpGauge, bossController.Hp, bossController.MaxHp);
            GaugeUpdate(totalHpGauge, bossController.TotalHp, bossController.MaxTotalHp);
            GaugeUpdate(iceGauge, playerController.IceGauge, CommonConst.MAX_ELEMENT);
            GaugeUpdate(thunderGauge, playerController.ThunderGauge, CommonConst.MAX_ELEMENT);
            GaugeUpdate(fireGauge, playerController.FireGauge, CommonConst.MAX_ELEMENT);
        }

        /// <summary>
        /// ゲージの現在値表示
        /// </summary>
        public void GaugeUpdate(Image gaugeImage, float currentValue, float maxValue)
        {
            // 0除算回避
            if (maxValue == 0)
            {
                gaugeImage.fillAmount = 0;
            }
            else
            {
                gaugeImage.fillAmount = currentValue / maxValue;
            }
        }

        /// <summary>
        /// 敵の体力に応じたHPバーの色に更新する関数
        /// </summary>
        private void UpdateHPColor(float hp, float maxhp)
        {
            float red, green, blue;

            // 0で割ることを回避
            if (maxhp == 0)
            {
                return;
            }
            // hp半分で区切るので変化量も半分となる
            // hpが半分以上の時(緑→黄色)
            if (hp >= maxhp / 2)
            {
                red = 355 - 243 * hp / maxhp;
                green = 255;
                blue = 112;
            }
            // hpが半分未満の時(黄色→赤)
            else
            {
                red = 255;
                green = 510 * hp / maxhp;
                blue = 224 * hp / maxhp;
            }
            modeHpGauge.color = new Color(red / 255f, green / 255f, blue / 255f);
        }
    }
}