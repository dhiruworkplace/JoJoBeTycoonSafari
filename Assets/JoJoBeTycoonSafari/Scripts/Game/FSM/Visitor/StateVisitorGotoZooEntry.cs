/*******************************************************************
* FileName:     StateVisitorGotoZooEntry.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-12
* Description:  
* other:    
********************************************************************/


using Game.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using Game.MessageCenter;
using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 游客的第一个状态
    /// 游客到走到空闲入口，或者没有空闲入口离开的过程。
    /// 去走那条路是由EntryGateModule决定，这里负责请求，并接收返回。
    /// 没有空闲离开走哪条路也是。
    /// </summary>
    public class StateVisitorGotoZooEntry : FSMState
    {
        bool isToVisitorStateEntryQueue = false;
        bool isToVisitorStateLeaveZooEntryQueueFull = false;
        bool onePlayer = true;

        /// <summary>
        /// 是否到观察点位置
        /// </summary>
        bool isArrivedObservePos = false;
        bool isHold = false;

        public StateVisitorGotoZooEntry(int stateName, FSMMachine fsmCtr):
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            //LogWarp.Log("VisitorGotoZooEntry.Enter");
            base.Enter(preStateName);

            isToVisitorStateLeaveZooEntryQueueFull = false;
            isToVisitorStateEntryQueue = false;
            //isStartGotoEntry = false;

            isArrivedObservePos = false;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} VisitorGotoZooEntry.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            entity.Reset();
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}, {2}, {3}", 
                entity.entityID, Config.globalConfig.getInstace().VisitorWalk, 
                entity.mainGameObject.GetInstanceID(),
                entity.anim.animation.gameObject.GetInstanceID()
                );
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToEntryQueuePlaceHolderReply, this.OnAddVisitorToEntryQueuePlaceHolderReply);
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastForwardOneStepInEntryGateQueue, OnBroadcastForwardOneStepInExitGateQueue);
            //去观察点
            string pathName;
            EntityFuncType entityFuncType = (EntityFuncType)(entity.entityFuncType);
            switch (entityFuncType)
            {
                case EntityFuncType.Visitor_From_Ship:
                    pathName = Config.globalConfig.getInstace().AdvertVisitorInto_2;
                    EntityVisitor.GodownPath(entity, pathName, true);
                    break;
                case EntityFuncType.Visitor_From_Car:
                    pathName = Config.globalConfig.getInstace().EntryQueueObservePath;
                    EntityVisitor.GodownPath(entity, pathName, true);
                    break;
                case EntityFuncType.Visitor_From_GroundParking:
                    //LogWarp.LogErrorFormat("entityID={0} StateVisitorGotoZooEntry  groupID={1}, idx={2}", entity.entityID, entity.groundParkingGroupID, entity.groundParkingIdx);
                    var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(entity.groundParkingGroupID, entity.groundParkingIdx);
                    List<Vector3> pathList = null;
                    if (!GroundParingSpacePathManager.IsExist(pathUnit.entryObservePath))
                    {
                        pathList = GroundParingSpacePathManager.GenObservePath(entity.groundParkingGroupID, entity.groundParkingIdx);
                        GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.EntryObservePath, pathUnit, pathList, null);
                    }
                    pathList = pathUnit.entryObservePath;
                    entity.pathList.Clear();
                    entity.pathList.AddRange(pathList);
                    EntityVisitor.GodownPath(entity, entity.pathList, true);

                    ///*新手引导阶段    添加跟随对象 不做步骤调用(仅限于步骤9)*/
                    if (GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
                    {
                        UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                        if (uIGuidePage == null)
                        {
                            string e1 = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                            throw new System.Exception(e1);
                        }
                        //LogWarp.LogError("测试：  entity.mainGameObject.name " + entity.mainGameObject.name+ "    uIGuidePage.procedure= "+ uIGuidePage.procedure);
                        if (uIGuidePage.procedure == 9&&uIGuidePage.number == 0)
                        {
                            uIGuidePage.entity = entity;
                            uIGuidePage.number = 1;
                        }
                    }

                    break;
                default:
                    string e = string.Format("没有这种游客类型{0}", entityFuncType);
                    throw new System.Exception(e);
            }
            
        }

        public override void Tick(int deltaTimeMS)
        {

        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToEntryQueuePlaceHolderReply, this.OnAddVisitorToEntryQueuePlaceHolderReply);
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastForwardOneStepInEntryGateQueue, OnBroadcastForwardOneStepInExitGateQueue);
            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            this.AddConvertCond((int)VisitorState.EntryQueue, this.ToVisitorStateEntryQueue);
            this.AddConvertCond((int)VisitorState.LeaveZooEntryQueueFull, this.ToVisitorStateLeaveZooEntryQueueFull);
        }

        protected bool ToVisitorStateEntryQueue()
        {
            return this.isToVisitorStateEntryQueue;
        }

        protected bool ToVisitorStateLeaveZooEntryQueueFull()
        {
            return this.isToVisitorStateLeaveZooEntryQueueFull;
        }

        protected void OnAddVisitorToEntryQueuePlaceHolderReply(Message msg)
        {
            var _msg = msg as AddVisitorToEntryQueuePlaceHolderReply;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            //无论成功失败，都有入口ID
            entity.zooEntryID = _msg.entryID;

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态 收到{1}", entity.entityID, msg);
            if (!_msg.result)
            {
                LogWarp.Log("入口所有排队满了, 准备离开, 转离开");
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态 收到{1}入口所有排队满了, 准备离开, 转离开", entity.entityID, msg);
                this.isToVisitorStateLeaveZooEntryQueueFull = true;
                return;
            }

            isHold = true;
            Vector3 endPos = UnityEngine.Vector3.zero;
            EntityFuncType entityFuncType = (EntityFuncType)(entity.entityFuncType);
            switch (entityFuncType)
            {
                case EntityFuncType.Visitor_From_Ship:
                    entity.pathList.Clear();
                    entity.pathList.Add(entity.position);
                    endPos = UnityEngine.Vector3.zero;
                    PathManager.GetInstance().GetPathLastPos(_msg.pathName, ref endPos);
                    entity.pathList.Add(endPos);
                    EntityVisitor.GodownPath(entity, entity.pathList);
                    break;
                case EntityFuncType.Visitor_From_Car:
                    EntityVisitor.GodownPath(entity, _msg.pathName, true);
                    break;
                case EntityFuncType.Visitor_From_GroundParking:
                    entity.pathList.Clear();
                    entity.pathList.Add(entity.position);
                    endPos = Vector3.zero;
                    PathManager.GetInstance().GetPathLastPos(_msg.pathName, ref endPos);
                    entity.pathList.Add(endPos);
                    EntityVisitor.GodownPath(entity, entity.pathList, true);

                    break;
                default:
                    string e = string.Format("没有这种游客类型{0}", entityFuncType);
                    throw new System.Exception(e);
            }

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

            //到达观察点
            if (_msg.followPath.isArrivedEnd && !isArrivedObservePos)
            {
                isArrivedObservePos = true;
                //请求占位
                var toMsg = AddVisitorToEntryQueuePlaceHolderApply.Send(entity.entityID);
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态，发送{1}", entity.entityID, toMsg);

                return;
            }

            //到达终点
            if (_msg.followPath.isArrivedEnd)
            {
                //转排队状态
                //LogWarp.LogErrorFormat("{0} 在VisitorGotoZooEntry状态  准备转 VisitorStateEntryQueue状态", entity.entityID);
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在VisitorGotoZooEntry状态  准备转 VisitorStateEntryQueue状态", entity.entityID);
                this.isToVisitorStateEntryQueue = true;
            }
        }

        protected void OnBroadcastForwardOneStepInExitGateQueue(Message msg)
        {
            var _msg = msg as BroadcastForwardOneStepInQueue;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            if (entity.zooEntryID == _msg.entryID && entity.entityID != _msg.entityID && isHold)
            {
                //entity.numOfZooEntryQueueForwardOne++;
                //string e = string.Format("{0} 在StateVisitorGotoZooEntry 收到了BroadcastForwardOneStepInQueue", entity.entityID);
                //throw new System.Exception(e);
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在StateVisitorGotoZooEntry 收到步进", entity.entityID);
            }
        }
    }
}
