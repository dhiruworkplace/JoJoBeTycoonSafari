/*******************************************************************
* FileName:     StateVisitorGotoStartOfExitGateEntryPath.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-26
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

namespace Game
{
    /// <summary>
    /// 走向出口的路的起点
    /// 
    /// </summary>
    public class StateVisitorGotoStartOfExitGateEntryPath : FSMState
    {
        bool isToStateVisitorGotoExitGateEntryQueue = false;

        int extendSceneTime = 0;

        List<Vector3> extendPathList;

        static Vector3 extendOffset = Vector3.zero;

        public StateVisitorGotoStartOfExitGateEntryPath(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
            extendPathList = new List<Vector3>();
            if (extendOffset == Vector3.zero)
            {
                extendOffset.x -= Config.globalConfig.getInstace().ZooPartResLen;
            }
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorGotoStartOfExitGateEntryPath.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isToStateVisitorGotoExitGateEntryQueue = false;
            extendSceneTime = 0;

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath,
                this.OnBroadcastAfterExtendSceneAndModifiedPath);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue,
                this.OnBroadcastForwardOneStepInExitGateQueue);
            
            var cellBuildUp = Config.buildupConfig.getInstace().getCell(entity.stayBuildingID);
            
            entity.pathList.Clear();
            Vector3 pos = Vector3.zero;
            if (PathManager.GetInstance().GetPathLastPos(cellBuildUp.outpath, ref pos))
            {
                entity.pathList.Add(pos);
            }

            var cellExitGate = Config.exitgateConfig.getInstace().getCell(entity.ExitGateEntryID);
            if (cellBuildUp.pathtype == 0)
            {
                PathManager.GetInstance().GetPathFirstPos(cellExitGate.positiveexitgate, ref pos);
            }
            else
            {
                PathManager.GetInstance().GetPathFirstPos(cellExitGate.negativeexitgate, ref pos);
            }

            entity.pathList.Add(pos);

            //LogWarp.LogErrorFormat("littlezoo={0}, outpath={1}, pathtype={2}, positive={3}, negative={4} ",
            //    entity.stayBuildingID, cellBuildUp.outpath, cellBuildUp.pathtype,
            //    cellExitGate.positiveexitgate, cellExitGate.negativeexitgate);

            EntityVisitor.GodownPath(entity, entity.pathList, true);
        }

        public override void Tick(int deltaTimeMS)
        {

        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath,
                this.OnBroadcastAfterExtendSceneAndModifiedPath);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue,
                this.OnBroadcastForwardOneStepInExitGateQueue);
            extendPathList.Clear();

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)VisitorState.GotoExitGateEntryQueue, this.ToStateVisitorGotoExitGateEntryQueue);
        }

        protected bool ToStateVisitorGotoExitGateEntryQueue()
        {
            return isToStateVisitorGotoExitGateEntryQueue;
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

            //到达终点, 但场景扩了, 需要继续往前走
            if (_msg.followPath.isArrivedEnd && extendSceneTime > 0)
            {
                --extendSceneTime;
                extendPathList.Add(entity.position);
                extendPathList.Add(entity.position + extendOffset);
                EntityVisitor.GodownPath(entity, extendPathList);
            }

            //单纯的到达终点
            if (_msg.followPath.isArrivedEnd)
            {
                isToStateVisitorGotoExitGateEntryQueue = true;
            }
        }


        protected void OnBroadcastAfterExtendSceneAndModifiedPath(Message msg)
        {
            extendSceneTime++;
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

            LogWarp.LogFormat("{0} GotoStarOfExitEntry recv {1}, entity.numOfExitGateQueueForwardOne={2}", entity.entityID, _msg, entity.numOfExitGateQueueForwardOne);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} GotoStarOfExitEntry recv {1}, entity.numOfExitGateQueueForwardOne={2}", entity.entityID, _msg, entity.numOfExitGateQueueForwardOne);
        }

    }

}
