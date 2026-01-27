/*******************************************************************
* FileName:     LittleZoo.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-17
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace ZooGame
{
    /// <summary>
    /// 动物栏抽象
    /// </summary>
    public class LittleZoo
    {
        /// <summary>
        /// 动物栏ID
        /// </summary>
        public int littleZooID;

        /// <summary>
        /// 内置点,编辑器导出后管理器可查
        /// </summary>
        public LittleZooBuildinPos buildinPos;

        /// <summary>
        /// 游览CD(毫秒)
        /// </summary>
        public int visitCDValue;

        /// <summary>
        /// 观光位上最大的游客数
        /// </summary>
        public int maxLenthOfVisitQueue;

        /// <summary>
        /// 观光位排队
        /// </summary>
        public List<int> visitQueue;

        /// <summary>
        /// 等待位上最大的游客数
        /// </summary>
        public int maxLenthOfWaitQueue;

        /// <summary>
        /// 等待位排队
        /// </summary>
        public List<int> waitQueue;

        public LittleZoo(int littleZooID, LittleZooBuildinPos buildinPos, int visitCDValue, int maxNumOfVisitor, int maxNumOfWaitVisitor)
        {
            this.littleZooID = littleZooID;
            this.buildinPos = buildinPos;
            this.visitCDValue = visitCDValue;
            this.maxLenthOfVisitQueue = maxNumOfVisitor;
            this.maxLenthOfWaitQueue = maxNumOfWaitVisitor;
            visitQueue = new List<int>(maxLenthOfVisitQueue);
            for(int i = 0; i < maxLenthOfVisitQueue; i++)
            {
                visitQueue.Add(Const.Invalid_Int);
            }
            waitQueue = new List<int>(maxLenthOfWaitQueue);
            for (int i = 0; i < maxLenthOfWaitQueue; i++)
            {
                waitQueue.Add(Const.Invalid_Int);
            }
        }

        public void Realse()
        {
            waitQueue.Clear();
            visitQueue.Clear();
            buildinPos.Realse();
        }

        public int AddVisitorToVisitQueue(int entityID, int idx)
        {
#if UNITY_EDITOR
            if (idx < 0 || idx >= this.maxLenthOfVisitQueue || idx >= maxLenthOfWaitQueue || idx > visitQueue.Count)
            {
                string e = string.Format("异常等待索引 {0}", idx);
                throw new System.Exception(e);
            }
            if (entityID == Const.Invalid_Int)
            {
                string e = "entityID 异常";
                throw new System.Exception(e);
            }
#endif
            if (visitQueue[idx]  != Const.Invalid_Int)
            {
                string e = "观光位还有人";
                throw new System.Exception(e);
            }
            visitQueue[idx] = entityID;

            return idx;

        }

        public int RemoveVisitorFromVisitQueue(int entityID)
        {
#if UNITY_EDITOR
            if (entityID == Const.Invalid_Int)
            {
                string e = "entityID 异常";
                throw new System.Exception(e);
            }
#endif

            int idx = visitQueue.IndexOf(entityID);

#if UNITY_EDITOR
            if (idx < 0)
            {
                for (int i = 0; i < visitQueue.Count; i++)
                {
                    Logger.LogWarp.LogError(visitQueue[i]);
                }
                string e = string.Format("观光位上没有这个游客 {0}", entityID);
                throw new System.Exception(e);
            }
#endif
            visitQueue[idx] = Const.Invalid_Int;

            return idx;
        }

        /// <summary>
        /// 等待位加入，返回位置
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public bool AddVisitorToWaitQueue(int entityID, ref int indexInQueue)
        {
#if UNITY_EDITOR
            if (entityID == Const.Invalid_Int)
            {
                string e = "entityID 异常";
                throw new System.Exception(e);
            }
#endif
            for (int i = 0; i < waitQueue.Count; i++)
            {
                if (waitQueue[i] == Const.Invalid_Int)
                {
                    waitQueue[i] = entityID;
                    indexInQueue = i;
                    return true;
                }
            }

            return true;
            //waitQueue.Add(entityID);
            //return waitQueue.Count - 1;
        }

        public int RemoveVisitorFromWaitQueue(int entityID)
        {
            if (entityID == Const.Invalid_Int)
            {
                string e = "entityID 异常";
                throw new System.Exception(e);
            }

            int idx = waitQueue.IndexOf(entityID);
            if (idx < 0)
            {
                string e = string.Format("等待位上没有这个游客 {0}", entityID);
                throw new System.Exception(e);
            }

            waitQueue[idx] = Const.Invalid_Int;

            return idx;
        }

        public bool IsFreeWaitQueue()
        {
            //看现有空出来的位置 (-1的)
            //if (this.waitQueue.Count <= this.maxLenthOfWaitQueue)
            {
                for (int i = 0; i < this.waitQueue.Count; i++)
                {
                    if (this.waitQueue[i] == Const.Invalid_Int)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsAtFreeVisitQueue(int idx)
        {
#if UNITY_EDITOR
            if (idx < 0 || idx >= this.maxLenthOfVisitQueue || idx >= maxLenthOfWaitQueue)
            {
                string e = string.Format("{0}异常等待索引 {1}, {2}, {3}", 
                    this.littleZooID, idx, maxLenthOfVisitQueue, maxLenthOfWaitQueue);
                throw new System.Exception(e);
            }
#endif
            //观光位有这个索引, 且是-1
            if (idx < this.visitQueue.Count)
            {
                if (this.visitQueue[idx] == Const.Invalid_Int)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFreeVisitQueue(ref int idx)
        {
            for (int i = 0; i < this.visitQueue.Count; i++)
            {
                if (this.visitQueue[i] == Const.Invalid_Int)
                {
                    idx = i;
                    return true;
                }
            }

            return false;
        }
    }

}
