using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Player
{
    /// <summary>
    /// 弾の種類
    /// </summary>
    public enum BulletType
    {
        MainShot,
        CrescentCutter,
        IceCutter,
        ThunderCutter,
        FireCutter,
        BlackMissile,
        IceMissile,
        ThunderMissile,
        FireMissile,
        FrushBulletRight,
        FrushBulletLeft,
        FrushBulletUpAndDown,
        IceFrushBullet,
        ThunderFrushBullet,
        FireFrushBullet
    }

    /// <summary>
    /// 自機の弾1つのデータ
    /// </summary>
    [Serializable]
    public class PlayerSingleBulletData
    {
        /// <summary>
        /// 攻撃力
        /// </summary>
        public float BaseAtk;
        /// <summary>
        /// 攻撃力(Power依存の倍率)
        /// </summary>
        public float PowerAtkRate;
        /// <summary>
        /// 弾の速度(X方向)
        /// </summary>
        public float Vx;
        /// <summary>
        /// 弾の速度(Y方向)
        /// </summary>
        public float Vy;
        /// <summary>
        /// ショット発射位置
        /// </summary>
        public Vector2 ShotPos;
    }

    /// <summary>
    /// 自機の弾幕データ
    /// </summary>
    [Serializable]
    public class PlayerBarrageData
    {
        /// <summary>
        /// 弾の種類
        /// </summary>
        public BulletType BulletType;
        /// <summary>
        /// NWayの弾データ
        /// </summary>
        public List<PlayerSingleBulletData> PlayerSingleBulletDataList;
    }

    [CreateAssetMenu(menuName = "ScriptableObject/PlayerBullet Setting", fileName = "PlayerBulletSetting")]
    public class PlayerBulletSetting : ScriptableObject
    {
        /// <summary>
        /// 自機の各弾データをすべて入れたリスト(Inspecterから設定用)
        /// </summary>
        [SerializeField]
        private List<PlayerBarrageData> PlayerBarrageDataList;

        /// <summary>
        /// 自機の各弾データをすべて入れたDictionary(Inspecterから設定不可)
        /// </summary>
        public Dictionary<BulletType, List<PlayerSingleBulletData>> PlayerBarrageDataSet { get; private set; }

        /// <summary>
        /// 初期化(AwakeだとUnityから作業時にInspecterを開いたタイミングで呼ばれてしまうので別途呼び出す)
        /// </summary>
        public void Initialize()
        {
            // リストのままだと処理時間が遅くなるので、Dictionaryに入れなおす
            PlayerBarrageDataSet = new Dictionary<BulletType, List<PlayerSingleBulletData>>();
            foreach (var playerBarrageData in PlayerBarrageDataList)
            {
                PlayerBarrageDataSet.Add(playerBarrageData.BulletType, playerBarrageData.PlayerSingleBulletDataList);
            }
        }
    }
}