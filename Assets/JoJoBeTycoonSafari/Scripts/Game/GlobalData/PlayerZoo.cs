using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using Logger;

namespace ZooGame.GlobalData
{
    [Serializable]
    public class PlayerZoo
    {
        /// <summary>
        /// 玩家的动物数据
        /// </summary>
        public PlayerAnimal playerAnimal = new PlayerAnimal();

        /// <summary>
        /// 金币
        /// </summary>
        public string coin = "0";

        /// <summary>
        /// 金币大数据==coin
        /// </summary>
        public System.Numerics.BigInteger coinBigInt = 0;

        /// <summary>
        /// 钻石
        /// </summary>
        public int diamond = 0;

        /// <summary>
        /// 星星
        /// </summary>
        public int star = 0;

        /// <summary>
        /// 拥有但还没使用的道具
        /// </summary>
        //public List<Item> itemList = new List<Item>();
        public List<int> itemList = new List<int>();

        /// <summary>
        /// 上次登出时间
        /// </summary>
        public long lastLogoutTime = 0;

        /// <summary>
        /// 动物栏ID对应的数据
        /// </summary>
        public List<LittleZooModuleData> littleZooModuleDatas = new List<LittleZooModuleData>();

        /// <summary>
        /// 出口等级
        /// </summary>
        public int exitGateLevel = 1;

        /// <summary>
        /// 售票口门票等级
        /// </summary>
        public int entryTicketsLevel = 1;

        /// <summary>
        /// 小游戏等级(过马路)
        /// </summary>
        public int littleGameLevel = 1;

        /// <summary>
        /// 全部动物数量
        /// </summary>
        public int allZooNumber;

        /// <summary>
        /// 建筑升级的Transform
        /// </summary>
        public Transform BuildShowTransform;


        /// <summary>
        /// 是否开启声音
        /// </summary>
        public bool isSound = true;
        /// <summary>
        /// 是否开启新手引导
        /// </summary>
        public bool isGuide = false;

        /// <summary>
        /// 出口cd buff计算值
        /// </summary>
        public float buffExitEntryCDVal = UFrame.Const.Invalid_Float;

        /// <summary>
        /// 浏览cd buff计算值
        /// </summary>
        public float buffVisitCDVal = UFrame.Const.Invalid_Float;

        /// <summary>
        /// 入口cd buff计算值
        /// </summary>
        public float buffEntryGateCDVal = UFrame.Const.Invalid_Float;

        /// <summary>
        /// 金币收入倍数相加 buff影响值
        /// </summary>
        public float buffRatioCoinInComeAdd = 1;

        /// <summary>
        /// 金币收入倍数相乘法 buff影响值
        /// </summary>
        public float buffRatioCoinInComeMul = 1;

        /// <summary>
        /// 是否开启动物培养功能（默认为否）
        /// </summary>
        public bool isShowAnimalCultivate = false;

        /// <summary>
        /// Buff列表
        /// </summary>
        public List<Buff> buffList = new List<Buff>();

        /// <summary>
        /// 离线buff列表
        /// </summary>
        public List<Buff> offlineBuffList = new List<Buff>();

        /// <summary>
        /// 开启的入口数量
        /// </summary>
        public int numEntryGate = 1;

        /// <summary>
        /// 开启的入口，List形式存储每个入口的数据
        /// </summary>
        public List<EntryGateData> entryGateList = new List<EntryGateData>();

        /// <summary>
        /// 停车场数据源
        /// </summary>
        public ParkingCenterData parkingCenterData = new ParkingCenterData();


        public PlayerNumberOfVideosWatched playerNumberOfVideosWatched = new PlayerNumberOfVideosWatched();

        /// <summary>
        /// 记录上次登录日期（天，不可大于31）
        /// </summary>
        public int LastLogingDate_Day = 0;

        /// <summary>
        /// 场景加载判定是否需要离线，用于在主界面用于场景加载后
        /// 离线界面是否显示逻辑
        /// </summary>
        public bool isLoadingShowOffline = false;

