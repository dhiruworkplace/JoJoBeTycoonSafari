using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Game.GlobalData;
using Logger;
namespace Game
{
    public partial class ParkingCenter : GameModule
    {
        /// <summary>
        /// 停车场每分钟招揽游客速度
        /// </summary>
        /// <param name="level">停车场每分钟招揽游客等级(流量等级)</param>
        /// <returns></returns>
        public static int GetParkingEnterCarSpawn(int level = -1)
        {   /*  Y=base+lv*1   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingEnterCarSpawnLevel;
            }
            int baseVal = Config.parkingConfig.getInstace().getCell(1).touristbase;
            int number = baseVal + level * 1;

            return number;
        }
        /// <summary>
        /// (每分钟)停车场来人速度  返回升级多数后增加的客流量
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="getUpLevel">变换等级</param>
        /// <returns></returns>
        public static int GetParkingEnterCarSpawn(int level, int getUpLevel)
        {
            return GetParkingEnterCarSpawn(level + getUpLevel) - GetParkingEnterCarSpawn(level);
        }

        /// <summary>
        /// 每分钟招揽游客升级消耗
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeEnterCarSpawnConsumption(int level = -1)
        {   /*   Y=base*停售消耗基数【来客速度期望等级】*加成预期【来客速度期望等级】   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingEnterCarSpawnLevel;
            }
            level = ParkingEnterCarSpawnExpectLevel(level);
            string str = Config.parkingConfig.getInstace().getCell(1).touristcastbase;
            System.Numerics.BigInteger baseVal = System.Numerics.BigInteger.Parse(str);
            var baseConsumption = PlayerDataModule.GetUpGradeBaseConsumption(level);
            int number = (int)(PlayerDataModule.GetAdditionExpect(level) * 100);

            var price = baseVal * baseConsumption * number / 100;
            return price;
        }


        /// <summary>
        /// 停车场停车位数量最大位置
        /// </summary>
        /// <param name="level">停车场停车位等级</param>
        /// <returns></returns>
        public static int GetParkingSpace(int level = -1)
        {   /*  Y=base+lv*6   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingSpaceLevel;
            }
            int baseVal = Config.parkingConfig.getInstace().getCell(1).spacebase;
            int number = baseVal + level * 6;
            return number;
        }
        /// <summary>
        /// 停车场最大位置 返回升级多数后增加的停车位数量
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="getUpLevel">变换值</param>
        /// <returns></returns>
        public static int GetParkingSpace(int level, int getUpLevel)
        {
            return GetParkingSpace(level + getUpLevel) - GetParkingSpace(level);
        }
        /// <summary>
        /// 停车位数量最大位置升级消耗
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeNumberConsumption(int level = -1)
        {   /*   Y=base*停售消耗基数【停车位数期望等级】*加成预期【停车位数期望等级】   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingSpaceLevel;
            }
            level = ParkingSpaceExpectLevel(level);
            string str = Config.parkingConfig.getInstace().getCell(1).spaceupcastbase;
            System.Numerics.BigInteger baseVal = System.Numerics.BigInteger.Parse(str);
            var baseConsumption = PlayerDataModule.GetUpGradeBaseConsumption(level);
            int number = (int)(PlayerDataModule.GetAdditionExpect(level) * 100);

            var price = baseVal * baseConsumption * number / 100;
            return price;
        }


        /// <summary>
        /// 停车场利润提升   返回值扩大100倍
        /// </summary>
        /// <param name="level">停车场利润等级</param>
        /// <returns></returns>
        public static int GetParkingProfit(int level = -1)
        {   /*  Y=0.05*lv   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingProfitLevel;
            }
            int number = 5 * level;
            return number;
        }
        /// <summary>
        /// 利润 升级若干规模的变化值
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="BasicPrice">等级变换</param>
        /// <returns></returns>
        public static int GetParkingProfit(int level, int getUpLevel)
        {
            int price;
            price = GetParkingProfit(level + getUpLevel) - GetParkingProfit(level);
            return price;
        }

        /// <summary>
        /// 停车场利润升级消耗
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeParkingProfitConsumption(int level = -1)
        {   /*   Y=base*停售消耗基数【停车位数期望等级】*加成预期【停车位数期望等级】   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingProfitLevel;
            }
            level = ParkingProfitExpectLevel(level);
            string str = Config.parkingConfig.getInstace().getCell(1).depletebase;
            System.Numerics.BigInteger baseVal = System.Numerics.BigInteger.Parse(str);
            var baseConsumption = PlayerDataModule.GetUpGradeBaseConsumption(level);
            int number = (int)(PlayerDataModule.GetAdditionExpect(level) * 100);

            var price = baseVal * baseConsumption * number / 100;
            return price;
        }
        /// <summary>
        /// 停车场升级花费  升级若干规模的变化值   
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="getUpLevel">升级规模的数量</param>
        /// <returns></returns>
        public static BigInteger GetUpGradeParkingProfitConsumption(int level, int changeNumber)
        {
            var price = System.Numerics.BigInteger.Parse("0");
            for (int i = 0; i < changeNumber; i++)
            {
                price = price + GetUpGradeParkingProfitConsumption(level + i);
            }
            return price;
        }





        /// <summary>
        /// 地面停车场的停车位数量
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetGroundParkingNumber(int level=-1)
        {
            /*  Y=min（base+lv*6，48）   */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingSpaceLevel;
            }
            int baseVal = Config.parkingConfig.getInstace().getCell(1).spacebase;
            int number01 = Config.globalConfig.getInstace().NumGroundParkingGroupSpace * Config.globalConfig.getInstace().MaxNumGroundParkingGroup;
            int number = Mathf.Min(baseVal + level * 6, number01);
            return number;
        }

        /// <summary>
        /// 来客速度期望等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int ParkingEnterCarSpawnExpectLevel(int level = -1)
        {  
            /* Y=MIN(1+lv*0.4,10)+MIN(MAX((lv-8)*0.64,0),10)+MIN(MAX((lv-18)*1.6,0),16)+MIN(MAX((lv-28)*2.56,0),28.8)+MAX((lv-30.4)*5.36,0)     */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingEnterCarSpawnLevel;
            }
            float number01 = Mathf.Min(1+level*0.4f, 10f);
            float number02 = Mathf.Max((level - 8) * 0.64f,0);
            float number03 = Mathf.Min(number02, 10);
            float number04 = Mathf.Max((level - 18) * 1.6f, 0);
            float number05 = Mathf.Min(number04, 16);
            float number06 = Mathf.Max((level - 28) * 2.56f, 0);
            float number07 = Mathf.Min(number06, 28.8f);
            float number08 = Mathf.Max((level - 30.4f) * 5.36f, 0);

            return (int)(number01 + number03 + number05 + number07 + number08);
        }
        /// <summary>
        /// 停车位数期望等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int ParkingSpaceExpectLevel(int level = -1)
        {  
            /*  Y=（MIN(lv^1.6,70)+MAX((lv-15)*20,0）)*1.4 */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingSpaceLevel;
            }
            float number = Mathf.Min(Mathf.Pow(level, 1.6f), 70) + Mathf.Max((level - 15) * 20, 0);

            //Logger.LogWarp.LogErrorFormat(" {0}    {1}    {2}  ", level, Mathf.Pow(level, 1.6f), number * 1.5f);

            return (int)(number * 1.4f);
        }
        /// <summary>
        /// 利润提升期望等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int ParkingProfitExpectLevel(int level = -1)
        {   /*  Y=lv*1.5 */
            if (level == -1)
            {
                level = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingProfitLevel;
            }
            return (int)(level * 1.4f);
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


    }
}
