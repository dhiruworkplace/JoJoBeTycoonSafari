/*******************************************************************
* FileName:     StateShipGoto.cs
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
    public class StateShipGoto : FSMState
    {
        bool isToStateGoback = false;
        bool isArrivedZoo = false;
        int spawnVisitorNum = 0;
        int getOffAccumulativeTime = 0;
        int getOffInterval = 100;

        public StateShipGoto(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineShip).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateShipGoto.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "Ship_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isToStateGoback = false;
            isArrivedZoo = false;
            spawnVisitorNum = 0;
            getOffAccumulativeTime = 0;
            getOffInterval = entity.visitorGetOffInterval;
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
        }

        public override void Tick(int deltaTimeMS)
        {
            TickSpawnVisitor(deltaTimeMS);
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)ShipState.Goback, this.ToStateGoback);
        }

        protected bool ToStateGoback()
        {
            return isToStateGoback;
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
                isArrivedZoo = true;
            }            
        }

        protected void TickSpawnVisitor(int deltaTimeMS)
        {
            if (!isArrivedZoo)
            {
                return;
            }

            var entity = (this.fsmCtr as FSMMachineShip).ownerEntity;

            getOffAccumulativeTime += deltaTimeMS;
            if (getOffAccumulativeTime >= getOffInterval
                && spawnVisitorNum < entity.maxSpawnVisitorNum)
            {
                SpawnVisitorFromCar.Send(VisitorStage.GotoZoo, EntityFuncType.Visitor_From_Ship);
                spawnVisitorNum++;
                getOffAccumulativeTime -= getOffInterval;

                if (spawnVisitorNum == entity.maxSpawnVisitorNum)
                {
                    isToStateGoback = true;
                }
            }
        }
    }

}
