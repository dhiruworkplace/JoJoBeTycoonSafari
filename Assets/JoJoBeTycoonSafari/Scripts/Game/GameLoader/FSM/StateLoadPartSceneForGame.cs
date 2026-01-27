/*******************************************************************
* FileName:     StateLoadPartSceneForGame.cs
* Author:       Fan Zheng Yong
* Date:         2019-12-18
* Description:  
* other:    
********************************************************************/


using ZooGame.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using ZooGame.MessageCenter;
using System;
using UnityEngine;
using System.Collections.Generic;
using UFrame.MiniGame;
using ZooGame.GlobalData;

namespace ZooGame
{
    /// <summary>
    /// 按需分块加载场景
    /// </summary>
    public class StateLoadPartSceneForGame : FSMState
    {
        PlayerData playerData;

        bool isToStateLoadAnimal = false;
        public StateLoadPartSceneForGame(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            playerData = GlobalDataManager.GetInstance().playerData;

            isToStateLoadAnimal = false;

            LoadLittleZoo();
            LoadParking(); 
            FindEntryUISceneNode();
            SetEntrySceneObject();
            //LogWarp.LogFormat("总地块={0}", loadGroup.Count);
            float value = 1 / 6f;
            (PageMgr.allPages["UILoading"] as UILoading).SliderValueLoading(value);

            isToStateLoadAnimal = true;
        }

        /// <summary>
        /// 加载动物栏
        /// </summary>
        protected void LoadLittleZoo()
        {
            var loadGroup = (this.fsmCtr as FSMGameLoad).loadGroup;

            GlobalDataManager.GetInstance().zooGameSceneData.Realse();
            GameObject camera = GameObject.Find("Camera");
            GlobalDataManager.GetInstance().zooGameSceneData.camera = camera;
            var littleZooRoot = GameObject.Find("LittleZoo").transform;
            GlobalDataManager.GetInstance().zooGameSceneData.littleZooParentNode = littleZooRoot;
            int littleZooID = Const.Invalid_Int;
            int groupID = Const.Invalid_Int;
            for (int i = 0; i < this.playerData.playerZoo.littleZooModuleDatas.Count; i++)
            {
                littleZooID = this.playerData.playerZoo.littleZooModuleDatas[i].littleZooID;
                groupID = GlobalDataManager.GetInstance().logicTableGroup.FindGroupID(littleZooID);
                if (!loadGroup.Contains(groupID))
                {
                    loadGroup.Add(groupID);
                    LogWarp.LogFormat("loadGroup {0}", groupID);
                }

                //加载动物栏
                int level = this.playerData.playerZoo.littleZooModuleDatas[i].littleZooTicketsLevel;
                var cellBuild = Config.buildupConfig.getInstace().getCell(littleZooID);
                int buildResIdx = LittleZooModule.FindLevelRangIndex(cellBuild.lvmodel, level);

                LittleZooModule.LoadLittleZoo(littleZooID, buildResIdx, littleZooRoot, false);


            }
            loadGroup.Sort();

            Config.resourceCell cellRes;
            int idx = 0;
            float offset = Config.globalConfig.getInstace().ZooPartResLen;
            float extendOffset = 0;
            Config.groupCell preCell = null;
            Config.groupCell lastCell = null;
            for (int i = 0; i < loadGroup.Count; i++)
            {
                var cellGroup = Config.groupConfig.getInstace().getCell(loadGroup[i]);

                //加载Group
                if (cellGroup.zoopartresID > 0 && i >= (Config.globalConfig.getInstace().DefaultOpenGroup))
                //if (cellGroup.zoopartresID > 0 && i >= (Config.globalConfig.getInstace().DefaultOpenGroup + 1))
                {
                    cellRes = Config.resourceConfig.getInstace().getCell(cellGroup.zoopartresID);
                    var goPart = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
                    //goPart.transform.position = new Vector3(goPart.transform.position.x - idx * offset, 0, 0);
                    if (preCell != null)
                    {
                        extendOffset += preCell.groundsize;
                    }
                    goPart.transform.position = new Vector3(goPart.transform.position.x - extendOffset, 0, 0);
                    goPart.name = string.Format("Group_{0}", cellGroup.zoopartresID);
                    ++idx;
                    preCell = cellGroup;
                    lastCell = cellGroup;
                    GlobalDataManager.GetInstance().zooGameSceneData.AddExtendLoadGroup(loadGroup[i], goPart);
                }
            }
            if (lastCell != null)
            {
                extendOffset += lastCell.groundsize;
            }

            LittleZooModule.LoadExitGate(idx, extendOffset);
        }

