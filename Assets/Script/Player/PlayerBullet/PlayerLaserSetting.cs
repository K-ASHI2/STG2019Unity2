using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Player
{
    /// <summary>
    /// レーザーの種類
    /// </summary>
    public enum LaserType
    {
        SunLaser,
        IceLaser,
        ThunderLaser,
        FireLaser,
    }

    /// <summary>
    /// 自機のレーザー1本のデータ
    /// </summary>
    [Serializable]
    public class PlayerSingleLaserData
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
        /// ショット発射位置
        /// </summary>
        public Vector2 ShotPos;
    }

    /// <summary>
    /// 自機の全レーザーのデータ
    /// </summary>
    [Serializable]
    public class PlayerMultipleLaserData
    {
        /// <summary>
        /// レーザーの種類
        /// </summary>
        public LaserType LaserType;
        /// <summary>
        /// NWayの弾データ
        /// </summary>
        public List<PlayerSingleLaserData> PlayerSingleLaserDataList;
    }

    [CreateAssetMenu(menuName = "ScriptableObject/PlayerLaser Setting", fileName = "PlayerLaserSetting")]
    public class PlayerLaserSetting : ScriptableObject
    {
        /// <summary>
        /// 自機のレーザーデータをすべて入れたリスト(Inspecterから設定用)
        /// </summary>
        [SerializeField]
        private List<PlayerMultipleLaserData> PlayerLaserDataList;

        /// <summary>
        /// 自機の各弾データをすべて入れたDictionary(Inspecterから設定不可)
        /// </summary>
        public Dictionary<LaserType, List<PlayerSingleLaserData>> PlayerLaserDataSet { get; private set; }

        /// <summary>
        /// 初期化(AwakeだとUnityから作業時にInspecterを開いたタイミングで呼ばれてしまうので別途呼び出す)
        /// </summary>
        public void Initialize()
        {
            // リストのままだと処理時間が遅くなるので、Dictionaryに入れなおす
            PlayerLaserDataSet = new Dictionary<LaserType, List<PlayerSingleLaserData>>();
            foreach (var playerLaserData in PlayerLaserDataList)
            {
                PlayerLaserDataSet.Add(playerLaserData.LaserType, playerLaserData.PlayerSingleLaserDataList);
            }
        }
    }
}