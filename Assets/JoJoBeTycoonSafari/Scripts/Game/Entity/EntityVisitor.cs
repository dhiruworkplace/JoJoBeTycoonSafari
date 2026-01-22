/*******************************************************************
* FileName:     EntityVisitor.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-7
* Description:  
* other:    
********************************************************************/


using UFrame;
using UFrame.BehaviourFloat;
using UFrame.EntityFloat;
using Logger;
using System.Collections.Generic;
using UnityEngine;
using Game.GlobalData;

namespace Game
{
    public partial class EntityVisitor : EntityMovable
    {
        public static ObjectPool<EntityVisitor> pool = new ObjectPool<EntityVisitor>();

        public FollowPath followPath;
        public FSMMachine fsmMachine;
        public SimpleAnimation anim;
        /// <summary>
        /// 在门口排队的队伍中的位置
        /// </summary>
        public int indexInZooEntryQueue;

        /// <summary>
        /// 处于的入口编号
        /// </summary>
        public int zooEntryID;

        /// <summary>
        /// 是否获准进入动物园
        /// </summary>
        public bool isApproveEnterZoo;

        /// <summary>
        /// 门口排队需要前进的次数
        /// </summary>
        public int numOfZooEntryQueueForwardOne;

        /// <summary>
        /// 处在的建筑ID
        /// </summary>
        public int stayBuildingID;

        /// <summary>
        /// 将要去的组
        /// </summary>
        public int stayGroupID;

        /// <summary>
        /// 在等待位队伍中的位置
        /// </summary>
        public int indexInWaitQueue;

        /// <summary>
        /// 等待位所在的动物栏
        /// </summary>
        //public int littleZooIDWaitQueue;
        /// <summary>
        /// 是否允许进入观光位
        /// </summary>
        public bool isApproveVisitSeat;

        /// <summary>
        /// 在观光位队伍中的位置
        /// </summary>
        public int indexInVisitQueue;

        /// <summary>
        /// 观光位所在的动物栏
        /// </summary>
        //public int littleZooIDVisitQueue;

        /// <summary>
        /// 游览过的组 key = groupID , List里放的是游览过的动物栏ID
        /// </summary>
        public Dictionary<int, List<int>> visitedGroupMap = new Dictionary<int, List<int>>();

        /// <summary>
        /// 观光位CD，这个属性在游客上，cd值是所在动物栏决定的
        /// </summary>
        public IntCD visitCD;

        /// <summary>
        /// 路径，好几个状态需要用，提到entity上，避免浪费
        /// </summary>
        public List<Vector3> pathList = new List<Vector3>();

        /// <summary>
        /// 生成跨组寻路的动物栏
        /// </summary>
        public List<int> crossLittleZooIDs = new List<int>();

        /// <summary>
        /// 出口入口编号
        /// </summary>
        public int ExitGateEntryID;

        /// <summary>
        /// 出口排队队伍中的位置
        /// </summary>
        public int indexInExitGateEntryQueue;

        /// <summary>
        /// 出口排队需要向前移动1个单位的次数
        /// </summary>
        public int numOfExitGateQueueForwardOne;

        /// <summary>
        /// 地面停车场车位组ID
        /// </summary>
        public int groundParkingGroupID;

        /// <summary>
        /// 地面停车场车位索引
        /// </summary>
        public int groundParkingIdx;

        public List<int> animWeight = new List<int>();


        public override void Active()
        {
            base.Active();

            this.fsmMachine.Run();

            Reset();
        }

        public override void Deactive()
        {
            //移动到看不见的地方
            //this.position = UFrame.Const.Invisible_Postion;
            Hide();

            this.followPath.Stop();
            this.fsmMachine.Stop();

            //this.indexInZooEntryQueue = Const.Invalid_Int;
            this.entityID = Const.Invalid_Int;
            Reset();
            base.Deactive();
            //LogWarp.LogError("游客Deactive");
        }

        public void Reset()
        {
            this.indexInZooEntryQueue = Const.Invalid_Int;
            this.zooEntryID = Const.Invalid_Int;
            this.isApproveEnterZoo = false; //是否获准进入
            this.numOfZooEntryQueueForwardOne = 0;//门口排队需要前进的次数
            

            this.stayBuildingID = Const.Invalid_Int; //处在的建筑ID
            this.stayGroupID = Const.Invalid_Int;
            indexInWaitQueue = Const.Invalid_Int;
            //littleZooIDWaitQueue = Const.Invalid_Int;

            this.isApproveVisitSeat = false;
            indexInVisitQueue = Const.Invalid_Int;
            //littleZooIDVisitQueue = Const.Invalid_Int;

            ExitGateEntryID = Const.Invalid_Int;

            indexInExitGateEntryQueue = Const.Invalid_Int;
            numOfExitGateQueueForwardOne = 0;

            //pathList.Clear();
            crossLittleZooIDs.Clear();

            foreach (var v in visitedGroupMap.Values)
            {
                v.Clear();
            }
            visitedGroupMap.Clear();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldActive())
            {
                return;
            }

            this.fsmMachine.Tick(deltaTimeMS);
            this.followPath.Tick(deltaTimeMS);
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

            animWeight.Clear();

            anim.Release();
            anim = null;

            groundParkingGroupID = Const.Invalid_Int;
            groundParkingIdx = Const.Invalid_Int;
        }

        public void Hide()
        {
            this.position = UFrame.Const.Invisible_Postion;
        }

        protected void TickAnim()
        {
            if (followPath.IsRunning())
            {
                if (anim.lastAnimName != Config.globalConfig.getInstace().VisitorWalk)
                {
                    anim.Play(Config.globalConfig.getInstace().VisitorWalk);
                }

                return;
            }

            if (anim.lastAnimName != Config.globalConfig.getInstace().VisitorIdle)
            {
                anim.Play(Config.globalConfig.getInstace().VisitorIdle);
            }
        }

        public void InitAnim()
        {
            anim.Init(mainGameObject);
            animWeight.Clear();
            //for (int i = 0; i < Config.npcactionConfig.getInstace().RowNum; i++)
            //{
            //    var cell = Config.npcactionConfig.getInstace().getCell(i);
            //    if (anim.animation[cell.actionname] != null)
            //    {
            //        animWeight.Add(cell.weight);
            //    }
            //    else
            //    {
            //        animWeight.Add(0);
            //    }
            //}
            foreach (var kv in Config.npcactionConfig.getInstace().AllData)
            {
                if (anim.animation[kv.Value.actionname] != null)
                {
                    animWeight.Add(kv.Value.weight);
                }
                else
                {
                    animWeight.Add(0);
                }
            }
        }

        public Config.npcactionCell PlayActionAnim()
        {
            int idx = Const.Invalid_Int;
            Math_F.TableProbability(animWeight, ref idx);
#if UNITY_EDITOR
            if (idx == Const.Invalid_Int)
            {
                string e = string.Format("随机动画异常{0}", mainGameObject.name);
                throw new System.Exception(e);
            }
#endif
            //var cell = Config.npcactionConfig.getInstace().getCell(idx);
            var cell = GlobalDataManager.GetInstance().logicTableVisitorAction.actionList[idx];
#if UNITY_EDITOR
            if (cell == null)
            {
                string e = string.Format("随机动画异常{0}, {1}", mainGameObject.name, idx);
                throw new System.Exception(e);
            }
#endif
            anim.Play(cell.actionname);
            DebugFile.GetInstance().WriteKeyFile(entityID, "{0} PlayAnim {1}", entityID, cell.actionname);

            return cell;
        }

        public void PlayActionAnim(string animName)
        {
            anim.Play(animName);
        }
    }

}
