using ZooGame.GlobalData;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UFrame.EntityFloat;
using UnityEngine;

namespace ZooGame
{
    [Serializable]
    public enum AnimalState
    {
        NoneOwn,        //未拥有
        StayOpen,       //等待开启
        AlreadyOpen,    //已经开启可以升级
    }


    /// <summary>
    /// 动物栏动物数量类
    /// <动物ID,动物数量>
    /// </summary>
    [Serializable]
    public class LittleAnimal
    {
        /// <summary>
        /// AnimalState状态
        /// </summary>
        public AnimalState animalState = AnimalState.NoneOwn;
        /// <summary>
        /// 动物等级
        /// </summary>
        public int animalLevel = 0;
        /// <summary>
        /// 动物的模型ID
        /// </summary>
        public int animalEntityID = 0;

        public LittleAnimal()
        {
        }
        public LittleAnimal(AnimalState state, int level = 0)
        {
            this.animalState = state;
            this.animalLevel = level;
        }
    }
    [Serializable]
    public class PlayerAnimal
    {

        /// <summary>
        /// 动物对应的ID
        /// </summary>
        public List<string> animalID = new List<string>();
        /// <summary>
        /// 动物ID对应的的动物数据类
        /// </summary>
        public List<LittleAnimal> littleAnimalList = new List<LittleAnimal>();

        /// <summary>
        /// 玩家拥有的动物数量
        /// </summary>
        public int playerAnimalsNumber;

        /// <summary>
        /// 玩家拥有的所有动物等级和
        /// </summary>
        public int playerAllAnimalsLevel;


        public PlayerAnimal()
        {
            InitPlayerAnimalDic();
            playerAnimalsNumber = 0;
        }

        /// <summary>
        /// 初始化玩家的playerAnimalDic
        /// </summary>
        private void InitPlayerAnimalDic()
        {
            var cellAnimalupAllData = Config.animalupConfig.getInstace().AllData;
            foreach (var item in cellAnimalupAllData)
            {
                string id = item.Key;
                LittleAnimal littleAnimal = new LittleAnimal
                {
                    animalLevel = 0,
                    animalState = AnimalState.NoneOwn,
                    animalEntityID = 0,
                };
                //if (id == "10101")
                //{
                //    littleAnimal.animalLevel = 1;
                //    playerAllAnimalsLevel = 1;
                //    littleAnimal.animalState = AnimalState.AlreadyOpen;
                //}
                animalID.Add(id);
                littleAnimalList.Add(littleAnimal);
            }
        }

        /// <summary>
        /// 修改玩家动物为解锁状态
        /// </summary>
        /// <param name="id">动物ID</param>
        public void SetPlayerAnimalDataStayOpen(int id)
        {
            LittleAnimal littleAnimal = getPlayerAnimalCell(id);
            littleAnimal.animalState = AnimalState.StayOpen;
        }

        /// <summary>
        /// 修改玩家动物为可升级状态
        /// </summary>
        /// <param name="id"></param>
        public void SetPlayerAnimalDataAlreadyOpen(int id, int entityID)
        {
            var LittleAnimal = getPlayerAnimalCell(id);
            if (LittleAnimal.animalLevel>0)
            {
                return;
            }
            else
            {   //新开启的动物显示在动物栏
                LittleAnimal.animalLevel = 1;
                LittleAnimal.animalState = AnimalState.AlreadyOpen;
                LittleAnimal.animalEntityID = entityID;
                playerAnimalsNumber += 1;
                playerAllAnimalsLevel += 1;
            }
        }

        /// <summary>
        /// 修改玩家动物的等级
        /// </summary>
        /// <param name="id"></param>
        public void SetPlayerAnimalLevelData(int id)
        {
            LittleAnimal littleAnimal = getPlayerAnimalCell(id);
            littleAnimal.animalLevel += 1;
            playerAllAnimalsLevel += 1;
        }

        public int GetAnimalEntityID(int animalID)
        {
            var littleAnimal = getPlayerAnimalCell(animalID);
            return littleAnimal.animalEntityID;
        }

        /// <summary>
        /// 获取ID对应的动物数据
        /// </summary>
        /// <param name="key">动物ID</param>
        /// <returns></returns>
        public LittleAnimal getPlayerAnimalCell(string key)
        {
            LittleAnimal t = null;
            int idx = animalID.IndexOf(key);
            t = this.littleAnimalList[idx];
            return t;
        }

        /// <summary>
        /// 获取ID对应的动物数据
        /// </summary>
        /// <param name="key">动物ID</param>
        /// <returns></returns>
        public LittleAnimal getPlayerAnimalCell(int key)
        {
            LittleAnimal t = null;
            int idx = animalID.IndexOf(key.ToString());

            t = this.littleAnimalList[idx];
            return t;
        }

    }
}
