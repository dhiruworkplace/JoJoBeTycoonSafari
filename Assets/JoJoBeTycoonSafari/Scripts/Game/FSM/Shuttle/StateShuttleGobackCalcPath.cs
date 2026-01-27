/*******************************************************************
* FileName:     StateShuttleGobackCalcPath.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-27
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
using UFrame.BehaviourFloat;

namespace ZooGame
{
    /// <summary>
    /// 摆渡车下完人回来
    /// 场景扩
    ///     车不变
    ///     改路的终点
    /// </summary>
    public class StateShuttleGobackCalcPath : FSMState
    {
        bool isStateShuttleGobackDynamicPath = false;
        public StateShuttleGobackCalcPath(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateShuttleGobackCalcPath.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "Shuttle_{0}_{1}_{2}", entity.entityID, (ShuttleState)this.preStateName, (ShuttleState)this.stateName);

            isStateShuttleGobackDynamicPath = false;

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived,
                this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath,
                this.OnBroadcastAfterExtendSceneAndModifiedPath);


            //var cfgPath = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().Path_ShuttleGobackDynamic);
            //var pathList = new List<Vector3>();
            //pathList.Add(entity.position);
            //pathList.AddRange(cfgPath);
            //entity.followPath.Init(entity, pathList.ToArray(), pathList[0], 0, entity.moveSpeed, false);
            //entity.followPath.Run();
            

            var cfgPathStatic = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().Path_ShuttleGobackStatic);
            var cfgPathDynamic = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().Path_ShuttleGobackDynamic);
            //var pathList = new List<Vector3>();
            entity.pathList.Clear();
            entity.pathList.AddRange(cfgPathStatic);
            entity.pathList.Add(cfgPathDynamic[0]);
            entity.followPath.Init(entity, entity.pathList, entity.pathList[0], 0, entity.moveSpeed, false);
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
            AddConvertCond((int)ShuttleState.GobackDynamicPath, this.ToStateShuttleGobackDynamicPath);

        }

        protected bool ToStateShuttleGobackDynamicPath()
        {
            return isStateShuttleGobackDynamicPath;
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
                //EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                isStateShuttleGobackDynamicPath = true;
            }
        }

        protected void OnBroadcastAfterExtendSceneAndModifiedPath(Message msg)
        {
            Vector3 extendOffset = Vector3.zero;
            extendOffset.x -= Config.globalConfig.getInstace().ZooPartResLen;
            var entity = (this.fsmCtr as FSMMachineShuttle).ownerEntity;
            entity.followPath.ModifyPath(OnModifyPath, extendOffset);
        }

        /// <summary>
        /// 改终点
        /// </summary>
        /// <param name="followPath"></param>
        /// <param name="offset"></param>
        protected void OnModifyPath(FollowPath followPath, Vector3 offset)
        {
            //if (followPath.pathPosList.Length > 1)
            //{
            //    for (int i = 1; i < followPath.pathPosList.Length; i++)
            //    {
            //        followPath.pathPosList[i] += offset;
            //    }

            //    followPath.nextPos += offset;
            //}

            int pathNodeNum = followPath.pathPosList.Count;
            if (pathNodeNum > 1)
            {
                followPath.pathPosList[pathNodeNum - 1] += offset;
                if (followPath.nextPosIdx == pathNodeNum - 1)
                {
                    followPath.nextPos += offset;
                }
            }
        }

    }

}
