/*******************************************************************
* FileName:     EntityVisitorCar.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-7
* Description:  
* other:    
********************************************************************/


using UFrame;
using UFrame.BehaviourFloat;
using UFrame.EntityFloat;

namespace Game
{
    public class EntityVisitorCar : EntityMovable
    {
        //public FollowPath followPath;

        public FollowPathRightAngles  followPath;

        public SimpleAnimation anim;

        public static ObjectPool<EntityVisitorCar> pool = new ObjectPool<EntityVisitorCar>();

        public override void Active()
        {
            base.Active();

            this.followPath.Run();
        }

        public override void Deactive()
        {
            DebugFile.GetInstance().WriteKeyFile(entityID, "{0} Deactive {1} , pos {2}", entityID, mainGameObject.GetInstanceID(), Const.Invisible_Postion);
            DebugFile.GetInstance().WriteKeyFile(mainGameObject.GetInstanceID(), "{0} Deactive {1} pos {2}", mainGameObject.GetInstanceID(), entityID, Const.Invisible_Postion);
            //移动到看不见的地方
            this.position = Const.Invisible_Postion;
            base.Deactive();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldActive())
            {
                return;
            }

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
            this.Deactive();
            base.OnRecovery();
            followPath.Release();
            followPath = null;
            anim.Release();
            anim = null;
        }

        protected void TickAnim()
        {
            if (followPath.IsRunning() != anim.GetAutoPlay())
            {
                anim.SetAutoPlay(followPath.IsRunning());
            }
        }
    }

}
