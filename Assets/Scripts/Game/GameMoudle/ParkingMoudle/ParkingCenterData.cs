using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    [Serializable]
    public class ParkingCenterData
    {
        /// <summary>
        /// 利润等级
        /// </summary>
        public int parkingProfitLevel;

        /// <summary>
        /// 停车位数量等级
        /// </summary>
        public int parkingSpaceLevel;

        /// <summary>
        /// 流量等级
        /// </summary>
        public int parkingEnterCarSpawnLevel;

        public ParkingCenterData()
        {
            parkingSpaceLevel = 1;
            parkingProfitLevel = 1;
            parkingEnterCarSpawnLevel = 1;
        }

    }
}
