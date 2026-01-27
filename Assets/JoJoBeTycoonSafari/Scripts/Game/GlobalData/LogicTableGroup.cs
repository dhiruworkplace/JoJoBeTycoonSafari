/*******************************************************************
* FileName:     LogicTableGroup.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-16
* Description:  
* other:    
********************************************************************/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger;
using UFrame;
using System;

namespace ZooGame.GlobalData
{
    public class LittleZooWeight
    {
        public List<int> littleZooIDs = new List<int>();
        public List<int> weights = new List<int>();

        public int GetLittleZooID(int idx)
        {
#if UNITY_EDITOR
            if (idx < 0 || idx > littleZooIDs.Count)
            {
                string e = string.Format("查找 LittleZoo 索引错误{0}", idx);
                throw new System.Exception(e);
            }
#endif
            return littleZooIDs[idx];
        }

        public void Add(int littleZooID, int weight)
        {
            littleZooIDs.Add(littleZooID);
            weights.Add(weight);
        }
    }


    /// <summary>
    /// 根据游戏逻辑，对group表的数据结构进行二次加工，方便使用
    /// </summary>
    public class LogicTableGroup
    {
        /// <summary>
        /// 最后一组了
        /// </summary>
        public static int EndOfGroupID = -2;


        public static int EntryGroupID = -3;

        /// <summary>
        /// 排序后的组ID
        /// </summary>
        public List<int> sortedGroupID { get; protected set; }

        /// <summary>
        /// weight和Startid的map，key 为groupid，key2为weight
        /// </summary>
        public Dictionary<int, LittleZooWeight> weightStartIDMaps { get; protected set; }

        /// <summary>
        /// 排序后的weight，从小到大, key= groupid
        /// </summary>
        public Dictionary<int, List<int>> sortedWeights { get; protected set; }


        /// <summary>
        /// 排序后动物栏id，key= groupid, 本身并无排序，是做到和 sortedWeights 对应。
        /// </summary>
        public Dictionary<int, List<int>> sortedLittleZooIDs { get; protected set; }

        public LogicTableGroup()
        {
            Init();
        }

        protected void Init()
        {
            sortedGroupID = new List<int>();
            weightStartIDMaps = new Dictionary<int, LittleZooWeight>();
            sortedWeights = new Dictionary<int, List<int>>();
            sortedLittleZooIDs = new Dictionary<int, List<int>>();

            foreach (var kv in Config.groupConfig.getInstace().AllData)
            {
                var cell = kv.Value;
                int groupID;
                if (!int.TryParse(kv.Key, out groupID))
                {
                    string e = string.Format("group 表错误，groupid 不是数字型 {0}", kv.Key);
                    throw new System.Exception(e);
                }
                sortedGroupID.Add(groupID);
                var littleZooWeight = new LittleZooWeight();
                if (cell.startid.Length != cell.groupweight.Length)
                {
                    string e = string.Format("group 表startid和groupweight个数不一致");
                    throw new System.Exception(e);
                }
                
                for(int i = 0; i < cell.startid.Length; i++)
                {
                    littleZooWeight.Add(cell.startid[i], cell.groupweight[i]);
                }
                weightStartIDMaps.Add(groupID, littleZooWeight);

                var sortedWeight = new List<int>(cell.groupweight);
                //sortedWeight.Sort();
                sortedWeights.Add(groupID, sortedWeight);
            }
            sortedGroupID.Sort();

            //把startid按weight顺序排
            foreach (var kv in sortedWeights)
            {
                int groupID = kv.Key;
                var sortedWeight = kv.Value;
                var sortedLittleZooID = new List<int>();
                sortedLittleZooIDs.Add(groupID, sortedLittleZooID);
                for (int i = 0; i < sortedWeight.Count; i++)
                {
                    //查到weight对应的liitlezoo，然后插入
                    //Dictionary<int, int> weightStartIDMap = null;
                    LittleZooWeight weightStartIDMap = null;
                    if (!weightStartIDMaps.TryGetValue(groupID, out weightStartIDMap))
                    {
                        string e = string.Format("weightStartIDMaps 没找到 {0}", groupID);
                        throw new System.Exception(e);
                    }
                    int littleZooID = weightStartIDMap.GetLittleZooID(i);
                    sortedLittleZooID.Add(littleZooID);
                }
            }
        }


        /// <summary>
        /// 返回当前所在组的下一组的id
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public int GetNextGroupID(int groupID)
        {
            int idx = sortedGroupID.IndexOf(groupID);

            //没找到
            if (idx < 0)
            {
                return Const.Invalid_Int;
            }

            //当前就是最后一个了
            if (idx == sortedGroupID.Count - 1)
            {
                return EndOfGroupID;
            }

            return sortedGroupID[idx + 1];
        }

        public int GetNextGroupID_New(int groupID, out bool isLastGroupID)
        {
            isLastGroupID = false;
            int idx = sortedGroupID.IndexOf(groupID);
            
            //没找到
            if (idx < 0)
            {
                return Const.Invalid_Int;
            }

            //当前就是最后一个了
            if (idx == sortedGroupID.Count - 1)
            {
                isLastGroupID = true;
                return sortedGroupID[idx];
            }

            return sortedGroupID[idx + 1];
        }

