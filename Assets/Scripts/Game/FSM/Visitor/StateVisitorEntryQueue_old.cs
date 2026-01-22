///*******************************************************************
//* FileName:     StateVisitorEntryQueue.cs
//* Author:       Fan Zheng Yong
//* Date:         2019-8-9
//* Description:  
//* other:    
//********************************************************************/


//using System.Collections.Generic;
//using UFrame;
//using UFrame.MessageCenter;
//using UnityEngine;
//using Logger;
//using Game.MessageCenter;
//using Game.GlobalData;
//using Game.Path.StraightLine;

//namespace Game
//{
//    /// <summary>
//    /// 负责游客在入口排队到进入动物园的过程。能走到这个状态的游客都能进入动物园。
//    /// 进入这个状态后,第一步需要走到自己在上一个状态获得的最初排队位。
//    /// 接着在Tick检查是否需要前进一步(前面有人进去了),如果需要就前进一步
//    /// 直到收到获准进入的消息,进入动物园转下一个状态
//    /// </summary>
//    public class StateVisitorEntryQueue : FSMState
//    {
//        /// <summary>
//        /// 是否转选择动物栏状态
//        /// </summary>
//        bool isToVisitorStateChoseLittleZoo = false;

//        ///// <summary>
//        ///// 是否到达排队位
//        ///// </summary>
//        //bool isArrivedQueuePos = false;

//        /// <summary>
//        /// 是否开始走入
//        /// </summary>
//        bool isBeginEnterZoo = false;

//        /// <summary>
//        /// 是否走到初始排队位
//        /// </summary>
//        bool isArrivedOrgPosOfQueue = false;

//        /// <summary>
//        /// 向前走计数
//        /// </summary>
//        int stepOfForward = 0;

//        public StateVisitorEntryQueue(int stateName, FSMMachine fsmCtr) :
//            base(stateName, fsmCtr) { }

//        public override void Enter(int preStateName)
//        {
//            //console.log("VisitorStateEntryQueue");
//            base.Enter(preStateName);

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} VisitorStateEntryQueue.Enter", entity.entityID);
//            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

//            isToVisitorStateChoseLittleZoo = false;

            
//            //this.isArrivedQueuePos = false;
//            this.isBeginEnterZoo = false;
//            isArrivedOrgPosOfQueue = false;
//            this.stepOfForward = 0;

//            MessageManager.GetInstance().Regist((int)GameMessageDefine.ZooEntryCDFinshed, this.OnZooEntryCDFinshed);
//            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

//            //走到自己最初的排队位置
//            //LogWarp.Log("去排队");
//            this.GoToOrgPosOfQueue();
//        }

//        public override void Leave()
//        {
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.ZooEntryCDFinshed, this.OnZooEntryCDFinshed);
//            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

//            base.Leave();
//        }

//        public override void AddAllConvertCond()
//        {
//            this.AddConvertCond((int)VisitorState.ChoseLittleZoo, this.ToVisitorStateChoseLittleZoo);
//        }

//        protected bool ToVisitorStateChoseLittleZoo()
//        {
//            return this.isToVisitorStateChoseLittleZoo;
//        }

//        public override void Tick(int deltaTimeMS)
//        {
//            if (this.CouldRun())
//            {
//                return;
//            }

//            ApproveForwardOne();
//            ApproveEnterZoo();
//        }

//        /// <summary>
//        /// 先走到最初的排队位置
//        /// </summary>
//        protected void GoToOrgPosOfQueue()
//        {
//            ////生成路径
//            ////起点自己的位置，前面10个单位为第一个位置
//            var fsm = this.fsmCtr as FSMMachineVisitor;
//            var entity = fsm.ownerEntity;
//            if(entity.indexInZooEntryQueue < 0)
//            {
//                string e = string.Format("{0} 初始排队位异常{1}", entity.entityID, entity.indexInZooEntryQueue);
//                throw new System.Exception(e);
//            }

//            float distance = 10 - entity.indexInZooEntryQueue;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 走向入口初始排队位{1}", entity.entityID, entity.indexInZooEntryQueue);
//            MoveForward(entity, distance);
//        }

//        //protected void OnArrived_old(Message msg)
//        //{
//        //    var _msg = msg as MessageArrived;
//        //    var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//        //    //自己的entity
//        //    if (_msg.followPath.ownerEntity.entityID != entity.entityID)
//        //    {
//        //        return;
//        //    }

//        //    //转选择动物栏
//        //    if (_msg.followPath.isArrivedEnd && isBeginEnterZoo)
//        //    {
//        //        //在这个状态选择动物栏，必然要去第一组
//        //        //var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//        //        entity.wouldGotoBuildingGroupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
//        //        //ZooEntry 的id 是1000
//        //        entity.stayBuildingID = 1000;
//        //        this.isToVisitorStateChoseLittleZoo = true;
//        //        //todo
//        //        //为什么不return？
//        //    }

//        //    if (_msg.followPath.isArrivedEnd && !isArrivedQueuePos)
//        //    {
//        //        this.isArrivedQueuePos = true;
//        //    }

//        //}

//        protected void OnArrived(Message msg)
//        {
//            var _msg = msg as MessageArrived;
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            //自己的entity
//            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
//            {
//                return;
//            }
            
//            //判定是否走到初始排队位
//            if (_msg.followPath.isArrivedEnd && !isArrivedOrgPosOfQueue)
//            {
//                isArrivedOrgPosOfQueue = true;
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 到达初始排队位 {1}",
//                    entity.entityID, entity.indexInZooEntryQueue);

//                return;
//            }

