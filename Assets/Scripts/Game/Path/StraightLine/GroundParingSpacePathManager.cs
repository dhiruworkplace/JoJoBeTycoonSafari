using Game.GlobalData;
using Game.MiniGame;
using Logger;
using System.Collections.Generic;
using UFrame.BehaviourFloat;
using UFrame.Common;
using UFrame.Path.StraightLine;
using UnityEngine;

namespace Game.Path.StraightLine
{
    public enum GroundParingSpacePathType
    {
        /// <summary>
        /// 进停车位的路线
        /// </summary>
        InPath,

        InPathRac,
        EntryObservePath,
        InPathReverse,
        InPathBackRac,
        OutPath,

        /// <summary>
        /// 离开的路的控制点
        /// </summary>
        OutPathRac,

        /// <summary>
        /// 游客离开走向停车位
        /// </summary>
        VisitorBackPath,
    }
    /// <summary>
    /// 一个停车位的路线集
    /// </summary>
    public class GroundParingSpacePathUnit
    {
        /// <summary>
        /// 进停车位的路线
        /// </summary>
        public List<Vector3> inPath = new List<Vector3>();

        /// <summary>
        /// 进停车位的路线的控制点
        /// </summary>
        public List<RightAnglesControllNode> inPathRac = new List<RightAnglesControllNode>();

        /// <summary>
        /// 入口排队观察路线
        /// </summary>
        public List<Vector3> entryObservePath = new List<Vector3>();

        /// <summary>
        /// 进来的路的反路 倒车用
        /// </summary>
        public List<Vector3> inPathReverse = new List<Vector3>();

        /// <summary>
        /// 倒车控制点 倒车用
        /// </summary>
        public List<RightAnglesControllNode> inPathBackRac = new List<RightAnglesControllNode>();

        /// <summary>
        /// 离开的路
        /// </summary>
        public List<Vector3> outPath = new List<Vector3>();

        /// <summary>
        /// 离开的路的控制点
        /// </summary>
        public List<RightAnglesControllNode> outPathRac = new List<RightAnglesControllNode>();

        /// <summary>
        /// 游客离开动物走向停车位路线
        /// </summary>
        public List<Vector3> visitorBackPath = new List<Vector3>();
    }

    public partial class GroundParingSpacePathManager : Singleton<GroundParingSpacePathManager>, ISingleton
    {
        public void Init()
        {

        }

        Dictionary<int, Dictionary<int, GroundParingSpacePathUnit>> pathUnitTowLevelDic = new Dictionary<int, Dictionary<int, GroundParingSpacePathUnit>>();

        public GroundParingSpacePathUnit GetPathUnit(int groupID, int idx)
        {
            GroundParingSpacePathUnit pathUnit = null;

            Dictionary<int, GroundParingSpacePathUnit> groupPath = null;
            //如果没有找到组,创建组,并把组内的idx的指向的那个也创建
            if (!pathUnitTowLevelDic.TryGetValue(groupID, out groupPath))
            {
                groupPath = new Dictionary<int, GroundParingSpacePathUnit>();
                pathUnit = new GroundParingSpacePathUnit();
                groupPath.Add(idx, pathUnit);
                pathUnitTowLevelDic.Add(groupID, groupPath);

                return pathUnit;
            }

            //如果找到组，但没有找到组内idx,把组内的idx的指向的那个也创建
            if (!groupPath.TryGetValue(idx, out pathUnit))
            {
                pathUnit = new GroundParingSpacePathUnit();
                groupPath.Add(idx, pathUnit);
            }

            return pathUnit;
        }