        /// <summary>
        /// 根据groupID获取下一个groupID
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="nextGroupID"></param>
        /// <returns></returns>
        public bool GetNextGroupID(int groupID, ref int nextGroupID)
        {
            int idx = sortedGroupID.IndexOf(groupID);

            //没找到 或者 当前就是最后一个
            if (idx < 0 || idx == sortedGroupID.Count - 1)
            {
                return false;
            }

            nextGroupID = sortedGroupID[idx + 1];

            return true;
        }

        ///// <summary>
        ///// 返回将要去的动物栏概率列表
        ///// 要剔除已经游览过的动物栏
        ///// todo 需要优化
        ///// 
        ///// </summary>
        ///// <param name="groupID"></param>
        ///// <param name="vistedLittleZooIDs"></param>
        ///// <returns></returns>
        //List<int> wouldGotoWeights = new List<int>();
        //List<int> visitedIdxs = new List<int>();
        //public List<int> GetWouldGotoWeights(int groupID, List<int> vistedLittleZooIDs)
        //{
        //    List<int> sortedWeight = null;
        //    if (!sortedWeights.TryGetValue(groupID, out sortedWeight))
        //    {
        //        string e = string.Format("组{0} 没有找到权重!!!!!!!!!!", groupID);
        //        throw new System.Exception(e);
        //    }

        //    //没有游览过
        //    if (null == vistedLittleZooIDs || vistedLittleZooIDs.Count <= 0)
        //    {
        //        return sortedWeight;
        //    }

        //    wouldGotoWeights.Clear();
        //    visitedIdxs.Clear();

        //    List<int> sortedLittleZooID = null;
        //    if (!sortedLittleZooIDs.TryGetValue(groupID, out sortedLittleZooID))
        //    {
        //        string e = string.Format("组{0} 没有找到LittleZooID!!!!!!!!!!", groupID);
        //        throw new System.Exception(e);
        //    }

        //    for (int i = 0; i < vistedLittleZooIDs.Count; i++)
        //    {
        //        int vistedLittleZooID = vistedLittleZooIDs[i];
        //        int idx = sortedLittleZooID.IndexOf(vistedLittleZooID);
        //        if (idx < 0 )
        //        {
        //            string e = string.Format("逻辑数据异常 vistedLittleZooIDs {0}", vistedLittleZooID);
        //            throw new System.Exception(e);
        //        }
        //        visitedIdxs.Add(idx);
        //    }

        //    for(int i = 0; i < sortedWeight.Count; i++)
        //    {
        //        if (visitedIdxs.IndexOf(i) < 0)
        //        {
        //            wouldGotoWeights.Add(sortedWeight[i]);
        //        }
        //    }

        //    return wouldGotoWeights;
        //}

        public int FindGroupID(int littleZooID)
        {
            int groupID = Const.Invalid_Int;
            foreach(var kv in Config.groupConfig.getInstace().AllData)
            {
                var cell = kv.Value;
                for (int i = 0; i < cell.startid.Length; i++)
                {
                    if (cell.startid[i] == littleZooID)
                    {
                        int.TryParse(kv.Key, out groupID);
                        return groupID;
                    }
                }
            }

            return groupID;
        }

        /// <summary>
        /// 是不是组中最后一个动物栏（配表顺序）
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool IsLastLittleZooID(int littleZooID, ref int groupID)
        {
            foreach (var kv in Config.groupConfig.getInstace().AllData)
            {
                var cell = kv.Value;
                for (int i = 0; i < cell.startid.Length; i++)
                {
                    if (cell.startid[i] == littleZooID)
                    {
                        int.TryParse(kv.Key, out groupID);
                        return i == cell.startid.Length - 1;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 是否触发加载下一个Group
        /// 1.是在所在组的最后一个动物栏(配表顺序)
        /// 2.所在组不是最后一组(组ID排序)
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="nextGroupID"></param>
        /// <returns></returns>
        public bool IsTrigerLoadNextGroupID(int littleZooID, ref int nextGroupID)
        {
            int groupID = Const.Invalid_Int;

            if (!IsLastLittleZooID(littleZooID, ref groupID))
            {
                return false;
            }

            if (!GetNextGroupID(groupID, ref nextGroupID))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据groupID查找组内开启的动物栏，找到就退出
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="littleZooID"></param>
        /// <returns></returns>
        public bool GetOpenedLittleZooID(int groupID, ref int littleZooID)
        {
            var cell = Config.groupConfig.getInstace().getCell(groupID);
            littleZooID = Const.Invalid_Int;
           
            var littleZooModuleDatas = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas;
            int idx = Const.Invalid_Int;
            for (int i = 0; i < cell.startid.Length; i++)
            {
                littleZooID = cell.startid[i];
                idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(littleZooID);  //获取动物栏ID  下标
                if (idx >= 0 && littleZooModuleDatas[idx].littleZooTicketsLevel > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
