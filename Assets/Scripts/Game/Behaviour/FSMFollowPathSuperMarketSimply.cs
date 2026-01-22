using Game.MessageCenter;
using Game.Path.StraightLine;
using Logger;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.BehaviourFloat;
using UnityEngine;

namespace Game
{
    public enum FollowPathSuperMarketSimplyState
    {
        /// <summary>
        /// 入库
        /// </summary>
        InParkingSpace,

        /// <summary>
        /// 在停车场不动
        /// </summary>
        StayParking,

        /// <summary>
        /// 出库
        /// </summary>
        OutParkingSpace,
    }

    public class FSMFollowPathSuperMarketSimply : FSMMachine
    {
        public FollowPathSuperMarketSimply owner;

        public FSMFollowPathSuperMarketSimply(FollowPathSuperMarketSimply owner)
        {
            this.owner = owner;
        }

        public override void Release()
        {
            owner = null;
            base.Release();
        }
    }

    public class StateFollowPathSuperMarketSimply : FSMState
    {
        public StateFollowPathSuperMarketSimply(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var follow = (this.fsmCtr as FSMFollowPathSuperMarketSimply).owner;
            var entity = follow.ownerEntity as EntityGroundParkingCar;
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "GroundCar_{0}_{1}_{2}",
                entity.entityID, (FollowPathSuperMarketSimplyState)preStateName,
                (FollowPathSuperMarketSimplyState)this.stateName);

        }

        public override void AddAllConvertCond()
        {
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        public virtual void WhenArrivedEnd() { }
    }

    public class StateFollowPathSuperMarketSimply_InParkingSpace : StateFollowPathSuperMarketSimply
    {
        bool isToStateStayParking = false;
        public StateFollowPathSuperMarketSimply_InParkingSpace(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);
            var follow = (this.fsmCtr as FSMFollowPathSuperMarketSimply).owner;
            var entity = follow.ownerEntity as EntityGroundParkingCar;
            entity.anim.SetAutoPlay(true);

            isToStateStayParking = false;
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        public override void AddAllConvertCond()
        {
            this.AddConvertCond((int)FollowPathSuperMarketSimplyState.StayParking, ToStateStayParking);
        }

        public override void WhenArrivedEnd()
        {
            //转到StayParking
            isToStateStayParking = true;
        }

        protected bool ToStateStayParking()
        {
            return isToStateStayParking;
        }
    }

    public class StateFollowPathSuperMarketSimply_StayParking : StateFollowPathSuperMarketSimply
    {
        public StateFollowPathSuperMarketSimply_StayParking(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);
            var follow = (this.fsmCtr as FSMFollowPathSuperMarketSimply).owner;
            var entity = follow.ownerEntity as EntityGroundParkingCar;
            entity.anim.Stop();


            //LogWarp.LogErrorFormat("{0} car StateFollowPathSuperMarketSimply_StayParking {1}, {2}", entity.entityID,
            //    entity.groundParkingGroupID, entity.groundParkingIdx);
            MessageGroundParkingSpace.Send((int)GameMessageDefine.ParkingCarInGroundParking, entity.groundParkingGroupID, entity.groundParkingIdx);
            SpawnVisitorFromGroundParking.Send(VisitorStage.GotoParking, EntityFuncType.Visitor_From_GroundParking, 
                entity.groundParkingGroupID, entity.groundParkingIdx);

            /*  新手引导阶段  进行步骤4  添加跟随对象   */
            if (GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                if (uIGuidePage.procedure == 3)
                {
                    PageMgr.ShowPage<UIGuidePage>();  //开启新手引导UI
                    uIGuidePage.entity = entity;
                }
            }
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        public override void AddAllConvertCond()
        {
            
        }
    }

    public class StateFollowPathSuperMarketSimply_OutParkingSpace : StateFollowPathSuperMarketSimply
    {
        /// <summary>
        /// 是否完成倒车
        /// </summary>
        bool isFinishedBack = false;
        public StateFollowPathSuperMarketSimply_OutParkingSpace(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);
            var follow = (this.fsmCtr as FSMFollowPathSuperMarketSimply).owner;
            var entity = follow.ownerEntity as EntityGroundParkingCar;
            entity.anim.SetAutoPlay(true);

            isFinishedBack = false;
            //构建倒车转弯控制点
            this.ResetRacToReverse(follow);
            //构建倒车路径
            this.ResetPathToReverse(follow);

            follow.idxCtr = 0;
            //设置方向为反
            follow.isForward = false;

            //选离开的路
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        public override void AddAllConvertCond()
        {

        }

        public override void WhenArrivedEnd()
        {
            var follow = (this.fsmCtr as FSMFollowPathSuperMarketSimply).owner;
            if (!isFinishedBack)
            {
                isFinishedBack = true;
                LogWarp.LogError("完成倒车，准备离开");

                //生成离开的路

                var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(follow.groupID, follow.idx);
                List<Vector3> outPath = null;
                List<RightAnglesControllNode> outRac = null;
                if (!GroundParingSpacePathManager.IsExist(pathUnit.outPath))
                {
                    outPath = GroundParingSpacePathManager.GenOutPath(follow.groupID, follow.idx);
                    GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.OutPath, pathUnit, outPath, null);

                    outRac = GroundParingSpacePathManager.GenRAC(outPath, follow.turnOffset);
                    GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.OutPathRac, pathUnit, null, outRac);

                }
                follow.pathPosList = pathUnit.outPath;
                follow.ctrList = pathUnit.outPathRac;

                follow.idxCtr = 0;
                //设置方向为反
                follow.isForward = true;

                MessageGroundParkingSpace.Send((int)GameMessageDefine.LetGroundParingCarLeave, follow.groupID, follow.idx);

                return;
            }

            // 回POOL
            EntityManager.GetInstance().RemoveFromEntityMovables(follow.ownerEntity);
        }

        /// <summary>
        /// 构建倒车路径
        /// </summary>
        /// <param name="follow"></param>
        protected void ResetPathToReverse(FollowPathSuperMarketSimply follow)
        {
            var entity = follow.ownerEntity as EntityGroundParkingCar;
            var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(entity.groundParkingGroupID, entity.groundParkingIdx);
            List<Vector3> reversePath = null;
            if (!GroundParingSpacePathManager.IsExist(pathUnit.inPathReverse))
            {
                reversePath = GroundParingSpacePathManager.GenReversePath(follow.pathPosList);
                GroundParingSpacePathManager.GetInstance().AddPath(
                    GroundParingSpacePathType.InPathReverse, pathUnit, reversePath, null);
            }
            follow.pathPosList = pathUnit.inPathReverse;
        }

        /// <summary>
        /// 构建倒车转弯控制点
        /// </summary>
        /// <param name="follow"></param>
        protected void ResetRacToReverse(FollowPathSuperMarketSimply follow)
        {
            var entity = follow.ownerEntity as EntityGroundParkingCar;
            var pathUnit = GroundParingSpacePathManager.GetInstance().GetPathUnit(entity.groundParkingGroupID, entity.groundParkingIdx);
            List<RightAnglesControllNode> backRac = null;
            if (!GroundParingSpacePathManager.IsExist(pathUnit.inPathBackRac))
            {
                backRac = GroundParingSpacePathManager.GenBackRac(follow.ctrList);
                GroundParingSpacePathManager.GetInstance().AddPath(GroundParingSpacePathType.InPathBackRac, pathUnit, null, backRac);
            }
            follow.ctrList = pathUnit.inPathBackRac;
        }
    }
}
