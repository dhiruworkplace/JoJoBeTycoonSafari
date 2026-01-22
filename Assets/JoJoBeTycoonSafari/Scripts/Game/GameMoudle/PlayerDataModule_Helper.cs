using Game;
using Game.GlobalData;
using Logger;
using System.Collections;
using System.Collections.Generic;
using Game.MessageCenter;
using UFrame.MessageCenter;
using UnityEngine;
using UFrame;

namespace Game
{
    public partial class PlayerDataModule : GameModule
    {
        

        /// <summary>
        /// 购买动物
        /// </summary>
        private void BuyAnimal(SetAchievementObjectData msg)
        {
            /* 扣钱   扣钱成功后修改商品数量     发送扣钱的通知和商品修改的通知  */
            //todo 涉及金币减扣
            System.Numerics.BigInteger bigDelta = LittleZooModule.GetAnimalUpLevelPriceFormula(msg.goodsID);
            bool retCode = VaryDataCoin(bigDelta);
            if (!retCode) 
            {
                //string e = string.Format("购买动物扣钱失败");
                //throw new System.Exception(e);
                return;
            }
            
            //修改金币扣钱
            BroadcastValueOfPlayerData.Send((int)GameMessageDefine.BroadcastCoinOfPlayerData,
                0, 0, System.Numerics.BigInteger.Parse(this.playerData.playerZoo.coin), bigDelta);
            //增加动物数据
            Config.buildupCell cellBuildUp = Config.buildupConfig.getInstace().getCell(msg.littleZooID);

            var playerAnimal = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal;
            playerAnimal.SetPlayerAnimalLevelData(msg.goodsID);
            MessageInt.Send((int)GameMessageDefine.AnimalPlayLevelUpEffect, playerAnimal.GetAnimalEntityID(msg.goodsID));

            GetAddNewAnimalData.Send((int)GameMessageDefine.GetAnimalLevel, msg.goodsID, 1001);
        }

        /// <summary>
        /// 购买buff
        /// </summary>
        private static void BuyBuff(SetAchievementObjectData msg)
        {
            LogWarp.LogError("PlayerDataModule：进入购买 buff 的实现方法");
        }

        /// <summary>
        /// 购买礼包
        /// </summary>
        private static void BuyGift(SetAchievementObjectData msg)
        {
            LogWarp.LogError("PlayerDataModule：进入购买 礼包 的实现方法");
        }

        /// <summary>
        /// 购买钻石
        /// </summary>
        private static void BuyDiamond(SetAchievementObjectData msg)
        {
            LogWarp.LogError("PlayerDataModule：进入购买 钻石 的实现方法");
        }

        /// <summary>
        /// 购买现金
        /// </summary>
        private static void BuyCash(SetAchievementObjectData msg)
        {
            LogWarp.LogError("PlayerDataModule：进入购买 现金 的实现方法");
        }

        /// <summary>
        /// 给定一个等级范围数组，等级，返回等级所在范围数组的索引
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int FindLevelRangIndex(int[] levels, int level)
        {
            int idx = Const.Invalid_Int;
            if (level == 0)
            {
                return levels[0];
            }

            for (int j = 1; j < levels.Length; j++)
            {
                if (level <= levels[j] - 1)
                {
                    idx = j;
                    break;
                }
            }
            if (idx == Const.Invalid_Int)    //若idx超过数组范围则等于最大返回结果
            {
                idx = levels.Length - 1;
            }
            return idx;
        }


        /// <summary>
        /// 给定一个等级范围数组，等级，返回等级所在范围数组的索引
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int FindLevelRangIndex01(int[] levels, int level)
        {
            int idx = Const.Invalid_Int;
            if (level == 0)
            {
                return levels[0];
            }

            for (int j = 1; j < levels.Length; j++)
            {
                if (level < levels[j])
                {
                    idx = j-1;
                    break;
                }
            }
            if (idx == Const.Invalid_Int)    //若idx超过数组范围则等于最大返回结果
            {
                idx = levels.Length - 1;
            }
            return idx;
        }

    }
}
