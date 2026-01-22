/*******************************************************************
* FileName:     LittleZooModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-17
* Description:  
* other:    
********************************************************************/


using Game.GlobalData;
using Game.MessageCenter;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UFrame.MiniGame;
using UnityEngine;

namespace Game
{
    public partial class LittleZooModule : GameModule
    {
        public LittleZooModule(int orderID) : base(orderID) { }

        public Dictionary<int, LittleZoo> littleZooMap = new Dictionary<int, LittleZoo>();

        List<int> openedLittleZooIDs = new List<int>();
        List<int> freeLittleZooIDs = new List<int>();
        List<int> wouldGotoLittleZooIDs = new List<int>();
        List<int> wouldGotoWeights = new List<int>();
        List<int> crossGroupIDs = new List<int>();
        List<int> crossLittleZooIDs = new List<int>();

        int broadcastInterval = 1000;
        int broadcastAccumulate = 0;

        int newLittleZooFlagInterval = 1000;
        int newLittleZooFlagAccumulate = 0;
        Transform newLittleZooFlagTrans = null;
        SimpleParticle newLittleZooFlagSp = new SimpleParticle();
        float temporaryTime;
        public override void Init()
        {
            InitLittleZooData();

            MessageManager.GetInstance().Regist((int)GameMessageDefine.LittleZooData, this.OnLittleZooData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorVisitCDFinshed, this.OnVisitorVisitCDFinshed);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastOpenNewLittleZoo, OnBroadcastOpenNewLittleZoo);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLittleZooTicketsLevelPlayerData, OnBroadcastLittleZooTicketsLevelPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLittleZooVisitorLocationLevelOfPlayerData, OnBroadcastLittleZooVisitorLocationLevelOfPlayerData);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData, OnBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData);

            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorGetRandomLittleZooApply, OnVisitorGetRandomLittleZooApply);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.VisitorGetVisitSeatApply, OnVisitorGetVisitSeatApply);
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.LittleZooData, this.OnLittleZooData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorVisitCDFinshed, this.OnVisitorVisitCDFinshed);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastOpenNewLittleZoo, OnBroadcastOpenNewLittleZoo);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLittleZooTicketsLevelPlayerData, OnBroadcastLittleZooTicketsLevelPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLittleZooVisitorLocationLevelOfPlayerData, OnBroadcastLittleZooVisitorLocationLevelOfPlayerData);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData, OnBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData);

            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorGetRandomLittleZooApply, OnVisitorGetRandomLittleZooApply);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.VisitorGetVisitSeatApply, OnVisitorGetVisitSeatApply);


            foreach (var val in littleZooMap.Values)
            {
                val.Realse();
            }
            littleZooMap.Clear();

            openedLittleZooIDs.Clear();
            freeLittleZooIDs.Clear();
            wouldGotoLittleZooIDs.Clear();
            wouldGotoWeights.Clear();
            crossGroupIDs.Clear();
            crossLittleZooIDs.Clear();

            Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }

            TickBroadcast(deltaTimeMS);
            temporaryTime -= deltaTimeMS;

            TickOpenNewLittleZooFlag(deltaTimeMS);

        }

        protected void InitLittleZooData()
        {
            int level = 0;
            int littleZooID = Const.Invalid_Int;
            var playerData = GlobalDataManager.GetInstance().playerData;

            for (int i = 0; i < playerData.playerZoo.littleZooModuleDatas.Count; i++)
            {
                level = playerData.playerZoo.littleZooModuleDatas[i].littleZooTicketsLevel;
                if (level > 0)
                {
                    littleZooID = playerData.playerZoo.littleZooModuleDatas[i].littleZooID;
                    ModifyLittleZooMap(littleZooID, littleZooMap);

                    //ModifyLittleZooMap(littleZooID, level, littleZooMap);
                }
            }

        }

        protected void OnLittleZooData(Message msg)
        {
            var _msg = msg as LittleZooData;

            LittleZoo littleZoo = null;

            if (!littleZooMap.TryGetValue(_msg.littleZooID, out littleZoo))
            {
                string e = string.Format("没找到动物栏信息 {0}", _msg.littleZooID);
                throw new System.Exception(e);
            }

            //var littleZoo = littleZooMap[_msg.littleZooID];
            LittleZooDataReply.Send(_msg.entityID, littleZoo);
        }

        /// <summary>
        /// 游客的观光时间到了，要把这个观光位腾出来，给对应的等待位上的游客
        /// </summary>
        /// <param name="msg"></param>
        protected void OnVisitorVisitCDFinshed(Message msg)
        {
            var _msg = msg as VisitorVisitCDFinshed;
            LittleZoo littleZoo = null;
            if (!littleZooMap.TryGetValue(_msg.littleZooID, out littleZoo))
            {
                string e = string.Format("查不到LittleZoo {0}", _msg.littleZooID);
                throw new System.Exception(e);
            }

            //游客退出观光位,查找游客在观光位的位置
            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "LittleZooModule 收到{0}, 把{1}移除观光位",
                _msg, _msg.entityID);
            int indexInVisitQueue = littleZoo.RemoveVisitorFromVisitQueue(_msg.entityID);
            var toMsgCDTo = VisitorVisitCDFinshedReply.Send(_msg.entityID, _msg.indexInVisitQueue, _msg.littleZooID);
            DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "LittleZooModule 收到{0}, 把{1}移除观光位{2}, 发送{3}",
                _msg, _msg.entityID, indexInVisitQueue, toMsgCDTo);

            PlayEffect(_msg);
            ////根据这个位置查找等待位上的游客, 如果有, 通知他，并把他移除出等待位
            //int entityID = littleZoo.waitQueue[indexInVisitQueue];
            //if (entityID != Const.Invalid_Int)
            //{
            //    littleZoo.AddVisitorToVisitQueue(entityID, indexInVisitQueue);
            //    littleZoo.RemoveVisitorFromWaitQueue(entityID);
            //    var toMsgV = WaitSeatToVisitSeat.Send(entityID, indexInVisitQueue, _msg.littleZooID);
            //    DebugFile.GetInstance().WriteKeyFile(entityID, "LittleZooModule 清空了{0}的观光位{1}上的{2},把{3}放入观光位, 发送{4}",
            //        _msg.littleZooID, indexInVisitQueue, _msg.entityID, entityID, toMsgV);
            //}
        }

        /// <summary>
        /// 设置动物栏门票等级广播
        /// </summary>
        /// <param name="msg"></param>
        protected void OnBroadcastLittleZooTicketsLevelPlayerData(Message msg)
        {
            var _msg = msg as BroadcastDetailValueOfPlayerData;

            int littleZooID = _msg.detailVal;
            var buildUpCell = Config.buildupConfig.getInstace().getCell(littleZooID);
            int oldLevel = _msg.currVal - _msg.deltaVal;
            int currLevel = _msg.currVal;

            //查看新的等级有没有导致动物栏外观变化
            int oldResIdx = FindLevelRangIndex(buildUpCell.lvmodel, oldLevel);
            int currResIdx = FindLevelRangIndex(buildUpCell.lvmodel, currLevel);
            if (oldResIdx != currResIdx)
            {
                //卸载旧的，加载新的
                LoadLittleZoo(littleZooID, currResIdx, null);
            }

            ////查看新的等级有没有导致动物加载数量有变化
            //int oldAnimalNum = FindLevelRangAnimalIndex(buildUpCell.lvanimal, oldLevel);
            //int currAnimalNum = FindLevelRangAnimalIndex(buildUpCell.lvanimal, currLevel);

            //if (oldAnimalNum != currAnimalNum)
            //{
            //    var cellAnimalUp = Config.animalupConfig.getInstace().getCell(buildUpCell.animalid[currAnimalNum]);

            //    LoadAnimal(littleZooID, buildUpCell.animalid[currAnimalNum],
            //           cellAnimalUp.moveradius, buildUpCell.animalwanderoffset);
            //    //关于Ui等级打点（在旋转相机的时候）
            //    UIZooPage uIZooPage = PageMgr.GetPage<UIZooPage>();
            //    if (uIZooPage != null)
            //    {
            //        uIZooPage.OnGetBroadcastLittleZooTicketsLevelPlayerData(null);
            //        uIZooPage.Hide();
            //    }

            //    var resourceID = Config.animalupConfig.getInstace().getCell(buildUpCell.animalid[currAnimalNum]).resourceload;
            //    //旋转视角UI
            //    PageMgr.ShowPage<UIReceivePage>(resourceID);
            //    MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButHidePart, "UIMainPage");
            //}

        }

        /// <summary>
        /// 广播动物栏观光数量等级
        /// </summary>
        /// <param name="obj"></param>
        private void OnBroadcastLittleZooVisitorLocationLevelOfPlayerData(Message obj)
        {
            var _msg = obj as BroadcastDetailValueOfPlayerData;

            int littleZooID = _msg.detailVal;
            var cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
            int currLevel = _msg.currVal;

            //重新绘制地图   观光点数量
            ModifyLittleZooMap(littleZooID, littleZooMap);

            //ModifyLittleZooMap(littleZooID, currLevel, littleZooMap);
        }

        /// <summary>
        /// 广播动物栏观光游客数量等级
        /// </summary>
        /// <param name="obj"></param>
        private void OnBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData(Message obj)
        {
            var _msg = obj as BroadcastDetailValueOfPlayerData;

            int littleZooID = _msg.detailVal;
            var cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
            int currLevel = _msg.currVal;

            //重新绘制地图   观光点的来客速度
            ModifyLittleZooMap(littleZooID, littleZooMap);

            //ModifyLittleZooMap(littleZooID, currLevel, littleZooMap);
        }

        /// <summary>
        /// 动物加载数量有变化
        /// </summary>
        /// <param name="obj"></param>
        private void OnGetNewAnimalNumberAdd(Message obj)
        {
            //var _mag = obj as GetAddNewAnimalData;
            //int littleZooID = _mag.littleZooID;
            //int animalID = _mag.animalID;
            //LogWarp.LogError("测试:littleZooID "+ littleZooID+ "  ,animalID" + animalID);

            ////加载新的动物  
            //var cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
            //LoadAnimal(littleZooID, animalID, cellBuildUp.animalwanderradius, cellBuildUp.animalwanderoffset);
            ////卸载第一个动物
            //Dictionary<int, EntityMovable> entityMovables = EntityManager.GetInstance().entityMovables;
            //LogWarp.LogError("测试：entityMovables.Count  " + entityMovables.Count);
            //if (entityMovables.Count>5)
            //{
            //    EntityManager.GetInstance().RemoveFromEntityMovables(entityMovables[0]);
            //}

        }

        protected void OnBroadcastOpenNewLittleZoo(Message msg)
        {
            var _msg = msg as BroadcastOpenNewLittleZoo;

            //增加数据
            ModifyLittleZooMap(_msg.littleZooID, littleZooMap);
            //自身外观变化从0级别变1级
            LoadLittleZoo(_msg.littleZooID, 1, null);
            var cellBuildUp = Config.buildupConfig.getInstace().getCell(_msg.littleZooID);
            var cellAnimalUp = Config.animalupConfig.getInstace().getCell(cellBuildUp.animalid[0]);
            //动物栏默认出现第一个动物
            LoadAnimal(_msg.littleZooID, cellBuildUp.animalid[0],
                cellAnimalUp.moveradius, cellBuildUp.animalwanderoffset);
           
            //是否需要加载额外地块
            if (!_msg.isTriggerExtend)
            {
                return;
            }

            //加载新地块
            float extendLen = 0;
            if (_msg.triggerLoadGroupID != Const.Invalid_Int)
            {
                extendLen = LoadExtendGroup(_msg.triggerLoadGroupID);
            }

            int triggerLittleZooID;
            for(int i = 0; i < _msg.triggerLoadLittleZooIDs.Count; i++)
            {
                triggerLittleZooID = _msg.triggerLoadLittleZooIDs[i];
                //加载动物栏
                LoadLittleZoo(triggerLittleZooID, 0, GlobalDataManager.GetInstance().zooGameSceneData.littleZooParentNode);
                //加载动物
                //LoadAnimal(triggerLittleZooID, cellBuildUp.animalid[0],
                //    cellBuildUp.animalwanderradius, cellBuildUp.animalwanderoffset);
                //GlobalDataManager.GetInstance().playerAnimal.SetPlayerAnimal(triggerLittleZooID, cellBuildUp.animalid[0], 1);

            }

            //出口后移
            MoveExitGate(extendLen);

            MessageManager.GetInstance().Send((int)GameMessageDefine.BroadcastAfterExtendSceneAndModifiedPath);
        }

