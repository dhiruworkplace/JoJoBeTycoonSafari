/*******************************************************************
* FileName:     StateVisitorStayFirstPosInEntryQueue.cs
* Author:       Fan Zheng Yong
* Date:         2019-11-12
* Description:  
* other:    
********************************************************************/


using DG.Tweening;
using Game.GlobalData;
using Game.MessageCenter;
using Game.Path.StraightLine;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// 入口排队首位状态
    /// 一进入本状态,就要获取cd时间
    /// cd结束进入进入动物园第一条路的起点，到达这个点后切到选择动物栏状态
    /// </summary>
    public class StateVisitorStayFirstPosInEntryQueue : FSMState
    {

        IntCD checkInCD;
        GameObject visitCDGameObject;//获取对应的技能CD
        Image image_VisitCD; //获取对应的技能CDImage
        bool isToVisitorStateChoseLittleZoo = false;
        bool isRecvCDVal = false;
        bool isSendCDFinshed = false;

        int maxUpdateUIDuration = 100;
        int accumulativeUpdateUITime = 0;

        public StateVisitorStayFirstPosInEntryQueue(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        { }

        public override void AddAllConvertCond()
        {
            this.AddConvertCond((int)VisitorState.ChoseLittleZoo, this.ToVisitorStateChoseLittleZoo);
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorStayFirstPosInEntryQueue.Enter", entity.entityID);
            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorIdle);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorIdle);
            isToVisitorStateChoseLittleZoo = false;

            accumulativeUpdateUITime = 0;

            if (checkInCD != null)
            {
                checkInCD.Stop();
            }

            isRecvCDVal = false;
            isSendCDFinshed = false;
            MessageManager.GetInstance().Regist((int)GameMessageDefine.GetEntryGateDataReply, OnGetEntryGateDataReply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.EntryGateCheckInCDFinshedReply, OnEntryGateCheckInCDFinshedReply);
            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

            /*   模型到达售票口   调用新手引导的内容  显示步骤10   */
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                if (uIGuidePage.procedure == 10)
                {
                    PageMgr.ShowPage<UIGuidePage>();
                }
                else if (uIGuidePage.procedure == 9)
                {
                    uIGuidePage.procedure = 10;
					GameEventManager.SendEvent("guild_" + uIGuidePage.procedure);
                    uIGuidePage.OnClickDialogBoxButton();
                }
            }

            //进入这个状态开始计算游览cd
            GetEntryGateDataApply.Send(entity.entityID, entity.zooEntryID);
        }

        public override void Leave()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.GetEntryGateDataReply, OnGetEntryGateDataReply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.EntryGateCheckInCDFinshedReply, OnEntryGateCheckInCDFinshedReply);
            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);

            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);

            base.Leave();
        }

        /// <summary>
        /// CD递减
        /// </summary>
        /// <param name="deltaTimeMS"></param>
        public override void Tick(int deltaTimeMS)
        {
            if (!isRecvCDVal)
            {
                return;
            }
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
            {
                if (visitCDGameObject==null)
                {
                    GetSceneUIGameObject();
                }
            }
            checkInCD.Tick(deltaTimeMS);  // cd时间递减
#if UNITY_EDITOR && !NOVICEGUIDE
            if (visitCDGameObject == null)
            {
                string e = string.Format("visitCDGameObject == null");
                throw new System.Exception(e);
                //GetSceneUIGameObject();
            }
#endif
            accumulativeUpdateUITime += deltaTimeMS;
            if (!checkInCD.IsFinish() && accumulativeUpdateUITime >= maxUpdateUIDuration)
            {
                image_VisitCD.fillAmount = 1 - (float)checkInCD.cd / checkInCD.org;
                accumulativeUpdateUITime = 0;
            }

            if (checkInCD.IsRunning() && checkInCD.IsFinish())
            {
                image_VisitCD.fillAmount = 0;
                visitCDGameObject.transform.position = UFrame.Const.Invisible_Postion;
                GetFlutterTextGameObject();
                checkInCD.Stop();
                WhenCDFinished();
            }
        }

        protected void OnGetEntryGateDataReply(Message msg)
        {
            var _msg = msg as GetEntryGateDataReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            //LogWarp.LogErrorFormat("{0}, {1}", _msg != null, entity != null);
            if (_msg.entityID != entity.entityID)
            {
                return;
            }

            int cdVal = _msg.entryGate.checkInCDVal;
#if UNITY_EDITOR
            if (cdVal <=0)
            {
                string e = string.Format("cdVal = {0}", cdVal);
                throw new System.Exception(e);
            }
#endif
            var playerData = GlobalDataManager.GetInstance().playerData;
            if (playerData.playerZoo.buffEntryGateCDVal != UFrame.Const.Invalid_Float)
            {
                int buffCD = Math_F.FloatToInt1000(playerData.playerZoo.buffEntryGateCDVal);
                cdVal = Mathf.Min(cdVal, buffCD);
            }

            if (checkInCD == null)
            {
                checkInCD = new IntCD(cdVal);
            }
            else
            {
                checkInCD.ResetOrg(cdVal);
            }

            if (!GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
            {
                GetSceneUIGameObject();
            }

#if !NOVICEGUIDE
            //visitCDGameObject.name += string.Format("_{0}", entity.entityID);
            LogWarp.LogError("测试： OnGetEntryGateDataReply   GetSceneUIGameObject ");
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} recv cd = {1}", entity.entityID, cdVal);
#endif


            checkInCD.Run();

            isRecvCDVal = true;
        }

        /// <summary>
        /// 在进入动物栏检测CD的消息
        /// </summary>
        /// <param name="msg"></param>
        protected void OnEntryGateCheckInCDFinshedReply(Message msg)
        {
            var _msg = msg as EntryGateCheckInCDFinshedReply;
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            if (_msg.entityID != entity.entityID)
            {
                return;
            }
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} GotoFirstPathOfZoo", entity.entityID);
            //要进入动物园了，先走 到1001的起点
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} Play {1}", entity.entityID, Config.globalConfig.getInstace().VisitorWalk);
            entity.PlayActionAnim(Config.globalConfig.getInstace().VisitorWalk);
            GotoFirstPathOfZoo(entity);
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
//#if UNITY_EDITOR
                if (!isRecvCDVal || !isSendCDFinshed)
                {
                    //string e = string.Format("{0} 没收收到CD值", entity.entityID);
                    //throw new System.Exception(e);
                    return;
                }
