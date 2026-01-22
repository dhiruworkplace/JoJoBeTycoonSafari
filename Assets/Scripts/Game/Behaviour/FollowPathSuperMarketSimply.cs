using UnityEngine;
using UFrame.EntityFloat;
using Logger;
using System.Collections.Generic;
using Game;
using UFrame;
using UFrame.BehaviourFloat;
using Game.MessageCenter;
using UFrame.MessageCenter;

namespace Game
{
    public class FollowPathSuperMarketSimply : FollowPathRightAngles
    {
        public FSMFollowPathSuperMarketSimply fsm;

        //public override void Init(EntityMovable ownerEntity, List<Vector3> pathPosList, Vector3 orgPos, int nextPosIdx, float speed)
        //{
        //    base.Init(ownerEntity, pathPosList, orgPos, nextPosIdx, speed);
        //    AddState();
        //}

        public override void Init(EntityMovable ownerEntity, int groupID, int idx, List<Vector3> pathPosList, Vector3 orgPos, int nextPosIdx, float speed)
        {
            base.Init(ownerEntity, groupID, idx, pathPosList, orgPos, nextPosIdx, speed);
            AddState();
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLetGroundParingCarLeave, OnBroadcastLetGroundParingCarLeave);
        }

        public override void Release()
        {
            fsm.Release();
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLetGroundParingCarLeave, OnBroadcastLetGroundParingCarLeave);
            base.Release();
        }

        public override void Tick(int deltaTimeMS)
        {
            base.Tick(deltaTimeMS);
            fsm.Tick(deltaTimeMS);
        }

        public override void Run()
        {
            base.Run();
            fsm.Run();
        }

        public override void Stop()
        {
            base.Stop();
            fsm.Stop();
        }

        public override void WhenArrivedEndPos()
        {
            var state = fsm.GetCurrentState() as StateFollowPathSuperMarketSimply;
            state.WhenArrivedEnd();
        }

        protected void AddState()
        {
            if (fsm == null)
            {
                fsm = new FSMFollowPathSuperMarketSimply(this);
                fsm.AddState(new StateFollowPathSuperMarketSimply_InParkingSpace(
                    (int)(FollowPathSuperMarketSimplyState.InParkingSpace), fsm));
                fsm.AddState(new StateFollowPathSuperMarketSimply_StayParking(
                    (int)(FollowPathSuperMarketSimplyState.StayParking), fsm));
                fsm.AddState(new StateFollowPathSuperMarketSimply_OutParkingSpace(
                    (int)(FollowPathSuperMarketSimplyState.OutParkingSpace), fsm));

                fsm.SetDefaultState((int)(FollowPathSuperMarketSimplyState.InParkingSpace));

                return;
            }

            fsm.GotoState((int)(FollowPathSuperMarketSimplyState.InParkingSpace));
        }

        protected void OnBroadcastLetGroundParingCarLeave(Message msg)
        {
            var _msg = msg as MessageGroundParkingSpace;

            //不是发给我的
            if (this.groupID != _msg.groupID || this.idx != _msg.idx)
            {
                return;
            }

            this.fsm.GotoState((int)FollowPathSuperMarketSimplyState.OutParkingSpace);
        }
    }
}