        public void AddPath(GroundParingSpacePathType pathType, GroundParingSpacePathUnit pathUnit, List<Vector3> path, List<RightAnglesControllNode> rac)
        {
            switch (pathType)
            {
                case GroundParingSpacePathType.InPath:
                    pathUnit.inPath = path;
                    break;
                case GroundParingSpacePathType.InPathRac:
                    pathUnit.inPathRac = rac;
                    break;
                case GroundParingSpacePathType.EntryObservePath:
                    pathUnit.entryObservePath = path;
                    break;
                case GroundParingSpacePathType.InPathReverse:
                    pathUnit.inPathReverse = path;
                    break;
                case GroundParingSpacePathType.InPathBackRac:
                    pathUnit.inPathBackRac = rac;
                    break;
                case GroundParingSpacePathType.OutPath:
                    pathUnit.outPath = path;
                    break;
                case GroundParingSpacePathType.OutPathRac:
                    pathUnit.outPathRac = rac;
                    break;
                case GroundParingSpacePathType.VisitorBackPath:
                    pathUnit.visitorBackPath = path;
                    break;
                default:
                    string e = string.Format("地面停车场路径单元不支持这种路{0}", pathType);
                    throw new System.Exception(e);
            }
        }

        public static bool IsExist(List<Vector3> path)
        {
            return (path != null) && (path.Count > 0);
        }

        public static bool IsExist(List<RightAnglesControllNode> rac)
        {
            return (rac != null) && (rac.Count > 0);
        }

        public static List<Vector3> GenInPath(int groupID, int idx, float roadOffset, float psHeight, float psOffset, int psDir)
        {
            var basePath = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseInPath);

