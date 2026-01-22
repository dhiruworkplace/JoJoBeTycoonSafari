using Game.GlobalData;
using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using System.Numerics;
using Logger;

namespace Game
{
    public partial class PlayerDataModule : GameModule
    {

        protected void OnSetEntryGatePureLevelOfPlayerData(Message msg)
        {
            var _msg = msg as SetDetailValueOfPlayerData;
            int entryID = _msg.detailVal;
            var entryGate = GlobalDataManager.GetInstance().playerData.GetEntryGateIDIndexOfDataIdx(entryID);
           
            //升级扣钱
            BigInteger bigDelta = EntryGateModule.GetUpGradeCheckinSpeedConsumption(_msg.detailVal, entryGate.level);

            Logger.LogWarp.LogErrorFormat("_msg.channelID = {0}    {1}    {2}", _msg.channelID, bigDelta, GlobalDataManager.GetInstance().playerData.playerZoo.coin);

            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("售票口扣钱失败");
                throw new System.Exception(e);
                return;
            }
            int deltaLevel = _msg.deltaVal;
            entryGate.level += deltaLevel;

            //广播金钱变化
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
            0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            //LogWarp.LogError("测试：    升级单售票口   "+ entryID);
            //广播某入口升级
            SetDetailValueOfPlayerData.Send((int)GameMessageDefine.BroadcastEntryGatePureLevelOfPlayerData,
                entryID, deltaLevel, 0);
        }

        protected void OnSetEntryGateNumOfPlayerData(Message msg)
        {
            var _msg = msg as SetValueOfPlayerData;

            var sortGateIDs = GlobalDataManager.GetInstance().logicTableEntryGate.sortGateIDs;

            int currLastEntryID = playerData.playerZoo.entryGateList[playerData.playerZoo.numEntryGate - 1].entryID;
            int idx = sortGateIDs.IndexOf(currLastEntryID);
#if UNITY_EDITOR
            if (idx < 0)
            {
                string e = string.Format("设置入口开启异常! 原来{0} 增加 {1}", playerData.playerZoo.numEntryGate, _msg.deltaVal);
                throw new System.Exception(e);
            }
#endif
            //已经开完了。
            if (idx == sortGateIDs.Count - 1)
            {
                return;
            }
            int entryID = 0;
            for (int i = idx + 1; i < idx + 1 + _msg.deltaVal; i++)
            {
                var entryGateData = new EntryGateData();
                entryGateData.entryID = sortGateIDs[i];
                entryID = sortGateIDs[i];
                entryGateData.level = 1;
                playerData.playerZoo.entryGateList.Add(entryGateData);
            }

            this.playerData.playerZoo.numEntryGate += _msg.deltaVal;


            //开启扣钱
            var parce = Config.ticketConfig.getInstace().getCell(idx).number;
            BigInteger bigDelta = BigInteger.Parse(parce);
            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("售票口开启扣钱失败");
                throw new System.Exception(e);
                return;
            }
            //广播金钱变化
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
            0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            //广播新开启了几个入口
            SetValueOfPlayerData.Send((int)GameMessageDefine.BroadcastEntryGateNumOfPlayerData, entryID, 0, _msg.channelID);
        }

    }
}