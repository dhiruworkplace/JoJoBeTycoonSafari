using ZooGame.GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZooGame
{
    /// <summary>
    /// 动物加成   buff都没有做
    /// </summary>
    public partial class EntryGateModule : GameModule
    {
        
        public static float GetAllEntryChargeValMs()
        {
            int level = GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.entryTicketsLevel;
            return GetAllEntryChargeValMs(level);
        }
        /// <summary>
        /// 全部售票口的收费速度（返回的是分钟）
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static float GetAllEntryChargeValMs(int level)
        { /*  Y=售票口收费速度（1）+售票口收费速度（2）+售票口收费速度（3）……+售票口收费速度（8）  */
            float number = 0;
            var entryGateList = GlobalDataManager.GetInstance().playerData.playerZoo.entryGateList;
            foreach (var item in entryGateList)
            {
                number += GetCheckinSpeed(item.entryID, item.level);
            }
            return number;
        }
        /// <summary>
        /// 升级后全部售票口收费速度的变化值
        /// </summary>
        /// <param name="level"></param>
        /// <param name="changeNumber"></param>
        /// <returns></returns>
        public static float GetAllEntryChargeValMs(int level, int changeNumber)
        {
            var price = GetAllEntryChargeValMs(level + changeNumber) - GetAllEntryChargeValMs(level);
            return price;

        }
              

        /// <summary>
        /// 售票口收费速度（单）   Y=base+0.1*lv  
        /// </summary>
        /// <param name="baseVal"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static float GetCheckinSpeed(int entryID, int level)
        { /*      y=base+lv*0.1     */
            int baseVal = Config.ticketConfig.getInstace().getCell(entryID).speedbase;
            float number = level * 0.1f;
            return baseVal + number;
        }
        /// <summary>
        /// 售票口收费速度（单）升级变化值   Y=base+0.1*lv  
        /// </summary>
        /// <param name="entryID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static float GetCheckinSpeed(int entryID, int level, int changeNumber)
        {
            var number = GetCheckinSpeed(entryID, level+changeNumber) - GetCheckinSpeed(entryID, level);
            return number;
        }
        /// <summary>
        /// 售票口cd时间(单位秒)   Y=60/售票口收费速度（单）
        /// 转毫秒 * 1000
        /// </summary>
        /// <param name="entryID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetCheckinCDValMs(int entryID, int level)
        {
            return (int)(60  * 1000 / GetCheckinSpeed(entryID, level));
        }
        /// <summary>
        /// 升级收费速度的花费  Y=lv*10
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>


        /// <summary>
        /// 每分钟购票顾客升级消耗
        /// </summary>
        /// <param name="entryID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeCheckinSpeedConsumption(int entryID, int level)
        {   /*  Y=base*停售消耗基数【进客速度期望等级】*加成预期【进客速度期望等级】   */
            int isLevel = level;
            System.Numerics.BigInteger baseVal = System.Numerics.BigInteger.Parse(Config.ticketConfig.getInstace().getCell(entryID).lvupcastbase);
            level = GetUpVisitorSpeedExpectLevel(entryID, level);
            var baseConsumption = PlayerDataModule.GetUpGradeBaseConsumption(level);
            int number = (int)(PlayerDataModule.GetAdditionExpect(level) * 100);
            var price = baseVal * baseConsumption * number / 100;
            //Logger.LogWarp.LogErrorFormat("测试：baseVal={0}  ，baseConsumption={1} number={2}", baseVal, baseConsumption, PlayerDataModule.GetAdditionExpect(level));
            return price+ isLevel;
        }

        /// <summary>
        /// 售票口基础产出公式：Y=base*0.05*1.07^((lv*1.6)-1)
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetEntryBaseTollPrice(int level)
        {
            if (level<1)
            {
                //Logger.LogWarp.LogError("等级为0");
                level = 1;
            }
            int baseprice = Config.ticketConfig.getInstace().getCell(1).pricebase;
            int number = (int)(level * 1.6f) - 1;
            //1.07^((lv*1.6)-1)
            var numerator = System.Numerics.BigInteger.Parse("107");
            numerator = System.Numerics.BigInteger.Pow(numerator, number);
            var denominator = System.Numerics.BigInteger.Parse("100");
            denominator = System.Numerics.BigInteger.Pow(denominator, number);

            //base * 0.05 * numerator/(denominator*100)
            var price = numerator * baseprice*5 / (denominator*100);

            return price;
        }
        /// <summary>
        /// 售票口门票价格
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetEntryPrice(int level = -1)
        {   /* Y=base*0.05*1.07^((lv*1.6)-1)*BUFF加成   */
            /* Y=pricebase*1.07^((lv*1.6)-1)*BUFF加成*MIN(0.03*A3,1)    */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.entryTicketsLevel;
            }
            int baseVal = Config.ticketConfig.getInstace().getCell(1).pricebase;
            float number = Mathf.Min(0.03f*level,1);

            int number1 = GetInteger(level * 1.4f - 1);
            var numerator = System.Numerics.BigInteger.Parse("107");
            numerator = System.Numerics.BigInteger.Pow(numerator, number1);
            var denominator = System.Numerics.BigInteger.Parse("100");
            denominator = System.Numerics.BigInteger.Pow(denominator, number1);

            float number02 = PlayerDataModule.PlayerRatioCoinInComeAll();
            int number03 = (int)(number * number02 * 100);

            var price = baseVal*number03 * numerator / (denominator * 100);
            //Logger.LogWarp.LogErrorFormat("测试：售票口门票价格 number1={0}，number02={1} ,price={2}   level={3}", number1,number02, price,level);
            return price;
        }
        /// <summary>
        /// 获取升级规模后售票口票价变化  
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetEntryPrice(int level,int changeNumber)
        {
            var price = GetEntryPrice(level+ changeNumber) - GetEntryPrice(level);
            return price;
        }



        /// <summary>
        /// 售票口门票升级 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeConsumption(int level)
        {
            /*  Y=depbase*停售消耗基数【门票升级期望等级】*加成预期【门票升级期望等级】   */
            System.Numerics.BigInteger baseVal = System.Numerics.BigInteger.Parse(Config.ticketConfig.getInstace().getCell(1).depbase);
            level = GetUpTicketExpectLevel(0, level);
            var baseConsumption = PlayerDataModule.GetUpGradeBaseConsumption(level);
            int number = (int)(PlayerDataModule.GetAdditionExpect(level)*100);
            var price = baseVal * baseConsumption * number / 100;
            return price;
        }
        /// <summary>
        /// 获取售票口门票 升级规模的花费
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="number">等级变化</param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeConsumption(int level,int changeNumber)
        {
            var price = GetUpGradeConsumption(level);
            for (int i = 0; i < changeNumber; i++)
            {
                price = price + GetUpGradeConsumption(level + i);
            }
            return price;
        }


        /// <summary>
        /// 售票口收费口增加价格
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetAddEntryPrice(int entryID, int level)
        {   /*  Y=base  */
            string str = Config.ticketConfig.getInstace().getCell(entryID).number;
            System.Numerics.BigInteger bigInteger = System.Numerics.BigInteger.Parse(str);
            return bigInteger;
        }

        /// <summary>
        /// 进客速度期望等级
        /// </summary>
        /// <returns></returns>
        public static int GetUpVisitorSpeedExpectLevel(int entryID, int level)
        {   //Y=（MAX((lv-5),0)*(1+lvratio2)+(0.5+lv*0.1)*lvratio1）
            float baseVal = Config.ticketConfig.getInstace().getCell(entryID).lvratio;
            int baseVal1 = Config.ticketConfig.getInstace().getCell(entryID).lvratio1;

            float number01 = Mathf.Max(level - 5, 0) * (1 + baseVal);
            float number02 = Mathf.Min((0.5f + level * 0.1f),1) * baseVal1;
            float number03 = number01 + number02;
            return GetInteger( number03);
        }
        /// <summary>
        /// 门票升级期望等级
        /// </summary>
        /// <returns></returns>
        public static int GetUpTicketExpectLevel(int entryID, int level)
        {   /*      Y=lv *1.4f       */
            return (int)(level*1.4f);
        }

        /// <summary>
        /// 获取一个浮点数的整数，向上取整
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        static int GetInteger(float number)
        {
            int number1 = (int)(number + 0.999999f);
            return number1;
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        private static int text001(string str)
        {
            return 0;
        }

    }
}
