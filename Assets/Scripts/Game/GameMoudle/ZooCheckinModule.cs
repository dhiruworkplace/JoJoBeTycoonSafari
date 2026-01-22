///*******************************************************************
//* FileName:     ZooCheckinModule.cs
//* Author:       Fan Zheng Yong
//* Date:         2019-8-9
//* Description:  
//* other:    
//********************************************************************/


//using System.Collections;
//using System.Collections.Generic;
//using UFrame;
//using UFrame.MessageCenter;
//using UnityEngine;
//using Logger;
//using Game.MessageCenter;

//namespace Game
//{
//    /// <summary>
//    /// 入口检票逻辑模块
//    /// 1.分配游客到哪个入口
//    /// 2.每个入口的排队的控制逻辑
//    /// </summary>
//    public class ZooCheckinModule : GameModule
//    {
//        /// <summary>
//        /// 所有入口
//        /// </summary>
//        public Dictionary<int, ZooEntry> zooEntrys = new Dictionary<int, ZooEntry>();

//        /// <summary>
//        /// 空闲的入口
//        /// </summary>
//        List<ZooEntry> idleEntrys = new List<ZooEntry>();

//        /// <summary>
//        /// 是否随机空闲入口
//        /// </summary>
//        bool isRandomEntry = true;

//        /// <summary>
//        /// 是否只开一个入口
//        /// </summary>
//        bool isOnlyOneEntry = false;


//        public ZooCheckinModule(int orderID) : base(orderID) { }

//        public override void Init()
//        {
//            // console.log("ZooCheckinMoudle Init()");
//            this.InitEntryData();
//            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToEntryQueue, this.OnAddVisitorToEntryQueue);
//            //this.Run();
//        }

//        public override void Realse()
//        {
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToEntryQueue, this.OnAddVisitorToEntryQueue);

//            isRandomEntry = true;
//            isOnlyOneEntry = false;

//            foreach(var val in zooEntrys.Values)
//            {
//                val.Realse();
//            }
//            zooEntrys.Clear();

//            foreach(var val in idleEntrys)
//            {
//                val.Realse();
//            }
//            idleEntrys.Clear();

//            Stop();
//        }

//        public override void Tick(int deltaTimeMS)
//        {
//            if (!this.CouldRun())
//            {
//                return;
//            }


//            //Tick每个入口的等待时间
//            this.TickEntryCD(deltaTimeMS);

//            //玩家的入口数量是动态增加的 满足条件增加entry,增加queue

//        }

//        protected void InitEntryData()
//        {
//            foreach (var kv in Config.ticketConfig.getInstace().AllData)
//            {
//                int entryID;
//                if (!int.TryParse(kv.Key, out entryID))
//                {
//                    LogWarp.LogError("表数据错误,ticket表的主键不是数字");
//                }
 
//                //LogWarp.LogFormat("entry {0} cd = {1}", entryID, kv.Value.speed * 1000);
//                var zooEntry = new ZooEntry(entryID, kv.Value.speed *1000, kv.Value.gotozooentry, kv.Value.maxnumofperqueue);
//                zooEntry.Run();

//                this.zooEntrys.Add(entryID, zooEntry);
//            }
//        }

//        protected void OnAddVisitorToEntryQueue(Message msg)
//        {
//            var _msg = msg as AddVisitorToEntryQueue;
//            bool result = false;
//            int entryID = Const.Invalid_Int;
//            int indexInQueue = Const.Invalid_Int;

//            this.idleEntrys.Clear();
//            if (this.isOnlyOneEntry)
//            {
//                var entry = zooEntrys[1];
//                if (!entry.IsQueueFull())
//                {
//                    this.idleEntrys.Add(entry);
//                }
//                //保留最后一个遍历的
//                entryID = entry.entryID;
//            }
//            else
//            {
//                foreach (var kv in this.zooEntrys)
//                {
//                    if (!kv.Value.IsQueueFull())
//                    {
//                        this.idleEntrys.Add(kv.Value);
//                    }
//                    //保留最后一个遍历的
//                    entryID = kv.Value.entryID;
//                }
//            }

//            //LogWarp.LogFormat("还能排队的入口数{0}", idleEntrys.Count);
//            ZooEntry idleEntry = null;
//            string pathName = null;
//            if (idleEntrys.Count > 0)
//            {
//                //int rVal = Random.Range(0, idleEntrys.Count);
//                int rVal = 0;
//                if (isRandomEntry)
//                {
//                    rVal = Random.Range(0, idleEntrys.Count);
//                }
//                idleEntry = idleEntrys[rVal];
//                result = true;
//                entryID = idleEntry.entryID;
//                pathName = idleEntry.entryPathNameFromCar;
//                idleEntry.AddVisitorToQueue(_msg.entityID);
//                indexInQueue = idleEntry.GetIndexInQueue() - 1;
//                LogWarp.LogFormat("给出的排队位{0}", indexInQueue);
//            }

//            AddVisitorToEntryQueueResult.Send(result, _msg.entityID, pathName, indexInQueue, entryID);
//        }
        
//        protected void TickEntryCD(int deltaTimeMS)
//        {
//            foreach (var kv in this.zooEntrys)
//            {
//                var entry = kv.Value;
//                entry.Tick(deltaTimeMS);
//                //cd结束, 有人就进一个人
//                if (entry.entryCD.IsFinish() && entry.GetQueueCount() > 0)
//                {
//                    entry.entryCD.Reset();
//                    LogWarp.LogFormat("entry {0} cd 结束，且有人排队，进一个人begin", entry.entryID);
//                    //第一个出队，进入
//                    int entityID = entry.RemoveVisitorFromQueue();
//                    ZooEntryCDFinshed.Send(entry.entryID, entityID, true);
//                    //后面的前进一个单位
//                    foreach (var val in entry.visitorQueue)
//                    {
//                        ZooEntryCDFinshed.Send(entry.entryID, val, false);
//                    }
//                    LogWarp.LogFormat("entry {0} cd 结束，且有人排队，进一个人end", entry.entryID);
//                }
//            }
//        }
//    }

//}

