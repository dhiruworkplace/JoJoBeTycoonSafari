using Game.MiniGame;
using System.Collections.Generic;
using UFrame.BehaviourFloat;
using UFrame.Common;
using UFrame.Path.StraightLine;
using UnityEngine;

namespace Game.Path.StraightLine
{
    public class CalcPathManager : Singleton<CalcPathManager>, ISingleton
    {
        /// <summary>
        /// 路的控制点
        /// </summary>
        Dictionary<string, List<RightAnglesControllNode>> racDic = new Dictionary<string, List<RightAnglesControllNode>>();

        /// <summary>
        /// 路的反路
        /// </summary>
        Dictionary<string, List<Vector3>> reversePathDic = new Dictionary<string, List<Vector3>>();

        /// <summary>
        /// 倒车控制点
        /// </summary>
        Dictionary<string, List<RightAnglesControllNode>> outParkingRacDic = new Dictionary<string, List<RightAnglesControllNode>>();


        Dictionary<string, List<Vector3>> normalPathDic = new Dictionary<string, List<Vector3>>();

        public void Init()
        {

        }

        public List<RightAnglesControllNode> GetRac(string pathName)
        {
            List<RightAnglesControllNode> racList = null;

            racDic.TryGetValue(pathName, out racList);

            return racList;
        }

        public void AddRac(string pathName, List<RightAnglesControllNode> racList)
        {
#if UNITY_EDITOR
            if (racDic.ContainsKey(pathName))
            {
                string e = string.Format("重复添加RAC {0}", pathName);
                throw new System.Exception(e);
            }
#endif
            var _racList = new List<RightAnglesControllNode>();
            _racList.AddRange(racList);
            racDic.Add(pathName, _racList);
        }

        public List<Vector3> GetReversePath(string pathName)
        {
            List<Vector3> reversePath = null;

            reversePathDic.TryGetValue(pathName, out reversePath);

            return reversePath;
        }

        public void AddReversePath(string pathName, List<Vector3> path)
        {
#if UNITY_EDITOR
            if (reversePathDic.ContainsKey(pathName))
            {
                string e = string.Format("重复添加ReversePath {0}", pathName);
                throw new System.Exception(e);
            }
#endif
            var _path = new List<Vector3>();
            _path.AddRange(path);
            reversePathDic.Add(pathName, _path);
        }

        public List<RightAnglesControllNode> GetOutParingRac(string pathName)
        {
            List<RightAnglesControllNode> racList = null;

            outParkingRacDic.TryGetValue(pathName, out racList);

            return racList;
        }

        public void AddOutParingRac(string pathName, List<RightAnglesControllNode> racList)
        {
#if UNITY_EDITOR
            if (outParkingRacDic.ContainsKey(pathName))
            {
                string e = string.Format("重复添加AddOutParingRac {0}", pathName);
                throw new System.Exception(e);
            }
#endif
            var _racList = new List<RightAnglesControllNode>();
            _racList.AddRange(racList);
            outParkingRacDic.Add(pathName, _racList);
        }

        public static List<Vector3> GenInGroundParkingSpacePath(int groupID, int idx, float roadOffset, float psHeight, float psOffset, int psDir)
        {
            var basePath = PathManager.GetInstance().GetPath("path_touristcar_intoup1");
            var baseLine = PathManager.GetInstance().GetPath("path_touristcar_intoupline");
            return GenInGroundParkingSpacePath(basePath, baseLine, groupID, idx, roadOffset, psHeight, psOffset, psDir);
        }

        public static List<Vector3> GenInGroundParkingSpacePath(List<Vector3> basePath, List<Vector3> baseLine,
            int groupID, int idx, float roadOffset, float psHeight, float psOffset, int psDir)
        {
            List<Vector3> result = new List<Vector3>();
            result.AddRange(basePath);
            result.Add(baseLine[groupID]);

            var lastCorner = baseLine[groupID] - GlobalData.GlobalDataManager.GetInstance().SceneForward * (roadOffset + idx * psHeight);
            result.Add(lastCorner);

            var end = lastCorner;
            end.z += psDir * psOffset;
            result.Add(end);

            return result;
        }

        public List<Vector3> GetNormalPath(string pathName)
        {
            List<Vector3> normalPath = null;

            normalPathDic.TryGetValue(pathName, out normalPath);

            return normalPath;
        }

        public void AddNormalPath(string pathName, List<Vector3> path)
        {
#if UNITY_EDITOR
            if (normalPathDic.ContainsKey(pathName))
            {
                string e = string.Format("重复添加NormalPath {0}", pathName);
                throw new System.Exception(e);
            }
#endif
            var _path = new List<Vector3>();
            _path.AddRange(path);
            normalPathDic.Add(pathName, _path);
        }
    }
}
