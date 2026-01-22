/*******************************************************************
* FileName:     StateVisitorLeaveFromZooEntry.cs
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

namespace Game
{
    /// <summary>
    /// 没有动物栏可选的情况下准备离开了
    /// 找到1000的路
    /// 从大门出去
    /// </summary>
    public class StateVisitorLeaveFromZooEntry : FSMState
    {
        /// <summary>
        /// 到达1000的路名
        /// </summary>
        public string pathOfTo1000 = "";

        public string pathOfLeaveZoo = "";

        /// <summary>
        /// 到达去1000的起点
        /// </summary>
        public bool isArrivedStartOfPathTo1000 = false;

        /// <summary>
        /// 到达去1000路的终点(到达入口)
        /// </summary>
        public bool isArrivedEndOfPathTo1000 = false;

        bool isToStateVisitorGotoGroundParking = false;

        public StateVisitorLeaveFromZooEntry(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorLeaveFromZooEntry.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isArrivedStartOfPathTo1000 = false;
            isArrivedEndOfPathTo1000 = false;
            isToStateVisitorGotoGroundParking = false;

            entity.moveSpeed = Config.globalConfig.getInstace().ZooVisitorBackSpeed;

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorWhereLeaveFromReply, this.OnVisitorWhereLeaveFromReply);

            pathOfLeaveZoo = Config.globalConfig.getInstace().NaturalVisitorOut_1;
            //选当前所在位置到1000的路，走过去。
            if (entity.stayBuildingID == 1000)
            {
                LogWarp.LogFormat("{0}  刚入口排队结束进入动物后就返回!", entity.entityID);
                isArrivedStartOfPathTo1000 = true;
                EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().EntryGoBackPath);
                return;
            }

            pathOfTo1000 = EntityVisitor.GetPath(entity.stayBuildingID, 1000);
            if (string.IsNullOrEmpty(pathOfTo1000))
            {
                string e = string.Format("StateVisitorLeaveNonLittleZoo 没找到{0}->{1}的路!!!!", entity.stayBuildingID, 1000);
                throw new Exception(e);
            }
            EntityVisitor.GotoStartOfPath(entity, pathOfTo1000);
        }


        public override void Tick(int deltaTimeMS)
        {

        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorWhereLeaveFromReply, this.OnVisitorWhereLeaveFromReply);
            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)VisitorState.GotoGroundParking, ToStateVisitorGotoGroundParking);
        }

        protected bool ToStateVisitorGotoGroundParking()
        {
            return isToStateVisitorGotoGroundParking;
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

            var entityFuncType = (EntityFuncType)entity.entityFuncType;

            //到达离开的起点,向1000走
            if (_msg.followPath.isArrivedEnd && !isArrivedStartOfPathTo1000)
            {
                isArrivedStartOfPathTo1000 = true;
                EntityVisitor.GodownPath(entity, pathOfTo1000);
                return;
            }

            //向动物园外走(已经走到1000了)
            if (_msg.followPath.isArrivedEnd && !isArrivedEndOfPathTo1000)
            {
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 往回走", entity.entityID);
                isArrivedEndOfPathTo1000 = true;

                //switch(entityFuncType)
                //{
                //    case EntityFuncType.Visitor_From_Ship:
                //        EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().AdvertVisitorLeavePath);
                //        break;
                //    case EntityFuncType.Visitor_From_Car:
                //        EntityVisitor.GodownPath(entity, pathOfLeaveZoo);
                //        break;
                //    case EntityFuncType.Visitor_From_GroundParking:
                //        EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().GroundParkingVistorBasePath);
                //        break;
                //    default:
                //        string e = string.Format("没有这种游客类型{0}", entityFuncType);
                //        throw new System.Exception(e);
                //}

                //如果是广告游客直接离开
                //非广告游客请求是从地面还是地下走
                switch (entityFuncType)
                {
                    case EntityFuncType.Visitor_From_Ship:
                        EntityVisitor.GodownPath(entity, Config.globalConfig.getInstace().AdvertVisitorLeavePath);
                        break;
                    case EntityFuncType.Visitor_From_Car:
                    case EntityFuncType.Visitor_From_GroundParking:
                        VisitorWhereLeaveFromApply.Send(entity.entityID);
                        break;
                    default:
                        string e = string.Format("没有这种游客类型{0}", entityFuncType);
                        throw new System.Exception(e);
                }

                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                //LogWarp.Log("结束离开");
                //DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 回收", entity.entityID);
                //EntityManager.GetInstance().RemoveFromEntityMovables(entity);

                ////通知生成离开的汽车
                //if (entity.entityFuncType == (int)EntityFuncType.Visitor_From_Car)
                //{
                //    MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
                //}

                //switch (entityFuncType)
                //{
                //    case EntityFuncType.Visitor_From_Car:
                //        //EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                //        ////通知生成离开的汽车
                //        //MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
                //        //break;
                //    case EntityFuncType.Visitor_From_GroundParking:
                //        //转向走向地面停车场
                //        //isToStateVisitorGotoGroundParking = true;
                //        VisitorWhereLeaveFromApply.Send(entity.entityID);
                //        break;
                //    case EntityFuncType.Visitor_From_Ship:
                //        EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                //        break;
                //    default:
                //        string e = string.Format("没有这种游客类型{0}", entityFuncType);
                //        throw new System.Exception(e);
                //}

                //地下游客清除自己，通知生成离开的车
                //广告游客清除自己
                //地上游客，不可能，是异常
                switch (entityFuncType)
                {
                    case EntityFuncType.Visitor_From_Ship:
                        EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                        break;
                    case EntityFuncType.Visitor_From_Car:
                        DebugFile.GetInstance().WriteKeyFile("FromUnder_", "{0}", entity.entityID);
                        EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                        ////通知生成离开的汽车
                        MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
                        break;
                    default:
                        string e = string.Format("这里不可能出现这种游客类型{0}", entityFuncType);
                        throw new System.Exception(e);
                }
            }
        }

        protected void OnVisitorWhereLeaveFromReply(Message msg)
        {
            var _msg = msg as VisitorWhereLeaveFromReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            //地下走
            if (!_msg.isFromGround)
            {
                entity.entityFuncType = (int)EntityFuncType.Visitor_From_Car;
                entity.groundParkingGroupID = Const.Invalid_Int;
                entity.groundParkingIdx = Const.Invalid_Int;
                EntityVisitor.GodownPath(entity, pathOfLeaveZoo);
                return;
            }

            //地上走
            entity.entityFuncType = (int)EntityFuncType.Visitor_From_GroundParking;
            entity.groundParkingGroupID = _msg.groupID;
            entity.groundParkingIdx = _msg.idx;
            isToStateVisitorGotoGroundParking = true;
        }
    }
}
