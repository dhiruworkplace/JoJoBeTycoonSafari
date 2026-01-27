/*******************************************************************
* FileName:     StateVisitorEntryQueue.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-12
* Description:  
* other:    
********************************************************************/


using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using Logger;
using ZooGame.MessageCenter;
using ZooGame.GlobalData;
using ZooGame.Path.StraightLine;

namespace ZooGame
{
    /// <summary>
    /// 负责游客在入口排队的过程,排队排到第一位时,转入下一个状态。
    /// 能走到这个状态的游客都能进入动物园。
    /// 进入这个状态时,该游客已经在入口某个排队中占位了。这时需要再次请求,获得当前
    /// 的排队位置,把占位转真正的位置。把申请队伍分成请求占位和请求具体位,两个步骤
    /// 是为了避免:先申请,但后走到这种情况出现空位。
    /// </summary>
    public class StateVisitorEntryQueue : FSMState
    {
        bool isToStayFirstPosInEntryQueue = false;

        /// <summary>
        /// 是否走到初始排队位
        /// </summary>
        bool isArrivedOrgPosOfQueue = false;

        /// <summary>
        /// 从初始排队位到排第一个需要的步数
        /// </summary>
        int stepOfToFirstPos = 0;

        public StateVisitorEntryQueue(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} VisitorStateEntryQueue.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isArrivedOrgPosOfQueue = false;

            //entity.numOfZooEntryQueueForwardOne = 0;
            entity.indexInZooEntryQueue = Const.Invalid_Int;
            isToStayFirstPosInEntryQueue = false;
            stepOfToFirstPos = 0;
            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToEntryQueueReply, this.OnAddVisitorToEntryQueueReply);
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastForwardOneStepInEntryGateQueue, OnBroadcastForwardOneStepInExitGateQueue);
            
            //申请排队位置
            AddVisitorToEntryQueueApply.Send(entity.entityID, entity.zooEntryID);
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToEntryQueueReply, this.OnAddVisitorToEntryQueueReply);
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastForwardOneStepInEntryGateQueue, OnBroadcastForwardOneStepInExitGateQueue);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            this.AddConvertCond((int)VisitorState.StayFirstPosInEntryQueue, ToStayFirstPosInEntryQueue);
        }

        protected bool ToStayFirstPosInEntryQueue()
        {
            return this.isToStayFirstPosInEntryQueue;
        }

        public override void Tick(int deltaTimeMS)
        {
            if (this.CouldRun())
            {
                return;
            }

            TickStepByStep();
        }

        /// <summary>
        /// 先走到最初的排队位置
        /// </summary>
        protected void GoToOrgPosOfQueue()
        {
            //生成路径
            //起点自己的位置，前面cell.maxnumofperqueue个单位为第一个位置
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if(entity.indexInZooEntryQueue < 0)
            {
                string e = string.Format("{0} 初始排队位异常{1}", entity.entityID, entity.indexInZooEntryQueue);
                throw new System.Exception(e);
            }

            var cell = Config.ticketConfig.getInstace().getCell(entity.zooEntryID);
            //float queueFirstPosOffset = 10f;
            float queueFirstPosOffset = Config.globalConfig.getInstace().EntryQueueFirstPosOffset;
            float distance = cell.maxnumofperqueue - entity.indexInZooEntryQueue + queueFirstPosOffset;

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 走向入口初始排队位{1}", entity.entityID, entity.indexInZooEntryQueue);
            MoveForward(entity, distance);
        }

        protected void OnArrived(Message msg)
        {
            var _msg = msg as MessageArrived;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            //自己的entity
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorIdle);
                entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorIdle);
            }

            if (_msg.followPath.isArrivedEnd && !isArrivedOrgPosOfQueue)
            {
                this.isArrivedOrgPosOfQueue = true;
                
                //entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorIdle);
            }

            //已经走到队伍第一位了
            if (_msg.followPath.isArrivedEnd && isArrivedOrgPosOfQueue && stepOfToFirstPos == 0)
            {
#if UNITY_EDITOR
                if (stepOfToFirstPos == 0 && entity.numOfZooEntryQueueForwardOne != 0)
                {
                    entity.numOfZooEntryQueueForwardOne = 0;
                    DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 步进错误 stepOfToFirstPos={1},numOfZooEntryQueueForwardOne ={2}",
                        entity.entityID, stepOfToFirstPos, entity.numOfZooEntryQueueForwardOne);
                }
#endif
                this.isToStayFirstPosInEntryQueue = true;
            }


        }

        protected void OnAddVisitorToEntryQueueReply(Message msg)
        {
            var _msg = msg as AddVisitorToEntryQueueReply;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //自己的entity
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            entity.indexInZooEntryQueue = _msg.indexInQueue;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 初始排队位是{1}", entity.entityID, entity.indexInZooEntryQueue);
            stepOfToFirstPos = entity.indexInZooEntryQueue;
            GoToOrgPosOfQueue();
        }

        protected void OnBroadcastForwardOneStepInExitGateQueue(Message msg)
        {
            var _msg = msg as BroadcastForwardOneStepInQueue;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            if (entity.zooEntryID == _msg.entryID && entity.entityID != _msg.entityID)
            {
                entity.numOfZooEntryQueueForwardOne++;
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 收到{1}的步进", entity.entityID, _msg.entityID);
            }
        }

        protected void MoveForward(EntityVisitor entity, float distance)
        {
            var startPos = entity.position;
            var endPos = startPos;
            endPos += distance * GlobalDataManager.GetInstance().SceneForward;
            entity.pathList.Clear();
            entity.pathList.Add(startPos);
            entity.pathList.Add(endPos);
            float queueMoveSpeed = Config.globalConfig.getInstace().ZooVisitorQueueSpeed;
            entity.followPath.Init(entity, entity.pathList, startPos, 0, queueMoveSpeed, false);
            entity.followPath.Run();
        }

        protected void GotoFirstPathOfZoo(EntityVisitor entity)
        {
            var startPos = entity.position;
            string pathName = Config.globalConfig.getInstace().FirstPathOfZoo;
            EntityVisitor.GotoStartOfPath(entity, pathName);
        }

        protected void TickStepByStep()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //在走 或者 还没到初始排队位 不执行
            if (entity.followPath.IsRunning() || !isArrivedOrgPosOfQueue)
            {
                return;
            }

            if (entity.numOfZooEntryQueueForwardOne > 0)
            {
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
                entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
                MoveForward(entity, 1);
                --stepOfToFirstPos;
                --entity.numOfZooEntryQueueForwardOne;
            }
        }
    }

}
