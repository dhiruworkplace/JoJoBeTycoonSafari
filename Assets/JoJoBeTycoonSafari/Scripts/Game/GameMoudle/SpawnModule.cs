/*******************************************************************
* FileName:     SpawnVisitorModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-9
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.BehaviourFloat;
using UFrame.MessageCenter;
using UnityEngine;
using Logger;
using ZooGame.MessageCenter;
using UFrame.OrthographicCamera;
using UFrame.EntityFloat;
using ZooGame.GlobalData;
using ZooGame.Path.StraightLine;

namespace ZooGame
{
    public class SpawnModule : GameModule
    {
        bool isRandomSpawn = true;
        bool isTracedVisitor = false;
        bool isTracedCar = false;
        bool onePlayer = true;
        public SpawnModule(int orderID) : base(orderID) { }

        public override void Init()
        {
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SpawnVisitorFromCar, this.OnSpawnVisitorFromCar);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SpawnVisitorFromShip, this.OnSpawnVisitorFromShip);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SpawnShuttle, this.OnSpawnShuttle);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SpawnVisitorFromGroundParking, this.OnSpawnVisitorFromGroundParking);
            
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SpawnVisitorFromCar, this.OnSpawnVisitorFromCar);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SpawnVisitorFromShip, this.OnSpawnVisitorFromShip);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SpawnShuttle, this.OnSpawnShuttle);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SpawnVisitorFromGroundParking, this.OnSpawnVisitorFromGroundParking);
            isRandomSpawn = true;
            this.Stop();
        }

        protected void OnSpawnVisitorFromCar(Message msg)
        {
            var _msg = msg as SpawnVisitorFromCar;
            EntityVisitor entity = null;

            VisitorState defaultState = VisitorState.GotoZooEntry;
            switch (_msg.stage)
            {
                case VisitorStage.GotoZoo:
                    //defaultState = VisitorState.EnterZoo;
                    defaultState = VisitorState.GotoZooEntry;
                    break;
                case VisitorStage.GotoParking:
                    defaultState = VisitorState.GotoParking;
                    break;
                default:
                    string e = string.Format("VisitorStage 异常{0}", _msg.stage);
                    throw new System.Exception(e);
            }

            if (isRandomSpawn)
            {
                //entity = EntityManager.GetInstance().GetRandomEntity(
                //    ResType.Visitor, EntityFuncType.Visitor_From_Car) as EntityVisitor;
                entity = EntityManager.GetInstance().GetRandomEntity(
                    ResType.Visitor, _msg.funcType) as EntityVisitor;
            }
            else
            {
                //entity = EntityManager.GetInstance().GenEntityGameObject(
                //    101, EntityFuncType.Visitor_From_Car) as EntityVisitor;
                entity = EntityManager.GetInstance().GenEntityGameObject(
                   101, _msg.funcType) as EntityVisitor;
            }

            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}", entity.entityID);

            EntityManager.GetInstance().AddToEntityMovables(entity);

            LogWarp.LogError("测试：  生成游客  name=  "+ entity.mainGameObject.name);


            if (entity.anim == null)
            {
                entity.anim = new SimpleAnimation();
            }
            //entity.anim.Init(entity.mainGameObject);
            entity.InitAnim();

            entity.moveSpeed = Config.globalConfig.getInstace().ZooVisitorSpeed;
            if (entity.followPath == null)
            {
                entity.followPath = new FollowPath();
            }

            if (entity.fsmMachine == null) 
            {
                entity.fsmMachine = new FSMMachineVisitor(entity);
                //entity.fsmMachine.AddState(new StateVisitorEnterZoo((int)VisitorState.EnterZoo,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorGotoZooEntry((int)VisitorState.GotoZooEntry,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorLeaveZooEntryQueueFull((int)VisitorState.LeaveZooEntryQueueFull,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorEntryQueue((int)VisitorState.EntryQueue,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorStayFirstPosInEntryQueue((int)VisitorState.StayFirstPosInEntryQueue,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorChoseLittleZoo((int)VisitorState.ChoseLittleZoo,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorEnterLittleZooApply((int)VisitorState.EnterLittleZooApply,
                    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorStayWaitSeat((int)VisitorState.StayWaitSeat,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorStayVisitSeat((int)VisitorState.StayVisitSeat,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorLeaveNonLittleZoo((int)VisitorState.LeaveNonLittleZoo,
                    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorCrossGroupPath((int)VisitorState.CrossGroupPath,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorLeaveFromZooEntry((int)VisitorState.LeaveFromZooEntry,
                    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorGotoStartOfExitGateEntryPath((int)VisitorState.GotoStartOfExitGateEntryPath,
                //    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorGotoExitGateEntryQueue((int)VisitorState.GotoExitGateEntryQueue,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorGotoParking((int)VisitorState.GotoParking,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorGotoGroundParking((int)VisitorState.GotoGroundParking,
                    entity.fsmMachine));
                

                entity.fsmMachine.SetDefaultState((int)defaultState);
            }
            else
            {
                //LogWarp.LogError("pool游客");
                //entity.fsmMachine.GotoState((int)VisitorState.GotoZooEntry);

                entity.fsmMachine.GotoState((int)defaultState);
            }
            entity.Active();
            //entity.fsmMachine.Run();
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);

            TracedCamera(entity,isTracedVisitor);
        }

        protected void OnSpawnVisitorFromShip(Message msg)
        {

        }

        protected void OnSpawnShuttle(Message msg)
        {
            var _msg = msg as SpawnShuttle;

            EntityShuttle entity = null;
            if (isRandomSpawn)
            {
                entity = EntityManager.GetInstance().GetRandomEntity(
                    ResType.Shuttle, EntityFuncType.Shuttle) as EntityShuttle;
            }
            else
            {
                entity = EntityManager.GetInstance().GenEntityGameObject(
                    6001, EntityFuncType.Shuttle) as EntityShuttle;
            }

            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "Shuttle_{0}", entity.entityID);

            EntityManager.GetInstance().AddToEntityMovables(entity);

            entity.SetVisistorList(_msg.shuttleVisitorList);

            if (entity.anim == null)
            {
                entity.anim = new SimpleAnimation();
            }
            entity.anim.Init(entity.mainGameObject);

            entity.moveSpeed = Config.globalConfig.getInstace().ShuttleBaseSpeed;
            if (entity.followPath == null)
            {
                entity.followPath = new FollowPath();
            }

            if (entity.fsmMachine == null)
            {
                entity.fsmMachine = new FSMMachineShuttle(entity);

                entity.fsmMachine.AddState(new StateShuttleGotoDynamicPath((int)ShuttleState.GotoDynamicPath,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateShuttleGotoCalcPath((int)ShuttleState.GotoCalcPath,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateShuttleGobackCalcPath((int)ShuttleState.GobackCalcPath,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateShuttleGobackDynamicPath((int)ShuttleState.GobackDynamicPath,
                    entity.fsmMachine));
                
                entity.fsmMachine.SetDefaultState((int)ShuttleState.GotoDynamicPath);
            }
            else
            {
                entity.fsmMachine.SetDefaultState((int)ShuttleState.GotoDynamicPath);
            }
            entity.Active();

            TracedCamera(entity,isTracedCar);

        }

        protected void OnSpawnVisitorFromGroundParking(Message msg)
        {
            var _msg = msg as SpawnVisitorFromGroundParking;
            //path_touristcar_intoupline
            //"path_tourist_intoupline";
            //var path = PathManager.GetInstance().GetPath("path_touristcar_intoupline");
            //var carBaseLinePos = path[_msg.groupID];
            //var lastCorner = carBaseLinePos - GlobalDataManager.GetInstance().SceneForward * (8 + _msg.idx * 5);
            //var visitorBaseLinePos = PathManager.GetInstance().GetPath("path_tourist_intoupline")[_msg.idx];
            //LogWarp.LogErrorFormat("{0}-{1}:{2},{3}", _msg.groupID, _msg.idx, lastCorner, visitorBaseLinePos);

            EntityVisitor entity = null;

            //VisitorState defaultState = VisitorState.EnterZoo;

            //switch (_msg.stage)
            //{
            //    case VisitorStage.GotoZoo:
            //        //defaultState = VisitorState.EnterZoo;
            //        defaultState = VisitorState.GotoZooEntry;
            //        break;
            //    case VisitorStage.GotoParking:
            //        defaultState = VisitorState.GotoParking;
            //        break;
            //    default:
            //        string e = string.Format("VisitorStage 异常{0}", _msg.stage);
            //        throw new System.Exception(e);
            //}

            if (isRandomSpawn)
            {
                //entity = EntityManager.GetInstance().GetRandomEntity(
                //    ResType.Visitor, EntityFuncType.Visitor_From_Car) as EntityVisitor;
                entity = EntityManager.GetInstance().GetRandomEntity(
                    ResType.Visitor, _msg.funcType) as EntityVisitor;
            }
            else
            {
                //entity = EntityManager.GetInstance().GenEntityGameObject(
                //    101, EntityFuncType.Visitor_From_Car) as EntityVisitor;
                entity = EntityManager.GetInstance().GenEntityGameObject(
                   101, _msg.funcType) as EntityVisitor;
            }
            entity.groundParkingGroupID = _msg.groupID;
            entity.groundParkingIdx = _msg.idx;
            //LogWarp.LogErrorFormat("{0} visitor SpawnModule groupID={1}, idx={2}", entity.entityID,
            //    entity.groundParkingGroupID, entity.groundParkingIdx);

            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_ground_{0}", entity.entityID);

            EntityManager.GetInstance().AddToEntityMovables(entity);

            if (entity.anim == null)
            {
                entity.anim = new SimpleAnimation();
            }
            //entity.anim.Init(entity.mainGameObject);
            entity.InitAnim();

            entity.moveSpeed = Config.globalConfig.getInstace().ZooVisitorSpeed;
            if (entity.followPath == null)
            {
                entity.followPath = new FollowPath();
            }

            if (entity.fsmMachine == null)
            {
                entity.fsmMachine = new FSMMachineVisitor(entity);
                //entity.fsmMachine.AddState(new StateVisitorEnterZoo((int)VisitorState.EnterZoo,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorGotoZooEntry((int)VisitorState.GotoZooEntry,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorLeaveZooEntryQueueFull((int)VisitorState.LeaveZooEntryQueueFull,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorEntryQueue((int)VisitorState.EntryQueue,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorStayFirstPosInEntryQueue((int)VisitorState.StayFirstPosInEntryQueue,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorChoseLittleZoo((int)VisitorState.ChoseLittleZoo,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorEnterLittleZooApply((int)VisitorState.EnterLittleZooApply,
                    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorStayWaitSeat((int)VisitorState.StayWaitSeat,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorStayVisitSeat((int)VisitorState.StayVisitSeat,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorLeaveNonLittleZoo((int)VisitorState.LeaveNonLittleZoo,
                    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorCrossGroupPath((int)VisitorState.CrossGroupPath,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorLeaveFromZooEntry((int)VisitorState.LeaveFromZooEntry,
                    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorGotoStartOfExitGateEntryPath((int)VisitorState.GotoStartOfExitGateEntryPath,
                //    entity.fsmMachine));
                //entity.fsmMachine.AddState(new StateVisitorGotoExitGateEntryQueue((int)VisitorState.GotoExitGateEntryQueue,
                //    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorGotoParking((int)VisitorState.GotoParking,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateVisitorGotoGroundParking((int)VisitorState.GotoGroundParking,
                    entity.fsmMachine));

                entity.fsmMachine.SetDefaultState((int)VisitorState.GotoZooEntry);
            }
            else
            {
                //LogWarp.LogError("pool游客");
                //entity.fsmMachine.GotoState((int)VisitorState.GotoZooEntry);

                entity.fsmMachine.GotoState((int)VisitorState.GotoZooEntry);
            }
            entity.Active();
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
        }

        protected void TracedCamera(EntityGameObject entity,bool isBool)
        {
            if (IsTraceBool(isBool))
            {
                return;
            }
            if (isTracedVisitor == false)
            {
                isTracedVisitor = true;

            }else if (isTracedCar == false)
            {
                isTracedCar = true;
                ////模型到达出口   调用新手引导的内容  显示步骤13
                //if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
                //{
                //    UIGuide[] pAllObjects = (UIGuide[])Resources.FindObjectsOfTypeAll(typeof(UIGuide));
                //    //延时刷新关于出口的暂停和文本
                //    pAllObjects[0].VerdictExit(4f);
                //}

            }
            LogWarp.LogError("测试：     相机跟随");
            
        }
        private bool IsTraceBool(bool isBool)
        {
            var playerData = GlobalData.GlobalDataManager.GetInstance().playerData;
            if (!playerData.playerZoo.isGuide || isBool)
            {
                return true;
            }
            return false;
        }

    }

}