//        protected void OnVisitorGetRandomLittleZooApply_old(Message msg)
//        {
//            var _msg = msg as VisitorGetRandomLittleZooApply;
//            var entity = EntityManager.GetInstance().GetEntityMovable(_msg.entityID) as EntityVisitor;

//            int groupID = entity.stayGroupID;
//            if (_msg.isFirstApply)
//            {
//                groupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
//            }

//            openedLittleZooIDs.Clear();
//            crossGroupIDs.Clear();
//            bool result = false;
//            bool isCrossGroup = false;

//            crossGroupIDs.Add(groupID);
//            bool retGetOpened = GetOpenedLittleZooIDs(groupID, openedLittleZooIDs);
//            if (!retGetOpened)
//            {
//                int nextGroupID = Const.Invalid_Int;
//                while (true)
//                {
//                    bool retGetNext = GlobalDataManager.GetInstance().logicTableGroup.GetNextGroupID(groupID, ref nextGroupID);
//                    if (!retGetNext)
//                    {
//                        //找下一组失败
//                        break;
//                    }
//                    if (!crossGroupIDs.Contains(nextGroupID))
//                    {
//                        crossGroupIDs.Add(nextGroupID);
//                    }
//                    groupID = nextGroupID;
//                    retGetOpened = GetOpenedLittleZooIDs(groupID, openedLittleZooIDs);
//                    if (retGetOpened)
//                    {
//                        isCrossGroup = true;
//                        break;
//                    }
//                }
                