//#endif
                this.isToVisitorStateChoseLittleZoo = true;
                //在这个状态选择动物栏，必然要去第一组
                entity.stayGroupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
                //ZooEntry 的id 是1000
                entity.stayBuildingID = 1000;
            }
        }

        protected bool ToVisitorStateChoseLittleZoo()
        {
            return this.isToVisitorStateChoseLittleZoo;
        }

        /// <summary>
        /// CD 结束   发送消息
        /// </summary>
        protected void WhenCDFinished()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            var msg = EntryGateCheckInCDFinshedApply.Send(entity.entityID, entity.zooEntryID);
            isSendCDFinshed = true;

            if (GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                if (uIGuidePage.procedure == 15)
                {
                    PageMgr.ShowPage<UIGuidePage>();  //开启新手引导UI
                }
                else if (uIGuidePage.procedure < 15)
                {
                    uIGuidePage.procedure = 15;
					GameEventManager.SendEvent("guild_" + 15);
	                     //本阶段显示文本
                    uIGuidePage.OnClickDialogBoxButton();
                }
            }

            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} WhenCDFinished send {1}", entity.entityID, msg);
        }

        protected void GotoFirstPathOfZoo(EntityVisitor entity)
        {
            //var startPos = entity.position;
            //string pathName = Config.globalConfig.getInstace().FirstPathOfZoo;
            //EntityVisitor.GotoStartOfPath(entity, pathName);
            float zooFirstPosOffset = Config.globalConfig.getInstace().ZooFirstPosOffset;
            var startPos = entity.position;
            var midPos = startPos + GlobalDataManager.GetInstance().SceneForward * zooFirstPosOffset;
            Vector3 endPos = Vector3.zero;
            bool retCode = PathManager.GetInstance().GetPathFirstPos(Config.globalConfig.getInstace().FirstPathOfZoo, ref endPos);
#if UNITY_EDITOR
            if (!retCode)
            {
                string e = string.Format("取{0} 得起点异常", Config.globalConfig.getInstace().FirstPathOfZoo);
                throw new System.Exception(e);
            }
#endif
            entity.pathList.Clear();
            entity.pathList.Add(startPos);
            entity.pathList.Add(midPos);
            entity.pathList.Add(endPos);
            EntityVisitor.GodownPath(entity, entity.pathList);
        }


        /// <summary>
        /// 获取售票口CD对象
        /// </summary>
        private void GetSceneUIGameObject()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

            int index = entity.zooEntryID;//观光位置

            visitCDGameObject = EntryGateModule.GetVisitCDGameObject(index);
            image_VisitCD = visitCDGameObject.transform.Find("Text_UI").Find("Image_Skill").GetComponent<Image>();
            image_VisitCD.fillAmount = 0f;
        }

        /// <summary>
        /// 获取售票口飘字对象
        /// </summary>
        private void GetFlutterTextGameObject()
        {
            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
            int littleZooID = entity.stayBuildingID;//动物栏的ID
            int index = entity.zooEntryID;//观光位置
            image_VisitCD.fillAmount = 0f;

            //LogWarp.LogError("测试：  选择了下标为 " + index + "  的售票口，数据个数"+ GlobalDataManager.GetInstance().entryCoinSpList.Count);

            EntryGateModule.GetFlutterTextGameObject(index);
            //金钱粒子特效
            var sp = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.entryCoinSpList[index];
            if (!sp.isInit)
            {
                Vector3 visitPos = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.entryGatesVector[index];
                visitPos.y = Config.globalConfig.getInstace().TicketEffectOffsetY;
                GameObject effectGo = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().CoinEffectRes);
                effectGo.transform.position = visitPos;
                sp.Init(effectGo);
            }
            sp.Play();


        }
    }
}