        /// <summary>
        /// 加载停车场地块
        /// </summary>
        protected void LoadParking()
        {
            GlobalDataManager.GetInstance().zooGameSceneData.ParckingSencePos = GameObject.Find("ParckingSencePos");

            var cellBuildUp = Config.parkingConfig.getInstace().getCell(1);
            var parkingSpaceLevel = this.playerData.playerZoo.parkingCenterData.parkingSpaceLevel;
            int currResIdx = ParkingCenter.FindLevelRangIndex(cellBuildUp.openlv, parkingSpaceLevel);
            var cellBuild = Config.parkingConfig.getInstace().getCell(1);

            var cellRes = Config.resourceConfig.getInstace().getCell(cellBuild.openggroup[currResIdx]);
            var parkingModel = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
            //LogWarp.LogErrorFormat("AAAA  currResIdx={0},cellBuild.openggroup[currResIdx].name={1},parkingSpaceLevel={2}", currResIdx, cellBuild.openggroup[currResIdx], parkingSpaceLevel);

            //LogWarp.LogError(" 测试：当前停车场模型 name=" + packModel.name);
            parkingModel.transform.position = new UnityEngine.Vector3(0, 0, 0);
            parkingModel.SetActive(true);
            parkingModel.transform.SetParent(GlobalDataManager.GetInstance().zooGameSceneData.ParckingSencePos.transform, false);
        }

        /// <summary>
        /// 查找售票口的下标
        /// </summary>
        protected void FindEntryUISceneNode()
        {
            if (GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.entryGatesVector.Count < 1)
            {
                ////循环查找所有的出口坐标  放在entryGatesVector
                for (int i = 0; i < Config.ticketConfig.getInstace().AllData.Count; i++)
                {
                    var cell = Config.ticketConfig.getInstace().getCell(i);
                    //LogWarp.LogError(cell);
                    Vector3 vector = GameObject.Find(cell.gameobjectpath).transform.position;
                    GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.entryGatesVector.Add(vector);
                    var simpleParticle = new SimpleParticle();
                    GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.entryCoinSpList.Add(simpleParticle);
                    Transform transform = GameObject.Find(Config.ticketConfig.getInstace().getCell(i).prohibitroute).transform;
                    GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.EntrySubscriptGB.Add(transform);
                }
            }
        }

        /// <summary>
        /// 设置售票口部件的显示或隐藏
        /// </summary>
        protected void SetEntrySceneObject()
        {
            var entryGateList = playerData.playerZoo.entryGateList;
            int entryNum = entryGateList.Count; ;
            for (int i = 0; i < entryNum; i++)
            {
                HideExitGateForbidGameObject(i);
            }
        }

        /// <summary>
        /// 隐藏开启的售票口对应的禁止牌
        /// </summary>
        /// <param name="number">出口ID</param>
        void HideExitGateForbidGameObject(int number)
        {
            if (number == 0)
            {
                return;
            }
            GameObject gameObject = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.EntrySubscriptGB[number].Find("jinzhitongxing").gameObject;
            GameObject gameObject1 = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.EntrySubscriptGB[number].Find("damen_shoufei").gameObject;

            if (gameObject != null)
            {
                gameObject.SetActive(false);
                gameObject1.SetActive(true);
            }
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        public override void Leave()
        {
            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)GameLoaderState.LoadAnimalInLittleZoo, ToStateLoadAnimal);
        }

        protected bool ToStateLoadAnimal()
        {
            return isToStateLoadAnimal;
        }
    }

}
