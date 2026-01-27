using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using ZooGame.MiniGame;
using ZooGame.Path.StraightLine;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UFrame;
using UFrame.MessageCenter;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace ZooGame
{
    public partial class LittleZooModule : GameModule
    {
        /// <summary>
        /// 返回动物id在动物栏的动物数组中的下标
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="val">动物ＩＤ</param>
        /// <returns></returns>
        public static int FindArrayRangIndex(int littleZooID, int val)
        {
            var arrays = Config.buildupConfig.getInstace().getCell(littleZooID).animalid;

            int idx = -1;
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i] == val)
                {
                    idx = i;
                    return idx;
                }
            }
            if (idx ==-1)
            {
                string e = string.Format("动物ID{0}不属于动物栏{1}", val, littleZooID);
                throw new System.Exception(e);
            }
            return idx;
        }


        /// <summary>
        /// 给定一个等级范围数组，等级，返回等级所在范围数组的索引
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int FindLevelRangIndex(int [] levels, int level)
        {
            int idx = Const.Invalid_Int;
            if (level == 0)
            {
                return levels[0];
            }

            for (int j = 1; j < levels.Length; j++)
            {
                if (level <= levels[j]-1)
                {
                    idx = j;
                    break;
                }
            }
            if (idx == Const.Invalid_Int)    //若idx超过数组范围则等于最大返回结果
            {
                idx = levels.Length - 1;
            }
            return idx;
        }
        /// <summary>
        /// 查找等级在动物栏的等级数组中范围的下标
        /// </summary>1|10|25|50|100
        /// <param name="levels"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int FindLevelRangAnimalIndex(int[] levels, int level)
        {
            int idx = Const.Invalid_Int;
            if (level == 0)
            {
                string e = string.Format("报错：获取动物ID的时候等级={0}，不能为零");
                throw new System.Exception(e);
            }

            for (int j = 1; j < levels.Length; j++)
            {
                if (level < levels[j])
                {
                    idx = j-1;
                    break;
                }
            }
            if (idx == Const.Invalid_Int)    //若idx超过数组范围则等于最大返回结果
            {
                idx = levels.Length - 1;
            }
            return idx;
        }
        /// <summary>
        /// A获取新开启动物栏的花费
        /// </summary>
        /// <param name="littleZooID">要开启的动物栏</param>
        /// <returns></returns>
        public static int AddNewlittleZooCoin(int littleZooID)
        {
#if DEBUG_VISIT
            return 0;
#else
            //var cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
            //if (cellBuildUp == null)
            //{
            //    string e = string.Format("动物栏id 异常{0}", littleZooID);
            //    throw new System.Exception(e);
            //}
            //var needCoin = System.Numerics.BigInteger.Parse(cellBuildUp.number);
            var cellBuildUp = Config.buildupConfig.getInstace().getCell(littleZooID);
            int needStar = cellBuildUp.needstar;

            return needStar;
#endif
        }
        
        /// <summary>
        /// 加载动物
        /// </summary>
        /// <param name="littleZooID">加载动物栏ID</param>
        /// <param name="animalID">动物ID</param>
        /// <param name="animalWanderradius"></param>
        /// <param name="animalWanderoffset"></param>
        public static void LoadAnimal(int littleZooID, int animalID, float animalWanderradius, float[] animalWanderoffset)
        {
            var cellAnimalUp = Config.animalupConfig.getInstace().getCell(animalID);

            var entity = EntityManager.GetInstance().GenEntityGameObject(cellAnimalUp.resourceload, EntityFuncType.Animal_Wander) as EntityAnimalWander;
            entity.animalID = animalID;
            //MessageInt.Send((int)GameMessageDefine.AnimalPlayLevelUpEffect, entity.entityID);

            //对玩家的动物数据进行修改
            var playerAnimal = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal;
            playerAnimal.SetPlayerAnimalDataAlreadyOpen(animalID, entity.entityID);
            //playerAnimal.SetPlayerAnimalDataAlreadyOpen(animalID);

            Vector3 offset = Vector3.zero;
            offset.x = animalWanderoffset[0];
            offset.y = animalWanderoffset[1];
            offset.z = animalWanderoffset[2];

            var littleZooBuildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(littleZooID);
            int idx = FindArrayRangIndex(littleZooID, animalID);
            var animalPos = littleZooBuildinPos.animalPosList[idx];

            //entity.position = LittleZooPosManager.GetInstance().GetPos(littleZooID) + offset;
            entity.position = animalPos;
            //if (entity.wander == null)
            //{
            //    entity.wander = new UFrame.BehaviourFloat.Wander();
            //}
            //entity.wander.Init(entity, entity.position, animalWanderradius, cellAnimalUp.animalwanderspeed);

            // PathWander
            if (entity.pathWander == null)
            {
                entity.pathWander = new UFrame.BehaviourFloat.PathWander();
            }
            entity.pathWander.Init(entity, 
                PathManager.GetInstance().GetPath(cellAnimalUp.walkingpath), 
                cellAnimalUp.animalwanderspeed);

            if (entity.anim == null)
            {
                entity.anim = new SimpleAnimation();
            }
            entity.anim.Init(entity.mainGameObject);

            entity.Active();
            EntityManager.GetInstance().AddToEntityMovables(entity);
            //向动物栏添加动物数据的方法
            //GlobalDataManager.GetInstance().playerAnimal.SetPlayerAnimal(entity.entityID, littleZooID,animalID, 1);
        }

        public static float LoadExtendGroup(int groupID)
        {
            var cellGroup = Config.groupConfig.getInstace().getCell(groupID);

            //加载Group
            //索引=额外加载的
            //int idx = GlobalDataManager.GetInstance().zooGameSceneData.GetExtendLoadGroupCount();
            //float offset = Config.globalConfig.getInstace().ZooPartResLen;
            var sortedGroupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID;
            float extendLen = 0;
            for (int i = 0; i < sortedGroupID.Count; i++)
            {
                if (sortedGroupID[i] == groupID)
                {
                    break;
                }
                var cell = Config.groupConfig.getInstace().getCell(sortedGroupID[i]);
                if (cell.groundsize > 0 && cell.zoopartresID > 0)
                {
                    extendLen += cell.groundsize;
                }
            }

            if (cellGroup.zoopartresID > 0)
            {
                if (!GlobalDataManager.GetInstance().zooGameSceneData.IsExtendGroupContains(groupID))
                {
                    var cellRes = Config.resourceConfig.getInstace().getCell(cellGroup.zoopartresID);
                    var goPart = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
                    goPart.transform.position += GlobalDataManager.GetInstance().SceneForward * extendLen;
                    goPart.name = string.Format("ExtendPart_{0}", groupID);
                    GlobalDataManager.GetInstance().zooGameSceneData.AddExtendLoadGroup(groupID, goPart);
                }
                extendLen += cellGroup.groundsize;
            }

            return extendLen;
            //if (cellGroup.zoopartresID > 0)
            //{
            //    var cellRes = Config.resourceConfig.getInstace().getCell(cellGroup.zoopartresID);
            //    var goPart = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
            //    goPart.transform.position = new Vector3(goPart.transform.position.x - idx * offset, 0, 0);
            //    goPart.name = string.Format("ExtendPart_{0}", groupID);
            //    GlobalDataManager.GetInstance().zooGameSceneData.AddExtendLoadGroup(groupID, goPart);
            //}  

        }

        public static void LoadLittleZoo(int littleZooID, int idx, Transform partnet,bool isUpLevelShow=true)
        {
            GlobalDataManager.GetInstance().zooGameSceneData.RemoveLoadLittleZoo(littleZooID);

            var cellBuild = Config.buildupConfig.getInstace().getCell(littleZooID);
            
            var cellRes = Config.resourceConfig.getInstace().getCell(cellBuild.loadresource[idx]);
            LogWarp.Log(cellRes.prefabpath);
            var goLittleZoo = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);

            goLittleZoo.name = littleZooID.ToString();

            if (partnet == null)
            {
                partnet = GlobalDataManager.GetInstance().zooGameSceneData.littleZooParentNode;
            }

            goLittleZoo.transform.SetParent(partnet, false);
            var pos = LittleZooPosManager.GetInstance().GetPos(littleZooID);
            goLittleZoo.transform.position = pos;

            GlobalDataManager.GetInstance().zooGameSceneData.AddLoadLittleZoo(littleZooID, goLittleZoo);

            if (littleZooID == 1001 && idx == 1)
            {
                return;
            }
            if (idx == 1 && isUpLevelShow == true)
            {
                GlobalDataManager.GetInstance().playerData.playerZoo.BuildShowTransform = goLittleZoo.transform;

                //关于Ui等级打点（在旋转相机的时候）
                UIZooPage uIZooPage = PageMgr.GetPage<UIZooPage>();
                if (uIZooPage!=null)
                {
                    uIZooPage.OnGetBroadcastLittleZooTicketsLevelPlayerData(null);
                    uIZooPage.Hide();

                }
                //PageMgr.ClosePage<UIMainPage>();
                MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButHidePart, "UIMainPage");
                PageMgr.ShowPage<UIBuildShowPage>(littleZooID);  //旋转视角UI
                System.Action action = null;
                action?.Invoke();
                var anim = goLittleZoo.GetComponentInChildren<Animation>();
                if (anim != null)
                {
                    SimpleAnimation buildingClickSa = new SimpleAnimation();
                    buildingClickSa.Init(anim);
                    buildingClickSa.Play(Config.globalConfig.getInstace().BuildClickAnim);
                }

            }
        }

        public static void LoadExitGate(int idx, float offset)
        {
            int endResID = Config.globalConfig.getInstace().EndPartResID;
            var cellRes = Config.resourceConfig.getInstace().getCell(endResID);
            var goEnd = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
            goEnd.name = goEnd.name.Replace("(Clone)", "");
            //goEnd.transform.position = new Vector3(goEnd.transform.position.x - idx * offset, 0, 0);
            //goEnd.transform.position = new Vector3(goEnd.transform.position.x - offset, 0, 0);
            goEnd.transform.position += GlobalDataManager.GetInstance().SceneForward * offset;
            GlobalDataManager.GetInstance().zooGameSceneData.endNode = goEnd.transform;
        }

        public static void MoveExitGate(float extendLen)
        {
            //float offset = Config.globalConfig.getInstace().ZooPartResLen;
            //Vector3 oldPos = GlobalDataManager.GetInstance().zooGameSceneData.endNode.position;
            //Vector3 newPos = new Vector3(oldPos.x - offset, oldPos.y, oldPos.z);
            //GlobalDataManager.GetInstance().zooGameSceneData.endNode.position = newPos;

            GlobalDataManager.GetInstance().zooGameSceneData.endNode.position = GlobalDataManager.GetInstance().SceneForward * extendLen;
        }

        public static bool ModifyExitGateEntryPath(Vector3 offset)
        {
            bool retCode = false;
            foreach(var kv in Config.exitgateConfig.getInstace().AllData)
            {
                var cell = kv.Value;

                retCode = PathManager.GetInstance().ModifyPath(cell.positiveexitgate, offset);
                if (!retCode)
                {
                    return false;
                }

                retCode = PathManager.GetInstance().ModifyPath(cell.negativeexitgate, offset);
                if (!retCode)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ModifyExitGateShuttlePath(Vector3 offset)
        {
            bool retCode = false;

            retCode = PathManager.GetInstance().ModifyPath(Config.globalConfig.getInstace().Path_ShuttleGotoDynamic, offset);
            if (!retCode)
            {
                return false;
            }

            retCode = PathManager.GetInstance().ModifyPath(Config.globalConfig.getInstace().Path_ShuttleGobackDynamic, offset);
            if (!retCode)
            {
                return false;
            }

            return true;
        }

        public static void ModifyLittleZooMap(int littleZooID, Dictionary<int, LittleZoo> littleZooMap)
        {
            LittleZoo littleZoo = null;
            var buildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(littleZooID);
            var littleZooModuleDatas = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas;
            int idx = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(littleZooID);  //获取动物栏ID  下标
            var cell = Config.buildupConfig.getInstace().getCell(littleZooID);
            //int visitNum = OpenVisitPosNUmber(littleZooModuleDatas[idx].littleZooVisitorSeatLevel);
            int visitNum = OpenVisitPosNumber(littleZooModuleDatas[idx].littleZooID, littleZooModuleDatas[idx].littleZooVisitorSeatLevel);
            int visitDuartion = (int)GetComeVisitorSpeedCD(littleZooID, littleZooModuleDatas[idx].littleZooEnterVisitorSpawnLevel);

            //LogWarp.LogErrorFormat("测试：   当前动物栏{0}的观光点数{1}",littleZooID,visitNum);

            if (!littleZooMap.TryGetValue(littleZooID, out littleZoo))
            {
                littleZoo = new LittleZoo(littleZooID, buildinPos, visitDuartion, visitNum, visitNum);
                littleZooMap.Add(littleZooID, littleZoo);

                return;
            }

            littleZoo.visitCDValue = visitDuartion;

            if (visitNum > littleZoo.maxLenthOfVisitQueue)
            {
                int extendLen = visitNum - littleZoo.maxLenthOfVisitQueue;
                for (int i = 0; i < extendLen; i++)
                {
                    littleZoo.visitQueue.Add(Const.Invalid_Int);
                    littleZoo.waitQueue.Add(Const.Invalid_Int);
                }
                littleZoo.maxLenthOfVisitQueue = visitNum;
                littleZoo.maxLenthOfWaitQueue = visitNum;
            }
        } 

        /// <summary>
        /// 组概率判定是否去这组
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static bool WouldGotoGroupByGotoweight(int groupID)
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            int weight = 0;
            var cell = Config.groupConfig.getInstace().getCell(groupID);
            if (null == cell)
            {
                string e = string.Format("WouldGotoGroupByGotoweight 没找到组{0}", groupID);
                throw new System.Exception(e);
            }
            weight = cell.gotoweight;

            return (r * 100) <= weight;
        }

        /// <summary>
        /// 再次访问概率判定是否去这组
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static bool WouldGotoGroupByAgainweight(int groupID)
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            int weight = 0;
            var cell = Config.groupConfig.getInstace().getCell(groupID);
            if (null == cell)
            {
                string e = string.Format("WouldGotoGroupByAgainweight 没找到组{0}", groupID);
                throw new System.Exception(e);
            }
            weight = cell.againweight;

            return (r * 100) <= weight;
        }

        public static bool AddVisitorToLittleZooApply(
            EntityVisitor entity, bool isFirstApply, Dictionary<int, LittleZoo> littleZooMap,
            ref int groupID, ref int littleZooID, ref bool isCrossGroup, List<int> openedLittleZooIDs, 
            List<int> freeLittleZooIDs, List<int> wouldGotoLittleZooIDs, 
            List<int> wouldGotoWeights, List<int> crossGroupIDs)
        {
            bool applyResult = false;
            int nextGroupID = Const.Invalid_Int;
            if (isFirstApply)
            {
                groupID = GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0];
                if (!crossGroupIDs.Contains(groupID))
                {
                    crossGroupIDs.Add(groupID);
                }
            }

            //判定goto概率
            applyResult = WouldGotoGroupByGotoweight(groupID);
            if (!applyResult)
            {
                return applyResult;
            }

            //获得这组开放的动物栏
            applyResult = GetOpenedLittleZooIDs(groupID, openedLittleZooIDs);
            if (!applyResult)
            {
                return applyResult;
            }

            applyResult = GetFreeLittleZooIDs(openedLittleZooIDs, littleZooMap, freeLittleZooIDs);
            if (!applyResult)
            {
                applyResult = GlobalDataManager.GetInstance().logicTableGroup.GetNextGroupID(groupID, ref nextGroupID);
                isCrossGroup = true;
                groupID = nextGroupID;

                if (!applyResult)
                {
                    return applyResult;
                }

                if (!crossGroupIDs.Contains(groupID))
                {
                    crossGroupIDs.Add(groupID);
                }

                if (!crossGroupIDs.Contains(nextGroupID))
                {
                    crossGroupIDs.Add(nextGroupID);
                }

                return AddVisitorToLittleZooApply(entity, false, littleZooMap, ref groupID, 
                    ref littleZooID, ref isCrossGroup, openedLittleZooIDs, freeLittleZooIDs, wouldGotoLittleZooIDs, 
                    wouldGotoWeights, crossGroupIDs);
            }

            //从有空位的中筛选出没有浏览过的
            applyResult = GetWouldGotoLittleZooIDs(entity, groupID, freeLittleZooIDs, wouldGotoLittleZooIDs);
            if (!applyResult)
            {
                applyResult = GlobalDataManager.GetInstance().logicTableGroup.GetNextGroupID(groupID, ref nextGroupID);
                isCrossGroup = true;
                groupID = nextGroupID;

                if (!applyResult)
                {
                    return applyResult;
                }

                if (!crossGroupIDs.Contains(groupID))
                {
                    crossGroupIDs.Add(groupID);
                }

                if (!crossGroupIDs.Contains(nextGroupID))
                {
                    crossGroupIDs.Add(nextGroupID);
                }

                return AddVisitorToLittleZooApply(entity, false, littleZooMap, ref groupID,
                    ref littleZooID, ref isCrossGroup, openedLittleZooIDs, freeLittleZooIDs, wouldGotoLittleZooIDs,
                    wouldGotoWeights, crossGroupIDs);
            }

            applyResult = GetWouldGotoWeights(groupID, wouldGotoLittleZooIDs, wouldGotoWeights);