        public void SetDefault()
        {
            /*初始化停车场数据结构*/
            SetDefaultParkingCenterData();

            exitGateLevel = 1;
            entryTicketsLevel = 1;
            isSound = true;
            isGuide = false;
            lastLogoutTime = DateTime.Now.Ticks;
            LastLogingDate_Day = DateTime.Now.Day;
            coin = Config.globalConfig.getInstace().InitialGoldNumber;
            star = Config.globalConfig.getInstace().InitialStarNumber;
            allZooNumber = 1;
            isShowAnimalCultivate = false;
#if NOVICEGUIDE
            isGuide = true;

#endif
            SetDefaultEntryGateData();

            littleZooModuleDatas.Clear();
            //coin = "1000";
            /* 动物栏的数据初始化 */
            SetDefaultlittleZooData();

        }

        public void SetDefaultlittleZooData()
        {
            int openLittleZoo = 0;
            int defaultOpenGroup = Config.globalConfig.getInstace().DefaultOpenGroup;
            int defaultLoadGroup = defaultOpenGroup + Config.globalConfig.getInstace().ExtendLoadGroup;
            int defaultOpenLittleZoo = Config.globalConfig.getInstace().DefaultOpenLittleZoo;
#if NO_BIGINT
            defaultLoadGroup = 8;
            defaultOpenLittleZoo = 19;
#endif
            for (int i = 0; i < defaultLoadGroup; i++)
            {
                int groupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[i];
                var cfgLittleZooIDs = GlobalDataManager.GetInstance().logicTableGroup.sortedLittleZooIDs[groupID];

                for (int j = 0; j < cfgLittleZooIDs.Count; j++)
                {
                    //littleZooIDs.Add(cfgLittleZooIDs[j]);
                    LittleZooModuleData littleZooModuleData = new LittleZooModuleData();
                    littleZooModuleDatas.Add(littleZooModuleData);
                    littleZooModuleData.littleZooID = cfgLittleZooIDs[j];
                    if (openLittleZoo < defaultOpenLittleZoo)
                    {
                        
                        littleZooModuleData.littleZooTicketsLevel = 1;
                        littleZooModuleData.littleZooVisitorSeatLevel = 1;
#if NO_BIGINT
                        littleZooModuleData.littleZooVisitorSeatLevel = 10;
#endif
                        littleZooModuleData.littleZooEnterVisitorSpawnLevel = 1;
                        ++openLittleZoo;
                    }
                    else
                    {
                        littleZooModuleData.littleZooTicketsLevel = 0;
                        littleZooModuleData.littleZooVisitorSeatLevel = 0;
                        littleZooModuleData.littleZooEnterVisitorSpawnLevel = 0;
                    }
                }
            }


        }

        private void SetDefaultParkingCenterData()
        {
#if !NO_BIGINT
            parkingCenterData = new ParkingCenterData
            {
                parkingSpaceLevel = 1,
                parkingProfitLevel = 1,
                parkingEnterCarSpawnLevel = 1,
            };
#else
            parkingCenterData = new ParkingCenterData
            {
                parkingSpaceLevel = 43,
                parkingProfitLevel = 2000,
                parkingEnterCarSpawnLevel = 16,
            };
#endif
        }

        public void SetDefaultEntryGateData()
        {
            var sortGateIDs = GlobalDataManager.GetInstance().logicTableEntryGate.sortGateIDs;
            EntryGateData entryGataData = null;
#if !NO_BIGINT
            numEntryGate = 1;
            entryGataData = new EntryGateData();
            int firstID = sortGateIDs[0];
            entryGataData.entryID = firstID;
            entryGataData.level = 1;
            this.entryGateList.Add(entryGataData);
#else
            numEntryGate = 8;
            for(int i = 0; i < sortGateIDs.Count; i++)
            {
                entryGataData = new EntryGateData();
                entryGataData.entryID = sortGateIDs[i];
                entryGataData.level = 1000;
                this.entryGateList.Add(entryGataData);
            }
#endif
        }
    }
}

