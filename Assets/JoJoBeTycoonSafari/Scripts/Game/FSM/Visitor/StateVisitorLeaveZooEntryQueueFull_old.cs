///*******************************************************************
//* FileName:     StateVisitorLeaveZooEntryQueueFull.cs
//* Author:       Fan Zheng Yong
//* Date:         2019-8-14
//* Description:  
//* other:    
//********************************************************************/


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UFrame;
//using UFrame.MessageCenter;
//using Logger;
//using Game.Path.StraightLine;

//namespace Game
//{
//    public class StateVisitorLeaveZooEntryQueueFull : FSMState
//    {
//        public StateVisitorLeaveZooEntryQueueFull(int stateName, FSMMachine fsmCtr) :
//            base(stateName, fsmCtr) { }

//        public override void Enter(int preStateName)
//        {
//            //LogWarp.LogError("StateVisitorLeaveZooEntryQueueFull.Enter");
//            base.Enter(preStateName);

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorLeaveZooEntryQueueFull.Enter", entity.entityID);
//            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);
            
//            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

//            //走回去的路
//            var cell = Config.ticketConfig.getInstace().getCell(entity.zooEntryID.ToString());

//            var path = PathManager.GetInstance().GetPath(cell.queuefullpath);
//            entity.position = path[0];
//            entity.followPath.Init(entity, path.ToArray(), entity.position, 0, entity.moveSpeed, false);
//            entity.followPath.Run();
//        }

//        public override void Leave()
//        {
//            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
//            base.Leave();
//        }

//        public override void AddAllConvertCond()
//        {
//        }

//        public override void Tick(int deltaTimeMS)
//        {
//        }

//        protected void OnArrived(Message msg)
//        {
//            var _msg = (MessageArrived)msg;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            //自己的entity
//            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
//            {
//                return;
//            }

//            //到达终点
//            if (_msg.followPath.isArrivedEnd)
//            {
//                EntityManager.GetInstance().RemoveFromEntityMovables(entity);
//                // 回POOL
//                //LogWarp.LogError("游客回POOL");
//                //EntityVisitor.pool.Delete(entity);

                
//            }
//        }



//    }
//}