//                if (!retGetOpened)
//                {
//                    //失败
//                    VisitorGetRandomLittleZooReply.Send(_msg.entityID, false, Const.Invalid_Int, Const.Invalid_Int, false, null);
//                    return;
//                }
//            }

//            //wouldGotoLittleZooIDs = //从开放的动物栏中 剔除 去过的
//            bool retGetWouldGoto = GetWouldGotoLittleZooIDs(entity, groupID, openedLittleZooIDs, wouldGotoLittleZooIDs);
//            if (!retGetWouldGoto)
//            {
//                //失败
//                VisitorGetRandomLittleZooReply.Send(_msg.entityID, false, Const.Invalid_Int, Const.Invalid_Int, false, null);
//                return;
//            }

//            //构建 wouldGotoLittleZooIDs 对应的权重
//            bool retGetWeights = GetWouldGotoWeights(groupID, wouldGotoLittleZooIDs, wouldGotoWeights);
//            if (!retGetWeights)
//            {
//                //失败
//                VisitorGetRandomLittleZooReply.Send(_msg.entityID, false, Const.Invalid_Int, Const.Invalid_Int, false, null);
//                return;
//            }

//            int idx = Const.Invalid_Int;
//            Math_F.TableProbability(wouldGotoWeights, ref idx);

