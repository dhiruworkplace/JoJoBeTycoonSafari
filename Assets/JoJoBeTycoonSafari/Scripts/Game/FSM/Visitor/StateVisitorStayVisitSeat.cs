/*******************************************************************
* FileName:     StateVisitorStayVisitSeat.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-21
* Description:  
* other:    
********************************************************************/


using DG.Tweening;
using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using ZooGame.MiniGame;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using UnityEngine.UI;

namespace ZooGame
{
    /// <summary>
    /// 观光位状态
    /// 一进入,说明游客已经走到观光位，就要开始计算观光cd
    /// cd结束进入选择动物栏的状态
    /// </summary>
    public class StateVisitorStayVisitSeat : FSMState
    {
        /// <summary>
        /// 是否转选择动物栏状态
        /// </summary>
        bool isToVisitorStateChoseLittleZoo = false;

        bool isToStateVisitorLeaveNonLittleZoo = false;

        IntCD visitCD;

        GameObject visitCDGameObject;//获取对应的技能CD
        Image image_VisitCD; //获取对应的技能CDImage
        //bool isVisitCDFinished = false;

        bool isRevCDVal = false;

        GameObject effGo = null;

        Transform effTrans = null;

        Vector3 effPos;

        int effID = Const.Invalid_Int;

        public StateVisitorStayVisitSeat(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr) { }

        public override void AddAllConvertCond()
        {
            this.AddConvertCond((int)VisitorState.ChoseLittleZoo, this.ToVisitorStateChoseLittleZoo);
            this.AddConvertCond((int)VisitorState.LeaveNonLittleZoo, this.ToStateVisitorLeaveNonLittleZoo);
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorStayVisitSeat.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            isToVisitorStateChoseLittleZoo = false;
            isToStateVisitorLeaveNonLittleZoo = false;

            effID = Const.Invalid_Int;
            effGo = null;

            isRevCDVal = false;
            if (visitCD != null)
            {
                visitCD.Stop();
            }

            // LogWarp.LogFormat("-->Visitor enter tour state: littleZooId ={0}visitorId ={1}", entity.stayBuildingID, entity.entityID);
            // 观光游客面朝动物栏中心
            entity.LookAt(LittleZooPosManager.GetInstance().GetPos(entity.stayBuildingID));

            MessageManager.GetInstance().Regist((int)GameMessageDefine.LittleZooDataReply, OnLittleZooDataReply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorVisitCDFinshedReply, OnVisitorVisitCDFinshedReply);
            //模型到达动物栏等待位   调用新手引导的内容  显示步骤16
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                //LogWarp.LogError("测试：     uIGuidePage.procedure=      " + uIGuidePage.procedure);
                if (uIGuidePage.procedure == 16 )
                {
                    uIGuidePage.procedure = 17;
					GameEventManager.SendEvent("guild_" + 17);                    //本阶段显示文本
                    uIGuidePage.OnClickDialogBoxButton();
                }
                else if (uIGuidePage.procedure ==17)
                {
                    PageMgr.ShowPage<UIGuidePage>();  //开启新手引导UI
                }
            }
            //进入这个状态开始计算游览cd
            LittleZooData.Send(entity.entityID, entity.stayBuildingID);
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.LittleZooDataReply, OnLittleZooDataReply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorVisitCDFinshedReply, OnVisitorVisitCDFinshedReply);
            //发送消息关闭技能刷新