            var baseLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseInLine);
            return GenInPath(basePath, baseLine, groupID, idx, roadOffset, psHeight, psOffset, psDir);
        }

        public static List<Vector3> GenInPath(List<Vector3> basePath, List<Vector3> baseLine,
            int groupID, int idx, float roadOffset, float psHeight, float psOffset, int psDir)
        {
            List<Vector3> result = new List<Vector3>();
            result.AddRange(basePath);
#if UNITY_EDITOR
            if (groupID < 0 || groupID >= baseLine.Count)
            {
                string e = string.Format("不存在的地面GroupID={0}", groupID);
                throw new System.Exception(e);
            }
#endif 
            result.Add(baseLine[groupID]);

            var lastCorner = baseLine[groupID] - GlobalData.GlobalDataManager.GetInstance().SceneForward * (roadOffset + idx * psHeight);
            result.Add(lastCorner);

            var end = lastCorner;
            end.z += psDir * psOffset;
            result.Add(end);

            return result;
        }

        public static List<RightAnglesControllNode> GenRAC(List<Vector3> pathPosList, float turnOrgOffset)
        {
            List<RightAnglesControllNode> ctrList = new List<RightAnglesControllNode>();
            int posLen = pathPosList.Count;
            if (posLen < 2)
            {
                return ctrList;
            }

            for (int i = 1; i < posLen - 1; i++)
            {
                //前点
                var forwardDir = (pathPosList[i + 1] - pathPosList[i]).normalized;
                var forwardPos = pathPosList[i] + forwardDir * turnOrgOffset;
#if DEBUG_RAC
                var forwardGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                forwardGo.transform.position = forwardPos + new Vector3(0, 8, 0);
                forwardGo.name = string.Format("ctr_forward_{0}", i);
#endif
                //后点
                var backDir = (pathPosList[i] - pathPosList[i - 1]).normalized;
                var backPos = pathPosList[i] - backDir * turnOrgOffset;
#if DEBUG_RAC
                var backGo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                backGo.transform.position = backPos + new Vector3(0, 8, 0);
                backGo.name = string.Format("ctr_back_{0}", i);
#endif
                //旋转原点
                var cross = Vector3.Cross(forwardDir, backDir);
                short turnSign = 1;
                if (cross.y > 0)
                {
                    turnSign = -1;
                }
                var turnOrgDir = Quaternion.Euler(0, UFrame.Const.RightAngles * turnSign, 0) * backDir;
                var turnOrgPos = backPos + turnOrgDir * turnOrgOffset;
#if DEBUG_RAC
                var turnOrgGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                turnOrgGo.transform.position = turnOrgPos + new Vector3(0, 8, 0);
                turnOrgGo.name = string.Format("ctr_turnOrg{0}", i);
#endif
                var ctr = new RightAnglesControllNode();
                ctr.forwardPos = forwardPos;
                ctr.backPos = backPos;
                ctr.turnOrg = turnOrgPos;
                ctr.turnSign = turnSign;
                ctrList.Add(ctr);

                ////旋转点
                //var ctrForward = backNode - ctrNode;
                //float turnAngle = 30f;
                //turnSignList[idxCtr - 1] = 1;
                //if (cross.y > 0)
                //{
                //    turnAngle = -turnAngle;
                //    turnSignList[idxCtr - 1] = -1;
                //}
                //var turnDir = Quaternion.Euler(0, turnAngle, 0) * ctrForward;
                //var turnNode = ctrNode + turnDir;
                //var turnGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //turnGo.transform.position = turnNode + new Vector3(0, 8, 0);
                //turnGo.name = string.Format("turn_{0}", i);
            }

            return ctrList;
        }

        public static List<Vector3> GenReversePath(List<Vector3> path)
        {
            var reversePath = new List<Vector3>();
            int pathLen = path.Count;
            int reversePathLen = 2;
            for (int i = 0; i < reversePathLen; i++)
            {
                reversePath.Add(path[pathLen - 1 - i]);
            }
            reversePath.Add(path[pathLen - 1 - 1] + GlobalDataManager.GetInstance().SceneForward * (Config.globalConfig.getInstace().CarRightAnglesRadius + 1));
            return reversePath;
        }

        public static List<RightAnglesControllNode> GenBackRac(List<RightAnglesControllNode> inRac)
        {
            List<RightAnglesControllNode> outRac = new List<RightAnglesControllNode>();
            RightAnglesControllNode ctrNode = new RightAnglesControllNode(inRac[inRac.Count - 1]);
            var tmp = ctrNode.backPos;
            ctrNode.backPos = ctrNode.forwardPos;
            ctrNode.forwardPos = tmp;
            ctrNode.turnSign *= -1;
            outRac.Add(ctrNode);
            return outRac;
        }

        public static List<Vector3> GenObservePath(int groupID, int idx)
        {
            List<Vector3> path = new List<Vector3>();
            var carBaseLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseInLine); 
            var carBaseLinePos = carBaseLine[groupID];
            var lastCorner = carBaseLinePos - GlobalDataManager.GetInstance().SceneForward * (Config.globalConfig.getInstace().GroundParkingFristSpaceOffset
                + idx * Config.globalConfig.getInstace().GroundParkingSpace);
            
            var visitorBaseLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingVisitorObserveBaseLine);
            var visitorBaseLinePos = visitorBaseLine[groupID];
            path.Add(lastCorner);
            path.Add(visitorBaseLinePos);
            //LogWarp.LogErrorFormat("GenObservePath {0}, {1}, {2}, {3}", groupID, idx, lastCorner, visitorBaseLinePos);
            return path;
        }

        public static List<Vector3> GenOutPath(int groupID, int idx)
        {
            List<Vector3> path = new List<Vector3>();
            var carInBaseLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseInLine);
            var carBaseLinePos = carInBaseLine[groupID];
            
            var lastCorner = carBaseLinePos - GlobalDataManager.GetInstance().SceneForward * (Config.globalConfig.getInstace().GroundParkingFristSpaceOffset
                + idx * Config.globalConfig.getInstace().GroundParkingSpace);

            var start = lastCorner + GlobalDataManager.GetInstance().SceneForward * (Config.globalConfig.getInstace().CarRightAnglesRadius + 1);
            path.Add(start);
            var carOutBaseLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseOutLine);
            path.Add(carOutBaseLine[groupID]);
            var carOutBasePath = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseOutPath);
            path.AddRange(carOutBasePath);
            return path;
        }

        public static List<Vector3> GenVisitorBackPath(int groupID, int idx)
        {
            List<Vector3> path = new List<Vector3>();
            Vector3 start = Vector3.zero;
            PathManager.GetInstance().GetPathLastPos(Config.globalConfig.getInstace().GroundParkingVistorBasePath, ref start);
            path.Add(start);
            //观察点
            var baseObLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingVisitorObserveBaseLine);
            path.Add(baseObLine[groupID]);
            //终点
            var baseInLine = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().GroundParkingBaseInLine);
            var end = baseInLine[groupID] - GlobalDataManager.GetInstance().SceneForward * (
                Config.globalConfig.getInstace().GroundParkingFristSpaceOffset + idx * Config.globalConfig.getInstace().GroundParkingSpace);
            path.Add(end);
            return path;
        }
    }

}
