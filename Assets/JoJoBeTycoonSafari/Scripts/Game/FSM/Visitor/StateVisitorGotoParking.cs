/*******************************************************************
* FileName:     StateVisitorGotoParking.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-09
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

namespace ZooGame
{
    /// <summary>
    /// 走向停车场
    /// 到了终点，刷出离开的车
    /// 
    /// </summary>
    public class StateVisitorGotoParking : FSMState
    {
        public StateVisitorGotoParking(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorGotoParking.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, OnArrived);

            Vector3 startPos = Vector3.zero;
            if (entity.entityFuncType == (int)EntityFuncType.Visitor_From_Car)
            {
                PathManager.GetInstance().GetPathFirstPos(Config.globalConfig.getInstace().ShuttleVisitorLeavePath, ref startPos);
                entity.position = startPos;
                EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().ShuttleVisitorLeavePath);
                return;
            }

            PathManager.GetInstance().GetPathFirstPos(Config.globalConfig.getInstace().AdvertVisitorLeavePath, ref startPos);
            entity.position = startPos;
            EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().AdvertVisitorLeavePath);
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

            if (_msg.followPath.isArrivedEnd)
            {
                EntityManager.GetInstance().RemoveFromEntityMovables(entity);

                //通知生成离开的汽车
                //MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
                //通知生成离开的汽车
                if (entity.entityFuncType == (int)EntityFuncType.Visitor_From_Car)
                {
                    DebugFile.GetInstance().WriteKeyFile("FromUnder_", "{0}", entity.entityID);
                    MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
                }
            }
        }
    }

}
