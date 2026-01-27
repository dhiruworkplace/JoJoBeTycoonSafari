/*******************************************************************
* FileName:     BuffModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-22
* Description:  
* other:    
********************************************************************/


using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace ZooGame
{
    public class BuffModule : GameModule
    {
        public BuffModule(int orderID) : base(orderID) { }

        List<Buff> buffs = new List<Buff>();
        PlayerData playerData;

        long calcOffineTick = 0;

        /// <summary>
        /// buff的相加比例参数
        /// </summary>
        float buffRatioCoinInComeAdd = 1f;
        /// <summary>
        /// buff的相乘比例参数
        /// </summary>
        float buffRatioCoinInComeMul = 1f;
        /// <summary>
        /// 争对与动物栏观光CD
        /// </summary>
        float buffVisitCDVal = float.MaxValue;
        /// <summary>
        /// 争对与出口CD
        /// </summary>
        float buffExitEntryCDVal = float.MaxValue;
        /// <summary>
        ///  争对与售票口CD
        /// </summary>
        float buffEntryGateCDVal = float.MaxValue;

        /// <summary>
        /// 删除buff列表
        /// </summary>
        List<int> removeBuffList = new List<int>();
        /// <summary>
        /// 对是否在加载场景过程中做判断
        /// </summary>
        bool isLoadScene = false;
        public override void Init()
        {
            isLoadScene = false;
            calcOffineTick = DateTime.Now.Ticks;
            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddBuff, OnAddBuff);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.CalcOffline, OnCalcOffline);
            playerData = GlobalDataManager.GetInstance().playerData;

        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddBuff, OnAddBuff);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.CalcOffline, OnCalcOffline);
            for (int i = 0; i < buffs.Count; i++)
            {
                buffs[i].Release();
            }
            buffs.Clear();

            playerData = null;
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!isLoadScene)
            {
                return;
            }
            var buffList = playerData.playerZoo.buffList;
            buffList.Clear();

            buffRatioCoinInComeAdd = 0;
            buffRatioCoinInComeMul = 1;
            buffVisitCDVal = float.MaxValue;
            buffExitEntryCDVal = float.MaxValue;
            buffEntryGateCDVal = float.MaxValue;
            for (int i = 0; i < buffs.Count; i++)
            {
                var buff = buffs[i];
                buff.Tick(deltaTimeMS);
                if (!buff.IsVaild())
                {
                    buff.CD.Stop();
                }
                else
                {
                    buff.CD.Run();
                    //用户数据只保存有效的
                    buffList.Add(buff);
                    switch (buff.buffType)
                    {
                        case BuffType.RatioCoinInComeAdd:
                            buffRatioCoinInComeAdd += buff.buffVal;
                            //buffRatioCoinInComeAdd--;
                            //Logger.LogWarp.LogError("测试：   buffRatioCoinInCome= "+ buffRatioCoinInComeAdd+ "   buff.buffVal= "+ buff.buffVal);
                            break;
                        case BuffType.RatioCoinInComeMul:
                            buffRatioCoinInComeMul *= buff.buffVal;
                            //Logger.LogWarp.LogError("测试：   buffRatioCoinInCome= " + buffRatioCoinInComeAdd + "   buff.buffVal= " + buff.buffVal);
                            break;
                        case BuffType.SetVisitCDVal:
                            if (buff.buffVal < buffVisitCDVal)
                            {
                                buffVisitCDVal = buff.buffVal;
                            }
                            break;
                        case BuffType.SetExitEntryCDVal:
                            if(buff.buffVal < buffExitEntryCDVal)
                            {
                                buffExitEntryCDVal = buff.buffVal;
                            }
                            break;
                        case BuffType.SetEntryGateCDVal:
                            if (buff.buffVal < buffEntryGateCDVal)
                            {
                                buffEntryGateCDVal = buff.buffVal;
                            }
                            break;
                        default:
                            string e = string.Format("没有这个buff类型{0}", buff.buffType);
                            throw new System.Exception(e);
                    }
                }
            }

            //收入系数相加buff
            if (buffRatioCoinInComeAdd != 0)
            {
                playerData.playerZoo.buffRatioCoinInComeAdd = buffRatioCoinInComeAdd;
            }

            //收入系数相乘buff
            //if (buffRatioCoinInComeMul != playerData.playerZoo.buffRatioCoinInComeMul)
            {
                playerData.playerZoo.buffRatioCoinInComeMul = buffRatioCoinInComeMul;
            }

            //游览CD
            playerData.playerZoo.buffVisitCDVal = Const.Invalid_Float;
            if (buffVisitCDVal != float.MaxValue)
            {
                playerData.playerZoo.buffVisitCDVal = buffVisitCDVal;
            }
            //else
            //{
            //    playerData.playerZoo.buffVisitCDVal = Const.Invalid_Float;
            //}

            //出口CD
            playerData.playerZoo.buffExitEntryCDVal = Const.Invalid_Float;
            if (buffExitEntryCDVal != float.MaxValue)
            {
                playerData.playerZoo.buffExitEntryCDVal = buffExitEntryCDVal;
            }
            //else
            //{
            //    playerData.playerZoo.buffExitEntryCDVal = Const.Invalid_Float;
            //}

            //入口CD
            playerData.playerZoo.buffEntryGateCDVal = Const.Invalid_Float;
            if (buffEntryGateCDVal != float.MaxValue)
            {
                playerData.playerZoo.buffEntryGateCDVal = buffEntryGateCDVal;
            }
            //else
            //{
            //    playerData.playerZoo.buffEntryGateCDVal = Const.Invalid_Float;
            //}
        }

        protected void OnAddBuff(Message msg)
        {
            var _msg = msg as BroadcastNum;

            BroadcastNum.Send((int)GameMessageDefine.AddBuffSucceed, _msg.currNum, 0, 0);

            int buffID = _msg.currNum;

            var buff = new Buff();
            buff.Init(buffID);
            buff.CD.Run();
            //不能合并，并且不是收入系数类buff都直接加入
            if (!buff.couldCombine || 
                (buff.buffType != BuffType.RatioCoinInComeAdd && buff.buffType != BuffType.RatioCoinInComeMul)
            )
            {
                buffs.Add(buff);
                return;
            }

            //查找存在的buff是否有本身可合并,相同的Bufftype和buffVal
            for (int i = 0; i < buffs.Count; i++)
            {
                var existBuff = buffs[i];
                if (existBuff.couldCombine && 
                    existBuff.buffType == buff.buffType && 
                    existBuff.buffVal == buff.buffVal)
                {
                    //合并的结果是时间相加
                    existBuff.CD.org += buff.CD.org;
                    existBuff.CD.cd += buff.CD.org;
                    return;
                }
            }

            //没找到能合并的，直接插入
            buffs.Add(buff);
        }

        /// <summary>
        /// 监听是否加载完成场景
        /// </summary>
        /// <param name="msg"></param>
        protected void OnLoadZooSceneFinished(Message msg)
        {
            //场景初始化后，强制计算离线
            bool retCode = CalcOffline(true);
            Logger.LogWarp.LogErrorFormat("CalcOffline {0}", retCode);
            if (retCode)
            {
                playerData.playerZoo.isLoadingShowOffline = true;
                //MessageManager.GetInstance().Send((int)GameMessageDefine.UIMessage_OpenOfflinePage);
            }
            
            isLoadScene = true;
        }

        protected void OnCalcOffline(Message msg)
        {
            if (isLoadScene)
            {
                bool retCode = CalcOffline(false);
                if (retCode)
                {
                    MessageManager.GetInstance().Send((int)GameMessageDefine.UIMessage_OpenOfflinePage);
                }
            }
        }

        protected bool CalcOffline(bool forceCalc)
        {
            long calcDuation = (DateTime.Now.Ticks - this.calcOffineTick) / 10000000;
            if (calcDuation <= 5 && !forceCalc)
            {
                Logger.LogWarp.LogErrorFormat("calcDuation <= 5 {0}", forceCalc);
                return false;
            }

            playerData = GlobalDataManager.GetInstance().playerData;

            if (playerData.playerZoo.isGuide)
            {
                //Logger.LogWarp.LogErrorFormat("playerData.playerZoo.isGuide {0}", playerData.playerZoo.isGuide);
                return false;
            }

            var buffList = playerData.playerZoo.buffList;
            this.buffs.Clear();
            this.buffs.AddRange(buffList);
            double offlineTime = playerData.GetOfflineSecond();
            if (offlineTime <=0)
            {
                return false;
            }

            //if (playerData.playerZoo.buffList == null)
            //{
            //    playerData.playerZoo.buffList = new List<Buff>();
            //}

            removeBuffList.Clear();

            var offlineBuffList = new List<Buff>();
            playerData.playerZoo.offlineBuffList = offlineBuffList;
            for (int i = 0; i < buffList.Count; i++)
            {
                var buff = buffList[i];
                //构建离线buff列表
                if ((offlineTime > 0) &&
                    (buff.buffType == BuffType.RatioCoinInComeAdd || buff.buffType == BuffType.RatioCoinInComeMul))
                {
                    double offlineBuffTime = Math.Min(buff.CD.cd, offlineTime);
                    if (offlineBuffTime > 0)
                    {
                        var offlineBuff = new Buff();
                        offlineBuff.Init(buff.buffID, offlineBuffTime);
                        //离线buff不涉及合并规则，因为离线buff来源于buff列表，已经合并过。
                        offlineBuffList.Add(offlineBuff);
                    }
                }
                //正常BUFF扣除离线时间
                buff.CD.cd -= offlineTime;
                if (buff.CD.cd <= 0)
                {
                    removeBuffList.Add(i);
                }
            }
            //记录要移除的buff载体
            List<Buff> removeList = new List<Buff>();
            //移除移除剩余时间<=0的buff
            for (int i = 0; i < removeBuffList.Count; i++)
            {
                int removeIdx = removeBuffList[i];
                removeList.Add(buffList[removeIdx]);
            }
            foreach (var item in removeList)
            {
                buffList.Remove(item);
            }

            calcOffineTick = DateTime.Now.Ticks;

            this.buffs.Clear();
            this.buffs.AddRange(buffList);

            return true;
        }
    }
}