//            ////走到初始排队位，走到完排队位，并且获准进入
//            if (_msg.followPath.isArrivedEnd &&
//                this.isArrivedOrgPosOfQueue &&
//                this.stepOfForward == entity.indexInZooEntryQueue &&
//                entity.isApproveEnterZoo &&
//                this.isBeginEnterZoo
//            )
//            {
//                //在这个状态选择动物栏，必然要去第一组
//                entity.stayGroupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
//                //ZooEntry 的id 是1000
//                entity.stayBuildingID = 1000;
//                this.isToVisitorStateChoseLittleZoo = true;
//                return;
//            }
//        }

//        protected void OnZooEntryCDFinshed(Message msg)
//        {
//            var _msg = msg as ZooEntryCDFinshed;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            //自己的entity
//            if (_msg.entityID != entity.entityID)
//            {
//                return;
//            }
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在 StateVisitorEntryQueue 收到 {1}", entity.entityID, _msg);
//            if (_msg.isHead)
//            {
//                entity.isApproveEnterZoo = true;
//            }
//            else
//            {
//                LogWarp.Log("State EntryQueue rev isApproveForwardOne");
//            }

//            entity.numOfZooEntryQueueForwardOne++;
//        }

//        ///// <summary>
//        ///// 获准进入,走进去
//        ///// </summary>
//        //protected void ApproveEnterZoo_old()
//        //{
//        //    var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//        //    if (!isArrivedQueuePos || !entity.isApproveEnterZoo || isBeginEnterZoo)
//        //    {
//        //        return;
//        //    }



//        //    isBeginEnterZoo = true;

//        //    LogWarp.Log("走到排队位, 并获准进入");
//        //    //MoveForward(entity, 30f);
//        //    //从全局表中获取FirstPathOfZoo中对应的path的起点为终点
//        //    GotoFirstPathOfZoo(entity);

//        //}

//        protected void ApproveEnterZoo()
//        {
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            ////走到初始排队位，走到完排队位，并且获准进入
//            if (!this.isArrivedOrgPosOfQueue || 
//                this.stepOfForward != entity.indexInZooEntryQueue || 
//                !entity.isApproveEnterZoo ||
//                entity.followPath.IsRunning() ||
//                isBeginEnterZoo
//            )
//            {
//                return;
//            }

//            isBeginEnterZoo = true;
//            LogWarp.Log("走到排队位, 并获准进入");
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID,
//                "{0} 走到初始排队位，走到完排队位，并且获准进入, isArrivedOrgPosOfQueue={1}" +
//                " stepOfForward = {2} " +
//                " indexInZooEntryQueue= {3}" +
//                " isApproveEnterZoo = {4}" +
//                " entity.followPath.IsRunning = {5}"
//                , entity.entityID, isArrivedOrgPosOfQueue, stepOfForward, entity.indexInZooEntryQueue,
//                entity.isApproveEnterZoo, entity.followPath.IsRunning());

//            //MoveForward(entity, 30f);
//            //从全局表中获取FirstPathOfZoo中对应的path的起点为终点
//            GotoFirstPathOfZoo(entity);

//        }

//        ///// <summary>
//        ///// 没有在移动，需要移动就像前移动1
//        ///// </summary>
//        //protected void ApproveForwardOne_old()
//        //{
//        //    var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//        //    //LogWarp.LogFormat("ApproveForwardOne {0}, {1}, {2}", isArrivedQueuePos, entity.isApproveForwardOne, entity.followPath.IsRunning());

//        //    //走到排队位，进一步计数>0
//        //    if (!isArrivedQueuePos || entity.followPath.IsRunning() || entity.numOfZooEntryQueueForwardOne <= 0)
//        //    {
//        //        return;
//        //    }
//        //    entity.numOfZooEntryQueueForwardOne--;
//        //    MoveForward(entity, 1f);
//        //}

//        protected void ApproveForwardOne()
//        {
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            //LogWarp.LogFormat("ApproveForwardOne {0}, {1}, {2}", isArrivedQueuePos, entity.isApproveForwardOne, entity.followPath.IsRunning());

//            //走到初始排队位，进一步计数>0
//            if (!this.isArrivedOrgPosOfQueue ||
//                entity.followPath.IsRunning() ||
//                entity.numOfZooEntryQueueForwardOne <= 0 || 
//                entity.indexInZooEntryQueue == 0 ||
//                stepOfForward == entity.indexInZooEntryQueue
//            )
//            {
//                return;
//            }
            
//            entity.numOfZooEntryQueueForwardOne--;
//            stepOfForward++;
//            MoveForward(entity, 1f);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在队伍中向前走一步", entity.entityID);
//        }

//        protected void MoveForward(EntityVisitor entity, float distance)
//        {
//            var startPos = entity.position;
//            var endPos = startPos;
//            endPos.x -= distance;
//            entity.pathList.Clear();
//            entity.pathList.Add(startPos);
//            entity.pathList.Add(endPos);
//            float queueMoveSpeed = Config.globalConfig.getInstace().ZooVisitorQueueSpeed;
//            entity.followPath.Init(entity, entity.pathList.ToArray(), startPos, 0, queueMoveSpeed, false);
//            entity.followPath.Run();
//        }

//        protected void GotoFirstPathOfZoo(EntityVisitor entity)
//        {
//            var startPos = entity.position;
//            string pathName = Config.globalConfig.getInstace().FirstPathOfZoo;
//            //var endPos = PathManager.GetInstance().GetPath(pathName)[0];
//            //entity.pathList.Clear();
//            //entity.pathList.Add(startPos);
//            //entity.pathList.Add(endPos);
//            //entity.followPath.Init(entity, entity.pathList.ToArray(), startPos, 0, entity.moveSpeed, false);
//            //entity.followPath.Run();

//            EntityVisitor.GotoStartOfPath(entity, pathName);
//        }
//    }

//}
