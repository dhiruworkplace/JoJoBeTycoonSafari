/*******************************************************************
* FileName:     StateLoadAnimalInLittleZooForGame.cs
* Author:       Fan Zheng Yong
* Date:         2019-12-18
* Description:  
* other:    
********************************************************************/


using Game.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using Game.MessageCenter;
using System;
using UnityEngine;
using System.Collections.Generic;
using UFrame.MiniGame;
using Game.GlobalData;

namespace Game
{
    /// <summary>
    /// 加载动物栏的动物
    /// </summary>
    public class StateLoadAnimalInLittleZooForGame : FSMState
    {
        PlayerData playerData;

        bool isSendFinished = false;
        bool isClosed = false;

        IntCD waitCD;

        public StateLoadAnimalInLittleZooForGame(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            isSendFinished = false;
            isClosed = false;

            playerData = GlobalDataManager.GetInstance().playerData;

            waitCD = new IntCD(100);
            waitCD.Stop();

            LoadAnimalInLittleZoo();
            SendLoadFinised();
        }

        /// <summary>
        /// 加载动物栏的小动物
        /// MIN(1+INT(lv/100),10) 然后取buildup表中的animalid数组中的元素
        /// </summary>
        protected void LoadAnimalInLittleZoo()
        {
            int littleZooID;
            Config.buildupCell cellBuildUp;
            Config.animalupCell cellAnimalUp;
            var playerAnimal = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal;
            int animalID;

            var coin = GlobalDataManager.GetInstance().playerData.playerZoo.coin;
            var baseCoin = Config.globalConfig.getInstace().InitialGoldNumber;
#if NO_BIGINT
            for (int i = 0; i < this.playerData.playerZoo.littleZooModuleDatas.Count; i++)
            {
                littleZooID = playerData.playerZoo.littleZooModuleDatas[i].littleZooID;
                cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);

                for(int j = 0; j < cellBuildUp.animalid.Length; j++)
                {
                    cellAnimalUp = Config.animalupConfig.getInstace().getCell(cellBuildUp.animalid[j]);
                    LittleZooModule.LoadAnimal(littleZooID, cellBuildUp.animalid[j],
                               cellAnimalUp.moveradius, cellBuildUp.animalwanderoffset);
                }
            }

            (PageMgr.allPages["UILoading"] as UILoading).SliderValueLoading(1f);
            waitCD.Run();
            return;
#endif
            if (coin == baseCoin)
            {
                int i = 0;
                littleZooID = playerData.playerZoo.littleZooModuleDatas[i].littleZooID;
                cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
                cellAnimalUp = Config.animalupConfig.getInstace().getCell(cellBuildUp.animalid[0]);
                //level = playerData.playerZoo.littleZooModuleDatas[0].littleZooTicketsLevel;
                LittleZooModule.LoadAnimal(littleZooID, cellBuildUp.animalid[0],
                           cellAnimalUp.moveradius, cellBuildUp.animalwanderoffset);
                GameEventManager.SendEvent("first_start_loading");

            }
            else
            {
                for (int i = 0; i < this.playerData.playerZoo.littleZooModuleDatas.Count; i++)
                {
                    littleZooID = playerData.playerZoo.littleZooModuleDatas[i].littleZooID;
                    cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
                    //level = playerData.playerZoo.littleZooModuleDatas[idx].littleZooTicketsLevel;
                    for (int j = 0; j < cellBuildUp.animalid.Length; j++)
                    {
                        animalID = cellBuildUp.animalid[j];
                        cellAnimalUp = Config.animalupConfig.getInstace().getCell(animalID);
                        ////获取动物状态
                        var animalState = playerAnimal.getPlayerAnimalCell(animalID).animalState;
                        if (animalState == AnimalState.AlreadyOpen)
                        {   //若是可升级状态则显示在动物栏场景内
                            LittleZooModule.LoadAnimal(littleZooID, animalID,
                                cellAnimalUp.moveradius, cellBuildUp.animalwanderoffset);
                        }

                    }
                }
            }
            GameEventManager.SendEvent("start_loading");

            (PageMgr.allPages["UILoading"] as UILoading).SliderValueLoading(1f);
            waitCD.Run();
        }

        protected void SendLoadFinised()
        {
#if UNITY_EDITOR
            GameObject.Find("LittlezooBuildinPos").transform.GetChild(0).gameObject.SetActive(true);
#endif

            MessageManager.GetInstance().Send((int)GameMessageDefine.LoadZooSceneFinished);

            isSendFinished = true;
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!isSendFinished)
            {
                return;
            }

            if (isClosed)
            {
                return;
            }

            waitCD.Tick(deltaTimeMS);

            if (waitCD.IsRunning() && waitCD.IsFinish())
            {
                isClosed = true;
                PageMgr.ClosePage("UILoading");
                this.fsmCtr.Stop();
            }
        }

        public override void Leave()
        {
            base.Leave();
            PageMgr.ShowPage<UIMainPage>();

        }

        public override void AddAllConvertCond()
        {
        }
    }

}
