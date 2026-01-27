using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using ZooGame.GlobalData;
using Logger;

namespace ZooGame
{
    public partial class ExitGateModule : GameModule
    {
        
        /// <summary>
        /// 乘车口数量  返回初始数量
        /// </summary>
        /// <returns></returns>
        public static int GetEntryNum()
        {
            int level = GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.exitGateLevel;

            return GetEntryNum(level);
        }
        /// <summary>
        /// 乘车口数量    Y=min(1+int（lv/100）,8)
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetEntryNum(int level)
        {
#if DEBUG_VISIT
            return Mathf.Clamp(level, 1, 8);
#else
            return UnityEngine.Mathf.Min(level / 100 +1, 8);
#endif
        }
        /// <summary>
        /// 乘车口数量  返回升级多数后增加的乘车口数量
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="getUpLevel">升级规模</param>
        /// <returns></returns>
        public static int GetEntryNum(int level, int getUpLevel)
        {
            int number = GetEntryNum(level+getUpLevel)- GetEntryNum(level);
            return number;
        }

        /// <summary>
        /// 出口速度  返回当前等级的出口速度
        /// </summary>
        /// <returns></returns>
        public static int GetChinkinCDValMs()
        {
            int level = GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.exitGateLevel;
            return GetChinkinCDValMs(level);
        }
        /// <summary>
        /// 出口速度   checkin时间(毫秒)
        /// 所有出口都是一个值   Y=base-INT((lv-50)/100)/19*0.8*base
        /// Y=5-int(lv/50)*0.1
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetChinkinCDValMs(int level)
        {
            int cell = Config.exitgateConfig.getInstace().getCell(1).speed *1000;
            int number =(int) ((level - 50) * cell * 0.8) / 1900;
            //页面用换成毫秒
            return cell-number; 
        }
        /// <summary>
        /// 出口速度  返回升级多数后增加的出口速度
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="getUpLevel">升级规模</param>
        /// <returns></returns>
        public static int GetChinkinCDValMs(int level,int getUpLevel)
        {
            int number = GetChinkinCDValMs(level+getUpLevel);
            int number1 = GetChinkinCDValMs(level);

            return number - number1;
        }
        

        /// <summary>
        /// 出口大巴票价
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetExitPrice()
        {
            int level = GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.exitGateLevel;
            return GetExitPrice(level);
        }
        /// <summary>
        /// 出口票价   Y=base*(1+0.06)^(round(lv*1.5，0）-1)
        /// </summary>
        public static BigInteger GetExitPrice(int level)
        {
            //基础票价
            int baseprice = Config.exitgateConfig.getInstace().getCell(1).baseprice;

            int number = RoundToInt(level*1.5f)-1;
            //分子
            var numerator = BigInteger.Parse("106");
            numerator = BigInteger.Pow(numerator, number);
            //分母
            var denominator = BigInteger.Parse("100");
            denominator = BigInteger.Pow(denominator, number);

            var price = numerator*baseprice / denominator;
            return price;
        }
        /// <summary>
        /// 出口大巴票价  升级若干规模的变化值
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="getUpLevel">升级规模的数量</param>
        /// <returns></returns>
        public static BigInteger GetExitPrice(int level, int getUpLevel)
        {
            BigInteger price;
            price = GetExitPrice(level + getUpLevel) - GetExitPrice(level);
            return price;
        }

        public static BigInteger GetUpGradeConsumption()
        {
            int level = GlobalData.GlobalDataManager.GetInstance().playerData.playerZoo.exitGateLevel;
            return GetUpGradeConsumption(level);
        }
        /// <summary>
        /// 出口升级花费      Y=base*(1+0.06)^(round(lv*1.5，0）-1)*    2^(lv/100)   *   1.5^(lv*1.5/100)/5*lv*1.5
        /// Y = base*((10+  num1+(lv-1)*lv/40)/(5-INT(lv/25)/10)*(1+MIN(1,INT(lv/50))+INT(lv/100)))
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static BigInteger GetUpGradeConsumption(int level)
        {
#if DEBUG_VISIT
            return 0;
#endif
            //int baseVal = Config.parkingConfig.getInstace().getCell(1).deplete;
            //int key = 15;  //需要除以10
            //baseVal = baseVal * level * key / 10;
            //int number1 = level - 1;
            //int number2 = level / 100;
            //var big1 = System.Numerics.BigInteger.Parse("106");
            //var big2 = System.Numerics.BigInteger.Parse("30");
            //var big3 = System.Numerics.BigInteger.Parse("10");

            //var price = baseVal * System.Numerics.BigInteger.Pow(big1, number1) * System.Numerics.BigInteger.Pow(big2, number2) / (System.Numerics.BigInteger.Pow(big3, 2 * number1 + number2) * 5);

            return 0;
        }
        /// <summary>
        /// 出口升级若干规模的变化值   暂时写死为
        /// </summary>
        /// <param name="level">当前等级</param>
        /// <param name="getUpLevel">升级规模的数量</param>
        /// <returns></returns>
        public static BigInteger GetUpGradeConsumption(int level, int getUpLevel)
        {
            /*假写*/
            BigInteger price;
            if (getUpLevel < 101)
            {
                price = GetUpGradeConsumption(level + getUpLevel);
                for (int i = 0; i < getUpLevel; i++)
                {
                    price += GetUpGradeConsumption(level + i);
                }
                return price;
            }
            else
            {
                for (int i = 0; i < getUpLevel; i++)
                {
                    price += BigInteger.Parse("100");
                }
                return price;
            }
        }


        public static int RoundToInt(double number)
        {
            return (int)(double)(number + 0.5);
            
        }
        /// <summary>
        /// 乘车人数
        /// </summary>
        /// <returns></returns>
        public static int GetMaxShuttleVisitor()
        {
            var playData = GlobalDataManager.GetInstance().playerData;
            if (playData.playerZoo.isGuide)
            {
                return 1;
            }
            return Config.globalConfig.getInstace().MaxShuttleVisitor;
        }



    }   

}
