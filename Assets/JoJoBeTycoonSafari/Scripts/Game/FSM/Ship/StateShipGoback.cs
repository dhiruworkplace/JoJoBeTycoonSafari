/*******************************************************************
* FileName:     StateShipGoback.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-27
* Description:  
* other:    
********************************************************************/


using ZooGame.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using ZooGame.MessageCenter;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace ZooGame
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class StateShipGoback : FSMState
    {
        bool couldWait = false;
        int waitAccumulativeTime = 0;
        int waitInterval = 5000;
        public StateShipGoback(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineShip).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateShipGoback.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "Ship_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            waitAccumulativeTime = 0;
            waitInterval = 5000;
            couldWait = true;

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
        }

        public override void Tick(int deltaTimeMS)
        {
            TickWait(deltaTimeMS);
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
        }

        protected void OnArrived(Message msg)
        {
            var _msg = msg as MessageArrived;

            var entity = (this.fsmCtr as FSMMachineShip).ownerEntity;
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                EntityManager.GetInstance().RemoveFromEntityMovables(_msg.followPath.ownerEntity);
            }
        }

        protected void TickWait(int deltaTimeMS)
        {
            if (!couldWait)
            {
                return;
            }

            waitAccumulativeTime += deltaTimeMS;
            if (waitAccumulativeTime >= waitInterval)
            {
                couldWait = false;

                //返航
                string pathName = Config.globalConfig.getInstace().AdvertVisitorInto_1;
                var path = PathManager.GetInstance().GetPath(pathName, true);

                if (path == null || path.Count <= 0)
                {
                    string e = string.Format("路 {0} 数据异常", pathName);
                    throw new System.Exception(e);
                }
                var entity = (this.fsmCtr as FSMMachineShip).ownerEntity;

                entity.followPath.Init(entity, path, path[0], 0, entity.moveSpeed, false);
                entity.followPath.Run();
            }
        }
    }

}
