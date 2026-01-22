using UFrame;
using UFrame.BehaviourFloat;
using UFrame.EntityFloat;
using Logger;
using System.Collections.Generic;
using UnityEngine;
using Game.GlobalData;
using Game.Path.StraightLine;

namespace Game
{
    public partial class EntityVisitor : EntityMovable
    {
        public static bool IsApplyTooMuch(int groupID, Dictionary<int, int> applyRecord)
        {
            List<int> sortedLittleZooID = null;
            if (!GlobalDataManager.GetInstance().logicTableGroup.sortedLittleZooIDs.TryGetValue(groupID, out sortedLittleZooID))
            {
                string e = string.Format("查不到 sortedLittleZooID {0}", groupID);
                throw new System.Exception(e);
            }

            int applyTime = 0;
            if (!applyRecord.TryGetValue(groupID, out applyTime))
            {
                applyTime = 0;
            }

            return applyTime >= sortedLittleZooID.Count;
        }

        ////所在组是否都浏览过
        //public static bool IsVisitedAll(EntityVisitor entity, int GroupID, bool allowNoVisited = true)
        //{
        //    List<int> littleZooList = null;
        //    if (!entity.visitedGroupMap.TryGetValue(entity.wouldGotoBuildingGroupID, out littleZooList))
        //    {
        //        //都查不到组数据，肯定没浏览过
        //        if (allowNoVisited)
        //        {
        //            return false;
        //        }
        //        string e = string.Format("查不到浏览数据 {0}", entity.wouldGotoBuildingGroupID);
        //        throw new System.Exception(e);
        //    }
        //    if (littleZooList.Count <= 0)
        //    {
        //        if (allowNoVisited)
        //        {
        //            return false;
        //        }
        //        string e = string.Format("查不到浏览数据 {0}", entity.wouldGotoBuildingGroupID);
        //        throw new System.Exception(e);
        //    }

        //    List<int> sortedLittleZooID = null;
        //    if (!GlobalDataManager.GetInstance().logicTableGroup.sortedLittleZooIDs.TryGetValue(entity.wouldGotoBuildingGroupID, out sortedLittleZooID))
        //    {
        //        string e = string.Format("查不到 sortedLittleZooID {0}", entity.wouldGotoBuildingGroupID);
        //        throw new System.Exception(e);
        //    }

        //    if (littleZooList.Count > sortedLittleZooID.Count)
        //    {
        //        string e = string.Format("浏览数据异常 {0}", entity.wouldGotoBuildingGroupID);
        //        throw new System.Exception(e);
        //    }

        //    if (littleZooList.Count == sortedLittleZooID.Count)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        public static void GotoStartOfPath(EntityVisitor entity, string pathName)
        {
            var path = PathManager.GetInstance().GetPath(pathName);
            entity.pathList.Clear();
            entity.pathList.Add(entity.position);
            entity.pathList.Add(path[0]);
            entity.followPath.Init(entity, entity.pathList, entity.pathList[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
            LogWarp.LogFormat("GotoStartOfPath {0}", pathName);
        }

        public static void GodownPath(EntityVisitor entity, string pathName, bool isMoveToBegin = false)
        {
            var path = PathManager.GetInstance().GetPath(pathName);

            if (path == null || path.Count <=0)
            {
                string e = string.Format("路 {0} 数据异常", pathName);
                throw new System.Exception(e);
            }
            if (isMoveToBegin)
            {
                entity.position = path[0];
            }
            entity.followPath.Init(entity, path, path[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
            LogWarp.LogFormat("GodownPath {0}", pathName);
        }

        public static void GodownReversePath(EntityVisitor entity, string pathName, bool isMoveToBegin = false)
        {
            var path = PathManager.GetInstance().GetPath(pathName, true);

            if (path == null || path.Count <= 0)
            {
                string e = string.Format("路 {0} 数据异常", pathName);
                throw new System.Exception(e);
            }
            if (isMoveToBegin)
            {
                entity.position = path[0];
            }
            entity.followPath.Init(entity, path, path[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
            LogWarp.LogFormat("GodownPath {0}", pathName);
        }

        public static void GodownPath(EntityVisitor entity, List<Vector3> path, bool isMoveToBegin = false)
        {
            if (path == null || path.Count <= 0)
            {
                string e = string.Format("路数据异常");
                throw new System.Exception(e);
            }
            if (isMoveToBegin)
            {
                entity.position = path[0];
            }
            entity.followPath.Init(entity, path, path[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
            LogWarp.LogFormat("GodownPath ");
        }

        public static void GotoNextPos(EntityVisitor entity, Vector3 pos)
        {
            entity.pathList.Clear();
            entity.pathList.Add(entity.position);
            entity.pathList.Add(pos);
            entity.followPath.Init(entity, entity.pathList, entity.pathList[0], 0, entity.moveSpeed, false);
            entity.followPath.Run();
            LogWarp.Log("GotoNextPos ");
        }

        public static string GetPath(int startID, int endID)
        {
            string path = null;
            foreach (var kv in Config.pathConfig.getInstace().AllData)
            {
                var cell = kv.Value;
                if (cell.startid == startID && cell.endid == endID)
                {
                    path = cell.path;
                    break;
                }
            }

            return path;
        }

        public static void RecordVisitedLittleZoo(EntityVisitor entity, int groupID, int littleZooID)
        {
            List<int> littleZooList = null;
            if (!entity.visitedGroupMap.TryGetValue(groupID, out littleZooList))
            {
                littleZooList = new List<int>();
                entity.visitedGroupMap.Add(groupID, littleZooList);
            }
            if (littleZooList.Contains(littleZooID))
            {
                string e = string.Format("{0} 重复记录观光 {1}", entity.entityID, littleZooID);
                throw new System.Exception(e);
            }
            littleZooList.Add(littleZooID);
        }

        public static List<int> GetVisitedLittleZooIDs(EntityVisitor entity, int groupID)
        {
            List<int> littleZooList = null;

            entity.visitedGroupMap.TryGetValue(groupID, out littleZooList);

            return littleZooList;
        }






    }
}

