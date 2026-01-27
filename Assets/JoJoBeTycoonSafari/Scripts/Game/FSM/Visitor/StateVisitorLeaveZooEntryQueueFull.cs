/*******************************************************************
* FileName:     StateVisitorLeaveZooEntryQueueFull.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-14
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using ZooGame.Path.StraightLine;
using ZooGame.MessageCenter;
using ZooGame.MiniGame;
using DG.Tweening;

namespace ZooGame
{
    /// <summary>
    /// 入口排队满了
    /// </summary>
    public class StateVisitorLeaveZooEntryQueueFull : FSMState
    {
        /// <summary>
        /// 表情
        /// </summary>
        public GameObject expGo = null;
        public Transform expTrans = null;
        public Vector3 expPos;
        public StateVisitorLeaveZooEntryQueueFull(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorLeaveZooEntryQueueFull.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);
            entity.moveSpeed = Config.globalConfig.getInstace().ZooVisitorBackSpeed;
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorWhereLeaveFromReply, this.OnVisitorWhereLeaveFromReply);

            //走回去的路,广告游客(ship)没有停车场逻辑,地下游客走地下, 地上游客走申请
            EntityFuncType entityFuncType = (EntityFuncType)(entity.entityFuncType);
            switch (entityFuncType)
            {
                case EntityFuncType.Visitor_From_Ship:
                    EntityVisitor.GodownPath(entity, PathManager.GetInstance().GetPath(
                        Config.globalConfig.getInstace().AdvertVisitorOut));
                    break;
                case EntityFuncType.Visitor_From_Car:
                    //这里没有走申请，得发个消息，让底下停车场数量-1
                    MessageInt.Send((int)GameMessageDefine.DirectMinusOneUnderParkingNum, entity.entityID);
                    EntityVisitor.GodownReversePath(entity, Config.globalConfig.getInstace().EntryQueueObservePath);
                    break;
                case EntityFuncType.Visitor_From_GroundParking:
                    VisitorWhereLeaveFromApply.Send(entity.entityID);
                    break;
                default:
                    string e = string.Format("没有这种游客类型{0}", entityFuncType);
                    throw new System.Exception(e);
            }

            //生成愤怒表情
            var pool = PoolManager.GetInstance().GetGameObjectPool(9301);
            expGo = pool.New();
            expTrans = expGo.transform;
            expTrans.position = entity.position;
            ExpressionScaleAnim(expTrans, Config.globalConfig.getInstace().ExpressionScaleOrg,
                Config.globalConfig.getInstace().ExpressionScaleMax,
                Config.globalConfig.getInstace().ExpressionScaleDuration,
                Config.globalConfig.getInstace().ExpressionDuration);
        }


        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorWhereLeaveFromReply, this.OnVisitorWhereLeaveFromReply);

            //回收愤怒表情
            if (expGo != null)
            {
                var pool = PoolManager.GetInstance().IsBelongGameObjectPool(9301);
#if UNITY_EDITOR
                if (pool == null)
                {
                    string e = string.Format("异常,表情找不到回收的GameObjectPool {0}", 9301);
                    throw new System.Exception(e);
                }
#endif
                expTrans.position = Const.Invisible_Postion;
                pool.Delete(expGo);
                expGo = null;
                expTrans = null;
            }

            base.Leave();
        }

        public override void AddAllConvertCond()
        {
        }

        public override void Tick(int deltaTimeMS)
        {
            if (expTrans != null)
            {
                var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
                expPos = entity.position;
                expPos.y = Config.globalConfig.getInstace().ExpressionPosY;
                expTrans.transform.position = expPos;
            }
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

            //到达终点, 游客消失，地上游客地上发车，地下游客地下发车，广告游客什么车都不发
            if (_msg.followPath.isArrivedEnd)
            {
                if (entity.entityFuncType == (int)EntityFuncType.Visitor_From_Car)
                {
                    DebugFile.GetInstance().WriteKeyFile("FromUnder_", "{0}", entity.entityID);
                    MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
                }
                else if (entity.entityFuncType == (int)EntityFuncType.Visitor_From_GroundParking)
                {
                    MessageGroundParkingSpace.Send((int)GameMessageDefine.BroadcastLetGroundParingCarLeave, 
                        entity.groundParkingGroupID, entity.groundParkingIdx);
                }

                // 回POOL
                EntityManager.GetInstance().RemoveFromEntityMovables(entity);
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

#if UNITY_EDITOR
            if (entity.entityFuncType != (int)(EntityFuncType.Visitor_From_GroundParking))
            {
                string e = string.Format("{0} 这里只能是地上游客获得申请回复", entity.entityID);
                throw new System.Exception(e);
            }
#endif
            //地下走, 需要构建去地上观察路的路径的反路
            if (!_msg.isFromGround)
            {
                entity.entityFuncType = (int)EntityFuncType.Visitor_From_Car;
                entity.groundParkingGroupID = Const.Invalid_Int;
                entity.groundParkingIdx = Const.Invalid_Int;
                entity.pathList.Clear();
                entity.pathList.Add(entity.position);
                entity.pathList.AddRange(PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().EntryQueueObservePath, true));
                EntityVisitor.GodownPath(entity, entity.pathList);
                return;
            }

            //地上游客地上走
            //需要判定当前的位置和分配的位置，如果不一致得先走到分配的位置
            var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(_msg.groupID, _msg.idx);
            List<Vector3> pathList = null;
#if UNITY_EDITOR
            if (!GroundParingSpacePathManager.IsExist(pathUnit.entryObservePath))
            {
                string e = string.Format("大门排队满了,分配的地面停车场游客居然没有回去的观察路线groupID={0}, idx = {1}",
                    _msg.groupID, _msg.idx);
                throw new System.Exception(e);
            }
#endif
            pathList = pathUnit.entryObservePath;
            entity.pathList.Clear();
            entity.pathList.AddRange(pathList);
            entity.pathList.Reverse();
            if (entity.groundParkingGroupID == _msg.groupID && entity.groundParkingIdx == _msg.idx)
            {
                EntityVisitor.GodownPath(entity, entity.pathList, true);
                return;
            }
            entity.groundParkingGroupID = _msg.groupID;
            entity.groundParkingIdx = _msg.idx;
            entity.pathList.Insert(0, entity.position);
            EntityVisitor.GodownPath(entity, entity.pathList, true);
            return;
        }

        public void ExpressionScaleAnim(Transform trans, float orgScale, float maxScale, float animDuration, float expDuration)
        {
            Vector3 scaleOrg = Vector3.one * orgScale;
            trans.localScale = scaleOrg;
            Vector3 scale = scaleOrg;
            Tween twScale = DOTween.To(() => scale, x => scale = x, Vector3.one * maxScale, animDuration);
            twScale.SetEase(Ease.InSine);
            twScale.SetLoops((int)(expDuration / animDuration), LoopType.Yoyo);
            twScale.OnUpdate(() =>
            {
                if (trans != null)
                {
                    trans.localScale = scale;
                }
            });
            twScale.OnComplete(() =>
            {
                if (trans != null)
                {
                    trans.localScale = Vector3.zero;
                }
            });
        }
    }
}
