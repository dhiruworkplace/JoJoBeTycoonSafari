/*******************************************************************
* FileName:     StateShuttleGobackDynamicPath.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-31
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
    /// 摆渡车回来动态路线，到终点回收自己
    /// 场景扩
    ///     人和车往后移
    /// </summary>
    public class StateShuttleGobackDynamicPath : FSMState
    {
        //bool isToStateShuttleGotoCalcPath = false;

        public StateShuttleGobackDynamicPath(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateShuttleGobackDynamicPath.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "Shuttle_{0}_{1}_{2}", entity.entityID, (ShuttleState)this.preStateName, (ShuttleState)this.stateName);

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath,
                this.OnBroadcastAfterExtendSceneAndModifiedPath);

            //isToStateShuttleGotoCalcPath = false;
            var pathList = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().Path_ShuttleGobackDynamic);
            //entity.position = pathList[0];
            entity.followPath.Init(entity, pathList, pathList[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
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

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            //AddConvertCond((int)ShuttleState.GotoCalcPath, this.ToStateShuttleGotoCalcPath);
        }

        //protected bool ToStateShuttleGotoCalcPath()
        //{
        //    return isToStateShuttleGotoCalcPath;
        //}

        protected void OnArrived(Message msg)
        {
            var _msg = (MessageArrived)msg;
            //自己的entity
            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            //单纯的到达终点
            if (_msg.followPath.isArrivedEnd)
            {
                //isToStateShuttleGotoCalcPath = true;
                EntityManager.GetInstance().RemoveFromEntityMovables(entity);
            }
        }

        protected void OnBroadcastAfterExtendSceneAndModifiedPath(Message msg)
        {
            Vector3 extendOffset = Vector3.zero;
            extendOffset.x -= Config.globalConfig.getInstace().ZooPartResLen;
            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            entity.position += extendOffset;
            entity.followPath.ModifyPath(extendOffset);
        }

    }

}