#if UNITY_EDITOR
            if (!applyResult)
            {
                string e = string.Format("{0} GetWouldGotoWeights 异常", entity.entityID);
                throw new System.Exception(e);
            }
#endif
            int idx = Const.Invalid_Int;
            Math_F.TableProbability(wouldGotoWeights, ref idx);

            littleZooID = wouldGotoLittleZooIDs[idx];
            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} TableProbability group={1}, nextgroup={2} littlezoo={3}",
                entity.entityID, groupID, nextGroupID, littleZooID);
            return true;
        }

        /// <summary>
        /// 从空闲列表中剔除浏览过的
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="groupID"></param>
        /// <param name="freeLittleZooIDs"></param>
        /// <param name="wouldGotoLittleZooIDs"></param>
        /// <returns></returns>
        public static bool GetWouldGotoLittleZooIDs(EntityVisitor entity, int groupID, List<int> freeLittleZooIDs, List<int> wouldGotoLittleZooIDs)
        {
            wouldGotoLittleZooIDs.Clear();

            var visitedLittleZooIDs = EntityVisitor.GetVisitedLittleZooIDs(entity, groupID);
            int freeLittleZooID = Const.Invalid_Int;
            for(int i = 0; i < freeLittleZooIDs.Count; i++)
            {
                freeLittleZooID = freeLittleZooIDs[i];
                if (visitedLittleZooIDs == null || 
                    visitedLittleZooIDs.Count == 0 ||
                    visitedLittleZooIDs.IndexOf(freeLittleZooID) < 0)
                {
                    //没有浏览过
                    wouldGotoLittleZooIDs.Add(freeLittleZooID);
                }
            }

            return wouldGotoLittleZooIDs.Count > 0;
        }

        public static bool GetWouldGotoWeights(int groupID, List<int> wouldGotoLittleZooIDs, List<int> wouldGotoWeights)
        {
            wouldGotoWeights.Clear();
            var cell = Config.groupConfig.getInstace().getCell(groupID);
            int littleZooID = Const.Invalid_Int;
            int idx = Const.Invalid_Int;
            for (int i = 0; i < wouldGotoLittleZooIDs.Count; i++)
            {
                littleZooID = wouldGotoLittleZooIDs[i];
                idx = Const.Invalid_Int;
                for (int j = 0; j < cell.startid.Length; j++)
                {
                    if (littleZooID == cell.startid[j])
                    {
                        idx = j;
                        break;
                    }
                }

                if (idx != Const.Invalid_Int)
                {
                    wouldGotoWeights.Add(cell.groupweight[idx]);
                }
            }

#if UNITY_EDITOR
            if (wouldGotoWeights.Count != wouldGotoWeights.Count)
            {
                string e = string.Format("取 wouldGotoWeights 异常 {0}, {1}",
                    wouldGotoWeights.Count, wouldGotoWeights.Count);

                throw new System.Exception(e);
            }
#endif

            return wouldGotoWeights.Count > 0;
        }

        /// <summary>
        /// 获得开启的动物栏列表
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="openedLittleZooIDs"></param>
        /// <returns></returns>
        public static bool GetOpenedLittleZooIDs(int groupID, List<int> openedLittleZooIDs)
        {
            openedLittleZooIDs.Clear();
            var cell = Config.groupConfig.getInstace().getCell(groupID);
            int littleZooID = Const.Invalid_Int;
            int idx = Const.Invalid_Int;
            int level = Const.Invalid_Int;
            for (int i = 0; i < cell.startid.Length; i++)
            {
                littleZooID = cell.startid[i];
                bool retCode = GlobalDataManager.GetInstance().playerData.GetOpenedLittleZooIDIndexOfData(littleZooID, ref idx);
                if (retCode)
                {
                    //OPENed
                    openedLittleZooIDs.Add(littleZooID);
                }
            }
            

            return openedLittleZooIDs.Count > 0;
        }

        /// <summary>
        /// 从开启的动物栏中选择有空位的动物栏(等待位有空位)
        /// </summary>
        /// <param name="openedLittleZooIDs"></param>
        /// <param name="littleZooMap"></param>
        /// <param name="freeLittleZooIDs"></param>
        /// <returns></returns>
        public static bool GetFreeLittleZooIDs(List<int> openedLittleZooIDs, Dictionary<int, LittleZoo> littleZooMap, List<int> freeLittleZooIDs)
        {
            freeLittleZooIDs.Clear();
            LittleZoo littleZoo = null;
            int littleZooID = Const.Invalid_Int;
            for (int i = 0; i < openedLittleZooIDs.Count; i++)
            {
                littleZooID = openedLittleZooIDs[i];
                littleZooMap.TryGetValue(littleZooID, out littleZoo);
                if (littleZoo.IsFreeWaitQueue())
                {
                    freeLittleZooIDs.Add(littleZooID);
                }
            }

            return freeLittleZooIDs.Count > 0;
        }

        public static bool GenCrossLittleZooIDs(int sourLittleZooID, int destLittleZooID, List<int> crossGroupIDs, List<int> crossLittleZooIDs)
        {
            crossLittleZooIDs.Clear();
            crossLittleZooIDs.Add(sourLittleZooID);
            LogWarp.LogFormat("cross group {0}, littlezoo {1}", -1, sourLittleZooID);
            for (int i = 0; i < crossGroupIDs.Count; i++)
            {
                int openedLittleZooID = -1;
                GlobalDataManager.GetInstance().logicTableGroup.GetOpenedLittleZooID(
                    crossGroupIDs[i], ref openedLittleZooID);

                if (!crossLittleZooIDs.Contains(openedLittleZooID))
                {
                    crossLittleZooIDs.Add(openedLittleZooID);
                    LogWarp.LogFormat("cross group {0}, littlezoo {1}", crossGroupIDs[i], openedLittleZooID);
                }
            }
            
            if (crossLittleZooIDs[crossLittleZooIDs.Count - 1] != destLittleZooID)
            {
                crossLittleZooIDs.Add(destLittleZooID);
                LogWarp.LogFormat("cross group {0}, littlezoo {1}", -1, destLittleZooID);
            }

            return true;
        }

        /// <summary>
        /// 获取节点下的观光点技能CD对象，
        /// </summary>
        /// <param name="littleZooID">动物栏ID</param>
        /// <param name="vector">技能对象出现的位置</param>
        /// <returns></returns>
        public static GameObject GetVisitCDGameObject(int littleZooID, Vector3 vector,int idx)
        {
            GameObject visitCDPrefabs = GlobalDataManager.GetInstance().littleSceneUI.GetLittleZooVisitSeatGameObject(littleZooID,idx,0);

            if (visitCDPrefabs == null)
            {
                LogWarp.LogError("测试：visitCDPrefabs为空  littleZooID= "+ littleZooID+ "   idx"+ idx);
            }
            Vector3 excursionPosTion = new Vector3(Config.globalConfig.getInstace().CdTimeOffset_x, Config.globalConfig.getInstace().CdTimeOffset_y, Config.globalConfig.getInstace().CdTimeOffset_z);

            visitCDPrefabs.transform.position = vector+ excursionPosTion;
            GameObject skill = visitCDPrefabs.transform.Find("Text_UI").gameObject;
            float scale = Config.globalConfig.getInstace().VisitSeatCDScale;
            skill.transform.localScale = new Vector3(scale, scale, scale);
            Image image_Skill = skill.transform.Find("Image_Skill").GetComponent<Image>();
            image_Skill.fillAmount = 1f;
            return visitCDPrefabs;
        }

        /// <summary>
        /// 获取节点下的观光点飘字对象，
        /// </summary>
        /// <param name="littleZooID">动物栏ID</param>
        /// <param name="vector">技能对象出现的位置</param>
        /// <returns></returns>
        public static GameObject GetFlutterTextGameObject(int littleZooID, Vector3 vector,int idx)
        {
            GameObject SceneFlutterTextPrefabs = GlobalDataManager.GetInstance().littleSceneUI.GetLittleZooVisitSeatGameObject(littleZooID, idx,1);

            
            Text earnings = SceneFlutterTextPrefabs.transform.Find("SceneFlutterText01").GetComponent<Text>();
#if NO_BIGINT
            earnings.text = "+" + "2.71ab";
#else
            int zooLevel = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(littleZooID).littleZooTicketsLevel;
            earnings.text = "+"+MinerBigInt.ToDisplay(GetLittleZooPrice(littleZooID,zooLevel));
#endif
            Animator m_Animator = SceneFlutterTextPrefabs.transform.Find("SceneFlutterText01").GetComponent<Animator>();
            m_Animator.enabled = true;
            m_Animator.Play("UINumber", 0, 0f);
            return SceneFlutterTextPrefabs;
           
        }

    }
}
