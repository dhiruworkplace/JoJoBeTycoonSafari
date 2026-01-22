//using Game.GlobalData;
//using Game.MessageCenter;
//using Game.Path.StraightLine;
//using Logger;
//using System.Collections;
//using System.Collections.Generic;
//using UFrame;
//using UFrame.MessageCenter;
//using UnityEngine;

//namespace Game
//{
//    /// <summary>
//    /// 构建跨组的路径
//    /// </summary>
//    public class StateVisitorCrossGroupPath : FSMState
//    {
//        bool isToStateVisitorStayWaitSeat = false;

//        public StateVisitorCrossGroupPath(int stateName, FSMMachine fsmCtr) :
//            base(stateName, fsmCtr) { }

//        public override void Realse()
//        {
//            base.Realse();
//        }

//        public override void Enter(int preStateName)
//        {
//            base.Enter(preStateName);
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorCrossGroupPath.Enter", entity.entityID);
//            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

//            isToStateVisitorStayWaitSeat = false;

//#if CrossGroup_Optimize
//            entity.pathList.Clear();
//            AddEntityPosToPath(entity, entity.pathList);
//            string crossPathName = string.Format("{0}-{1}",
//                entity.crossLittleZooIDs[0], entity.crossLittleZooIDs[entity.crossLittleZooIDs.Count - 1]);
//            var crossPath = CalcPathManager.GetInstance().GetNormalPath(crossPathName);
//            if (crossPath != null && crossPath.Count > 0)
//            {
//                entity.pathList.AddRange(crossPath);
//            }
//            else
//            {
//                crossPath = new List<Vector3>();
//                GenCrossLittleZooPath(entity, crossPath);
//                CalcPathManager.GetInstance().AddNormalPath(crossPathName, crossPath);
//                entity.pathList.AddRange(crossPath);
//                crossPath.Clear();
//                crossPath = null;
//            }
//            AddWaitPosToPath(entity, entity.pathList);
//#else
//            this.GenCrossLittleZooPath(entity);
//#endif
//            EntityVisitor.GodownPath(entity, entity.pathList);
            
//            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, OnArrived);
//            MessageManager.GetInstance().Regist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);
//        }

//        public override void Leave()
//        {
//            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, OnArrived);
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);

//            base.Leave();
//        }

//        public override void AddAllConvertCond()
//        {
//            AddConvertCond((int)VisitorState.StayWaitSeat, OnToStateVisitorStayWaitSeat);
//        }

        
//        public override void Tick(int deltaTimeMS)
//        {
//        }

//        protected bool OnToStateVisitorStayWaitSeat()
//        {
//            return isToStateVisitorStayWaitSeat;
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
//            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0}在StateVisitorCrossGroupPath收到{1}", _msg.entityID, _msg);
//            entity.isApproveVisitSeat = true;
//            entity.indexInVisitQueue = _msg.indexInVisitQueue;
//            //entity.stayBuildingID = _msg.littleZooID;
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

//            if (_msg.followPath.isArrivedEnd)
//            {
//                LogWarp.Log("到达等待位,准备转下一个状态 StateVisitorStayWaitSeat");
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "到达等待位,准备转下一个状态 StateVisitorStayWaitSeat");
//                isToStateVisitorStayWaitSeat = true;
//                entity.stayBuildingID = entity.crossLittleZooIDs[entity.crossLittleZooIDs.Count - 1];
//                entity.stayGroupID = GlobalDataManager.GetInstance().logicTableGroup.FindGroupID(entity.stayBuildingID);
//            }
//        }

//#if CrossGroup_Optimize
//        protected void GenCrossLittleZooPath(EntityVisitor entity, List<Vector3> pathList)
//        {
//            //entity.pathList.Clear();
//            int startID = Const.Invalid_Int;
//            int endID = Const.Invalid_Int;

//            //entity.pathList.Add(entity.position);
//            for (int i = 0; i < entity.crossLittleZooIDs.Count - 1; i++)
//            {
//                startID = entity.crossLittleZooIDs[i];
//                endID = entity.crossLittleZooIDs[i + 1];

//                string pathName = EntityVisitor.GetPath(startID, endID);
//                if (string.IsNullOrEmpty(pathName))
//                {
//                    string e = string.Format("{0}找不到 {1} -> {2} 的路", entity.entityID, startID, endID);
//                    throw new System.Exception(e);
//                }
//                var onePath = PathManager.GetInstance().GetPath(pathName);
//                //entity.pathList.Add(onePath[0]);
//                //entity.pathList.AddRange(onePath);
//                pathList.AddRange(onePath);
//                LogWarp.LogFormat("GenCrossLittleZooPath {0}, {1}, {2}", pathName, startID, endID);
//            }

//            ////最后一个点是等待位的点
//            //var littleZooBuildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(endID);
//            //var endPos = littleZooBuildinPos.waitPosList[entity.indexInWaitQueue];
//            //entity.pathList.Add(endPos);

//            //return endID;
//        }

//        protected void AddEntityPosToPath(EntityVisitor entity, List<Vector3> pathList)
//        {
//            pathList.Add(entity.position);
//        }

//        protected void AddWaitPosToPath(EntityVisitor entity, List<Vector3> pathList)
//        {
//            //最后一个点是等待位的点
//            int endID = entity.crossLittleZooIDs[entity.crossLittleZooIDs.Count - 1];
//            var littleZooBuildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(endID);
//            var endPos = littleZooBuildinPos.waitPosList[entity.indexInWaitQueue];
//            pathList.Add(endPos);
//        }
//#else
//        protected void GenCrossLittleZooPath(EntityVisitor entity)
//        {
//            entity.pathList.Clear();
//            int startID = Const.Invalid_Int;
//            int endID = Const.Invalid_Int;

//            entity.pathList.Add(entity.position);
//            for (int i = 0; i < entity.crossLittleZooIDs.Count - 1; i++)
//            {
//                startID = entity.crossLittleZooIDs[i];
//                endID = entity.crossLittleZooIDs[i + 1];

//                string pathName = EntityVisitor.GetPath(startID, endID);
//                if (string.IsNullOrEmpty(pathName))
//                {
//                    string e = string.Format("{0}找不到 {1} -> {2} 的路", entity.entityID, startID, endID);
//                    throw new System.Exception(e);
//                }
//                var onePath = PathManager.GetInstance().GetPath(pathName);
//                entity.pathList.Add(onePath[0]);
//                entity.pathList.AddRange(onePath);
//                LogWarp.LogFormat("GenCrossLittleZooPath {0}, {1}, {2}", pathName, startID, endID);
//            }

//            //最后一个点是等待位的点
//            var littleZooBuildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(endID);
//            var endPos = littleZooBuildinPos.waitPosList[entity.indexInWaitQueue];
//            entity.pathList.Add(endPos);
//        }
//#endif
//    }
//}

    
