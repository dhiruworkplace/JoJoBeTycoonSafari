/*******************************************************************
* FileName:     ZooEntry.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-13
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 动物园入口抽象
    /// </summary>
    public class ZooEntry : TickBase
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
        /// 入口CD
        /// </summary>
        public IntCD entryCD;

        /// <summary>
        /// 下车后到入口的路径名
        /// </summary>
        public string entryPathNameFromCar;

        /// <summary>
        /// 排队最大人数
        /// </summary>
        public int maxNumOfQueues;

        public ZooEntry(int entryID, int cd, string entryPathNameFromCar, int maxNumOfQueues)
        {
            this.entryID = entryID;
            this.entryCD = new IntCD(cd);
            this.entryCD.Run();
            this.entryPathNameFromCar = entryPathNameFromCar;
            this.maxNumOfQueues = maxNumOfQueues;
        }

        public override void Tick(int deltaTimeMS)
        {
            entryCD.Tick(deltaTimeMS);
        }

        public void Realse()
        {
            visitorQueue.Clear();
            entryCD = null;
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
            visitorQueue.Enqueue(entityID);
        }

        public int RemoveVisitorFromQueue()
        {
            return visitorQueue.Dequeue();
        }

        public int GetIndexInQueue()
        {
            return visitorQueue.Count;
        }
    }


    

}
