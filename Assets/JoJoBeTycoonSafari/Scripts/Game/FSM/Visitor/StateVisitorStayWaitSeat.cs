/*******************************************************************
* FileName:     StateVisitorStayWaitSeat.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-21
* Description:  
* other:    
********************************************************************/


using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using Logger;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace ZooGame
{
    /// <summary>
    /// 等待位状态
    /// 进入这个状态说明已经走到了等待位
    /// 1.进入就检查是否已经在上一个状态收到观光位WaitSeatToVisitSeat
    /// 2.注册消息WaitSeatToVisitSeat
    /// </summary>
    public class StateVisitorStayWaitSeat : FSMState
    {
        bool isToStateVisitorStayVisitSeat = false;

        public StateVisitorStayWaitSeat(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            //LogWarp.LogError("StateVisitorStayWaitSeat.Enter");
            base.Enter(preStateName);

            isToStateVisitorStayVisitSeat = false;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            //等待位看向前面
            entity.LookAt(entity.position + GlobalDataManager.GetInstance().SceneForward);

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorStayWaitSeat.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorIdle);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorIdle);

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);
            //以进入就检查是否已经在上一个状态收到观光位WaitSeatToVisitSeat
            if (entity.isApproveVisitSeat)
            {
                GotoVisitSeat(entity, entity.stayBuildingID, entity.indexInVisitQueue);
                return;
            }
        }

        public override void Leave()
        { 
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, OnArrived);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)VisitorState.StayVisitSeat, OnToStateVisitorStayVisitSeat);
        }

        
        public override void Tick(int deltaTimeMS)
        {
        }

        protected bool OnToStateVisitorStayVisitSeat()
        {
            return isToStateVisitorStayVisitSeat;
        }

        /// <summary>
        /// 在这个状态收到这个消息，被通知走到观光位，直接走去观光位，走到后转下一个状态
        /// </summary>
        /// <param name="msg"></param>
        protected void OnWaitSeatToVisitSeat(Message msg)
        {
            var _msg = msg as WaitSeatToVisitSeat;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            //不是自己的
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            entity.isApproveVisitSeat = true;
            //entity.stayBuildingID = _msg.littleZooID;
            entity.indexInVisitQueue = _msg.indexInVisitQueue;
            LogWarp.LogFormat("goto GotoVisitSeat {0}, {1}, {2}", entity.entityID, _msg.littleZooID, _msg.indexInVisitQueue);
            GotoVisitSeat(entity, _msg.littleZooID, _msg.indexInVisitQueue);
        }

        protected void OnArrived(Message msg)
        {
            var _msg = msg as MessageArrived;

            //自己的entity
            if (_msg.followPath.ownerEntity.entityID != (this.fsmCtr as FSMMachineVisitor).ownerEntity.entityID)
            {
                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                LogWarp.Log("到达观光位,准备转下一个状态，观光位 StateVisitorStayVisitSeat");
                isToStateVisitorStayVisitSeat = true;
            }
        }

        protected void GotoVisitSeat(EntityVisitor entity, int littleZooID, int indexInVisitQueue)
        {
            LogWarp.Log("GotoVisitSeat");
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
            var littleZooBuildinPos =  LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(littleZooID);
            var endPos = littleZooBuildinPos.visitPosList[indexInVisitQueue];
            
            entity.pathList.Clear();
            entity.pathList.Add(entity.position);
            entity.pathList.Add(endPos);
            entity.followPath.Init(entity, entity.pathList, entity.position, 0, entity.moveSpeed, false);
            entity.followPath.Run();
        }
    }
}

    
