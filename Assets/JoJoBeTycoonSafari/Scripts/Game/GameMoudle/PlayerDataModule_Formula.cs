using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Game.GlobalData;
using Logger;
namespace Game
{
    public partial class PlayerDataModule : GameModule
    {
        private static readonly int length;

        /// <summary>
        /// 所有动物栏收益和：动物栏收益 =（停车场来人速度，售票口收费速度（全），60/观光速度* 观光点数）三值取最小后* 动物栏产出
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetAllZooPrice()
        {
            int parkNumber = ParkingCenter.GetParkingEnterCarSpawn();
            float entryGateNumber = EntryGateModule.GetAllEntryChargeValMs();
            System.Numerics.BigInteger littleZooNumber = 0;

            PlayerData playerData = GlobalDataManager.GetInstance().playerData;
            var littleZooModuleDatas = playerData.playerZoo.littleZooModuleDatas;
            for (int i = 0; i < littleZooModuleDatas.Count; i++)
            {
                if (littleZooModuleDatas[i].littleZooTicketsLevel == 0)
                {
                    break;
                }
                //每分钟动物栏基础收益=动物栏产出*min（停车场来人速度，售票口收费速度（全），60/(观光速度*观光点数)）
                //动物栏产出
                var number1 = LittleZooModule.GetLittleZooPrice(littleZooModuleDatas[i].littleZooID, littleZooModuleDatas[i].littleZooTicketsLevel);
                //停车场来人速度
                var number2 = ParkingCenter.GetParkingEnterCarSpawn();
                //售票口收费速度
                var number3 = EntryGateModule.GetAllEntryChargeValMs();
                //观光速度*观光点数
                var number4 = LittleZooModule.GetLittleZooVisitorNumberMS(littleZooModuleDatas[i].littleZooID, littleZooModuleDatas[i]);
                var number5 = (int)(Mathf.Min(number2, number3, number4)) * number1;
                littleZooNumber += number5;

            }
            return littleZooNumber;
        }


        /// <summary>
        /// 每分钟产出      
        /// 所有动物栏收益+售票口票价*min（停车场来人速度，售票口收费速度（全））
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger LeaveEarnings()
        {
            //所有动物栏收益
            var allZooPrice = GetAllZooPrice();
            //每分钟售票口基础收益=售票口票价*min（停车场来人速度，售票口收费速度（全））
            //售票口票价：
            var entryPrice = EntryGateModule.GetEntryPrice();
            //min（停车场来人速度，售票口收费速度（全））
            var number = Mathf.Min(ParkingCenter.GetParkingEnterCarSpawn(), EntryGateModule.GetAllEntryChargeValMs());
            //所有动物栏收益+售票口票价*min（停车场来人速度，售票口收费速度（全））
            System.Numerics.BigInteger coin = allZooPrice + (entryPrice * (int)(number * 100)) / 100;
            //Logger.LogWarp.LogErrorFormat("测试：   每分钟 收益    allZooPrice={0},    entryPrice={1},    number={2}", allZooPrice ,entryPrice ,number );
            return coin;
        }

        /// <summary>
        /// 计算离线收益（包括buff等计算）
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger OnGetCalculateOfflineSecondCoin()
        {
            //离线时间
            double offlineTime = GlobalDataManager.GetInstance().playerData.GetOfflineSecond();
            //获取离线Buff的列表
            List<Buff> offlineBuffList = GlobalDataManager.GetInstance().playerData.playerZoo.offlineBuffList;
            //离线buff按照时间的长短排序
            var orderBuffList = OnGetBuffCoefficient(offlineBuffList);
            //每秒收益
            var baseEarnings = LeaveEarnings() / ((int)PlayerRatioCoinInComeAll() * 60);
            var parce01 = System.Numerics.BigInteger.Parse("0");
            var parce02 = System.Numerics.BigInteger.Parse("0");
            
            if (orderBuffList.Count > 0)
            {
                if (offlineTime >= orderBuffList[orderBuffList.Count - 1].CD.org)
                {
                    parce02 = (int)(offlineTime - orderBuffList[orderBuffList.Count - 1].CD.org) * baseEarnings;
                }
                parce01 = OnCalculateCoin(baseEarnings, orderBuffList);
                return parce01 + parce02;
            }
            else
            {
                parce01 = LeaveEarnings() * (int)offlineTime;
                return parce01;
            }
        }

