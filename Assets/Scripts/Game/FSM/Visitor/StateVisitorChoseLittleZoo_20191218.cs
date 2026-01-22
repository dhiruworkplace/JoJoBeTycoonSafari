///*******************************************************************
//* FileName:     StateVisitorChoseLittleZoo.cs
//* Author:       Fan Zheng Yong
//* Date:         2019-8-9
//* Description:  
//* other:    
//********************************************************************/


//using System.Collections;
//using System.Collections.Generic;
//using UFrame;
//using UnityEngine;
//using Logger;
//using Game.GlobalData;
//using Game.MessageCenter;
//using UFrame.MessageCenter;
//using Game.Path.StraightLine;

//namespace Game
//{
//    /// <summary>
//    /// 选择动物栏逻辑
//    /// 门口排队结束后 或者 在观光位cd结束后 进入
//    /// 如果是在排队结束后进入，这时游客已经走到第一个点上。
//    /// 如果是从观光位进入，这时游客还在观光位上
//    /// 
//    /// 1.知道当前所在建筑位置的基础上，从组概率表中，获得要去的建筑。
//    /// 2.向动物栏模块请求进入
//    /// 3.根据请求返回结果做相应处理
//    /// (1)成功走入等待位, 转下一个状态,等待位 StateVisitorStayWaitSeat
//    /// (2)不成功请求组内其他的动物栏
//    /// (3)不成功请求下一个组
//    /// (4)组内，组外都没有可选的动物，转另一个状态,离开 StateVisitorLeaveNonLittleZoo
//    /// </summary>
//    public class StateVisitorChoseLittleZoo_20191218 : FSMState
//    {
//        bool isToStateVisitorStayWaitSeat = false;

//        bool isToStateVisitorLeaveNonLittleZoo = false;

//        bool isToStateVisitorCrossGroupPath = false;

//        bool isToStateVisitorLeaveFromZooEntry = false;

//        /// <summary>
//        /// 是否到达路的起点
//        /// </summary>
//        bool isArrivedStartOfPath = false;

//        /// <summary>
//        /// 是否到达动物栏
//        /// </summary>
//        bool isArrivedLittleZoo = false;

//        string pathOfGotoLittleZoo = "";

//        /// <summary>
//        /// 等待位的具体位置
//        /// </summary>
//        Vector3 waitPos;

//        /// <summary>
//        /// 暂存申请成功的动物栏
//        /// </summary>
//        int stayBuildingID;

//        /// <summary>
//        /// 暂存申请成功的动物栏所在的组
//        /// </summary>
//        int stayGroupID;
//        public StateVisitorChoseLittleZoo_20191218(int stateName, FSMMachine fsmCtr) :
//            base(stateName, fsmCtr)
//        { }

//        public override void Realse()
//        {
//            base.Realse();
//        }

//        public override void Enter(int preStateName)
//        {
//            //LogWarp.LogError("StateVisitorChoseLittleZoo.Enter");
//            base.Enter(preStateName);

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorChoseLittleZoo.Enter", entity.entityID);
//            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

//            isToStateVisitorStayWaitSeat = false;
//            isToStateVisitorLeaveNonLittleZoo = false;
//            isToStateVisitorCrossGroupPath = false;
//            isToStateVisitorLeaveFromZooEntry = false;
//            isArrivedLittleZoo = false;

//            isArrivedStartOfPath = false;
//            //if (preStateName == (int)VisitorState.EntryQueue)
//            //{
//            //    isArrivedStartOfPath = true;
//            //}
//            if (preStateName == (int)VisitorState.StayFirstPosInEntryQueue)
//            {
//                isArrivedStartOfPath = true;
//            }

//            entity.indexInWaitQueue = Const.Invalid_Int;
//            entity.isApproveVisitSeat = false;
//            entity.indexInVisitQueue = Const.Invalid_Int;

//            this.stayBuildingID = Const.Invalid_Int;
//            this.stayGroupID = Const.Invalid_Int;

//            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToLittleZooApplyReply, this.OnAddVisitorToLittleZooApplyReply);
//            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
//            MessageManager.GetInstance().Regist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);

//            //将去哪组逻辑
//            var msg = AddVisitorToLittleZooApply.Send(entity.entityID, this.preStateName == (int)VisitorState.StayFirstPosInEntryQueue);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Send {1}", entity.entityID, msg);
//            LogWarp.LogFormat("{0} Send {1}", entity.entityID, msg);
//        }

//        public override void Leave()
//        {
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToLittleZooApplyReply, this.OnAddVisitorToLittleZooApplyReply);
//            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);

//            base.Leave();
//        }

//        public override void AddAllConvertCond()
//        {
//            AddConvertCond((int)VisitorState.StayWaitSeat, this.ToStateVisitorStayWaitSeat);
//            AddConvertCond((int)VisitorState.LeaveNonLittleZoo, this.ToStateVisitorLeaveNonLittleZoo);
//            AddConvertCond((int)VisitorState.CrossGroupPath, ToStateVisitorCrossGroupPath);
//            AddConvertCond((int)VisitorState.LeaveFromZooEntry, ToStateVisitorLeaveFromZooEntry);

//        }

//        public override void Tick(int deltaTimeMS)
//        {
//        }

//        protected bool ToStateVisitorStayWaitSeat()
//        {
//            return this.isToStateVisitorStayWaitSeat;
//        }

//        protected bool ToStateVisitorLeaveNonLittleZoo()
//        {
//            return isToStateVisitorLeaveNonLittleZoo;
//        }

