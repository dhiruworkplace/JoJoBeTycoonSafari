/*******************************************************************
* FileName:     StateVisitorLeaveNonLittleZoo.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-21
* Description:  
* other:    
********************************************************************/


using Game.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using Game.MessageCenter;
using System;

namespace Game
{
    /// <summary>
    /// 没有动物栏可选的情况下准备离开了
    /// 
    /// </summary>
    public class StateVisitorLeaveNonLittleZoo_old : FSMState
    {
        /// <summary>
        /// 到达1000的路名
        /// </summary>
        public string pathOfTo1000 = "";

        public string pathOfLeaveZoo = "";
        
        /// <summary>
        /// 到达去1000的起点
        /// </summary>
        public bool isArrivedStartOfPathTo1000 = false;

        /// <summary>
        /// 到达去1000路的终点(到达入口)
        /// </summary>
        public bool isArrivedEndOfPathTo1000 = false;

        public StateVisitorLeaveNonLittleZoo_old(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorLeaveNonLittleZoo.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isArrivedStartOfPathTo1000 = false;
            isArrivedEndOfPathTo1000 = false;
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, OnArrived);

            pathOfLeaveZoo = Config.globalConfig.getInstace().NaturalVisitorOut_1;
            //选当前所在位置到1000的路，走过去。
            if (entity.stayBuildingID == 1000)
            {
                LogWarp.LogFormat("{0} 入口即返回!", entity.entityID);
                isArrivedStartOfPathTo1000 = true;

                //EntityVisitor.GodownPath(entity, pathOfLeaveZoo);
                EntityVisitor.GodownReversePath(entity, pathOfLeaveZoo);
                return;
            }

            pathOfTo1000 = EntityVisitor.GetPath(entity.stayBuildingID, 1000);
            if (string.IsNullOrEmpty(pathOfTo1000))
            {
                string e = string.Format("StateVisitorLeaveNonLittleZoo 没找到{0}->{1}的路!!!!", entity.stayBuildingID, 1000);
                throw new Exception(e);
            }
            EntityVisitor.GotoStartOfPath(entity, pathOfTo1000);
        }
        

        public override void Tick(int deltaTimeMS)
        {

        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

            base.Leave();
        }

        public override void AddAllConvertCond()
        {

        }

        protected void OnArrived(Message msg)
        {
            var _msg = (MessageArrived)msg;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //自己的entity
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            //到达离开的起点,向1000走
            if (_msg.followPath.isArrivedEnd && !isArrivedStartOfPathTo1000)
            {
                isArrivedStartOfPathTo1000 = true;
                //BeginLeave(entity, pathOfTo1000);
                EntityVisitor.GodownPath(entity, pathOfTo1000);
                return;
            }

            //向门口走
            if (_msg.followPath.isArrivedEnd && !isArrivedEndOfPathTo1000)
            {
                isArrivedEndOfPathTo1000 = true;
                //EntityVisitor.GodownPath(entity, pathOfLeaveZoo);
                EntityVisitor.GodownReversePath(entity, pathOfLeaveZoo);
                return;
            }

            if (_msg.followPath.isArrivedEnd)
            {
                LogWarp.Log("结束离开");
                EntityManager.GetInstance().RemoveFromEntityMovables(entity);
                //LogWarp.LogError("游客回POOL");
                //EntityVisitor.pool.Delete(entity);
                
                //通知生成离开的汽车
                MessageManager.GetInstance().Send((int)GameMessageDefine.SpawnVisitorCarLeaveZoo);
            }
        }
    }

}
