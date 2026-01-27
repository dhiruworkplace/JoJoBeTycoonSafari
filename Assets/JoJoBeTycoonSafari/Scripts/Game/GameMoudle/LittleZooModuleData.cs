using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZooGame
{

    [Serializable]
    public class LittleZooModuleData
    {
        public int littleZooID;
        /// <summary>
        /// 门票等级
        /// </summary>
        public int littleZooTicketsLevel;

        /// <summary>
        /// 观光位数量等级
        /// </summary>
        public int littleZooVisitorSeatLevel;

        /// <summary>
        /// 观光游客的流量等级
        /// </summary>
        public int littleZooEnterVisitorSpawnLevel;

    }
}
