/*******************************************************************
* FileName:     StateShuttleGobackCalcPath.cs
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

namespace Game
{
    /// <summary>
    /// 摆渡车出发走计算出来的路线，到终点下人
    /// 场景扩：
    ///     不处理
    /// </summary>
    public class StateShuttleGotoCalcPath : FSMState
    {
        bool isToStateShuttleGobackCalcPath = false;
        int getOffAccumulativeTime = 0;
        int lastVisitorIdx = 0;
        bool isGetOff = false;
        int shuttleVisitorGetOffInterval = 0;
        public StateShuttleGotoCalcPath(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateShuttleGotoCalcPath.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "Shuttle_{0}_{1}_{2}", entity.entityID, (ShuttleState)this.preStateName, (ShuttleState)this.stateName);

            isToStateShuttleGobackCalcPath = false;
            getOffAccumulativeTime = 0;
            lastVisitorIdx = 0;
            isGetOff = false;
            shuttleVisitorGetOffInterval = Math_F.FloatToInt1000(Config.globalConfig.getInstace().ShuttleVisitorGetOffInterval);

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);

            //var cfgPath = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().Path_ShuttleGotoStatuc);
            //var pathList = new List<Vector3>();
            //pathList.Add(entity.position);
            //pathList.AddRange(cfgPath);
            //entity.followPath.Init(entity, pathList.ToArray(), pathList[0], 0, entity.moveSpeed, false);
            //entity.followPath.Run();


            var cfgPathGoback = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().Path_ShuttleGobackStatic);
            entity.pathList.Clear();
            //var pathList = new List<Vector3>();
            entity.pathList.Add(entity.position);
            entity.pathList.Add(cfgPathGoback[0]);
            entity.followPath.Init(entity, entity.pathList, entity.pathList[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
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
            AddConvertCond((int)ShuttleState.GobackCalcPath, this.ToStateShuttleGobackCalcPath);
        }

        protected bool ToStateShuttleGobackCalcPath()
        {
            return isToStateShuttleGobackCalcPath;
        }

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
                //isToStateShuttleGobackCalcPath = true;
                //SpawnVisitorFromCar.Send(VisitorStage.GotoParking, EntityFuncType.Visitor_From_Car);

                getOffAccumulativeTime = 0;
                isGetOff = true;
            }
        }

        protected void TickSpawnVisitor(int deltaTimeMS)
        {
            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            getOffAccumulativeTime += deltaTimeMS;
            if (getOffAccumulativeTime >= shuttleVisitorGetOffInterval
                && lastVisitorIdx < ExitGateModule.GetMaxShuttleVisitor()
                && isGetOff && lastVisitorIdx < entity.shuttleVisitorList.Count)
            {
                SpawnVisitorFromCar.Send(VisitorStage.GotoParking, entity.shuttleVisitorList[lastVisitorIdx].entityFuncType);
                if (lastVisitorIdx == ExitGateModule.GetMaxShuttleVisitor() - 1)
                {
                    isGetOff = false;
                    lastVisitorIdx = 0;
                    isToStateShuttleGobackCalcPath = true;
                }
                else
                {
                    lastVisitorIdx++;
                }

                getOffAccumulativeTime -= shuttleVisitorGetOffInterval;
            }
        }

        //protected void TickSpawnVisitor(int deltaTimeMS)
        //{
        //    var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
        //    getOffAccumulativeTime += deltaTimeMS;
        //    lastVisitorIdx = entity.shuttleVisitorList.Count;//等于车上人数

        //    if (getOffAccumulativeTime >= shuttleVisitorGetOffInterval
        //        && lastVisitorIdx < ExitGateModule.GetMaxShuttleVisitor()
        //        && isGetOff )
        //    {
        //        SpawnVisitorFromCar.Send(VisitorStage.GotoParking, entity.shuttleVisitorList[lastVisitorIdx-1].entityFuncType);
        //        if (lastVisitorIdx == ExitGateModule.GetMaxShuttleVisitor() - 1)
        //        {
        //            isGetOff = false;
        //            lastVisitorIdx = 0;
        //            isToStateShuttleGobackCalcPath = true;
        //        }
        //        else
        //        {
        //            lastVisitorIdx--;
        //        }

        //        getOffAccumulativeTime -= shuttleVisitorGetOffInterval;
        //    }
        //}

    }

}