//            int littleZooID = wouldGotoLittleZooIDs[idx];

//            result = true;
//            EntityVisitor.RecordVisitedLittleZoo(entity, groupID, littleZooID);
//            if (isCrossGroup)
//            {
//                LogWarp.LogFormat("-----{0} isCrossGroup {1} -> {2}", entity.entityID, entity.stayBuildingID, littleZooID);

//#if UNITY_EDITOR
//                string str = "[";
//                for (int i = 0; i < crossGroupIDs.Count; i++)
//                {
//                    str += crossGroupIDs[i] + ",";
//                }
//                str += "]";
//                DebugFile.GetInstance().WriteKeyFile(_msg.entityID, "{0} LittleZooModule crossGroupIDs = {1}", entity.entityID, str);
//#endif
//                GenCrossLittleZooIDs(entity.stayBuildingID, littleZooID, crossGroupIDs, crossLittleZooIDs);


//                LogWarp.LogFormat("====={0} isCrossGroup", entity.entityID);
//            }
            
//            VisitorGetRandomLittleZooReply.Send(_msg.entityID, result, groupID, littleZooID, isCrossGroup, null);
//        }

        protected void OnVisitorGetRandomLittleZooApply(Message msg)
        {
            var _msg = msg as VisitorGetRandomLittleZooApply;
            var entity = EntityManager.GetInstance().GetEntityMovable(_msg.entityID) as EntityVisitor;

            int groupID = entity.stayGroupID;
            if (_msg.isFirstApply)
            {
                groupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
            }

            openedLittleZooIDs.Clear();

            bool retCode = false;
            int nextGroupID = Const.Invalid_Int;
            while(true)
            {
                retCode = GetOpenedLittleZooIDs(groupID, openedLittleZooIDs);
                if (!retCode)
                {
                    //失败
                    VisitorGetRandomLittleZooReply.Send(_msg.entityID, false, Const.Invalid_Int, Const.Invalid_Int, false, null);
                    return;
                }

                //wouldGotoLittleZooIDs = //从开放的动物栏中 剔除 去过的
                retCode = GetWouldGotoLittleZooIDs(entity, groupID, openedLittleZooIDs, wouldGotoLittleZooIDs);
                if (!retCode)
                {
                    retCode = GlobalDataManager.GetInstance().logicTableGroup.GetNextGroupID(groupID, ref nextGroupID);
                    if (!retCode)
                    {
                        //找下一组失败
                        VisitorGetRandomLittleZooReply.Send(_msg.entityID, false, Const.Invalid_Int, Const.Invalid_Int, false, null);
                        return;
                    }
                    groupID = nextGroupID;
                }
                else
                {
                    break;
                }
            }

            //构建 wouldGotoLittleZooIDs 对应的权重
            retCode = GetWouldGotoWeights(groupID, wouldGotoLittleZooIDs, wouldGotoWeights);
            if (!retCode)
            {
                //失败
                VisitorGetRandomLittleZooReply.Send(_msg.entityID, false, Const.Invalid_Int, Const.Invalid_Int, false, null);
                return;
            }

            int idx = Const.Invalid_Int;
            Math_F.TableProbability(wouldGotoWeights, ref idx);

            int littleZooID = wouldGotoLittleZooIDs[idx];

            EntityVisitor.RecordVisitedLittleZoo(entity, groupID, littleZooID);

            VisitorGetRandomLittleZooReply.Send(_msg.entityID, true, groupID, littleZooID, false, null);
        }

        protected void OnVisitorGetVisitSeatApply(Message msg)
        {
            var _msg = msg as VisitorGetVisitSeatApply;

            LittleZoo littleZoo = null;
            int littleZooID = _msg.littleZooID;
            int entityID = _msg.entityID;
            if (!littleZooMap.TryGetValue(littleZooID, out littleZoo))
            {
#if UNITY_EDITOR
                string e = string.Format("VisitorGetVisitSeatApply 异常 {0}, {1}", entityID, littleZooID);
                throw new System.Exception(e);
#endif
                return;

            }

            int indexInQueue = Const.Invalid_Int;
            bool retCode = littleZoo.IsFreeVisitQueue(ref indexInQueue);
            if (!retCode)
            {
                //没有空位 失败
                VisitorGetVisitSeatReply.Send(entityID, false, Const.Invalid_Int, Const.Invalid_Int);
                return;
            }

            littleZoo.AddVisitorToVisitQueue(_msg.entityID, indexInQueue);
            VisitorGetVisitSeatReply.Send(entityID, true, littleZooID, indexInQueue);
        }

        protected void PlayEffect(VisitorVisitCDFinshed msg)
        {
#if NO_EFFECT
            return;
#endif
            var littleZooBuildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(msg.littleZooID);
            var sp = littleZooBuildinPos.visitCoinSpList[msg.indexInVisitQueue];
            if (!sp.isInit)
            {
                Vector3 visitPos = littleZooBuildinPos.visitPosList[msg.indexInVisitQueue];
                visitPos.y = Config.globalConfig.getInstace().LittleZooCoinEffectOffsetY;
                GameObject effectGo = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().CoinEffectRes);
                effectGo.transform.position = visitPos;
                sp.Init(effectGo);
            }
            sp.Play();
        }

        protected void TickOpenNewLittleZooFlag(int deltaTimeMS)
        {
            this.newLittleZooFlagAccumulate += deltaTimeMS;
            if (newLittleZooFlagAccumulate >= this.newLittleZooFlagInterval)
            {
                newLittleZooFlagAccumulate -= this.newLittleZooFlagInterval;

                var playerData = GlobalDataManager.GetInstance().playerData;
                int littleZooID = playerData.GetFirstUnopenLittleZooID();
                if (littleZooID == Const.Invalid_Int)
                {
                    if (newLittleZooFlagTrans != null)
                    {
                        newLittleZooFlagTrans.gameObject.SetActive(false);
                    }
                    return;
                }
                var needStar = AddNewlittleZooCoin(littleZooID);
                if (newLittleZooFlagTrans==null)
                {
                    newLittleZooFlagTrans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().ExclamatoryEffectRes).transform;

                }
                var littleZoo = GlobalDataManager.GetInstance().zooGameSceneData.littleZooParentNode;
                string path = string.Format("{0}/animation/close", littleZooID);
                Transform littleZooGB = littleZoo.Find(path);
                if (littleZooGB == null)
                {
                    string e = string.Format("报错：动物栏{0}下没有子对象close",littleZooID);
                    throw new System.Exception(e);
                }
                else
                {
                    littleZooGB.gameObject.SetActive(false);
                }
                if (playerData.playerZoo.star < needStar)
                {
                    newLittleZooFlagTrans.gameObject.SetActive(false);
                    littleZooGB.gameObject.SetActive(true);
                    return;
                }
                else
                {
                    newLittleZooFlagTrans.gameObject.SetActive(true);
                    littleZooGB.gameObject.SetActive(false);
                }
                Vector3 pos = LittleZooPosManager.GetInstance().GetPos(littleZooID);
                pos.y = Config.globalConfig.getInstace().LittleZooExclamatoryEffectOffsetY;
                newLittleZooFlagTrans.position = pos;
               
            }
           
        }

        [Conditional("UNITY_EDITOR")]
        protected void TickBroadcast(int deltaTimeMS)
        {
            broadcastAccumulate += deltaTimeMS;
            if (broadcastAccumulate >= this.broadcastInterval)
            {
                broadcastAccumulate -= this.broadcastInterval;
                BroadcastAllLittleZooData.Send(this.littleZooMap);
            }
        }
    }

}
