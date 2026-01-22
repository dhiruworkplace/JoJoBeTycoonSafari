///*******************************************************************
//* FileName:     StateVisitorGotoZooEntry.cs
//* Author:       Fan Zheng Yong
//* Date:         2019-8-8
//* Description:  
//* other:    
//********************************************************************/


//using Game.Path.StraightLine;
//using UFrame;
//using UFrame.MessageCenter;
//using Logger;
//using Game.MessageCenter;

//namespace Game
//{
//    /// <summary>
//    /// 负责刷出游客到走到空闲入口，或者没有空闲入口离开的过程。
//    /// 去走那条路是由ZooCheckinModule决定，这里负责请求，并接收返回。
//    /// 没有空闲离开走哪条路也是。
//    /// </summary>
//    public class StateVisitorGotoZooEntry : FSMState
//    {
//        bool isToVisitorStateEntryQueue = false;
//        bool isToVisitorStateLeaveZooEntryQueueFull = false;

//        bool isStartGotoEntry = false;
//        public StateVisitorGotoZooEntry(int stateName, FSMMachine fsmCtr):
//            base(stateName, fsmCtr) { }

//        public override void Enter(int preStateName)
//        {
//            //LogWarp.Log("VisitorGotoZooEntry.Enter");
//            base.Enter(preStateName);

//            isToVisitorStateLeaveZooEntryQueueFull = false;
//            isToVisitorStateEntryQueue = false;
//            isStartGotoEntry = false;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} VisitorGotoZooEntry.Enter", entity.entityID);
//            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            
//            entity.Reset();

//            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToEntryQueueResult, this.OnAddVisitorToEntryQueueResult);
//            MessageManager.GetInstance().Regist((int)GameMessageDefine.ZooEntryCDFinshed, this.OnZooEntryCDFinshed);
//            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

//            var toMsg = AddVisitorToEntryQueue.Send(entity.entityID);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态，发送{1}", entity.entityID, toMsg);
//        }

//        public override void Tick(int deltaTimeMS)
//        {

//        }

//        public override void Leave()
//        {
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToEntryQueueResult, this.OnAddVisitorToEntryQueueResult);
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.ZooEntryCDFinshed, this.OnZooEntryCDFinshed);
//            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

//            base.Leave();
//        }

//        public override void AddAllConvertCond()
//        {
//            this.AddConvertCond((int)VisitorState.EntryQueue, this.ToVisitorStateEntryQueue);
//            this.AddConvertCond((int)VisitorState.LeaveZooEntryQueueFull, this.ToVisitorStateLeaveZooEntryQueueFull);
//        }

//        protected bool ToVisitorStateEntryQueue()
//        {
//            return this.isToVisitorStateEntryQueue;
//        }

//        protected bool ToVisitorStateLeaveZooEntryQueueFull()
//        {
//            return this.isToVisitorStateLeaveZooEntryQueueFull;
//        }

//        protected void OnAddVisitorToEntryQueueResult(Message msg)
//        {
//            var _msg = msg as AddVisitorToEntryQueueResult;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            if (_msg.entityID != entity.entityID)
//            {
//                return;
//            }

//            //无论成功失败，都有入口ID
//            entity.zooEntryID = _msg.entryID;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态 收到{1}", entity.entityID, msg);
//            if (!_msg.result)
//            {
//                LogWarp.Log("入口所有排队满了, 准备离开, 转离开");
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态 收到{1}入口所有排队满了, 准备离开, 转离开", entity.entityID, msg);
//                this.isToVisitorStateLeaveZooEntryQueueFull = true;
//                return;
//            }

//            entity.indexInZooEntryQueue = _msg.indexInQueue;
//            //var path = PathManager.GetInstance().GetPath(_msg.pathName);
//            //DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态 收到{1}，准备沿着{2} 走", entity.entityID, msg, _msg.pathName);
//            //entity.position = path[0];

//            //entity.followPath.Init(entity, path.ToArray(), entity.position, 0, entity.moveSpeed, false);
//            //entity.followPath.Run();

//            isStartGotoEntry = true;
//            EntityVisitor.GodownPath(entity, _msg.pathName, true);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态 收到{1}，准备沿着{2} 走", entity.entityID, msg, _msg.pathName);
//        }

//        protected void OnArrived(Message msg)
//        {
//            var _msg = (MessageArrived)msg;
//            //自己的entity
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
//            {
//                return;
//            }

//            //到达终点
//            if (_msg.followPath.isArrivedEnd && isStartGotoEntry)
//            {
//                //转排队状态
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态  准备转 VisitorStateEntryQueue状态", entity.entityID);
//                this.isToVisitorStateEntryQueue = true;
//            }
//        }

//        /// <summary>
//        /// 在这个状态收到这个消息，说明还在去入口排队位的路上，就收到了
//        /// 入口cd结束的消息。
//        /// </summary>
//        /// <param name="msg"></param>
//        protected void OnZooEntryCDFinshed(Message msg)
//        {
//            var _msg = msg as ZooEntryCDFinshed;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            //自己的entity
//            if (_msg.entityID != entity.entityID)
//            {
//                return;
//            }

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态  收到{1}", entity.entityID, _msg);
//            if (_msg.isHead)
//            {
//                entity.isApproveEnterZoo = true;
//            }
//            else
//            {
//                LogWarp.Log("State GotoZooEntry rev isApproveForwardOne");
//            }

//            entity.numOfZooEntryQueueForwardOne++;
//        }
//    }

//}
