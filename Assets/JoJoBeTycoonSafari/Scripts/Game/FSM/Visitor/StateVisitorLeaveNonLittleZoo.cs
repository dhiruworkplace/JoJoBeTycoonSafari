/*******************************************************************
* FileName:     StateVisitorLeaveNonLittleZoo.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-21
* Description:  
* other:    
********************************************************************/


using ZooGame.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using ZooGame.MessageCenter;
using System;

namespace ZooGame
{
    /// <summary>
    /// 没有动物栏可选的情况下准备离开了
    /// 向出口发出申请，返回成功走向出口排队位
    /// 返回失败去找到1000的路离开
    /// </summary>
    public class StateVisitorLeaveNonLittleZoo : FSMState
    {
        bool isToStateVisitorLeaveFromZooEntry = false;

        bool isToStateVisitorGotoStartOfExitGateEntryPath = false;

        public StateVisitorLeaveNonLittleZoo(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorLeaveNonLittleZoo.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isToStateVisitorLeaveFromZooEntry = false;
            isToStateVisitorGotoStartOfExitGateEntryPath = false;

            entity.ExitGateEntryID = Const.Invalid_Int;
            entity.indexInExitGateEntryQueue = Const.Invalid_Int;

            //MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToExitGateQueueApplyReply,
            //    OnAddVisitorToExitGateQueueApplyReply);
            //MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            //MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue,
            //    OnBroadcastForwardOneStepInExitGateQueue);

            //改成不去出口排队了，直接从前门离开
            //AddVisitorToExitGateQueueApply.Send(entity.entityID);
            isToStateVisitorLeaveFromZooEntry = true;
            entity.moveSpeed = Config.globalConfig.getInstace().ZooVisitorBackSpeed;
        }
        
        public override void Tick(int deltaTimeMS)
        {

        }

        public override void Leave()
        {
            //MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToExitGateQueueApplyReply,
            //    OnAddVisitorToExitGateQueueApplyReply);
            //MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            //MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue,
            //    OnBroadcastForwardOneStepInExitGateQueue);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)VisitorState.LeaveFromZooEntry, this.ToStateVisitorLeaveFromZooEntry);
            //AddConvertCond((int)VisitorState.GotoStartOfExitGateEntryPath, this.ToStateVisitorGotoStartOfExitGateEntryPath);
        }

        protected bool ToStateVisitorLeaveFromZooEntry()
        {
            return isToStateVisitorLeaveFromZooEntry;
        }

        protected bool ToStateVisitorGotoStartOfExitGateEntryPath()
        {
            return isToStateVisitorGotoStartOfExitGateEntryPath;
        }

        protected void OnAddVisitorToExitGateQueueApplyReply(Message msg)
        {
            var _msg = msg as AddVisitorToExitGateQueueApplyReply;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0} 收到 {1}", _msg.entityID, msg);
            
            //出口排队申请失败，转从大门走状态
            if (!_msg.result)
            {
                isToStateVisitorLeaveFromZooEntry = true;
                return;
            }

            entity.ExitGateEntryID = _msg.entryID;
            entity.indexInExitGateEntryQueue = _msg.indexInQueue;
            //从buildUp表中取第一段路
            //0 正
            //1 反
            var cell = Config.buildupConfig.getInstace().getCell(entity.stayBuildingID);
            EntityVisitor.GotoStartOfPath(entity, cell.outpath);
        }

        protected void OnArrived(Message msg)
        {
            var _msg = (MessageArrived)msg;
            //自己的entity
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            //到达终点
            if (_msg.followPath.isArrivedEnd)
            {
                isToStateVisitorGotoStartOfExitGateEntryPath = true;
            }
        }

        protected void OnBroadcastForwardOneStepInExitGateQueue(Message msg)
        {
            var _msg = msg as BroadcastForwardOneStepInExitGateQueue;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.entityIDs.IndexOf(entity.entityID) < 0)
            {
                return;
            }


            entity.numOfExitGateQueueForwardOne++;

            LogWarp.LogErrorFormat("{0} LeaveNonLittleZoo recv {1}, entity.numOfExitGateQueueForwardOne={2}", entity.entityID, _msg, entity.numOfExitGateQueueForwardOne);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} LeaveNonLittleZoo recv {1}, entity.numOfExitGateQueueForwardOne={2}", entity.entityID, _msg, entity.numOfExitGateQueueForwardOne);
        }

    }

}
