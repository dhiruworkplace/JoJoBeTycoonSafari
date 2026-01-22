/*******************************************************************
* FileName:     ParkingCenter.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-20
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame;
using UFrame.BehaviourFloat;
using Game.Path.StraightLine;
using UFrame.MessageCenter;
using Game.MiniGame;
using Config;
using Logger;
using Game.MessageCenter;
using UFrame.EntityFloat;
using UFrame.MiniGame;
using UFrame.OrthographicCamera;
using Game.GlobalData;
using System;
using DG.Tweening;

namespace Game
{
    /// <summary>
    /// 停车场模块，包含地上地下
    /// </summary>
    public partial class ParkingCenter : GameModule
    {
        GroundParking groundParking;

        int enterCarAccumulativeTime;
        int enterCarSpawnInterval;

        /// <summary>
        /// 总的停车数量上限( 地上，地下总和)
        /// </summary>
        int numMaxParking;

        /// <summary>
        /// 地面停车场停车数量
        /// </summary>
        int numGroundParkingCar;

        /// <summary>
        /// 地下的停车场停车数量
        /// </summary>
        int numUnderParkingCar;

        bool isSpawnEnterCar;

        //bool isRandomCar = false;

        /// <summary>
        /// 停车牌个位数
        /// </summary>
        GameObject parkingCarNum0;

        /// <summary>
        /// 停车牌十位数
        /// </summary>
        GameObject parkingCarNum1;

        /// <summary>
        /// 显示牌显示数量
        /// </summary>
        int numShowParking = 0;

        int lastParkingCarNum0 = 0;
        int lastParkingCarNum1 = 0;

        PlayerData playerData;

        //bool oneCar = true;

        public ParkingCenter(int orderID) : base(orderID) { }

        public override void Init()
        {
            playerData = GlobalDataManager.GetInstance().playerData;
            MessageManager.GetInstance().Regist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.LetGroundParingCarLeave, OnLetGroundParingCarLeave);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorWhereLeaveFromApply, OnVisitorWhereLeaveFromApply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.ParkingCarInGroundParking, OnParkingCarInGroundParking);
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.ArrivedRightAngle, this.OnArrivedRightAngle);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SpawnVisitorCarLeaveZoo, this.OnSpawnVisitorCarLeaveZoo);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastParkingProfitLevelOfPlayerData, OnBroadcastParkingLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.DirectMinusOneUnderParkingNum, OnDirectMinusOneUnderParkingNum);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastParkingSpaceLevelOfPlayerData, this.OnBroadcastParkingSpaceLevelOfPlayerData);//接收停车场的位置数量等级的广播
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastParkingEnterCarSpawnLevelOfPlayerData, this.OnBroadcastParkingCoolingLevelOfPlayerData);//接收停车场的来客流量等级的广播

            InitGroupParkingData();
            CalcEnterCarSpawnInterval();
            CalcMaxParkingNum();
        }

        
        /// <summary>
        /// 初始化地面停车场信息
        /// </summary>
        protected void InitGroupParkingData()
        {
            groundParking = new GroundParking();
            groundParking.Init(playerData.playerZoo.parkingCenterData.parkingSpaceLevel);
        }

        protected void InitParkingNumObject()
        {
            parkingCarNum0 = GameObject.Find(globalConfig.getInstace().ParkingNumber0);
            parkingCarNum1 = GameObject.Find(globalConfig.getInstace().ParkingNumber1);

#if UNITY_EDITOR
            if (parkingCarNum0 == null || parkingCarNum1 == null)
            {
                string e = string.Format("查找停车场数字显示牌异常 个位=[{0}], 十位=[{1}]",
                    globalConfig.getInstace().ParkingNumber0,
                    globalConfig.getInstace().ParkingNumber1);
                throw new System.Exception(e);
            }
#endif
        }

        /// <summary>
        /// 显示剩余停车数量
        /// </summary>
        /// <param name="num"></param>
        protected void ShowLeftParkingNum(int num)
        {
            int numShowParkingClamp = Mathf.Clamp(num, 0, globalConfig.getInstace().MaxParkingNumber);
            int num0 = numShowParkingClamp % 10;
            int num1 = numShowParkingClamp / 10;
            //LogWarp.LogFormat("ShowLeftParkingNum {0}, {1}, {2}", numShowParkingClamp, num0, num1);
            ShowLeftParkingNum(num0, lastParkingCarNum0, parkingCarNum0);
            ShowLeftParkingNum(num1, lastParkingCarNum1, parkingCarNum1);
            lastParkingCarNum0 = num0;
            lastParkingCarNum1 = num1;
        }

        protected void ShowLeftParkingNum(int num, int lastNum, GameObject numGoRoot)
        {
#if UNITY_EDITOR
            if (num < 0 || lastNum < 0 || num > 9 || lastNum > 9)
            {
                string e = string.Format("ShowLeftParkingNum 异常 {0}, {1}", num, lastNum);
                throw new System.Exception(e);
            }
#endif
            numGoRoot.transform.GetChild(lastNum).gameObject.SetActive(false);
            numGoRoot.transform.GetChild(num).gameObject.SetActive(true);
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.LetGroundParingCarLeave, OnLetGroundParingCarLeave);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorWhereLeaveFromApply, OnVisitorWhereLeaveFromApply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.ParkingCarInGroundParking, OnParkingCarInGroundParking);
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.ArrivedRightAngle, this.OnArrivedRightAngle);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SpawnVisitorCarLeaveZoo, this.OnSpawnVisitorCarLeaveZoo);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastParkingProfitLevelOfPlayerData, OnBroadcastParkingLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.DirectMinusOneUnderParkingNum, OnDirectMinusOneUnderParkingNum);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastParkingSpaceLevelOfPlayerData, this.OnBroadcastParkingSpaceLevelOfPlayerData);//接收停车场的位置数量等级的广播
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastParkingEnterCarSpawnLevelOfPlayerData, this.OnBroadcastParkingCoolingLevelOfPlayerData);//接收停车场的来客流量等级的广播

            groundParking = null;

            Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }

            TickEnterCar(deltaTimeMS);
        }

        protected void OnLoadZooSceneFinished(Message msg)
        {
            InitParkingNumObject();
            //LogWarp.LogError("ASSSSSSSSSSSSSSSSSSS  "+ numMaxParking);
            this.numShowParking = numMaxParking;
            ShowLeftParkingNum(numShowParking);
        }

        protected void OnLetGroundParingCarLeave(Message msg)
        {
            var _msg = msg as MessageGroundParkingSpace;
            var parkingSpace = this.groundParking.GetSpace(_msg.groupID, _msg.idx);
            numShowParking++;
            ShowLeftParkingNum(numShowParking);
            parkingSpace.SetFree();

            /*强制关闭新手引导*/
            if (GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                uIGuidePage.procedure = 25;
                uIGuidePage.OnClickDialogBoxButton();
            }
        }

        protected void OnVisitorWhereLeaveFromApply(Message msg)
        {
            //根据地上地下的车位数量随机从地上回还是地下回
            var _msg = msg as VisitorWhereLeaveFromApply;

#if UNITY_EDITOR
            if (numUnderParkingCar < 1 && numGroundParkingCar < 1)
            {
                string e = string.Format("{0} 申请离开时地上地下都没有车numUnderParkingCar={1},numGroundParkingCar={2}", _msg.entityID,
                    numUnderParkingCar, numGroundParkingCar);
                throw new System.Exception(e);
            }
#endif
            float p = (float)numGroundParkingCar / (float)(numGroundParkingCar + Mathf.Max(0, numUnderParkingCar));
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r > p)
            {
                //从地下走
                DebugFile.GetInstance().WriteKeyFile("FromUnder", "{0}", _msg.entityID);
                this.numUnderParkingCar--;
                VisitorWhereLeaveFromReply.Send(_msg.entityID, false, 0, 0);
                return;
            }

            ParkingSpace parkingSpace = groundParking.GetParkingSpace();
            if (parkingSpace == null)
            {
                //随机到地上,但地上没有停好的车,从地下走
                DebugFile.GetInstance().WriteKeyFile("FromUnder", "{0}", _msg.entityID);
                this.numUnderParkingCar--;
                VisitorWhereLeaveFromReply.Send(_msg.entityID, false, 0, 0);
                return;
            }

            this.numGroundParkingCar--;
            parkingSpace.SetBeLocked();
            VisitorWhereLeaveFromReply.Send(_msg.entityID, true, parkingSpace.groupID, parkingSpace.idx);
        }

        protected void OnParkingCarInGroundParking(Message msg)
        {
            var _msg = msg as MessageGroundParkingSpace;
            var paringSpace = groundParking.GetSpace(_msg.groupID, _msg.idx);
            paringSpace.SetParking();
            numShowParking--;
            ShowLeftParkingNum(numShowParking);
        }

        protected void OnArrivedRightAngle(Message msg)
        {
            var _msg = msg as MessageArrivedEx;
            var followPath = _msg.followPath as FollowPathRightAngles;
            if (followPath.isArrivedEnd && followPath.ownerEntity.entityFuncType == (int)EntityFuncType.VisitorCar_EnterZoo)
            {
                DebugFile.GetInstance().WriteKeyFile(followPath.ownerEntity.entityID, "{0} {1}到达", followPath.ownerEntity.entityID, EntityFuncType.VisitorCar_EnterZoo);
                this.ArrivedVisitorCar(followPath, EntityFuncType.VisitorCar_EnterZoo);
            }
            else if (followPath.isArrivedEnd && followPath.ownerEntity.entityFuncType == (int)EntityFuncType.VisitorCar_LeaveZoo)
            {
                DebugFile.GetInstance().WriteKeyFile(_msg.followPath.ownerEntity.entityID, "{0} {1}到达", followPath.ownerEntity.entityID, EntityFuncType.VisitorCar_LeaveZoo);
                this.ArrivedVisitorCar(followPath, EntityFuncType.VisitorCar_LeaveZoo);
            }
        }

        protected void OnSpawnVisitorCarLeaveZoo(Message msg)
        {
            numShowParking++;
            ShowLeftParkingNum(numShowParking);
            this.SpawnVisitorCar(EntityFuncType.VisitorCar_LeaveZoo);
        }

        protected void OnBroadcastParkingLevelOfPlayerData(Message msg)
        {
            var _msg = msg as BroadcastValueOfPlayerData;
            ////重新计算频率
            //CalcEnterCarSpawnInterval();
            ////重新计算总量,并更新显示牌
            //int oldNumMaxParking = numMaxParking;
            //CalcMaxParkingNum();
            //numShowParking += (numMaxParking - oldNumMaxParking);
            ////numShowParking = numMaxParking - numUnderParkingCar - numGroundParkingCar;
            //ShowLeftParkingNum(numShowParking);
            ////检查地面停车场数量是否增加
            //int numSpace = GetGroundParkingNumber(this.playerData.playerZoo.parkingCenterData.parkingSpaceLevel);
            //int numGroup = numSpace / globalConfig.getInstace().NumGroundParkingGroupSpace;
            //int oldnumGroup = this.groundParking.groupParkingSpaceDic.Count;
            //for (int i = oldnumGroup; i < numGroup; i++)
            //{
            //    var group = new GroupParkingSpace(i, globalConfig.getInstace().NumGroundParkingGroupSpace);
            //    groundParking.groupParkingSpaceDic.Add(i, group);
            //}
            ////查看新的等级有没有导致停车场外观变化
            //int oldLevel = _msg.currVal - _msg.deltaVal;
            //int currLevel = _msg.currVal;
            //var cellBuildUp = Config.parkingConfig.getInstace().getCell(1);

            //int oldResIdx = FindLevelRangIndex(cellBuildUp.openlv, oldLevel);
            //int currResIdx = FindLevelRangIndex(cellBuildUp.openlv, currLevel);
            //if (oldResIdx != currResIdx)
            //{
            //    LogWarp.LogError("测试：换停车场下标    oldResIdx =" + oldResIdx + "   currResIdx=" + currResIdx);
            //    //卸载旧的，加载新的
            //    LoadParkingScene(currResIdx);

            //}
        }

        /// <summary>
        /// 监听停车场的停车位的数量升级
        /// </summary>
        /// <param name="obj"></param>
        private void OnBroadcastParkingSpaceLevelOfPlayerData(Message obj)
        {
            var _msg = obj as BroadcastValueOfPlayerData;

            //查看新的等级有没有导致停车场外观变化
            int oldLevel = _msg.currVal - _msg.deltaVal;
            int currLevel = _msg.currVal;
            var cellBuildUp = Config.parkingConfig.getInstace().getCell(1);

            int oldResIdx = FindLevelRangIndex(cellBuildUp.openlv, oldLevel);
            int currResIdx = FindLevelRangIndex(cellBuildUp.openlv, currLevel);

            if (oldResIdx != currResIdx)
            {
                //卸载旧的，加载新的
                LoadParkingScene(currResIdx);

            }

            //重新计算总量,并更新显示牌
            int oldNumMaxParking = numMaxParking;
            CalcMaxParkingNum();
            numShowParking += (numMaxParking - oldNumMaxParking);

            //LogWarp.LogErrorFormat("  测试：   小汽车刷新速度：{0}  停车场总停车的数量={1} 现在显示的数量={2}", enterCarSpawnInterval, numMaxParking, numShowParking);

            //numShowParking = numMaxParking - numUnderParkingCar - numGroundParkingCar;
            ShowLeftParkingNum(numShowParking);
            //检查地面停车场数量是否增加
            int numSpace = GetGroundParkingNumber(this.playerData.playerZoo.parkingCenterData.parkingSpaceLevel);

            //LogWarp.LogError("测试：检查地面停车场数量是否增加    numSpace=" + numSpace);

            int numGroup = numSpace / globalConfig.getInstace().NumGroundParkingGroupSpace;
            int oldnumGroup = this.groundParking.groupParkingSpaceDic.Count;
            for (int i = oldnumGroup; i < numGroup; i++)
            {
                var group = new GroupParkingSpace(i, globalConfig.getInstace().NumGroundParkingGroupSpace);
                groundParking.groupParkingSpaceDic.Add(i, group);
            }


        }

        /// <summary>
        /// 监听停车场来客速度的流量升级
        /// </summary>
        /// <param name="obj"></param>
        private void OnBroadcastParkingCoolingLevelOfPlayerData(Message obj)
        {
            var _msg = obj as BroadcastValueOfPlayerData;
            //重新计算频率
            CalcEnterCarSpawnInterval();
            
        }

        protected void OnDirectMinusOneUnderParkingNum(Message msg)
        {
            this.numUnderParkingCar--;
        }

        protected void ArrivedVisitorCar(FollowPathRightAngles followPath, EntityFuncType entityFuncType)
        {
            DebugFile.GetInstance().WriteKeyFile(followPath.ownerEntity.entityID, "{0} {1} ArrivedVisitorCar",
                followPath.ownerEntity.entityID, entityFuncType);

            var entity = followPath.ownerEntity as EntityVisitorCar;
            EntityManager.GetInstance().RemoveFromEntityMovables(entity);

            // 生成游客
            LogWarp.Log("car -> visistor");
            switch (entityFuncType)
            {
                case EntityFuncType.VisitorCar_EnterZoo:
                    //车到了，刷下游客停车场数量
                    numShowParking--;
                    ShowLeftParkingNum(numShowParking);
                    SpawnVisitorFromCar.Send(VisitorStage.GotoZoo, EntityFuncType.Visitor_From_Car);
#if DEBUG_VISIT
                    BroadcastNum.Send((int)GameMessageDefine.BroadcastVisitorNum, numGroundParkingCar + numUnderParkingCar, 0f, 0);
                    BroadcastNum.Send((int)GameMessageDefine.BroadcastMaxVisitorNum, numMaxParking, 0f, 0);
#endif
                    break;
                case EntityFuncType.VisitorCar_LeaveZoo:
                    break;
            }

        }

        protected void CalcEnterCarSpawnInterval()
        {
#if DEBUG_VISIT
            enterCarSpawnInterval = 60000 / GetNumOfVisitorPerMinute(2000);
#else
            enterCarSpawnInterval = 60000 / GetParkingEnterCarSpawn();
#endif
        }

        protected void CalcMaxParkingNum()
        {
#if DEBUG_VISIT
            numMaxParking = GetParkingNumber(2000);
#else
            numMaxParking = GetParkingSpace();
#endif
            LogWarp.LogFormat("parking numMaxParking = {0}", numMaxParking);
        }

        protected void TickEnterCar(int deltaTimeMS)
        {
            if ((numUnderParkingCar + numGroundParkingCar) >= this.numMaxParking)
            {
                return;
            }

            ParkingSpace parkingSpace = null;
            this.enterCarAccumulativeTime += deltaTimeMS;
            if (this.enterCarAccumulativeTime >= this.enterCarSpawnInterval || !isSpawnEnterCar)
            {
                //保留超出部分
                this.enterCarAccumulativeTime -= this.enterCarSpawnInterval;
                //生成
                LogWarp.Log("SpawnEnterZooCar");
                parkingSpace = groundParking.GetFreeParkingSpace();
                if (parkingSpace == null)
                {
                    int currGround = GetGroundParkingNumber(playerData.playerZoo.parkingCenterData.parkingSpaceLevel);
                    if (this.numMaxParking > currGround)
                    {
                        SpawnVisitorCar(EntityFuncType.VisitorCar_EnterZoo);
                        ++numUnderParkingCar;
                        this.isSpawnEnterCar = true;
                    }

                }
                else
                {
                    SpawnInGroundParkingCar(EntityFuncType.GroundParkingCar, parkingSpace);
                    ++numGroundParkingCar;
                    this.isSpawnEnterCar = true;
                }
            }
        }

        /// <summary>
        /// 地下停车场的车
        /// </summary>
        /// <param name="entityFuncType"></param>
        protected void SpawnVisitorCar(EntityFuncType entityFuncType)
        {
            EntityVisitorCar entity = null;
            //if (isRandomCar)
            {
                entity = EntityManager.GetInstance().GetRandomEntity(ResType.Car, entityFuncType) as EntityVisitorCar;
            }
            //else
            //{
            //    entity = EntityManager.GetInstance().GenEntityGameObject(2, EntityFuncType.VisitorCar_EnterZoo) as EntityVisitorCar;
            //}

            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "car_{0}_{1}_{2}", entity.mainGameObject.GetInstanceID(), entityFuncType, entity.entityID);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 生成 {1}", entity.entityID, entity.mainGameObject.GetInstanceID());
            DebugFile.GetInstance().WriteKeyFile(entity.mainGameObject.GetInstanceID(), "{0} 生成 {1}", entity.mainGameObject.GetInstanceID(), entity.entityID);

            EntityManager.GetInstance().AddToEntityMovables(entity);
            if (entity.anim == null)
            {
                entity.anim = new SimpleAnimation();
            }

            entity.anim.Init(entity.mainGameObject);
            if (entity.followPath == null)
            {
                //entity.followPath = new FollowPath();
                entity.followPath = new FollowPathRightAngles();
            }

            //从表里取速度
            entity.moveSpeed = Config.globalConfig.getInstace().ZooCarSpeed;
            //从表里取路径
            string pathName = "";
            switch (entityFuncType)
            {
                case EntityFuncType.VisitorCar_EnterZoo:
                    pathName = globalConfig.getInstace().NaturalVisitorInto;
                    break;
                case EntityFuncType.VisitorCar_LeaveZoo:
                    pathName = globalConfig.getInstace().NaturalVisitorOut_2;
                    break;
                default:
                    string e = string.Format("car 没有这种功能类型{0}", entityFuncType);
                    throw new System.Exception(e);
            }

            var path = PathManager.GetInstance().GetPath(pathName);
            entity.position = path[0];
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 移动位置到路径起点{1}, pos {2}", entity.entityID, entity.mainGameObject.GetInstanceID(), path[0]);
            DebugFile.GetInstance().WriteKeyFile(entity.mainGameObject.GetInstanceID(), "{0} 移动位置到路径起点{1}, pos {2}", entity.mainGameObject.GetInstanceID(), entity.entityID, path[0]);

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 沿着路径{1}出发", entity.entityID, pathName);
            //entity.followPath.Init(entity, path.ToArray(), path[0], 0, entity.moveSpeed, false);
            entity.followPath.Init(entity, pathName, path, path[0], 0, entity.moveSpeed);
            entity.Active();

        }

        protected void SpawnInGroundParkingCar(EntityFuncType entityFuncType, ParkingSpace parkingSpace)
        {
            EntityGroundParkingCar entity = null;
            //if (isRandomCar)
            {
                entity = EntityManager.GetInstance().GetRandomEntity(ResType.Car, entityFuncType) as EntityGroundParkingCar;
            }
            //else
            //{
            //    entity = EntityManager.GetInstance().GenEntityGameObject(2, EntityFuncType.VisitorCar_EnterZoo) as EntityGroundParkingCar;
            //}

            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "car_{0}_{1}_{2}", entity.mainGameObject.GetInstanceID(), entityFuncType, entity.entityID);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 生成 {1}", entity.entityID, entity.mainGameObject.GetInstanceID());
            DebugFile.GetInstance().WriteKeyFile(entity.mainGameObject.GetInstanceID(), "{0} 生成 {1}", entity.mainGameObject.GetInstanceID(), entity.entityID);

            EntityManager.GetInstance().AddToEntityMovables(entity);
            if (entity.anim == null)
            {
                entity.anim = new SimpleAnimation();
            }

            entity.anim.Init(entity.mainGameObject);
            if (entity.followPath == null)
            {
                entity.followPath = new FollowPathSuperMarketSimply();
            }

            //从表里取速度
            entity.moveSpeed = Config.globalConfig.getInstace().ZooCarSpeed;

            entity.groundParkingGroupID = parkingSpace.groupID;
            entity.groundParkingIdx = parkingSpace.idx;

            string pathName = "";
            switch (entityFuncType)
            {
                case EntityFuncType.GroundParkingCar:
                    //pathName = globalConfig.getInstace().NaturalVisitorInto;
                    pathName = string.Format("InGroundParking_{0}_{1}", parkingSpace.groupID, parkingSpace.idx);

                    break;
                //case EntityFuncType.VisitorCar_LeaveZoo:
                //    pathName = globalConfig.getInstace().NaturalVisitorOut_2;
                //    break;
                default:
                    string e = string.Format("car 没有这种功能类型{0}", entityFuncType);
                    throw new System.Exception(e);
            }

            //var path = PathManager.GetInstance().GetPath(pathName);
            //var path = CalcPathManager.GenInGroundParkingSpacePath(ps.groupID, ps.idx, 8f, 5f, 7f, -1);
            var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(parkingSpace.groupID, parkingSpace.idx);
            List<Vector3> path = null;
            if (!GroundParingSpacePathManager.IsExist(pathUnit.inPath))
            {
                //path = GroundParingSpacePathManager.GenInPath(parkingSpace.groupID, parkingSpace.idx, 8f, 5f, 7f, -1);
                path = GroundParingSpacePathManager.GenInPath(parkingSpace.groupID, parkingSpace.idx,
                    Config.globalConfig.getInstace().GroundParkingFristSpaceOffset,
                    Config.globalConfig.getInstace().GroundParkingSpace,
                    Config.globalConfig.getInstace().GroundParkingSpacePosOffset,
                    Config.parkingConfig.getInstace().getCell(1).openseatdir[parkingSpace.groupID]);

                GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.InPath, pathUnit, path, null);
            }
            path = pathUnit.inPath;

            entity.position = path[0];
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 移动位置到路径起点{1}, pos {2}", entity.entityID, entity.mainGameObject.GetInstanceID(), path[0]);
            DebugFile.GetInstance().WriteKeyFile(entity.mainGameObject.GetInstanceID(), "{0} 移动位置到路径起点{1}, pos {2}", entity.mainGameObject.GetInstanceID(), entity.entityID, path[0]);

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 沿着路径{1}出发", entity.entityID, pathName);
            //entity.followPath.Init(entity, path.ToArray(), path[0], 0, entity.moveSpeed, false);
            entity.followPath.Init(entity, parkingSpace.groupID, parkingSpace.idx, path, path[0], 0, entity.moveSpeed);
            entity.Active();
            //ps.parkingCar = entity;
            //TracedCamera(entity);
            parkingSpace.SetBeLocked(entity);

            /*若是新手引导阶段,附加跟随对象 不调用步骤*/
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                DelayedOperationNewbieGuideStage(entity);
            }
        }

        /// <summary>
        /// 延时，新手引导阶段的相机跟随赋值
        /// </summary>
        private void DelayedOperationNewbieGuideStage(EntityGroundParkingCar entity)
        {
            GameObject go = new GameObject();
            go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                if (uIGuidePage.procedure < 2&&uIGuidePage.number==0)
                {
                    uIGuidePage.entity = entity;
                }
            }));
        }
    }

    /// <summary>
    /// 地面停车场
    /// </summary>
    public class GroundParking
    {
        public Dictionary<int, GroupParkingSpace> groupParkingSpaceDic = new Dictionary<int, GroupParkingSpace>();

        public void Init(int level)
        {
            int numSpace = ParkingCenter.GetGroundParkingNumber(level);
            int numGroup = numSpace / globalConfig.getInstace().NumGroundParkingGroupSpace;
            for (int i = 0; i < numGroup; i++)
            {
                var group = new GroupParkingSpace(i, globalConfig.getInstace().NumGroundParkingGroupSpace);
                groupParkingSpaceDic.Add(i, group);
            }
        }

        /// <summary>
        /// 获得一个空位置车位
        /// </summary>
        /// <returns></returns>
        public ParkingSpace GetFreeParkingSpace()
        {
            ParkingSpace parkingSpace = null;

            foreach (var val in groupParkingSpaceDic.Values)
            {
                parkingSpace = val.GetFreeParingSpace();
                if (parkingSpace != null)
                {
                    return parkingSpace;
                }
            }

            return parkingSpace;
        }

        /// <summary>
        /// 得到停好车的车位
        /// </summary>
        /// <returns></returns>
        public ParkingSpace GetParkingSpace()
        {
            ParkingSpace parkingSpace = null;

            foreach (var val in groupParkingSpaceDic.Values)
            {
                parkingSpace = val.GetParkingSpace();
                if (parkingSpace != null)
                {
                    return parkingSpace;
                }
            }

            return parkingSpace;
        }

        /// <summary>
        /// 得到指定停车位
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public ParkingSpace GetSpace(int groupID, int idx)
        {
            ParkingSpace parkingSpace = null;
            GroupParkingSpace groupParkingSpace = null;
            if (groupParkingSpaceDic.TryGetValue(groupID, out groupParkingSpace))
            {
                parkingSpace = groupParkingSpace.GetSpace(idx);
            }

            return parkingSpace;
        }
    }
}