//        protected bool ToStateVisitorCrossGroupPath()
//        {
//            return isToStateVisitorCrossGroupPath;
//        }

//        protected bool ToStateVisitorLeaveFromZooEntry()
//        {
//            return isToStateVisitorLeaveFromZooEntry;
//        }



//        protected void OnAddVisitorToLittleZooApplyReply(Message msg)
//        {
//            var _msg = msg as AddVisitorToLittleZooApplyReply;
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            if (entity.entityID != _msg.entityID)
//            {
//                return;
//            }

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 收到了{1}, tickCount={2}",
//                entity.entityID, _msg, GameManager.GetInstance().tickCount);
//            LogWarp.LogFormat("{0} Recv {1}", entity.entityID, msg);
//            if (!_msg.result)
//            {
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 申请失败，准备离开", entity.entityID);
//                PrepareLeaveZoo(entity);
//                return;
//            }

//            this.stayBuildingID = _msg.littleZooID;
//            this.stayGroupID = _msg.groupID;
//            entity.indexInWaitQueue = _msg.indexInQueue;
//            if (_msg.isCrossGroup)
//            {
//                LogWarp.LogFormat("跨组了 {0} {1}", entity.stayBuildingID, _msg.littleZooID);
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 跨组了 {1} {2}", entity.entityID, entity.stayBuildingID, _msg.littleZooID);
//                isToStateVisitorCrossGroupPath = true;
//                entity.crossLittleZooIDs.Clear();
//                entity.crossLittleZooIDs.AddRange(_msg.crossLittleZooIDs);
//                return;
//            }

//            waitPos = _msg.waitPos;

//            this.pathOfGotoLittleZoo = EntityVisitor.GetPath(entity.stayBuildingID, _msg.littleZooID);
//            if (string.IsNullOrEmpty(pathOfGotoLittleZoo))
//            {
//                string e = string.Format("{0}没有找到路{1} -> {2} !!!!!!!!!!", entity.entityID, entity.stayBuildingID, _msg.littleZooID);
//                throw new System.Exception(e);
//            }
//            LogWarp.LogFormat("动物栏{0}, 等待位获得成功, 排号{1}， 路径{2}", _msg.littleZooID, _msg.indexInQueue, pathOfGotoLittleZoo);

//            if (this.isArrivedStartOfPath)
//            {
//                EntityVisitor.GodownPath(entity, pathOfGotoLittleZoo);
//                return;
//            }

//            EntityVisitor.GotoStartOfPath(entity, pathOfGotoLittleZoo);
//        }

//        protected void OnArrived(Message msg)
//        {
//            var _msg = msg as MessageArrived;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            //自己的entity
//            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
//            {
//                return;
//            }

//            //先判断走到起点
//            if (_msg.followPath.isArrivedEnd && !isArrivedStartOfPath)
//            {
//                isArrivedStartOfPath = true;
//                EntityVisitor.GodownPath(entity, pathOfGotoLittleZoo);
//                return;
//            }

//            if (_msg.followPath.isArrivedEnd && !isArrivedLittleZoo)
//            {
//                LogWarp.Log("到达动物栏,准备走向等待位");
//                isArrivedLittleZoo = true;

//                EntityVisitor.GotoNextPos(entity, this.waitPos);
//                return;
//            }

//            if (_msg.followPath.isArrivedEnd)
//            {
//                LogWarp.Log("到达动物栏等待位,转下一个状态");
//                isToStateVisitorStayWaitSeat = true;
//                entity.stayBuildingID = this.stayBuildingID;
//                entity.stayGroupID = this.stayGroupID;


//            }
//        }

//        /// <summary>
//        /// 在这个状态收到这个消息，被通知走到观光位，说明在去等待位的途中就收到了，
//        /// 直接设置标志位，在下一个状态处理
//        /// </summary>
//        /// <param name="msg"></param>
//        protected void OnWaitSeatToVisitSeat(Message msg)
//        {
//            var _msg = msg as WaitSeatToVisitSeat;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            //不是自己的
//            if (_msg.entityID != entity.entityID)
//            {
//                return;
//            }

//            LogWarp.LogFormat("途中收到去观光位 排位{0}", _msg.indexInVisitQueue);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID,
//                "{0}在状态StateVisitorChoseLittleZoo 收到{1}, tickCount={2}",
//                entity.entityID, msg, GameManager.GetInstance().tickCount);

//            entity.isApproveVisitSeat = true;
//            entity.indexInVisitQueue = _msg.indexInVisitQueue;
//            //entity.stayBuildingID = _msg.littleZooID;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID,
//                "{0}在状态StateVisitorChoseLittleZoo, 收到{1}, entity.followPath.isArrivedEnd={2}, tickCount={3}",
//                entity.entityID, msg, entity.followPath.isArrivedEnd, GameManager.GetInstance().tickCount);

//            if (entity.followPath.isArrivedEnd)
//            {
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0}在状态StateVisitorChoseLittleZoo, 并且处于到达状态 收到{1}",
//                    entity.entityID, msg);
//            }
//        }

//        protected void PrepareLeaveZoo(EntityVisitor entity)
//        {
//            LogWarp.Log("PrepareLeaveZoo");
//            if (entity.stayBuildingID == 1000)
//            {
//                isToStateVisitorLeaveFromZooEntry = true;
//                return;
//            }

//            isToStateVisitorLeaveNonLittleZoo = true;
//        }
//    }
//}