        /// <summary>
        /// 根据离线buff的时间进行排列
        /// </summary>
        public static List<Buff> OnGetBuffCoefficient(List<Buff> offlineBuffList)
        {
            for (int j = 1; j <= offlineBuffList.Count - 1; j++)//外层for循环用来控制子for循环执行的次数
            {
                //让下面的for循环执行length-1次
                for (int i = 0; i < offlineBuffList.Count - 1 - j + 1; i++)
                {
                    //numArray[i]  numArray[i+1]做比较 把最大的放在后面
                    if (offlineBuffList[i + 1].CD.org < offlineBuffList[i].CD.org)
                    {
                        var temp = offlineBuffList[i];
                        offlineBuffList[i] = offlineBuffList[i + 1];
                        offlineBuffList[i + 1] = temp;
                    }
                }
            }
            return offlineBuffList;
        }
        public static System.Numerics.BigInteger OnCalculateCoin(System.Numerics.BigInteger big, List<Buff> offlineBuffList, double isDouble = 0)
        {
            //时间 秒
            var time = offlineBuffList[0].CD.org - isDouble;
            //收益= time* 每分钟实际收益* buff加成
            /*   去除buff加成的每分钟实际收益: 所有动物栏收益+售票口票价*min（停车场来人速度，售票口收费速度（全）） */
            big = (int)(time * PlayerRatioCoinInComeAll_Calculate(offlineBuffList)) * big;
            offlineBuffList.RemoveAt(0);
            if (offlineBuffList.Count == 0)
            {
                return big;
            }
            else
            {
                OnCalculateCoin(big, offlineBuffList, time);
                return big;
            }
        }
        public static float PlayerRatioCoinInComeAll_Calculate(List<Buff> offlineBuffList)
        {
            float buffRatioCoinInComeAdd = 1f;
            float buffRatioCoinInComeMul = 1f;
            for (int i = 0; i < offlineBuffList.Count; i++)
            {
                Buff buff = offlineBuffList[i];
                switch (buff.buffType)
                {
                    case BuffType.RatioCoinInComeAdd:
                        buffRatioCoinInComeAdd += buff.buffVal;
                        //Logger.LogWarp.LogError("测试：   buffRatioCoinInCome= "+ buffRatioCoinInComeAdd+ "   buff.buffVal= "+ buff.buffVal);
                        break;
                    case BuffType.RatioCoinInComeMul:
                        buffRatioCoinInComeMul *= buff.buffVal;
                        //Logger.LogWarp.LogError("测试：   buffRatioCoinInCome= " + buffRatioCoinInComeAdd + "   buff.buffVal= " + buff.buffVal);
                        break;
                }
            }

            float number = LittleZooModule.GetAllAnimalsBuff() + ParkingCenter.GetParkingProfit() / 100f + buffRatioCoinInComeAdd;
            return number * buffRatioCoinInComeMul;
        }

        /// <summary>
        /// 轮船过来的游客数量
        /// </summary>
        /// <returns></returns>
        public static int SteameVisitorNameber()
        {
            int baseVal = UnityEngine.Random.Range(Config.globalConfig.getInstace().AdvertVisitorMin, Config.globalConfig.getInstace().AdvertVisitorMax);

            PlayerData playerData = GlobalDataManager.GetInstance().playerData;
            int entryLevel = playerData.playerZoo.entryTicketsLevel;
            int numberVisitor = baseVal * (1 + entryLevel / 500);
            //LogWarp.LogError("测试：轮船游客 "+ baseVal+ "  entryLevel="+ entryLevel+ "  numberVisitor="+ numberVisitor);
            return numberVisitor;
        }

        /// <summary>
        /// 玩家所有产出需要相加的倍数值  y=1+动物加成+停车场利润加成+道具BUFF
        /// </summary>
        /// <returns></returns>
        public static float PlayerRatioCoinInComeAdd()
        {
            float number = LittleZooModule.GetAllAnimalsBuff() + ParkingCenter.GetParkingProfit() / 100f + GlobalDataManager.GetInstance().playerData.playerZoo.buffRatioCoinInComeAdd;
            //Logger.LogWarp.LogErrorFormat("测试： 相加的倍数值为：{0}，其中动物加成={1}，停车场利润加成={2}，相加buff加成={3}",
            //    number, LittleZooModule.GetAnimalsBuff(), (ParkingCenter.GetParkingProfit() / 100f), GlobalDataManager.GetInstance().playerData.playerZoo.buffRatioCoinInComeAdd);
            return number;
        }

