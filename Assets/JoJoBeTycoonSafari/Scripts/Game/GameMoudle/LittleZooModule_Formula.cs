using ZooGame.GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZooGame
{
    public partial class LittleZooModule : GameModule
    {
        /// <summary>
        /// 动物期望等级
        /// </summary>
        /// <param name="animalID"></param>
        /// <returns></returns>
        public static int GetAnimalExpectLevel(int animalID)
        { // Y = animallvbase+Dvalue
            int animallvbase = Config.animalupConfig.getInstace().getCell(animalID).animallvbase;
            float dvalue = Config.animalupConfig.getInstace().getCell(animalID).Dvalue;
            int animalLevel = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal.getPlayerAnimalCell(animalID).animalLevel;
            return (int)(animallvbase + dvalue*(animalLevel - 1));
        }

        /// <summary>
        /// 动物升级消耗价格   Y=base*1.06^（lvsum-1）
        /// </summary>
        /// <param name="number">购买数量</param>
        public static System.Numerics.BigInteger GetAnimalUpLevelPriceFormula(int animalID)
        {//Y=AnimalPriceBase*动物消耗基数【动物期望等级】*加成预期【动物期望等级】
            int level = GetAnimalExpectLevel(animalID);
            int AnimalPriceBase = Config.globalConfig.getInstace().AnimalPriceBase;
            System.Numerics.BigInteger upGradeBaseConsumption = GetUpGradeBaseConsumption(level);
            float additionExpect = PlayerDataModule.GetAdditionExpect(level);

            System.Numerics.BigInteger big = (int)(AnimalPriceBase * additionExpect*100)* upGradeBaseConsumption / 100 ;
            return big;
        }

        /// <summary>
        /// 动物栏产出基数
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetLittleZooBaseTollPrice(int littleZooID, int level)
        {   /*  Y=1.07^(lv-1)  */
       
            int number1 = level - 1;
            if (number1 >0)
            {
                var numerator = System.Numerics.BigInteger.Parse("107");
                numerator = System.Numerics.BigInteger.Pow(numerator, number1);
                var denominator = System.Numerics.BigInteger.Parse("100");
                denominator = System.Numerics.BigInteger.Pow(denominator, number1);

                var number2 = numerator / denominator;
                
                //Logger.LogWarp.LogErrorFormat("测试： numerator={0},  denominator{1},  number2={2}", numerator, denominator, number2);
                return number2;
            }
            else
            {
                var number2 = 1;
                return number2;
            }

            
        }


        /// <summary>
        /// 动物栏门票：  
        /// </summary>
        /// <param name="level">动物栏等级</param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetLittleZooPrice(int littleZooID,int level)
        {   /*      Y=pricebase*动物栏产出基数*BUFF加成 +MIN(5*A3,500)    */
            System.Numerics.BigInteger baseVal = System.Numerics.BigInteger.Parse( Config.buildupConfig.getInstace().getCell(littleZooID).pricebase);
            System.Numerics.BigInteger price = GetLittleZooBaseTollPrice(littleZooID,level);
            int number1 = (int)(PlayerDataModule.PlayerRatioCoinInComeAll() * 100);
            //Logger.LogWarp.LogErrorFormat("测试：price={0}    baseVal={1}     number1={2}", price, baseVal, number1);

            price = (price* baseVal * number1) / 100;
            //Logger.LogWarp.LogErrorFormat("测试：price={0}    baseVal={1}     number1={2}", price, baseVal, number1);

            return price+Mathf.Min(5*level,500);
        }
        /// <summary>
        /// 动物栏门票   返回的是多次升级变化值
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="allbase">初始产出</param>
        /// <param name="getUpLevel">升级规模</param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetLittleZooPrice(int littleZooID,int level, int changeNumber)
        {
            return GetLittleZooPrice(littleZooID,level + changeNumber) - GetLittleZooPrice(littleZooID,level);
        }

        /// <summary>
        /// 动物栏升级消耗基数   
        /// </summary>
        /// <param name="littleZooID">动物栏ID</param>
        /// <param name="level">初始基数</param>   
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeBaseConsumption(int level)
        {           /*   Y=1.07^(lv-1)*IF(lv<=1000,0.5*lv^1.01,0.5*1000^1.01+lv/1000)+1  */
            //
            var numerator = System.Numerics.BigInteger.Parse("1");
            var denominator = System.Numerics.BigInteger.Parse("1");
            //1.07 ^ (lv - 1)
            if (level > 1)
            { 
                numerator = System.Numerics.BigInteger.Parse("107");
                numerator = System.Numerics.BigInteger.Pow(numerator, level - 1);
                denominator = System.Numerics.BigInteger.Parse("100");
                denominator = System.Numerics.BigInteger.Pow(denominator, level - 1);
            }

            //IF(lv<=1000,0.5*lv^1.01,0.5*1000^1.01+lv/1000)+1  */
            float number = 1f ;
            if (level <= 1000)
            {   /*0.5*lv^1.01*/
                number = 0.5f * Mathf.Pow(level,1.01f);
            }
            else
            {   /*0.5*1000^1.01+lv/1000)+1*/
                number = 0.5f * Mathf.Pow(1000, 1.01f)+level/1000f+1;
            }
            var price = (int)(number*100) * numerator / (100* denominator);
            //Logger.LogWarp.LogErrorFormat("测试：   number={0},number3={1},numerator/denominator={2}  ", number,number3,numerator/denominator);
            return price+1;
        }


        /// <summary>
        /// 动物栏门票价格升级消耗
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeConsumption(int littleZooID, int level)
        {   /*  Y=castbase*动物消耗基数【动物栏门票期望等级】*加成预期【动物栏门票期望等级】 *MIN(0.2*lv,1) */
            int isLevel = level;
            System.Numerics.BigInteger allbase = System.Numerics.BigInteger.Parse(Config.buildupConfig.getInstace().getCell(littleZooID).castbase);

            level = GetUpLittleZooPriceExpectLevel(littleZooID, level);

            System.Numerics.BigInteger upGradeBaseConsumption = GetUpGradeBaseConsumption( level);

            float additionExpect = PlayerDataModule.GetAdditionExpect(level);
            float number2 = Mathf.Min(0.2f * isLevel, 1);
            int number1 = (int)(additionExpect * number2 * 100);

            System.Numerics.BigInteger price = upGradeBaseConsumption * allbase * number1 / 100;
            //Logger.LogWarp.LogErrorFormat("测试：   upGradeBaseConsumption={0} allbase={1} additionExpect={2}", upGradeBaseConsumption , allbase , additionExpect);
            return price;
        }


        /// <summary>
        /// 当前动物栏开启的观光点数量   
        /// </summary>
        /// <returns></returns>
        public static int OpenVisitPosNumber(int littleZooID, int littleZooLevel)
        {   /*      Y=watchbase+lv-1            */
            int baseVal = Config.buildupConfig.getInstace().getCell(littleZooID).watchbase;
            int number = baseVal + littleZooLevel - 1;

            return number;
        }

        /// <summary>
        /// 当前动物栏的观光点开启数量  返回的是变化值
        /// </summary>
        /// <param name="littleZooLevel"></param>
        /// <param name="getUpLevel"></param>
        /// <returns></returns>
        public static int OpenVisitPosNumber(int littleZooID, int littleZooLevel, int getUpLevel)
        {
            int number = OpenVisitPosNumber(littleZooID,littleZooLevel + getUpLevel) - OpenVisitPosNumber(littleZooID,littleZooLevel);
            return number;
        }

        /// <summary>
        /// 当前动物栏的观光点数量升级花费
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeVisitorLocationLevelConsumption(int littleZooID, int level)
        {   /* Y=watchupbase*期望等级消耗基数【观光点期望等级】*加成预期【观光点期望等级】 */
            string baseVal = Config.buildupConfig.getInstace().getCell(littleZooID).watchupbase;
            level = GetUpGradeVisitorLocationExpectLevel(littleZooID, level);
            System.Numerics.BigInteger number01 = System.Numerics.BigInteger.Parse(baseVal);
            var number02 = GetUpGradeBaseConsumption(level);
            int number03 = (int)(PlayerDataModule.GetAdditionExpect(level) *100);
            var price = number01 * number02 * number03/100;
            return price;
        }


        /// <summary>
        /// 观光速率  
        /// </summary>
        /// <param name="littleZooID">动物栏ID</param>
        /// <param name="level">等级</param>
        /// <returns></returns>
        public static float GetVisitDurationMS(int littleZooID, int level)
        {   /*  Y=timebase+（lv-1）*base/20    */
            int allbase = Config.buildupConfig.getInstace().getCell(littleZooID).timebase;
            float fDuration = allbase+ (level - 1)*allbase/20f;
            return fDuration;
        }
        /// <summary>
        /// 观光速率   返回的是变化值
        /// </summary>
        /// <param name="littleZooID">动物栏ID</param>
        /// <param name="level"></param>
        /// <param name="getUpLeve"></param>
        /// <returns></returns>
        public static float GetVisitDurationMS(int littleZooID,int level, int getUpLeve)
        {
            return GetVisitDurationMS(littleZooID,level + getUpLeve) - GetVisitDurationMS(littleZooID,level);
        }
        /// <summary>
        /// 当前动物栏的观光游客数量升级花费
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetUpGradeEnterVisitorSpawnLevelConsumption(int littleZooID, int level)
        {
            // Y=timeupbase*期望等级消耗基数【动物栏速率期望等级】*加成预期【动物栏速率期望等级】
            string baseVal = Config.buildupConfig.getInstace().getCell(littleZooID).timeupbase;
            System.Numerics.BigInteger number01 = System.Numerics.BigInteger.Parse(baseVal);
            level = GetUpVisitDurationExpectLevel(littleZooID, level);
            var number02 = GetUpGradeBaseConsumption(level);
            int number03 = (int)(PlayerDataModule.GetAdditionExpect(level) * 100);
            var price = number01 * number02 * number03 / 100;
            return price;
        }


        /// <summary>
        /// 观光CD时间   Y=60/观光速率
        /// </summary>
        /// <returns></returns>
        public static float GetComeVisitorSpeedCD(int littleZooID, int level)
        {
            //            return (int)(60f/GetVisitDurationMS(littleZooID,level))*1000;
            var cdTime = 60 / GetVisitDurationMS(littleZooID,level) *1000;
            return (int)cdTime;
        }

        /// <summary>
        /// 动物栏展示每分钟观光游客数量  Y=观光速率*观光点数
        /// </summary>
        /// <returns></returns>
        public static float GetLittleZooVisitorNumberMS(int littleZooID, LittleZooModuleData littleZooModuleData)
        {
            var visitorNumber = GetVisitDurationMS(littleZooID, littleZooModuleData.littleZooEnterVisitorSpawnLevel) * OpenVisitPosNumber(littleZooID, littleZooModuleData.littleZooVisitorSeatLevel);
            return visitorNumber;
        }

        /// <summary>
        /// 所有动物Buff加成
        /// </summary>
        public static float GetAllAnimalsBuff()
        {
            //Y = 拥有的动物数量 * 2 + 动物总等级 / 100
            int animaleAllNumber = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal.playerAnimalsNumber*2;
            int animaleAlllevel = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal.playerAllAnimalsLevel;
            return animaleAllNumber+animaleAlllevel / 100f;
        }

        /// <summary>
        /// 单个动物加成算法
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetAnimalsBuff(int level)
        {   /*   Y=2+（动物等级-1）*10%   */
            if (level ==0)
            {
                level = 1;
            }
            return 200 + (level-1)*10;
        }

        /// <summary>
        /// 动物栏开启展示收益  Y=初始产出*观光速率
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetlittleZooShowExpenditure(int littleZooID, int level)
        {
            var number = System.Numerics.BigInteger.Parse( Config.buildupConfig.getInstace().getCell(littleZooID).pricebase);
            var visitorNumber = number * (int)GetVisitDurationMS(littleZooID, level);
            return visitorNumber;
        }

        /// <summary>
        /// 观光点期望等级
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetUpGradeVisitorLocationExpectLevel(int littleZooID, int level)
        {   /*      y=MAX((lv-2)*25,10)+动物栏系数     */
            int littleZooCoefficient = Config.buildupConfig.getInstace().getCell(littleZooID).lvcoefficient;
            int expectLevel = Mathf.Max((level - 2) * 25, 10) + littleZooCoefficient;
            return expectLevel;
        }
        /// <summary>
        /// 动物栏速率期望等级
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetUpVisitDurationExpectLevel(int littleZooID, int level)
        {   /*      y=MAX((lv-2)*25,10)+动物栏系数     */
            int littleZooCoefficient = Config.buildupConfig.getInstace().getCell(littleZooID).lvcoefficient;
            int expectLevel = Mathf.Max((level - 1) * 12, 5) + littleZooCoefficient;
            return expectLevel;
        }
        /// <summary>
        /// 动物栏门票期望等级
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int GetUpLittleZooPriceExpectLevel(int littleZooID, int level)
        {   /*  Y=lv+动物栏系数  */
            int littleZooCoefficient = Config.buildupConfig.getInstace().getCell(littleZooID).lvcoefficient;
            return littleZooCoefficient + level;
        }





        /// <summary>
        /// 获取一个浮点数的整数，向上取整
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        static int GetInteger(float number)
        {
            int number1 = (int)(number+0.999999f);
            return number1;
        }
        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        static int GetIntRoundingOff(float number)
        {
            float number01 = number + 0.5f;
            return (int)number01;
        }

    }
}
