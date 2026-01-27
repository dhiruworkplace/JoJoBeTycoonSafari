/*******************************************************************
* FileName:     PlayerDataModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-10
* Description:  
* other:    
********************************************************************/


using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using System.Numerics;
using Logger;
using System;

namespace ZooGame
{
    public partial class PlayerDataModule : GameModule
    {
        public PlayerDataModule(int orderID) : base(orderID) { }

        PlayerData playerData;
        List<int> trigerLoadLittleZooIDs;
        
        public override void Init()
        {
            playerData = GlobalDataManager.GetInstance().playerData;
            trigerLoadLittleZooIDs = new List<int>();

            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetParkingProfitLevelOfPlayerData, this.OnSetParkingProfitLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetParkingSpaceLevelOfPlayerData, this.OnSetParkingSpaceLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetParkingEnterCarSpawnLevelOfPlayerData, this.OnSetParkingEnterCarSpawnLevelOfPlayerData);

            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetCoinOfPlayerData, this.OnSetCoinOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetDiamondOfPlayerData, this.OnSetDiamondOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetStarOfPlayerData, this.OnSetStarOfPlayerData);

            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetLittleZooTicketsLevelPlayerData, this.OnSetLittleZooTicketsLevelPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetLittleZooVisitorLocationLevelOfPlayerData, this.OnSetLittleZooVisitorLocationLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetLittleZooEnterVisitorSpawnLevelOfPlayerData, this.OnSetLittleZooEnterVisitorSpawnLevelOfPlayerData);


            MessageManager.GetInstance().Regist((int)GameMessageDefine.OpenNewLittleZoo, this.OnOpenNewLittleZoo);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorVisitCDFinshedReply, OnVisitorVisitCDFinshedReply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.EntryGateCheckInCDFinshedReply, OnEntryGateCheckGoToZoo);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetAchievementObject, this.OnSetAchievementObjectData);

            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetEntryGateLevelOfPlayerData, this.OnSetEntryGateLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetEntryGatePureLevelOfPlayerData, this.OnSetEntryGatePureLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetEntryGateNumOfPlayerData, this.OnSetEntryGateNumOfPlayerData);

        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetParkingProfitLevelOfPlayerData, this.OnSetParkingProfitLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetParkingSpaceLevelOfPlayerData, this.OnSetParkingSpaceLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetParkingEnterCarSpawnLevelOfPlayerData, this.OnSetParkingEnterCarSpawnLevelOfPlayerData);


            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetCoinOfPlayerData, this.OnSetCoinOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetDiamondOfPlayerData, this.OnSetDiamondOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetStarOfPlayerData, this.OnSetStarOfPlayerData);

            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetLittleZooTicketsLevelPlayerData, this.OnSetLittleZooTicketsLevelPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetLittleZooVisitorLocationLevelOfPlayerData, this.OnSetLittleZooVisitorLocationLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetLittleZooEnterVisitorSpawnLevelOfPlayerData, this.OnSetLittleZooEnterVisitorSpawnLevelOfPlayerData);

            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.OpenNewLittleZoo, this.OnOpenNewLittleZoo);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorVisitCDFinshedReply, OnVisitorVisitCDFinshedReply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.EntryGateCheckInCDFinshedReply, OnEntryGateCheckGoToZoo);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetAchievementObject, this.OnSetAchievementObjectData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetEntryGateLevelOfPlayerData, this.OnSetEntryGateLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetEntryGatePureLevelOfPlayerData, this.OnSetEntryGatePureLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetEntryGateNumOfPlayerData, this.OnSetEntryGateNumOfPlayerData);


            playerData = null;
            trigerLoadLittleZooIDs.Clear();
            Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        /// <summary>
        /// 接受购买、收集消息进行数据修改
        /// </summary>
        /// <param name="obj"></param>
        private void OnSetAchievementObjectData(Message obj)
        {
            var _mag = obj as SetAchievementObjectData;
            //LogWarp.LogErrorFormat("测试：收到收集object：  商品归属类ID：{0}，商品ID：{1}， 采购数量：{2}，花费金额(大额)：{3}，花费金额：{4}", _mag.belongID, _mag.goodsID, _mag.purchaseQuantity, _mag.bigIntCostVal,_mag.costVal);
            switch (_mag.belongID)
            {
                case 1://商品类型为动物
                    BuyAnimal(_mag);
                    break;
                case 2://商品类型为buff
                    BuyBuff(_mag);
                    break;
                case 3://商品类型为礼包
                    BuyGift(_mag);
                    break;
                case 4://商品类型为钻石
                    BuyDiamond(_mag);
                    break;
                case 5://商品类型为现金
                    BuyCash(_mag);
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// 停车场的等级升级（利润）
        /// </summary>
        /// <param name="msg"></param>
        protected void OnSetParkingProfitLevelOfPlayerData(Message msg)
        {
            var _msg = msg as SetValueOfPlayerData;
            int parkingProfitLevel = this.playerData.playerZoo.parkingCenterData.parkingProfitLevel;
            BigInteger bigDelta = ParkingCenter.GetUpGradeParkingProfitConsumption(parkingProfitLevel);
            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("停车场升级扣钱失败");
                throw new System.Exception(e);
                return;
            }

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
            0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            this.playerData.playerZoo.parkingCenterData.parkingProfitLevel +=1;
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastParkingProfitLevelOfPlayerData,
                this.playerData.playerZoo.parkingCenterData.parkingProfitLevel, _msg.deltaVal, 0, 0);
            //收集星星：
            var lvshage = Config.parkingConfig.getInstace().getCell(1).lvshage;
            int idx = FindLevelRangIndex01(lvshage, this.playerData.playerZoo.parkingCenterData.parkingProfitLevel);
            int stageLevel = Config.parkingConfig.getInstace().getCell(1).lvshage[idx];
            if (this.playerData.playerZoo.parkingCenterData.parkingProfitLevel == stageLevel)
            {
                int awardType = Config.parkingConfig.getInstace().getCell(1).lvrewardtype[idx];
                if (awardType == 1)
                {
                    //发放奖励道具
                    int awardID = Config.parkingConfig.getInstace().getCell(1).lvreward[idx];
                    MessageInt.Send((int)GameMessageDefine.GetItem, awardID);
                    PageMgr.GetPage<UIMainPage>().OnMoneyEffect();
                    LogWarp.LogErrorFormat("停车场   当前等级为{0}，可以发放奖励道具{1}", stageLevel, awardID);
                }
                //发放星星
                MessageInt.Send((int)GameMessageDefine.GetItem, 4);
                LogWarp.LogErrorFormat("停车场   当前等级为{0}，可以发放星星", stageLevel);

            }

        }

        /// <summary>
        /// 停车场的停车位数量升级
        /// </summary>
        /// <param name="obj"></param>
        private void OnSetParkingSpaceLevelOfPlayerData(Message obj)
        {
            var _msg = obj as SetValueOfPlayerData;
            int parkingSpaceLevel = this.playerData.playerZoo.parkingCenterData.parkingSpaceLevel;
            BigInteger bigDelta = (ParkingCenter.GetUpGradeNumberConsumption(parkingSpaceLevel));
            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("停车场升级扣钱失败");
                throw new System.Exception(e);
                return;
            }

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
            0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            this.playerData.playerZoo.parkingCenterData.parkingSpaceLevel += 1;
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastParkingSpaceLevelOfPlayerData,
                this.playerData.playerZoo.parkingCenterData.parkingSpaceLevel, _msg.deltaVal, 0, 0);
        }

        /// <summary>
        /// 停车场的来客流量升级
        /// </summary>
        /// <param name="obj"></param>
        private void OnSetParkingEnterCarSpawnLevelOfPlayerData(Message obj)
        {
            var _msg = obj as SetValueOfPlayerData;
            int parkingEnterCarSpawnLevel = this.playerData.playerZoo.parkingCenterData.parkingEnterCarSpawnLevel;
            BigInteger bigDelta = ParkingCenter.GetUpGradeEnterCarSpawnConsumption(parkingEnterCarSpawnLevel);
            LogWarp.LogErrorFormat("测试：parkingEnterCarSpawnLevel={0}  bigDelta={1}", parkingEnterCarSpawnLevel, bigDelta);

            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("停车场的来客流量升级");
                throw new System.Exception(e);
                return;
            }

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
            0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            this.playerData.playerZoo.parkingCenterData.parkingEnterCarSpawnLevel += 1;
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastParkingEnterCarSpawnLevelOfPlayerData,
                this.playerData.playerZoo.parkingCenterData.parkingEnterCarSpawnLevel, _msg.deltaVal, 0, 0);
        }



        protected  bool VaryDataCoin( BigInteger big )
        {
            var left = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin) - big;
            //Logger.LogWarp.LogErrorFormat("测试：  {0}    {1}",GlobalDataManager.GetInstance().playerData.playerZoo.coin,big);
            if (left < 0)
            {
                return false;
            }
            GlobalDataManager.GetInstance().playerData.playerZoo.coin = left.ToString();
            return true;
        }
 
        protected void OnSetCoinOfPlayerData(Message msg)
        {
            var _msg = msg as SetValueOfPlayerData;

            //this.playerData.playerZoo.coin += _msg.deltaVal;
            var deltaVal = _msg.bigIntDeltaVal;
            var currVal = BigInteger.Parse(this.playerData.playerZoo.coin);
            currVal += deltaVal;
            this.playerData.playerZoo.coin = currVal.ToString();

            LogWarp.LogError("测试OnSetCoinOfPlayerData：收益  deltaVal=" + deltaVal);
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData, 0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), _msg.bigIntDeltaVal);
        }

        protected void OnSetDiamondOfPlayerData(Message msg)
        {
            var _msg = msg as SetValueOfPlayerData;
            this.playerData.playerZoo.diamond += _msg.deltaVal;
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastDiamondOfPlayerData, this.playerData.playerZoo.diamond, _msg.deltaVal, 0, 0);
        }

        protected void OnSetStarOfPlayerData(Message msg)
        {
            var _msg = msg as SetValueOfPlayerData;
            this.playerData.playerZoo.star += _msg.deltaVal;
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastStarOfPlayerData, this.playerData.playerZoo.star, _msg.deltaVal, 0, 0);
        }

        /// <summary>
        /// 动物栏的门票升级消息修改
        /// </summary>
        /// <param name="msg"></param>
        protected void OnSetLittleZooTicketsLevelPlayerData(Message msg)
        {
            var _msg = msg as SetDetailValueOfPlayerData;
            // 涉及金币减扣
            LittleZooModuleData littleZooModuleData = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(_msg.detailVal);

            BigInteger bigDelta = (LittleZooModule.GetUpGradeConsumption(_msg.detailVal, littleZooModuleData.littleZooTicketsLevel + _msg.deltaVal));

            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("升级动物栏扣钱失败");
                throw new System.Exception(e);
                return;
            }

            //修改动物栏等级
            int currVal = littleZooModuleData.littleZooTicketsLevel + _msg.deltaVal;
            //LogWarp.Log("测试：  等级原来是"+zooLevel+"   现在是  "+currVal);
            int idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(_msg.detailVal); //获取动物栏ID  下标
            this.playerData.playerZoo.littleZooModuleDatas[idx].littleZooTicketsLevel = currVal;

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
                0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            BroadcastDetailValueOfPlayerData.Send((int)GameMessageDefine.BroadcastLittleZooTicketsLevelPlayerData,
                _msg.detailVal, currVal, _msg.deltaVal);
            //收集星星：
            var lvshage = Config.buildupConfig.getInstace().getCell(_msg.detailVal).lvshage;

            int idx01 = FindLevelRangIndex01(lvshage, currVal);
            int stageLevel = Config.buildupConfig.getInstace().getCell(_msg.detailVal).lvshage[idx01];
            if (this.playerData.playerZoo.littleZooModuleDatas[idx].littleZooTicketsLevel == stageLevel)
            {
                int awardType = Config.buildupConfig.getInstace().getCell(_msg.detailVal).lvrewardtype[idx01];
                if (awardType==1)
                {
                    //发放奖励道具
                    int awardID = Config.buildupConfig.getInstace().getCell(_msg.detailVal).lvreward[idx01];
                    MessageInt.Send((int)GameMessageDefine.GetItem, awardID);
                    PageMgr.GetPage<UIMainPage>().OnMoneyEffect();
                    LogWarp.LogErrorFormat("动物栏   当前等级为{0}，可以发放奖励道具{1}", stageLevel, awardID);
                }
                else if(awardType ==2)
                {
                    var buildUpCell = Config.buildupConfig.getInstace().getCell(_msg.detailVal);

                    int animalID = buildUpCell.lvreward[idx01];
                    LogWarp.LogError("测试：AAAAAAAAAAAAAAAAAAAAAAA    animalID = "+animalID);
                    var animalUpCell = Config.animalupConfig.getInstace().getCell(animalID);

                    LittleZooModule.LoadAnimal(_msg.detailVal, animalID,
                           animalUpCell.moveradius, buildUpCell.animalwanderoffset);
                    //关于Ui等级打点（在旋转相机的时候）
                    UIZooPage uIZooPage = PageMgr.GetPage<UIZooPage>();
                    if (uIZooPage != null)
                    {
                        uIZooPage.OnGetBroadcastLittleZooTicketsLevelPlayerData(null);
                        uIZooPage.Hide();
                    }

                    var resourceID = Config.animalupConfig.getInstace().getCell(animalID).resourceload;
                    //旋转视角UI
                    PageMgr.ShowPage<UIReceivePage>(resourceID);
                    MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButHidePart, "UIMainPage");
                }
                //发放星星
                MessageInt.Send((int)GameMessageDefine.GetItem, 4);

                LogWarp.LogErrorFormat("动物栏  当前等级为{0}，可以发放星星", stageLevel);

            }


        }

        /// <summary>
        /// 动物栏的观光点数量消息修改
        /// </summary>
        /// <param name="obj"></param>
        private void OnSetLittleZooVisitorLocationLevelOfPlayerData(Message obj)
        {
            var _msg = obj as SetDetailValueOfPlayerData;
            // 涉及金币减扣
            LittleZooModuleData littleZooModuleData = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(_msg.detailVal);
            BigInteger bigDelta = (LittleZooModule.GetUpGradeVisitorLocationLevelConsumption(_msg.detailVal, littleZooModuleData.littleZooVisitorSeatLevel + _msg.deltaVal));

            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("升级动物栏扣钱失败");
                throw new System.Exception(e);
                return;
            }

            int currVal = littleZooModuleData.littleZooVisitorSeatLevel + _msg.deltaVal;
            //LogWarp.Log("测试：  等级原来是"+zooLevel+"   现在是  "+currVal);
            int idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(_msg.detailVal);  //获取动物栏ID  下标
            this.playerData.playerZoo.littleZooModuleDatas[idx].littleZooVisitorSeatLevel = currVal;

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
                0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            BroadcastDetailValueOfPlayerData.Send((int)GameMessageDefine.BroadcastLittleZooVisitorLocationLevelOfPlayerData,
                _msg.detailVal, currVal, _msg.deltaVal);
        }

        /// <summary>
        /// 动物栏的观光游客流量消息修改
        /// </summary>
        /// <param name="obj"></param>  
        private void OnSetLittleZooEnterVisitorSpawnLevelOfPlayerData(Message obj)
        {
            var _msg = obj as SetDetailValueOfPlayerData;
            // 涉及金币减扣
            LittleZooModuleData littleZooModuleData = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(_msg.detailVal);

            BigInteger bigDelta = (LittleZooModule.GetUpGradeEnterVisitorSpawnLevelConsumption(_msg.detailVal, littleZooModuleData.littleZooEnterVisitorSpawnLevel + _msg.deltaVal));

            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("升级动物栏扣钱失败");
                throw new System.Exception(e);
                return;
            }

            //修改动物栏等级
            int currVal = littleZooModuleData.littleZooEnterVisitorSpawnLevel + _msg.deltaVal;
            //LogWarp.Log("测试：  等级原来是"+zooLevel+"   现在是  "+currVal);
            int idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(_msg.detailVal); //获取动物栏ID  下标
            this.playerData.playerZoo.littleZooModuleDatas[idx].littleZooEnterVisitorSpawnLevel = currVal;

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
                0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            BroadcastDetailValueOfPlayerData.Send((int)GameMessageDefine.BroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData,
                _msg.detailVal, currVal, _msg.deltaVal);
        }

        protected void OnOpenNewLittleZoo(Message msg)
        {
            //判定是不是最后，是否需要加载新地块，以及新加载地块上的动物栏
            var _msg = msg as OpenNewLittleZoo;
            int littleZooID = _msg.littleZooID;
            string e;
            int idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(littleZooID);
            if (idx < 0)
            {
                e = string.Format("开启的动物栏 {0} 在用户数据中没有", littleZooID);
                throw new System.Exception(e);
            }

#if UNITY_EDITOR
            if (playerData.playerZoo.littleZooModuleDatas[idx].littleZooTicketsLevel != 0)
            {
                e = string.Format("开启的动物栏 {0} 在用户数据中等级!=0", littleZooID);
                throw new System.Exception(e);
            }
#endif

            playerData.playerZoo.littleZooModuleDatas[idx].littleZooTicketsLevel = 1;
            playerData.playerZoo.littleZooModuleDatas[idx].littleZooEnterVisitorSpawnLevel = 1;
            playerData.playerZoo.littleZooModuleDatas[idx].littleZooVisitorSeatLevel = 1;


            int nextGroupID = Const.Invalid_Int;
            bool trigerExtend = false;
            trigerLoadLittleZooIDs.Clear();
            if (GlobalDataManager.GetInstance().logicTableGroup.IsTrigerLoadNextGroupID(
                littleZooID, ref nextGroupID))
            {
                //触发额外的地块，用户数据中加数据
                if (!GlobalDataManager.GetInstance().zooGameSceneData.IsExtendGroupContains(nextGroupID))
                {
                    trigerExtend = true;
                    var cell = Config.groupConfig.getInstace().getCell(nextGroupID);
                    trigerLoadLittleZooIDs.AddRange(cell.startid);

                    for (int i = 0; i < trigerLoadLittleZooIDs.Count; i++)
                    {
                        //playerData.playerZoo.littleZooLevels.Add(0);
                        LittleZooModuleData littleZooModuleData = new LittleZooModuleData
                        {
                            littleZooID = trigerLoadLittleZooIDs[i],
                            littleZooTicketsLevel = 0,
                            littleZooVisitorSeatLevel = 0,
                            littleZooEnterVisitorSpawnLevel = 0,
                        };
                        playerData.playerZoo.littleZooModuleDatas.Add(littleZooModuleData);
                    }
                }
  
            }

            //PlayerData.Save(this.playerData);
            //var cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
            //BigInteger bigDelta = BigInteger.Parse(cellBuildUp.number);
            //bool retCode = VaryDataCoin(bigDelta);
            //if (!retCode)
            //{
            //    e = string.Format("开启新动物栏扣钱失败");
            //    throw new System.Exception(e);
            //    return;
            //}
 
            //BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
            //   0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);

            LogWarp.LogError("测试：开启了littleZooID"+ littleZooID);
            BroadcastOpenNewLittleZoo.Send(littleZooID, trigerExtend, nextGroupID, trigerLoadLittleZooIDs);
        }

        /// <summary>
        /// 收到设置售票口的消息
        /// </summary>
        /// <param name="msg"></param>
        protected void OnSetEntryGateLevelOfPlayerData(Message msg)
        {
            var _msg = msg as SetValueOfPlayerData;
            // 涉及金币减扣
            BigInteger bigDelta = EntryGateModule.GetUpGradeConsumption( GlobalDataManager.GetInstance().playerData.playerZoo.entryTicketsLevel);
            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode)
            {
                string e = string.Format("售票口升级扣钱失败");
                throw new System.Exception(e);
                return;
            }

            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
               0, 0, BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin), bigDelta);
            this.playerData.playerZoo.entryTicketsLevel += 1;
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastEntryGateLevelOfPlayerData, this.playerData.playerZoo.entryTicketsLevel, _msg.deltaVal, 0, 0);
            //收集星星：
            var lvshage = Config.ticketConfig.getInstace().getCell(1).lvshage;
            int idx = FindLevelRangIndex01(lvshage, this.playerData.playerZoo.entryTicketsLevel);
            int stageLevel = Config.ticketConfig.getInstace().getCell(1).lvshage[idx];
            if (this.playerData.playerZoo.entryTicketsLevel == stageLevel)
            {
                int awardType = Config.ticketConfig.getInstace().getCell(1).lvrewardtype[idx];
                if (awardType == 1)
                {
                    //发放奖励道具
                    int awardID = Config.ticketConfig.getInstace().getCell(1).lvreward[idx];
                    MessageInt.Send((int)GameMessageDefine.GetItem, awardID);
                    PageMgr.GetPage<UIMainPage>().OnMoneyEffect();
                    LogWarp.LogErrorFormat("售票口 当前等级为{0}，可以发放奖励道具{1}", stageLevel, awardID);
                }
                //发放星星
                MessageInt.Send((int)GameMessageDefine.GetItem, 4);
                LogWarp.LogErrorFormat("售票口 当前等级为{0}，可以发放星星", stageLevel);

            }
        }

        protected void AddCoin(BigInteger addNum)
        {
            BigInteger currCoin = BigInteger.Parse(this.playerData.playerZoo.coin);
            currCoin += addNum;
            this.playerData.playerZoo.coin = currCoin.ToString();
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
                0, 0, currCoin, addNum);
        }

        /// <summary>
        /// 动物栏CD收益
        /// </summary>
        /// <param name="msg"></param>
        protected void OnVisitorVisitCDFinshedReply(Message msg)
        {
#if NO_BIGINT
            PlaySceneMoneyMusic();
#else

            var _msg = msg as VisitorVisitCDFinshedReply;
            int littleZooEnterVisitorSpawnLevel = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(_msg.littleZooID).littleZooTicketsLevel;
            BigInteger price = LittleZooModule.GetLittleZooPrice(_msg.littleZooID,littleZooEnterVisitorSpawnLevel);
            //var ratioCoinInCome = (BigInteger)(playerData.playerZoo.buffRatioCoinInCome * 100);
            //price *= ratioCoinInCome;
            //price /= 100;
            PlaySceneMoneyMusic();
            AddCoin(price);
#endif
        }

        /// <summary>
        /// 售票口CD
        /// </summary>
        /// <param name="msg"></param>
        protected void OnEntryGateCheckGoToZoo(Message msg)
        {
#if NO_BIGINT
            PlaySceneMoneyMusic();
#else
            var price = EntryGateModule.GetEntryPrice();
            //var ratioCoinInCome = playerData.playerZoo.buffRatioCoinInCome * 100;
            //LogWarp.LogError("收益： OnSpawnVisitorCarLeaveZoo    " + price);
            PlaySceneMoneyMusic();
            AddCoin(price);
#endif
        }
        
        /// <summary>
        /// 播放场景音乐
        /// </summary>
        protected void PlaySceneMoneyMusic()
        {
            string btnSoundPath = Config.globalConfig.getInstace().SceneMoneyMusic;
            UFrame.MiniGame.SoundManager.GetInstance().PlaySound(btnSoundPath);
        }
    }
}

