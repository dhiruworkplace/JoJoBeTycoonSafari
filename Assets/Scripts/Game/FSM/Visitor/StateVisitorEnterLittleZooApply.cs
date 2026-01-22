/*******************************************************************
* FileName:     StateVisitorEnterLittleZooApply.cs
* Author:       Fan Zheng Yong
* Date:         2019-12-18
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;
using Logger;
using Game.GlobalData;
using Game.MessageCenter;
using UFrame.MessageCenter;
using Game.Path.StraightLine;

namespace Game
{
    /// <summary>
    /// 申请观光位
    /// 成功->走过去->转观光位状态
    /// 不成功->转向选择动物状态
    /// </summary>
    public class StateVisitorEnterLittleZooApply : FSMState
    {
        bool isToStateChoseLittleZoo = false;
        bool isToStateStayVisitSeat = false;
        public StateVisitorEnterLittleZooApply(int stateName, FSMMachine fsmCtr) : 
            base(stateName, fsmCtr) { }
        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorEnterLittleZooApply.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isToStateChoseLittleZoo = false;
            isToStateStayVisitSeat = false;

            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorGetVisitSeatReply, OnVisitorGetVisitSeatReply);
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

            VisitorGetVisitSeatApply.Send(entity.entityID, 0, entity.stayBuildingID);
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorGetVisitSeatReply, OnVisitorGetVisitSeatReply);
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            this.AddConvertCond((int)VisitorState.ChoseLittleZoo, ToStateChoseLittleZoo);
            this.AddConvertCond((int)VisitorState.StayVisitSeat, ToisToStateStayVisitSeat);
        }

        protected bool ToStateChoseLittleZoo()
        {
            return isToStateChoseLittleZoo;
        }

        protected bool ToisToStateStayVisitSeat()
        {
            return isToStateStayVisitSeat;
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        protected void OnVisitorGetVisitSeatReply(Message msg)
        {
            var _msg = msg as VisitorGetVisitSeatReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            if (!_msg.result)
            {
                isToStateChoseLittleZoo = true;
                return;
            }
            entity.indexInVisitQueue = _msg.idxOfQueue;
            entity.pathList.Clear();
            entity.pathList.Add(entity.position);
            var buildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(entity.stayBuildingID);
            Vector3 visitPos = buildinPos.visitPosList[_msg.idxOfQueue];
            entity.pathList.Add(visitPos);
            EntityVisitor.GodownPath(entity, entity.pathList, false);
        }

        protected void OnArrived(Message msg)
        {
            var _msg = msg as MessageArrived;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //自己的entity
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                entity.LookAt(entity.position + GlobalDataManager.GetInstance().SceneForward);
                isToStateStayVisitSeat = true;
            }
        }
   }
}