            base.Leave();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!isRevCDVal)
            {
                return;
            }

            visitCD.Tick(deltaTimeMS);  // cd时间递减

            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
            {
                if (visitCDGameObject == null)
                {
                    GetSceneUIGameObject();
                }
            }

            if (!visitCD.IsFinish())
            {
                image_VisitCD.fillAmount = 1 - (float)visitCD.cd / visitCD.org;
            }
            //cd结束腾出观光位
            //是再次进入选动物栏状态，还是已经到最后一组准备离开
            if (visitCD.IsRunning())
            {
                if (visitCD.IsFinish())
                {
                    visitCDGameObject.transform.position = UFrame.Const.Invisible_Postion;
                    GetFlutterTextGameObject();
                    visitCD.Stop();
                    WhenVisitCDFinished();
                }
            }
        }

        protected void OnLittleZooDataReply(Message msg)
        {
            var _msg = msg as LittleZooDataReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //LogWarp.LogErrorFormat("{0}, {1}", _msg != null, entity != null);
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            int cdVal = _msg.littleZoo.visitCDValue;
            var playerData = GlobalDataManager.GetInstance().playerData;
            if (playerData.playerZoo.buffVisitCDVal != UFrame.Const.Invalid_Float)
            {
                int buffVisitCDVal = Math_F.FloatToInt1000(playerData.playerZoo.buffVisitCDVal);
                cdVal = Mathf.Min(cdVal, buffVisitCDVal);
            }

            if (visitCD == null)
            {
                visitCD = new IntCD(cdVal);
            }
            else
            {
                visitCD.ResetOrg(cdVal);
            }
            if (!GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
            {
                GetSceneUIGameObject();
            }

            visitCD.Run();
            isRevCDVal = true;

            //cd时间太短就执行idle
            if (cdVal < Math_F.FloatToInt1000(Config.globalConfig.getInstace().VisitorMinAnimLen))
            {
                entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorIdle);
                return;
            }
            var cell = entity.PlayActionAnim();
            if (cell.effectresid > 0)
            {
                effID = cell.effectresid;
                var pool = PoolManager.GetInstance().GetGameObjectPool(effID);
                effGo = pool.New();
                effTrans = effGo.transform;
                effTrans.SetParent(GlobalDataManager.GetInstance().zooGameSceneData.littleZooParentNode, false);
                effPos = entity.position;
                effPos.y = cell.effectY;
                effTrans.position = effPos;
                effTrans.rotation = entity.rotation;
            }
        }
        /// <summary>
        /// 获取观光位CD对象
        /// </summary>
        private void GetSceneUIGameObject()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            int littleZooID = entity.stayBuildingID;//动物栏的ID
            int index = entity.indexInVisitQueue;//观光位置

            Vector3 pathPos = entity.followPath.pathPosList[entity.followPath.nextPosIdx];
            visitCDGameObject = LittleZooModule.GetVisitCDGameObject(littleZooID, pathPos, index);
            image_VisitCD = visitCDGameObject.transform.Find("Text_UI").Find("Image_Skill").GetComponent<Image>();
        }

        /// <summary>
        /// 获取观光位飘字对象
        /// </summary>
        private void GetFlutterTextGameObject()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            int littleZooID = entity.stayBuildingID;//动物栏的ID
            int index = entity.indexInVisitQueue;//观光位置
            Vector3 pathPos = entity.followPath.pathPosList[entity.followPath.nextPosIdx];
            LittleZooModule.GetFlutterTextGameObject(littleZooID, pathPos, index);
        }

        protected void OnVisitorVisitCDFinshedReply(Message msg)
        {
            var _msg = msg as VisitorVisitCDFinshedReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            PrepareChoseLittleZooAgain(entity);
        }

        protected bool ToVisitorStateChoseLittleZoo()
        {
            return this.isToVisitorStateChoseLittleZoo;
        }

        protected bool ToStateVisitorLeaveNonLittleZoo()
        {
            return isToStateVisitorLeaveNonLittleZoo;
        }

        protected void WhenVisitCDFinished()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            SendLeaveVisitSeat(entity);

            if (effID != Const.Invalid_Int && effGo != null)
            {
                var pool = PoolManager.GetInstance().IsBelongGameObjectPool(effID);
#if UNITY_EDITOR
                if (pool == null)
                {
                    string e = string.Format("异常,找不到回收的GameObjectPool {0}", effID);
                    throw new System.Exception(e);
                }
#endif
                effTrans.position = Const.Invisible_Postion;
                pool.Delete(effGo);
                effGo = null;
                effTrans = null;

            }

        }

        /// <summary>
        /// 发送离开观光位消息
        /// </summary>
        protected void SendLeaveVisitSeat(EntityVisitor entity)
        {
            var msg = VisitorVisitCDFinshed.Send(entity.entityID, entity.indexInVisitQueue, entity.stayBuildingID);
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} send {1}", entity.entityID, msg);
        }

        /// <summary>
        /// 准备再次选择动物栏，可能因为所有组都浏览过了，会转向离开
        /// </summary>
        /// <param name="entity"></param>
        protected void PrepareChoseLittleZooAgain(EntityVisitor entity)
        {
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
            if (effGo != null)
            {
                effTrans.position = Const.Invisible_Postion;
            }
            
            isToVisitorStateChoseLittleZoo = true;
        }

    }
}
