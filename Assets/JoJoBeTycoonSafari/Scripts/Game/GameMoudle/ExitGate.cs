using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    public class ExitGate
    {
        /// <summary>
        /// 入口ID
        /// </summary>
        public int entryID;

        /// <summary>
        /// 入口排队
        /// </summary>
        public Queue<int> visitorQueue = new Queue<int>();

        /// <summary>
        /// 出口checkin时长
        /// </summary>
        public int checkInCDVal;

        /// <summary>
        /// 左边开始到入口的路名
        /// </summary>
        public string leftEntryPathName;

        /// <summary>
        /// 右边开始到入口的路名
        /// </summary>
        public string rightEntryPathName;

        /// <summary>
        /// 排队最大人数
        /// </summary>
        public int maxNumOfQueues;

        public ExitGate(int entryID, int checkInCDVal, string leftEntryPathName, string rightEntryPathName, int maxNumOfQueues)
        {
            this.entryID = entryID;
            this.checkInCDVal = checkInCDVal;
            this.leftEntryPathName = leftEntryPathName;
            this.rightEntryPathName = rightEntryPathName;
            this.maxNumOfQueues = maxNumOfQueues;
        }

        public void Realse()
        {
            visitorQueue.Clear();
        }

        public bool IsQueueFull()
        {
            return !(this.visitorQueue.Count < maxNumOfQueues);
        }

        public int GetQueueCount()
        {
            return this.visitorQueue.Count;
        }

        public void AddVisitorToQueue(int entityID)
        {
#if UNITY_EDITOR
            string beforeIDs = "(";
            foreach (var val in visitorQueue)
            {
                beforeIDs += val.ToString() + ",";
            }
            beforeIDs += ")";
            DebugFile.GetInstance().WriteKeyFile(entityID, "{0} AddVisitorToQueue beforeIDs={1}", entityID, beforeIDs);
#endif
            visitorQueue.Enqueue(entityID);

        }

        public int RemoveVisitorFromQueue()
        {
            return visitorQueue.Dequeue();
        }

        public int GetLastIndexInQueue()
        {
            return visitorQueue.Count - 1;
        }

    }
}