        /// <summary>
        /// 玩家所有产出需要相乘的倍数值  y=1+广告buff+月卡buff
        /// </summary>
        /// <returns></returns>
        public static float PlayerRatioCoinInComeMul()
        {
            float number = GlobalDataManager.GetInstance().playerData.playerZoo.buffRatioCoinInComeMul;
            //Logger.LogWarp.LogErrorFormat("测试： 相乘的倍数值为：{0}", number);
            return number;
        }
        /// <summary>
        /// Buff加成
        /// </summary>
        /// <returns></returns>
        public static float PlayerRatioCoinInComeAll()
        {   /*Y=（1+动物加成+停车场利润加成+道具BUFF）*（1+广告BUFF+月卡BUFF）*/
            //LogWarp.LogErrorFormat("测试：  PlayerRatioCoinInComeAdd={0}   PlayerRatioCoinInComeMul={1}", PlayerRatioCoinInComeAdd(), PlayerRatioCoinInComeMul());
            return PlayerRatioCoinInComeAdd() * PlayerRatioCoinInComeMul();
        }

        /// <summary>
        /// 停车场和售票口 升级消耗基数 
        /// </summary>
        /// <returns></returns>
        public static BigInteger GetUpGradeBaseConsumption(int level)
        {  /*  Y=(1+lv/40)*1.07^(lv-1)*IF(lv<=1000,0.5*lv^1.01,0.5*1000^1.01+lv/1000)     */
            if (level <= 0)
            {
                level = 1;
            }
            var numerator = System.Numerics.BigInteger.Parse("107");
            var denominator = System.Numerics.BigInteger.Parse("100");
            int number1 = level  - 1;
            if (number1>0)
            {
                numerator = System.Numerics.BigInteger.Pow(numerator, number1);
                denominator = System.Numerics.BigInteger.Pow(denominator, number1);
            }
            else
            {
                numerator = System.Numerics.BigInteger.Parse("1");
                denominator = System.Numerics.BigInteger.Parse("1");
            }

            /*  IF(lv<=1000,0.5*lv^1.01,0.5*1000^1.01+lv/1000)   */

            float price01;
            if (level<=1000)
            {   /*0.5*lv^1.01*/
                price01 = GetInteger(0.5f * Mathf.Pow(level,1.01f));
            }
            else
            {   /*0.5*1000^1.01+lv/1000*/
                price01 = 0.5f * Mathf.Pow(1000, 1.01f)+level/1000f;
            }
            float number = 1 + level / 40f;
            int number5 = GetInteger((price01 * number * 100));

            var price = numerator * number5 / (denominator * 100);

            return price;
        }

        /// <summary>
        /// 加成预期
        /// </summary>
        /// <param name="level"></param>
        public static float GetAdditionExpect(int level)
        {   /*  Y=(2+MIN(LV/15*2,200)+
             *  IF(LV<=2000,LV/(10/(1+0.015*LV))+
             *  IF(AND(LV>1000,LV<=2000),(LV-1000)/(20/(10+0.01*(LV-1000))),0)+
             *  IF(LV>2000,7200+(LV-2000)*(6-0.003*(LV-2000))))/10
             *  +0.05*LV/1.6)    */


            float number01 = 2+ Mathf.Min(level/15f*2,200);
            float number02 =0;
            if (level <= 2000)
            {
                number02 = level / (10 / (1 + 0.015f * level));
                if (level > 1000 && level <= 2000)
                {
                    number02 += (level - 1000) / GetInteger(20 / (10 + 0.01f * (level - 1000)));
                }
            }
            
            else if(level>2000)
            {
                number02 = 7200 + (level - 2000) * (6 - 0.003f * (level - 2000)) ;
            }

            float number = number01 + number02/10f+ 0.05f * level / 1.6f;
            return number;
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
        /// 贵宾广告收益
        /// </summary>
        /// <returns></returns>
        public static System.Numerics.BigInteger GetFreeItemRwdCoinQuantity()
        {   /*   Y=/M*（2+（停车场LV+售票口lv）/2/260） */
            PlayerData playerData = GlobalDataManager.GetInstance().playerData;

            var price = LeaveEarnings();
            int number = 2 + (int)((playerData.playerZoo.parkingCenterData.parkingProfitLevel+playerData.playerZoo.entryTicketsLevel)/520);
            return price * number;
        }
    }
}
