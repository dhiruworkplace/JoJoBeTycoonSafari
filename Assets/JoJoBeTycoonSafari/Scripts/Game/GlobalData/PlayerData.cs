using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UFrame;

namespace Game.GlobalData
{
    [Serializable]
    public class PlayerData
    {
        public bool isFirst = false;
        public PlayerZoo playerZoo;
        public PlayerLittleGame playerLittleGame;

        public EntryGateData GetEntryGateIDIndexOfDataIdx(int entryID)
        {
            EntryGateData entryGate = null;
            var entryGateList = GlobalDataManager.GetInstance().playerData.playerZoo.entryGateList;
            for (int i = 0; i < entryGateList.Count; i++)
            {
                entryGate = entryGateList[i];
                if (entryGate.entryID == entryID)
                {
                    return entryGate;
                }
            }
            string e = string.Format("售票口ID在用户数据中没找到{0}", entryID);
            throw new System.Exception(e);
            return null;
        }

        public int GetLittleZooIDIndexOfDataIdx(int littleZooID)
        {
            int number = -1;
            var littleZooModuleDatas = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas;
            for (int i = 0; i < littleZooModuleDatas.Count; i++)
            {
                if (littleZooModuleDatas[i].littleZooID == littleZooID)
                {
                    number = i;
                }
            }
            if (number < 0)
            {
                string e = string.Format("动物栏ID在用户数据中没找到{0}", littleZooID);
                throw new System.Exception(e);
            }
            return number;
        }

        public bool GetOpenedLittleZooIDIndexOfData(int littleZooID, ref int idx)
        {
            idx = Const.Invalid_Int;
            var littleZooModuleDatas = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas;
            for (int i = 0; i < littleZooModuleDatas.Count; i++)
            {
                var littleZooModuleData = littleZooModuleDatas[i];
                if (littleZooModuleData.littleZooID == littleZooID && littleZooModuleData.littleZooTicketsLevel > 0)
                {
                    idx = i;
                    return true;
                }
            }

            return false;
        }


        public LittleZooModuleData GetLittleZooModuleData(int littleZooID)
        {
            int idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(littleZooID);
            if (idx < 0)
            {
                string e = string.Format("动物栏ID在用户数据中没找到{0}", littleZooID);
                throw new System.Exception(e);
            }
            return playerZoo.littleZooModuleDatas[idx];
        }

        /// <summary>
        /// 获取没有开启的动物栏ID
        /// </summary>
        /// <returns></returns>
        public int GetFirstUnopenLittleZooID()
        {
            for(int i = 0; i < playerZoo.littleZooModuleDatas.Count; i++)
            {
                if (playerZoo.littleZooModuleDatas[i].littleZooTicketsLevel == 0)
                {
                    return playerZoo.littleZooModuleDatas[i].littleZooID;
                }
            }

            return UFrame.Const.Invalid_Int;
        }

        public void Logout()
        {
            playerZoo.lastLogoutTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// 获取离线时间
        /// .Ticks 得到的值是自公历 0001-01-01 00:00:00:000 至此的以 100 ns（即 1/10000 ms）为单位的时间数。
        /// </summary>
        /// <returns></returns>
        public double GetOfflineSecond()
        {
            long realOffline = (DateTime.Now.Ticks - playerZoo.lastLogoutTime) / 10000000;
            Logger.LogWarp.LogErrorFormat("realOffline = {0}, {1}", realOffline, Config.globalConfig.getInstace().MinOfflineSecond);
            //30秒内离线不算离线
            if (realOffline <= Config.globalConfig.getInstace().MinOfflineSecond)
            {
                return 0;
            }
            return realOffline - Config.globalConfig.getInstace().MinOfflineSecond;
        }

        public double GetRealOfflineSecond()
        {
            long realOffline = (DateTime.Now.Ticks - playerZoo.lastLogoutTime) / 10000000;
            return realOffline;
        }

        public static PlayerData Load()
        {
            Logger.LogWarp.Log("PlayerData.Load");
            var playerData = GlobalDataManager.GetInstance().playerData;
            if (playerData == null)
            {
                Logger.LogWarp.Log("PlayerData.LoadFromPlayerPrefs");
                playerData = LoadFromPlayerPrefs();
                Logger.LogWarp.LogErrorFormat("playerData.playerZoo.isGuide {0}", playerData.playerZoo.isGuide);
                GlobalDataManager.GetInstance().playerData = playerData;
            }
            return playerData;
        }

        protected static PlayerData LoadFromPlayerPrefs()
        {
            PlayerData pd = null;
            string str = PlayerPrefs.GetString("PlayerData");
            bool isFirst = false;
            if (string.IsNullOrEmpty(str))
            {
                isFirst = true;
                var playerData = new PlayerData();
                playerData.playerZoo = new PlayerZoo();
                playerData.playerZoo.SetDefault();
                Logger.LogWarp.LogErrorFormat("playerData.playerZoo.isGuide {0}",
                    playerData.playerZoo.isGuide);
                Save(playerData);
                str = PlayerPrefs.GetString("PlayerData");
                if (string.IsNullOrEmpty(str))
                {
                    string e = string.Format("取本地数据PlayerData异常");
                    throw new System.Exception(e);
                }
            }

            pd = JsonUtility.FromJson<PlayerData>(str);
            if (pd == null)
            {
                pd = new PlayerData();
                pd.playerZoo.SetDefault();
                pd.isFirst = false;
                if (isFirst)
                {
                    pd.isFirst = true;
                }
                return pd;
            }
            Logger.LogWarp.LogErrorFormat("playerData.playerZoo.isGuide {0}", pd.playerZoo.isGuide);
            pd.isFirst = false;
            if (isFirst)
            {
                pd.isFirst = true;
            }
            if (pd.playerZoo.entryGateList == null)
            {
                pd.playerZoo.entryGateList = new List<EntryGateData>();
            }
            if (pd.playerZoo.entryGateList.Count == 0)
            {
                pd.playerZoo.SetDefaultEntryGateData();
            }

            if (pd.playerZoo.littleZooModuleDatas == null)
            {
                pd.playerZoo.littleZooModuleDatas = new List<LittleZooModuleData>();
            }
            if (pd.playerZoo.littleZooModuleDatas.Count == 0)
            {
                pd.playerZoo.SetDefaultlittleZooData();
            }

            if (pd.playerZoo.parkingCenterData == null)
            {
                pd.playerZoo.parkingCenterData = new ParkingCenterData();
            }
            return pd;
        }

        public static void Save(PlayerData playerData)
        {
            string str = JsonUtility.ToJson(playerData);
            if (string.IsNullOrEmpty(str))
            {
                string e = string.Format("存本地数据PlayerData异常");
                throw new System.Exception(e);
            }
            PlayerPrefs.SetString("PlayerData", str);
        }

        
    }
}

