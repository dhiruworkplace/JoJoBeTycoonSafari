/*******************************************************************
* FileName:     StateVisitorGotoGroundParking.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-23
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
    /// 走向地面停车场
    /// 到了终点，刷出离开的车
    /// 
    /// </summary>
    public class StateVisitorGotoGroundParking : FSMState
    {
        bool isFinishedBasePath = false;

        public StateVisitorGotoGroundParking(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorGotoGroundParking.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, OnArrived);

#if UNITY_EDITOR
            if (entity.entityFuncType != (int)EntityFuncType.Visitor_From_GroundParking)
            {
                string e = string.Format("{0}这里必须是地面停车场来的游客", entity.entityID);
                throw new System.Exception(e);
            }
#endif
            //先走完基路
            isFinishedBasePath = false;
            EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().GroundParkingVistorBasePath);
            
            ////构建去地面停车场的路
            //List<Vector3> pathList = null;
            //var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(entity.groundParkingGroupID, entity.groundParkingIdx);
            //if (!GroundParingSpacePathManager.IsExist(pathUnit.visitorBackPath))
            //{
            //    pathList = GroundParingSpacePathManager.GenVisitorBackPath(entity.groundParkingGroupID, entity.groundParkingIdx);
            //    GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.VisitorBackPath, pathUnit, pathList, null);
            //}
            //pathList = pathUnit.visitorBackPath;

            //EntityVisitor.GodownPath(entity, pathList);
        }


        public override void Tick(int deltaTimeMS)
        {

        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {

        }

        protected void OnArrived(Message msg)
        {
            var _msg = (MessageArrived)msg;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //自己的entity
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            if (_msg.followPath.isArrivedEnd && !isFinishedBasePath)
            {
                isFinishedBasePath = true;
                //构建去地面停车场的路
                List<Vector3> pathList = null;
                var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(entity.groundParkingGroupID, entity.groundParkingIdx);
                if (!GroundParingSpacePathManager.IsExist(pathUnit.visitorBackPath))
                {
                    pathList = GroundParingSpacePathManager.GenVisitorBackPath(entity.groundParkingGroupID, entity.groundParkingIdx);
                    GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.VisitorBackPath, pathUnit, pathList, null);
                }
                pathList = pathUnit.visitorBackPath;
                EntityVisitor.GodownPath(entity, pathList);

                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                MessageGroundParkingSpace.Send((int)GameMessageDefine.BroadcastLetGroundParingCarLeave,
                    entity.groundParkingGroupID, entity.groundParkingIdx);
                
                // 回POOL
                EntityManager.GetInstance().RemoveFromEntityMovables(entity);
            }
        }
    }

}
