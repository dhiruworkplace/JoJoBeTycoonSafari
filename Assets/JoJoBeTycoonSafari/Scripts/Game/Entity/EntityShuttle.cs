/*******************************************************************
* FileName:     EntityShuttle.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-27
* Description:  
* other:    
********************************************************************/


using UFrame;
using UFrame.BehaviourFloat;
using UFrame.EntityFloat;
using Logger;
using System.Collections.Generic;
using UnityEngine;

namespace ZooGame
{
    public partial class EntityShuttle : EntityMovable
    {
        public static ObjectPool<EntityShuttle> pool = new ObjectPool<EntityShuttle>();

        public FollowPath followPath;
        public FSMMachine fsmMachine;
        public SimpleAnimation anim;

        /// <summary>
        /// 摆渡车上载客人数
        /// </summary>
        public List<ShuttleVisitor> shuttleVisitorList = new List<ShuttleVisitor>();
        
        /// <summary>
        /// 路径，好几个状态需要用，提到entity上，避免浪费
        /// </summary>
        public List<Vector3> pathList = new List<Vector3>();

        public override void Active()
        {
            base.Active();

            this.fsmMachine.Run();

            Reset();
        }

        public override void Deactive()
        {
            //移动到看不见的地方
            this.position = UFrame.Const.Invisible_Postion;

            this.followPath.Stop();
            this.fsmMachine.Stop();

            this.entityID = Const.Invalid_Int;
            Reset();

            base.Deactive();
            //LogWarp.LogError("摆渡车Deactive");
        }

        public void Reset()
        {
            pathList.Clear();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldActive())
            {
                return;
            }

            this.fsmMachine.Tick(deltaTimeMS);
            this.followPath.Tick(deltaTimeMS);
            TickAnim();
        }

        public override void OnDeathToPool()
        {
            this.Deactive();
            base.OnDeathToPool();
        }

        public override void OnRecovery()
        {
            base.OnRecovery();
            this.Deactive();
            this.followPath.Release();
            followPath = null;

            fsmMachine.Release();
            this.fsmMachine = null;

            shuttleVisitorList.Clear();
        }

        protected void TickAnim()
        {
            if (followPath.IsRunning() != anim.GetAutoPlay())
            {
                anim.SetAutoPlay(followPath.IsRunning());
            }
        }

        public void SetVisistorList(List<ShuttleVisitor> lst)
        {
            this.shuttleVisitorList.Clear();
            this.shuttleVisitorList.AddRange(lst);
        }
    }

}
