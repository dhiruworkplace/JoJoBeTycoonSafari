using Logger;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    public class EntryGate
    {
        /// <summary>
        /// 入口ID
        /// </summary>
        public int entryID;

        /// <summary>
        /// 入口排队
        /// </summary>
        //public Queue<int> visitorQueue = new Queue<int>();
        public List<int> visitorQueue;

        /// <summary>
        /// 出口checkin时长
        /// </summary>
        public int checkInCDVal;

        /// <summary>
        /// 进入的路
        /// </summary>
        public string pathName;

        /// <summary>
        /// 排队最大人数
        /// </summary>
        public int maxNumOfQueue;

        /// <summary>
        /// 当前排队人数
        /// </summary>
        public int numOfQueue = 0;

        /// <summary>
        /// 入口等级
        /// </summary>
        public int level = 0;

        public EntryGate(int entryID, int level, int checkInCDVal, string pathName, int maxNumOfQueues)
        {
            this.entryID = entryID;
            this.level = level;
            this.checkInCDVal = checkInCDVal;
            this.pathName = pathName;
            this.maxNumOfQueue = maxNumOfQueues;
            visitorQueue = new List<int>(maxNumOfQueues);
            for(int i = 0; i < this.maxNumOfQueue; i++)
            {
                visitorQueue.Add(Const.Invalid_Int);
            }
        }

        public void Release()
        {
            visitorQueue.Clear();
        }

        public bool IsQueueFull()
        {
            for (int i = 0; i < visitorQueue.Count; ++i)
            {
                if (visitorQueue[i] == Const.Invalid_Int)
                {
                    return false;
                }
            }

            return true;
        }

        public int AddVisitorToQueue(int entityID)
        {
            for(int i = 0; i < visitorQueue.Count; i++)
            {
                //占位转正式位
                if (visitorQueue[i] == GameConst.Place_Holder_ID)
                {
                    visitorQueue[i] = entityID;
                    ++numOfQueue;
                    return numOfQueue - 1;

                }
            }

            return Const.Invalid_Int;
        }

        public void AddVisitorPlaceHolder(int entityID)
        {
            LogWarp.LogFormat("{0} AddVisitorPlaceHolder {1}", entityID, entryID);
            //空位转占位
            //int idx = visitorQueue.IndexOf(Const.Invalid_Int);
            //if (idx >=0)
            //{
            //    visitorQueue[idx] = GameConst.Place_Holder_ID;
            //    return;
            //}
            for (int i = 0; i < visitorQueue.Count; i++)
            {
                if (visitorQueue[i] == Const.Invalid_Int)
                {
                    visitorQueue[i] = GameConst.Place_Holder_ID;
                    return;
                }
            }

#if UNITY_EDITOR
            string e = string.Format("入口{0}, 没有空位, {1}不能去占位", this.entryID, entityID);
            throw new System.Exception(e);
#endif
        }

        public bool RemoveVisitorFromQueue(int entityID)
        {
            int idx = visitorQueue.IndexOf(entityID);
            if (idx <0)
            {
                return false;
            }

            visitorQueue[idx] = Const.Invalid_Int;
            --numOfQueue;
            return true;
        }
    }
}
