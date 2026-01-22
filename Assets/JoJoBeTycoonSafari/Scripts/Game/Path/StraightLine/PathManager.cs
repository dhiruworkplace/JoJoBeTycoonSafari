/*******************************************************************
* FileName:     PathManager.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-7
* Description:  
* other:    
********************************************************************/


using Game.MiniGame;
using System.Collections.Generic;
using UFrame.Common;
using UFrame.Path.StraightLine;
using UnityEngine;

namespace Game.Path.StraightLine
{
    public partial class PathManager : Singleton<PathManager>, ISingleton
    {
        Dictionary<string, List<Vector3>> paths = null;

        //List<Vector3> reversePath = null;

        public void Init()
        {
            paths = new Dictionary<string, List<Vector3>>();
            //reversePath = new List<Vector3>();
            this.AddAllPath();
        }

        public List<Vector3> GetPath(string pathName, bool reverse = false)
        {
            List<Vector3> path = null;
            paths.TryGetValue(pathName, out path);

            if (!reverse)
            {
                return path;
            }

            if (path == null)
            {
                return null;
            }
            List<Vector3> reversePath = new List<Vector3>();
            reversePath.AddRange(path);
            reversePath.Reverse();

            return reversePath;
        }

        public void AddPath(string pathName, List<Vector3> path)
        {
            paths.Add(pathName, path);
        }

        //public List<Vector3> GetReversePath(string pathName)
        //{
        //    List<Vector3> path = null;
        //    path = GetPath(pathName);
        //    if (path == null)
        //    {
        //        return null;
        //    }

        //    reversePath.Clear();
        //    reversePath.AddRange(path);
        //    reversePath.Reverse();

        //    return reversePath;
        //}

        public bool ModifyPath(string pathName, Vector3 offset)
        {
            var pathList = GetPath(pathName);
            if (pathList == null || pathList.Count ==0)
            {
                return false;
            }

            for(int i = 0; i < pathList.Count; i++)
            {
                pathList[i] += offset;
            }

            return true;
        }

        public bool GetPathLastPos(string pathName, ref Vector3 pos)
        {
            var pathList = GetPath(pathName);
            if (pathList == null || pathList.Count == 0)
            {
                return false;
            }

            pos = pathList[pathList.Count - 1];
            return true;
        }

        public bool GetPathFirstPos(string pathName, ref Vector3 pos)
        {
            var pathList = GetPath(pathName);
            if (pathList == null || pathList.Count == 0)
            {
                return false;
            }

            pos = pathList[0];
            return true;
        }

    }
}

