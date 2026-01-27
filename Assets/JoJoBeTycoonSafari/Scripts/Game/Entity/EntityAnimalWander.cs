/*******************************************************************
* FileName:     EntityAnimalWander.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-4
* Description:  
* other:    
********************************************************************/


using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using ZooGame.MiniGame;
using Logger;
using UFrame;
using UFrame.BehaviourFloat;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace ZooGame
{
    /// <summary>
    /// 散步的动物(动物园内动物栏的动物)
    /// </summary>
    public class EntityAnimalWander : EntityMovable
    {
        public static ObjectPool<EntityAnimalWander> pool = new ObjectPool<EntityAnimalWander>();

        //public Wander wander;

        public PathWander pathWander;

        public SimpleAnimation anim;

        /// <summary>
        /// 是否播放play动画
        /// </summary>
        bool isPlayWalk = true;

        /// <summary>
        /// 是否停止Wander
        /// </summary>
        bool isStopWander = false;

        /// <summary>
        /// 停止Wander的时间
        /// </summary>
        int stopWanderMS = 0;

        /// <summary>
        /// 停止Wander的累计时间，用完清零
        /// </summary>
        int stopWanderAm = 0;

        /// <summary>
        /// 动物ID
        /// </summary>
        public int animalID = Const.Invalid_Int;

        SimpleParticle sp = null;

        public override void Active()
        {
            base.Active();

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.WanderArrived, OnWanderArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.AnimalPlayLevelUpEffect, OnAnimalPlayLevelUpEffect);

            this.pathWander.Run();
            //this.wander.Run();
            anim.Play(GlobalDataManager.GetInstance().animalAnimation.walkName);
        }

        public override void Deactive()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.WanderArrived, OnWanderArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AnimalPlayLevelUpEffect, OnAnimalPlayLevelUpEffect);

            //移动到看不见的地方
            this.position = Const.Invisible_Postion;
            this.pathWander.Stop();
            //this.wander.Stop();
            anim.Stop();

            base.Deactive();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldActive())
            {
                return;
            }
            this.pathWander.Tick(deltaTimeMS);
            //this.wander.Tick(deltaTimeMS);

            if (!anim.IsRunning() && isPlayWalk)
            {
                anim.Play(GlobalDataManager.GetInstance().animalAnimation.walkName);
            }

            if(isStopWander)
            {
                stopWanderAm += deltaTimeMS;
                if (stopWanderAm >= stopWanderMS)
                {
                    isStopWander = false;
                    isPlayWalk = true;
                    anim.Stop();
                    pathWander.Run();
                    //wander.Run();
                    stopWanderAm = 0;
                }
            }

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

            pathWander.Release();
            pathWander = null;
            //wander.Release();
            //wander = null;

            anim.Release();
            anim = null;
        }

        protected void Reset()
        {
            isPlayWalk = true;
            isStopWander = false;
            stopWanderMS = 0;
            stopWanderAm = 0;
        }
        
        protected void OnWanderArrived(Message msg)
        {
            var _msg = msg as WanderArrived;

            if (_msg.entityID != this.entityID)
            {
                return;
            }

            //LogWarp.Log(UnityEngine.Time.realtimeSinceStartup + " OnWanderArrived ");
            pathWander.Stop();
            //wander.Stop();
            anim.Stop();
            //停止， 开始随机获取除了walking之外的两个动画之一，播放，
            string randomName = GlobalDataManager.GetInstance().animalAnimation.GetRandomName();
            anim.Play(randomName);

            isPlayWalk = false;
            //计时准备
            isStopWander = true;
            stopWanderAm = 0;
            if (anim == null|| string.IsNullOrEmpty(randomName))
            {
#if UNITY_EDITOR
                //LogWarp.LogErrorFormat("注意：动画名字为空： name= "+ anim.lastAnimName+ "   动物本身为空或未随机到动物动画名字  randomName");
                string e = string.Format("注意：动画名字为空： name= " + anim.lastAnimName + "   动物本身为空或未随机到动物动画名字  randomName");
                throw new System.Exception(e);
#endif
            }
            stopWanderMS = Math_F.FloatToInt1000(anim.GetClipLength(randomName));
        }

        protected void OnAnimalPlayLevelUpEffect(Message msg)
        {
            var _msg = msg as MessageInt;

            if (_msg.val != entityID)
            {
                return;
            }

            var cellAnimalUp = Config.animalupConfig.getInstace().getCell(this.animalID);
            var pool = PoolManager.GetInstance().GetGameObjectPool(cellAnimalUp.levelupeffect);
            var effGo = pool.New();
            effGo.transform.position = Vector3.zero;
            effGo.transform.localScale = Vector3.one;
            effGo.transform.SetParent(this.cacheTrans, false);
            if (sp != null)
            {
                sp.UnInit();
            }
            else
            {
                sp = new SimpleParticle();
            }
            sp.Init(effGo);
            sp.Play();
            effGo.transform.parent = null;
            pool.Delete(effGo);
        }
    }
}
