/*******************************************************************
* FileName:     StateVisitorEnterZoo.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-20
* Description:  
* other:    
********************************************************************/


using UFrame;
using UFrame.MessageCenter;
using Logger;
using ZooGame.MessageCenter;
using ZooGame.GlobalData;

namespace ZooGame
{
    /// <summary>
    /// 负责刷出游客,进入动物园
    /// </summary>
    public class StateVisitorEnterZoo : FSMState
    {
        bool isToVisitorStateChoseLittleZoo = false;

        bool isStartGotoEntry = false;
        public StateVisitorEnterZoo(int stateName, FSMMachine fsmCtr):
            base(stateName, fsmCtr) { }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            isToVisitorStateChoseLittleZoo = false;
            isStartGotoEntry = false;

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorEnterZoo.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            entity.Reset();

            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

            //查找进入的路，开始进入。
            string pathName = Config.globalConfig.getInstace().NaturalVisitorOut_1;
            if (entity.entityFuncType == (int)EntityFuncType.Visitor_From_Ship)
            {
                pathName = Config.globalConfig.getInstace().AdvertVisitorInto_2;
            }
            EntityVisitor.GodownPath(entity, pathName, true);
            isStartGotoEntry = true;
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
            this.AddConvertCond((int)VisitorState.ChoseLittleZoo, this.ToVisitorStateChoseLittleZoo);
        }

        protected bool ToVisitorStateChoseLittleZoo()
        {
            return this.isToVisitorStateChoseLittleZoo;
        }

        protected void OnArrived(Message msg)
        {
            var _msg = (MessageArrived)msg;
            //自己的entity
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
            {
                return;
            }

            //到达终点
            if (_msg.followPath.isArrivedEnd && isStartGotoEntry)
            {
                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 在StateVisitorEnterZoo状态  准备转 VisitorStateChoseLittleZoo状态", entity.entityID);
                this.isToVisitorStateChoseLittleZoo = true;
                //在这个状态选择动物栏，必然要去第一组
                entity.stayGroupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
                //ZooEntry 的id 是1000
                entity.stayBuildingID = 1000;
            }
        }
    }
}
