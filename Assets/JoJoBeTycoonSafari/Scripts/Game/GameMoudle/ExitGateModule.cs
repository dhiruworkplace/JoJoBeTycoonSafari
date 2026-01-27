/*******************************************************************
* FileName:     ExitGateModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-25
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame;
using UFrame.BehaviourFloat;
using ZooGame.Path.StraightLine;
using UFrame.MessageCenter;
using ZooGame.MiniGame;
using Config;
using Logger;
using ZooGame.MessageCenter;
using UFrame.EntityFloat;
using ZooGame.GlobalData;
using UFrame.OrthographicCamera;

namespace ZooGame
{
    public partial class ExitGateModule : GameModule
    {
        /// <summary>
        /// 所有出口
        /// </summary>
        public Dictionary<int, ExitGate> exitGates = new Dictionary<int, ExitGate>();

        /// <summary>
        /// 空闲的出口
        /// </summary>
        List<ExitGate> idleExitGates = new List<ExitGate>();

        /// <summary>
        /// 是否随机空闲入口
        /// </summary>
        bool isRandomEntry = true;

        /// <summary>
        /// 是否只开一个入口
        /// </summary>
        bool isOnlyOneEntry = false;

        protected int ExitVisitorsNum = 0;
        int lastExitVisitorsNum0 = 0;
        int lastExitVisitorsNum1 = 0;
        /// <summary>
        /// 出口游客排队个位数
        /// </summary>
        GameObject ExitVisitorsNum0;

        /// <summary>
        /// 出口游客排队十位数
        /// </summary>
        GameObject ExitVisitorsNum1;





        List<int> broadcastEntityIDs = new List<int>();

        Queue<EntityVisitor> shuttleVisitorQueue = new Queue<EntityVisitor>();
        //int maxShuttleVisitorQueue = 0;
        int maxShuttleInterval = 0;
        int shuttleaccumulativeTime = 0;
        List<ShuttleVisitor> shuttleVisitorList = new List<ShuttleVisitor>();

        public ExitGateModule(int orderID) : base(orderID) { }

        public override void Init()
        {
            InitExitGateData();

            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToExitGateQueueApply,
                this.OnAddVisitorToExitGateQueueApply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastExitGateLevelOfPlayerData,
                this.OnBroadcastExitGateLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SendExitGateCheckinCDFinish,
                this.OnSendExitGateCheckinCDFinish);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SendExitGateCheckinCDFinishReply,
                OnSendExitGateCheckinCDFinishReply);
            //MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastChangeBuff_SetExitEntryCDVal,
            //    this.OnBroadcastChangeBuffSetExitEntryCDVal);
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToExitGateQueueApply,
                this.OnAddVisitorToExitGateQueueApply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastExitGateLevelOfPlayerData,
                this.OnBroadcastExitGateLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SendExitGateCheckinCDFinish,
                this.OnSendExitGateCheckinCDFinish);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SendExitGateCheckinCDFinishReply,
                OnSendExitGateCheckinCDFinishReply);
            //MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastChangeBuff_SetExitEntryCDVal,
            //    this.OnBroadcastChangeBuffSetExitEntryCDVal);

            shuttleVisitorQueue.Clear();
            //maxShuttleVisitorQueue = 0;
            maxShuttleInterval = 0;
            shuttleaccumulativeTime = 0;
            shuttleVisitorList.Clear();

            foreach (var val in exitGates.Values)
            {
                val.Realse();
            }
            exitGates.Clear();
            broadcastEntityIDs.Clear();

            ExitVisitorsNum0 = null;
            ExitVisitorsNum1 = null;
            this.Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            TickShuttleVisistorNum(deltaTimeMS);
        }

        protected void InitExitGateData()
        {
#if DEBUG_VISIT
            int entryNum = GetEntryNum(2000);
            int checkInCDVal = GetChinkinCDValMs(2000);
#else
            int entryNum = GetEntryNum();
            int checkInCDVal = GetChinkinCDValMs();
#endif

            var sortExitGateIDs = GlobalDataManager.GetInstance().logicTableExitGate.sortExitGateIDs;
            int exitGateID = Const.Invalid_Int;

            for (int i = 0; i < entryNum; i++)
            {
                exitGateID = sortExitGateIDs[i];
                var cell = Config.exitgateConfig.getInstace().getCell(exitGateID);
                var exitGate = new ExitGate(exitGateID, checkInCDVal, cell.positiveexitgate, cell.positiveexitgate, cell.maxnumofperqueue);

                exitGates.Add(exitGateID, exitGate);
            }

            shuttleVisitorQueue.Clear();
            //maxShuttleVisitorQueue = ExitGateModule.GetMaxShuttleVisitor();
            maxShuttleInterval = Math_F.FloatToInt1000(Config.globalConfig.getInstace().MaxShuttleInterval);
            shuttleaccumulativeTime = 0;
            shuttleVisitorList.Clear();
        }

        protected void OnAddVisitorToExitGateQueueApply(Message msg)
        {
            var _msg = msg as AddVisitorToExitGateQueueApply;
            LogWarp.LogFormat("{0} ExitGateModule recv {1}", _msg.entityID, _msg);
            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0} ExitGateModule recv {1}", _msg.entityID, _msg);
            bool result = false;
            int entryID = Const.Invalid_Int;
            int indexInQueue = Const.Invalid_Int;

            this.idleExitGates.Clear();
            if (this.isOnlyOneEntry)
            {
                var exitGate = exitGates[1];
                if (!exitGate.IsQueueFull())
                {
                    this.idleExitGates.Add(exitGate);
                }
                //保留最后一个遍历的
                entryID = exitGate.entryID;
            }
            else
            {
                foreach (var kv in this.exitGates)
                {
                    if (!kv.Value.IsQueueFull())
                    {
                        this.idleExitGates.Add(kv.Value);
                    }
                    //保留最后一个遍历的
                    entryID = kv.Value.entryID;
                }
            }

            //LogWarp.LogFormat("还能排队的入口数{0}", idleEntrys.Count);
            ExitGate idleEntry = null;
            string pathName = null;
            if (idleExitGates.Count > 0)
            {
                int rVal = 0;
                if (isRandomEntry)
                {
                    rVal = Random.Range(0, idleExitGates.Count);
                }
                idleEntry = idleExitGates[rVal];
                result = true;
                entryID = idleEntry.entryID;
                pathName = idleEntry.leftEntryPathName;
                idleEntry.AddVisitorToQueue(_msg.entityID);
                indexInQueue = idleEntry.GetLastIndexInQueue();
                LogWarp.LogFormat("给出的排队位{0}", indexInQueue);

            }

            AddVisitorToExitGateQueueApplyReply.Send(result, _msg.entityID, pathName, indexInQueue, entryID);
            AlterValue(entryID);

            //模型到达出口   调用新手引导的内容  显示步骤13
            //if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            //{
            //    UIGuide[] pAllObjects = (UIGuide[])Resources.FindObjectsOfTypeAll(typeof(UIGuide));
            //    if (pAllObjects[0].procedure <12)
            //    {
            //        pAllObjects[0].VerdictAnimate(5f);
            //        pAllObjects[0].VerdictPause(5f);
            //    }
            //}

        }


        //刷新出口人数：
        void AlterValue(int number )
        {
            int surplus = 10 - exitGates[number].visitorQueue.Count;//paiduirenshu
            string geweiPath = Config.exitgateConfig.getInstace().getCell(number).gameobjectpath + "/gewei";
            string shiweiPath = Config.exitgateConfig.getInstace().getCell(number).gameobjectpath + "/shiwei";

            GameObject gewei = GameObject.Find(geweiPath);
            GameObject shiwei = GameObject.Find(shiweiPath);
            //当前count不为0   则十位隐藏
            if (exitGates[number].visitorQueue.Count>0)
            {
                //十位全部隐藏  只显示0
                for (int i = 0; i < shiwei.transform.childCount; i++)
                {
                    shiwei.transform.GetChild(i).gameObject.SetActive(false);
                }
                shiwei.transform.GetChild(0).gameObject.SetActive(true);
                //
                for (int i = 0; i < gewei.transform.childCount; i++)
                {
                    gewei.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (surplus > -1|| surplus < 10)
                {
                    gewei.transform.GetChild(surplus).gameObject.SetActive(true);
                }

            }
            else  //没有排队的
            {
                Debug.Log("//没有排队的");
                for (int i = 0; i < shiwei.transform.childCount; i++)
                {
                    shiwei.transform.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = 0; i < gewei.transform.childCount; i++)
                {
                    gewei.transform.GetChild(i).gameObject.SetActive(false);
                }
                shiwei.transform.GetChild(1).gameObject.SetActive(true);
                gewei.transform.GetChild(0).gameObject.SetActive(true);

            }

        }

        protected void OnBroadcastExitGateLevelOfPlayerData(Message msg)
        {
            //检查入口数量是否增加
            int entryNum = GetEntryNum();
            int checkInCDVal = GetChinkinCDValMs();
            var sortExitGateIDs = GlobalDataManager.GetInstance().logicTableExitGate.sortExitGateIDs;
            int exitGateID = Const.Invalid_Int;

            int currEntryNum = exitGates.Count;
            if (entryNum > currEntryNum)
            {
                for (int i = currEntryNum; i < entryNum; i++)
                {
                    exitGateID = sortExitGateIDs[i];
                    var cell = Config.exitgateConfig.getInstace().getCell(exitGateID);
                    var exitGate = new ExitGate(exitGateID, checkInCDVal, cell.positiveexitgate, cell.positiveexitgate, cell.maxnumofperqueue);
                    exitGates.Add(exitGateID, exitGate);
                    HideExitGateForbidGameObject(exitGateID);
                }
            }

            //全部重置checkInCDVal
            foreach (var val in exitGates.Values)
            {
                val.checkInCDVal = checkInCDVal;
            }
        }
        /// <summary>
        /// 隐藏开启的出口对应的禁止牌
        /// </summary>
        /// <param name="number">出口ID</param>
        void HideExitGateForbidGameObject(int number)
        {
            string gameObjectPath = Config.exitgateConfig.getInstace().getCell(number).gameobjectpath + "/ggchezhan_03";
            GameObject gameObject = GameObject.Find(gameObjectPath);
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }

        protected void OnSendExitGateCheckinCDFinish(Message msg)
        {
            var _msg = msg as SendExitGateCheckinCDFinish;
            var exitGate = exitGates[_msg.entryID];
            int entityID = exitGate.RemoveVisitorFromQueue();
            //int entityID = exitGate.visitorQueue.Dequeue();
            LogWarp.LogFormat("{0} Parkingmodule recv {1}", _msg.entityID, _msg);
            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0} Parkingmodule recv {1}", _msg.entityID, _msg);
#if UNITY_EDITOR
            if (entityID != _msg.entityID)
            {
                string e = string.Format("出口排队异常 entityID={0}, _msg.entityID={1}", entityID, _msg.entityID);
                throw new System.Exception(e);
            }
#endif
            broadcastEntityIDs.Clear();
            foreach (var val in exitGate.visitorQueue)
            {
                this.broadcastEntityIDs.Add(val);
                DebugFile.GetInstance().WriteKeyFile(val, "{0} BroadcastForwardOneStepInExitGateQueue by {1}", val, entityID);
            }

            BroadcastForwardOneStepInExitGateQueue.Send(broadcastEntityIDs);
            SendExitGateCheckinCDFinishReply.Send(_msg.entityID);
            AlterValue(_msg.entryID);
        }

        protected void OnSendExitGateCheckinCDFinishReply(Message msg)
        {
            var _msg = msg as SendExitGateCheckinCDFinishReply;
            //生成摆渡车计数
            EntityVisitor entity = EntityManager.GetInstance().GetEntityMovable(_msg.entityID) as EntityVisitor;
            shuttleVisitorQueue.Enqueue(entity);
#if DEBUG_VISIT
            BroadcastNum.Send((int)GameMessageDefine.BroadcastShuttleVisistorNum, shuttleVisitorQueue.Count, 0f, 0);
#endif
        }

        protected void TickShuttleVisistorNum(int deltaTimeMS)
        {
            this.shuttleaccumulativeTime += deltaTimeMS;
            if (shuttleVisitorQueue.Count >= ExitGateModule.GetMaxShuttleVisitor() && shuttleaccumulativeTime >= maxShuttleInterval)
            {
                shuttleVisitorList.Clear();
                for (int i = 0; i < ExitGateModule.GetMaxShuttleVisitor(); i++)
                {
                    var entity = shuttleVisitorQueue.Dequeue();
                    EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                    var shuttleVisitor = new ShuttleVisitor();
                    shuttleVisitor.Init(entity.entityID, (EntityFuncType)entity.entityFuncType);
                    shuttleVisitorList.Add(shuttleVisitor);
                }
                //MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnShuttle);
                SpawnShuttle.Send(shuttleVisitorList);
                shuttleaccumulativeTime -= maxShuttleInterval;
#if DEBUG_VISIT
                BroadcastNum.Send((int)GameMessageDefine.BroadcastShuttleVisistorNum, shuttleVisitorQueue.Count, 0f, 0);
#endif
            }
        }

        
    }

}