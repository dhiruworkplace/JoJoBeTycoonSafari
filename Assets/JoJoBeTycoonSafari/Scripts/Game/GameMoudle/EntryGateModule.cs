/*******************************************************************
* FileName:     EntryGateModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-12
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using Logger;
using Game.MessageCenter;
using Game.GlobalData;

namespace Game
{
    /// <summary>
    /// 入口检票逻辑模块
    /// 1.分配游客到哪个入口
    /// 2.每个入口的排队的控制逻辑
    /// </summary>
    public partial class EntryGateModule : GameModule
    {
        /// <summary>
        /// 所有入口
        /// </summary>
        public Dictionary<int, EntryGate> entryGates = new Dictionary<int, EntryGate>();

        /// <summary>
        /// 空闲的出口
        /// </summary>
        List<EntryGate> idleGates = new List<EntryGate>();

        /// <summary>
        /// 是否随机空闲入口
        /// </summary>
        bool isRandomEntry = true;

        /// <summary>
        /// 获取售票口下标的对应对象
        /// </summary>
        public List<Transform> EntrySubscriptGB = new List<Transform>();

        /// <summary>
        /// 是否只开一个入口
        /// </summary>
        bool isOnlyOneEntry = false;


        protected int EntryVisitorsNum = 0;
        
        /// <summary>
        /// 出口游客排队个位数
        /// </summary>
        GameObject EntryVisitorsNum0;

        /// <summary>
        /// 出口游客排队十位数
        /// </summary>
        GameObject EntryVisitorsNum1;

        PlayerData playerData;

        /// <summary>
        /// 入口数量
        /// </summary>
        int entryNum;

        public EntryGateModule(int orderID) : base(orderID) { }

        public override void Init()
        {
            playerData = GlobalDataManager.GetInstance().playerData;

            this.InitEntryData();
            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToEntryQueuePlaceHolderApply, OnAddVisitorToEntryQueuePlaceHolderApply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToEntryQueueApply, this.OnAddVisitorToEntryQueueApply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.GetEntryGateDataApply, OnGetEntryGateDataApply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.EntryGateCheckInCDFinshedApply, OnEntryGateCheckInCDFinshedApply);

            //MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastEntryGateLevelOfPlayerData,
            //    this.OnBroadcastEntryGateLevelOfPlayerData);

            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastEntryGatePureLevelOfPlayerData, OnBroadcastEntryGatePureLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastEntryGateNumOfPlayerData, OnBroadcastEntryGateNumOfPlayerData);
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToEntryQueuePlaceHolderApply, OnAddVisitorToEntryQueuePlaceHolderApply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToEntryQueueApply, this.OnAddVisitorToEntryQueueApply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.GetEntryGateDataApply, OnGetEntryGateDataApply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.EntryGateCheckInCDFinshedApply, OnEntryGateCheckInCDFinshedApply);

            //MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastEntryGateLevelOfPlayerData,
            //    this.OnBroadcastEntryGateLevelOfPlayerData);

            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastEntryGatePureLevelOfPlayerData, OnBroadcastEntryGatePureLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastEntryGateNumOfPlayerData, OnBroadcastEntryGateNumOfPlayerData);

            isRandomEntry = true;
            isOnlyOneEntry = false;

            foreach (var val in entryGates.Values)
            {
                val.Release();
            }
            entryGates.Clear();

            EntryVisitorsNum0 = null;
            EntryVisitorsNum1 = null;
            //broadcastEntityIDs.Clear();

            Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }
        }
        

        protected void InitEntryData()
        {
            var entryGateList = playerData.playerZoo.entryGateList;
            entryNum = entryGateList.Count;
            for (int i = 0; i < entryGateList.Count; i++)
            {
                var entryGateData = entryGateList[i];
                var cell = Config.ticketConfig.getInstace().getCell(entryGateData.entryID);
                int checkInCDVal = GetCheckinCDValMs(entryGateData.entryID, entryGateData.level);
                var entryGate = new EntryGate(entryGateData.entryID, entryGateData.level, checkInCDVal, cell.touristwalkinto, cell.maxnumofperqueue);
                this.entryGates.Add(entryGateData.entryID, entryGate);
            }
        }

        protected void OnAddVisitorToEntryQueueApply(Message msg)
        {
            var _msg = msg as AddVisitorToEntryQueueApply;
            LogWarp.LogFormat("{0} EntryGateModule recv {1}", _msg.entityID, _msg);
            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0} EntryGateModule recv {1}", _msg.entityID, _msg);

            EntryGate entryGate = null;
            if (!this.entryGates.TryGetValue(_msg.entryID, out entryGate))
            {
#if UNITY_EDITOR
                string e = string.Format("{0} 找不到入口ID {1}", _msg.entityID, _msg.entryID);
                throw new System.Exception(e);
#endif
            }

            int indexInQueue = entryGate.AddVisitorToQueue(_msg.entityID);
#if UNITY_EDITOR
            if (indexInQueue == Const.Invalid_Int)
            {
                string e = string.Format("{0} AddVisitorToQueue {1} 失败", _msg.entityID, _msg.entryID);
                throw new System.Exception(e);
            }
#endif
            AddVisitorToEntryQueueReply.Send(_msg.entityID, indexInQueue);
        }

        protected void OnGetEntryGateDataApply(Message msg)
        {
            var _msg = msg as GetEntryGateDataApply;

            EntryGate entryGate = null;
            if (!this.entryGates.TryGetValue(_msg.entryID, out entryGate))
            {
#if UNITY_EDITOR
                string e = string.Format("{0} 找不到入口ID {1}", _msg.entityID, _msg.entryID);
                throw new System.Exception(e);
#endif
            }
           
            GetEntryGateDataReply.Send(_msg.entityID, entryGate);
        }

        protected void OnEntryGateCheckInCDFinshedApply(Message msg)
        {
            var _msg = msg as EntryGateCheckInCDFinshedApply;
            EntryGate entryGate = null;
            if (!this.entryGates.TryGetValue(_msg.entryID, out entryGate))
            {
#if UNITY_EDITOR
                string e = string.Format("{0} 找不到入口ID {1}", _msg.entityID, _msg.entryID);
                throw new System.Exception(e);
#endif
            }

            bool retCode = entryGate.RemoveVisitorFromQueue(_msg.entityID);
#if UNITY_EDITOR   
            if (!retCode)
            {
                string e = string.Format("{0} cd结束, 入口{1}队伍中没有它", _msg.entityID, _msg.entryID);
                throw new System.Exception(e);
            }
#endif

            EntryGateCheckInCDFinshedReply.Send(_msg.entityID);

            //通知队伍中除了自己之外已经有正式位置的往前移动一步
            var sendMsg = BroadcastForwardOneStepInQueue.Send((int)(GameMessageDefine.BroadcastForwardOneStepInEntryGateQueue),
                _msg.entityID, _msg.entryID);
            DebugFile.GetInstance().WriteKeyFile(string.Format("entry_{0}", _msg.entryID), "{0} EntryGateModule {1}", _msg.entityID, sendMsg);
            AlterValue(_msg.entryID);
        }

        protected void OnAddVisitorToEntryQueuePlaceHolderApply(Message msg)
        {
            var _msg = msg as AddVisitorToEntryQueuePlaceHolderApply;
            LogWarp.LogFormat("{0} EntryGateModule recv {1}", _msg.entityID, _msg);
            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0} EntryGateModule recv {1}", _msg.entityID, _msg);
            bool result = false;
            int entryID = Const.Invalid_Int;
            int indexInQueue = Const.Invalid_Int;

            this.idleGates.Clear();
            if (this.isOnlyOneEntry)
            {
                var exitGate = entryGates[1];
                if (!exitGate.IsQueueFull())
                {
                    this.idleGates.Add(exitGate);
                }
                //保留最后一个遍历的
                entryID = exitGate.entryID;
            }
            else
            {
                foreach (var kv in this.entryGates)
                {
                    if (!kv.Value.IsQueueFull())
                    {
                        this.idleGates.Add(kv.Value);
                    }
                    //保留最后一个遍历的
                    entryID = kv.Value.entryID;
                }
            }

            //LogWarp.LogFormat("还能排队的入口数{0}", idleEntrys.Count);
            EntryGate idleGate = null;
            string pathName = null;
            if (idleGates.Count > 0)
            {
                int rVal = 0;
                if (isRandomEntry)
                {
                    rVal = Random.Range(0, idleGates.Count);
                }
                idleGate = idleGates[rVal];
                result = true;
                entryID = idleGate.entryID;
                pathName = idleGate.pathName;
                idleGate.AddVisitorPlaceHolder(_msg.entityID);
                LogWarp.LogFormat("给出的排队位{0}", indexInQueue);
            }

            //AddVisitorToEntryQueuePlaceHolderReply.Send(result, _msg.entityID, pathName, indexInQueue, entryID);
            AddVisitorToEntryQueuePlaceHolderReply.Send(result, _msg.entityID, pathName, entryID);
            //AddVisitorToEntryQueuePlaceHolderReply.Send(false, _msg.entityID, pathName, entryID);
            AlterValue(entryID);

        }

        protected void OnBroadcastEntryGatePureLevelOfPlayerData(Message msg)
        {
            var _msg = msg as SetDetailValueOfPlayerData;
            int entryID = _msg.detailVal;
            int deltaLevel = _msg.deltaVal;

            EntryGate entryGate = null;
            if (!entryGates.TryGetValue(entryID, out entryGate))
            {
                string e = string.Format("入口升级异常{0}", entryID);
                throw new System.Exception(e);
            }
            entryGate.level += deltaLevel;
            var cell = Config.ticketConfig.getInstace().getCell(entryID);
            entryGate.checkInCDVal = GetCheckinCDValMs(entryID, entryGate.level);
        }

        protected void OnBroadcastEntryGateNumOfPlayerData(Message msg)
        {
            //这里其实可以不需要消息内容，只需要和PlayerData中维护的入口数据同步即可
            entryNum = playerData.playerZoo.numEntryGate;
            for(int i = 0; i < playerData.playerZoo.entryGateList.Count; i++)
            {
                EntryGateData entryGateData = playerData.playerZoo.entryGateList[i];
                EntryGate entryGate = null;
                if (!entryGates.TryGetValue(entryGateData.entryID, out entryGate))
                {
                    var cell = Config.ticketConfig.getInstace().getCell(entryGateData.entryID);
                    int checkInCDVal = GetCheckinCDValMs(entryGateData.entryID, 1);
                    entryGate = new EntryGate(entryGateData.entryID, 1, checkInCDVal, cell.touristwalkinto, cell.maxnumofperqueue);
                    this.entryGates.Add(entryGateData.entryID, entryGate);
                }
            }
            // 需要 设置对应的售票口的显隐
            var _msg = msg as SetValueOfPlayerData;
            int entryID = _msg.channelID;
            HideExitGateForbidGameObject(entryID);

        }

        //protected void OnBroadcastEntryGateLevelOfPlayerData(Message msg)
        //{
        //    //检查入口数量是否增加,CD用最新的值
        //    int entryNum = GetEntryNum();
        //    int checkInCDVal =(int) GetChinkinCDValMs();
        //    var sortEntryGateIDs = GlobalDataManager.GetInstance().logicTableEntryGate.sortGateIDs;
        //    int entryGateID = Const.Invalid_Int;

        //    int currEntryNum = entryGates.Count;
        //    if (entryNum > currEntryNum)
        //    {
        //        for (int i = currEntryNum; i < entryNum; i++)
        //        {
        //            entryGateID = sortEntryGateIDs[i];
        //            var cell = Config.ticketConfig.getInstace().getCell(entryGateID);
        //            var entryGate = new EntryGate(entryGateID, checkInCDVal, cell.touristwalkinto, cell.maxnumofperqueue);
        //            entryGates.Add(entryGateID, entryGate);
                    //HideExitGateForbidGameObject(entryGateID);

        //        }
        //    }

        //    //全部重置checkInCDVal
        //    foreach (var val in entryGates.Values)
        //    {
        //        val.checkInCDVal = checkInCDVal;
        //    }
        //}

        /// <summary>
        /// 隐藏开启的出口对应的禁止牌
        /// </summary>
        /// <param name="number">出口ID</param>
        void HideExitGateForbidGameObject(int number)
        {

            GameObject gameObject = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.EntrySubscriptGB[number].Find("jinzhitongxing").gameObject;
            GameObject gameObject1 = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.EntrySubscriptGB[number].Find("damen_shoufei").gameObject;

            if (gameObject != null)
            {
                gameObject.SetActive(false);
                gameObject1.SetActive(true);
            }
        }


        //刷新出口人数：
        void AlterValue(int number)
        {
            var sortEntryGateIDs = GlobalDataManager.GetInstance().logicTableEntryGate.sortGateIDs;
            var maxnumofperqueue = Config.ticketConfig.getInstace().getCell(sortEntryGateIDs[number]).maxnumofperqueue;
            int surplus = maxnumofperqueue - entryGates[number].numOfQueue;//当前口剩余人数
            //LogWarp.LogError("测试  当前售票口为："+number +"   ,当前剩余空位置为= "+ surplus);
            GameObject gewei = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.EntrySubscriptGB[number].Find("damen_shoufei/gewei").gameObject;
            //GameObject shiwei = GlobalDataManager.GetInstance().EntrySubscriptGB[number].Find("damen_shoufei/shiwei").gameObject;
            //当前count不为0   则十位隐藏
            if (entryGates[number].numOfQueue > 0)
            {
                ////十位全部隐藏  只显示0
                //for (int i = 0; i < shiwei.transform.childCount; i++)
                //{
                //    shiwei.transform.GetChild(i).gameObject.SetActive(false);
                //}
                //shiwei.transform.GetChild(0).gameObject.SetActive(true);
                //
                for (int i = 0; i < gewei.transform.childCount; i++)
                {
                    gewei.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (surplus > -1 || surplus < maxnumofperqueue+1)
                {
                    gewei.transform.GetChild(surplus).gameObject.SetActive(true);
                }

            }
            else  //没有排队的
            {
                LogWarp.Log("//没有排队的");
                //for (int i = 0; i < shiwei.transform.childCount; i++)
                //{
                //    shiwei.transform.GetChild(i).gameObject.SetActive(false);
                //}
                for (int i = 0; i < gewei.transform.childCount; i++)
                {
                    gewei.transform.GetChild(i).gameObject.SetActive(false);
                }
                //shiwei.transform.GetChild(1).gameObject.SetActive(true);
                gewei.transform.GetChild(8).gameObject.SetActive(true);
            }

        }
    }

}

