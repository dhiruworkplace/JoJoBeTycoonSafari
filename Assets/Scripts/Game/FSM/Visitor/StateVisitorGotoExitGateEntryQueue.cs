/*******************************************************************
* FileName:     StateVisitorGotoExitGateEntryQueue.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-27
* Description:  
* other:    
********************************************************************/


using Game.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using Game.MessageCenter;
using System;
using UnityEngine;
using System.Collections.Generic;
using Game.GlobalData;

namespace Game
{
    /// <summary>
    /// 走向出口的排队位
    /// 
    /// </summary>
    public class StateVisitorGotoExitGateEntryQueue : FSMState
    {
        int finishQueueStep = 0;

        IntCD checkInCD;
        public StateVisitorGotoExitGateEntryQueue(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorGotoExitGateEntryQueue.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath,
                this.OnBroadcastAfterExtendSceneAndModifiedPath);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue,
                this.OnBroadcastForwardOneStepInExitGateQueue);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SendExitGateCheckinCDFinishReply,
                this.OnSendExitGateCheckinCDFinishReply);

            finishQueueStep = 0;
            
            GoToOrgPosOfQueue(entity);
            if (checkInCD == null)
            {
                checkInCD = new IntCD(0);
            }
            checkInCD.Stop();
            LogWarp.LogFormat("{0} GoToOrgPosOfQueue step = {1}", entity.entityID, finishQueueStep);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} GoToOrgPosOfQueue step = {1}", entity.entityID, finishQueueStep);
        }

        public override void Tick(int deltaTimeMS)
        {
            TickCheckInCD(deltaTimeMS);

            TickStepByStep();
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath,
                this.OnBroadcastAfterExtendSceneAndModifiedPath);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue,
                this.OnBroadcastForwardOneStepInExitGateQueue);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SendExitGateCheckinCDFinishReply,
                this.OnSendExitGateCheckinCDFinishReply);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
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

            //到达初始排队位
            if (_msg.followPath.isArrivedEnd  && finishQueueStep == 0)
            {
                RunCheckInCD();
            }
        }

        protected void OnBroadcastAfterExtendSceneAndModifiedPath(Message msg)
        {
            Vector3 extendOffset = Vector3.zero;
            extendOffset.x -= Config.globalConfig.getInstace().ZooPartResLen;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            entity.position += extendOffset;
            entity.followPath.ModifyPath(extendOffset);
        }

        protected void OnBroadcastForwardOneStepInExitGateQueue(Message msg)
        {
            var _msg = msg as BroadcastForwardOneStepInExitGateQueue;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if  (_msg.entityIDs.IndexOf(entity.entityID) < 0)
            {
                return;
            }

            entity.numOfExitGateQueueForwardOne++;

            LogWarp.LogErrorFormat("{0} GotoExitEntryQueue recv {1}, entity.numOfExitGateQueueForwardOne={2}", entity.entityID, _msg, entity.numOfExitGateQueueForwardOne);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} GotoExitEntryQueue recv {1}, entity.numOfExitGateQueueForwardOne={2}", entity.entityID, _msg, entity.numOfExitGateQueueForwardOne);
        }

        protected void OnSendExitGateCheckinCDFinishReply(Message msg)
        {
            var _msg = msg as SendExitGateCheckinCDFinishReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (entity.entityID != _msg.entityID)
            {
                return;
            }
            //生成摆渡车
            //MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnShuttle);
            //EntityManager.GetInstance().RemoveFromEntityMovables(entity);
            entity.Hide();
        }

        protected void GoToOrgPosOfQueue(EntityVisitor entity)
        {
            var cellBuildUp = Config.buildupConfig.getInstace().getCell(entity.stayBuildingID);

            var cellExitGate = Config.exitgateConfig.getInstace().getCell(entity.ExitGateEntryID);

            entity.pathList.Clear();
            if (cellBuildUp.pathtype == 0)
            {
                entity.pathList.AddRange(PathManager.GetInstance().GetPath(cellExitGate.positiveexitgate));
            }
            else
            {
                entity.pathList.AddRange(PathManager.GetInstance().GetPath(cellExitGate.negativeexitgate));
            }

            finishQueueStep = entity.indexInExitGateEntryQueue;
            float queuePosX = (float)(cellExitGate.maxnumofperqueue - entity.indexInExitGateEntryQueue);
            Vector3 queuePos = entity.pathList[entity.pathList.Count - 1];
            queuePos.x -= queuePosX;
            entity.pathList.Add(queuePos);
            EntityVisitor.GodownPath(entity, entity.pathList);
        }

        protected void MoveForward(EntityVisitor entity, float distance)
        {
            var startPos = entity.position;
            var endPos = startPos;
            endPos.x -= distance;
            entity.pathList.Clear();
            entity.pathList.Add(startPos);
            entity.pathList.Add(endPos);
            float queueMoveSpeed = Config.globalConfig.getInstace().ZooVisitorQueueSpeed;
            entity.followPath.Init(entity, entity.pathList, startPos, 0, queueMoveSpeed, false);
            entity.followPath.Run();
        }

        protected void WhenCheckinCDFinished()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            var msg = SendExitGateCheckinCDFinish.Send(entity.entityID, entity.ExitGateEntryID);
            LogWarp.LogFormat("{0} send {1}", entity.entityID, msg);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} send {1}", entity.entityID, msg);
        }

        protected void TickCheckInCD(int deltaTimeMS)
        {
            checkInCD.Tick(deltaTimeMS);
            if (checkInCD.IsRunning())
            {
                if (checkInCD.IsFinish())
                {
                    checkInCD.Stop();
                    WhenCheckinCDFinished();
                }
            }
        }

        protected void TickStepByStep()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (entity.followPath.IsRunning())
            {
                return;
            }

            if (entity.numOfExitGateQueueForwardOne > 0)
            {
                MoveForward(entity, 1);
                --finishQueueStep;
                --entity.numOfExitGateQueueForwardOne;
            }
        }

        protected void RunCheckInCD()
        {
            int cdVal = ExitGateModule.GetChinkinCDValMs();
            var playerData = GlobalDataManager.GetInstance().playerData;
            if (playerData.playerZoo.buffExitEntryCDVal != UFrame.Const.Invalid_Float)
            {
                int buffExitEntryCDValMS = Math_F.FloatToInt1000(playerData.playerZoo.buffExitEntryCDVal);
                cdVal = Mathf.Min(cdVal, buffExitEntryCDValMS);
            }
            checkInCD.ResetOrg(cdVal);
            checkInCD.Run();
#if UNITY_EDITOR
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            LogWarp.LogFormat("{0} RunCheckInCD cdVal={1}", entity.entityID, cdVal);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} RunCheckInCD cdVal={1}", entity.entityID, cdVal);
#endif
        }
    }
}
