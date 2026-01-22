using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToEntryQueueResult : Message
    {
        public bool result;
        public int entityID;
        public string pathName; //刷出人物到入口的路径名称
        public int indexInQueue; //处于排队中的位置
        public int entryID;//入口编号

        public static ObjectPool<AddVisitorToEntryQueueResult> pool = new ObjectPool<AddVisitorToEntryQueueResult>();

        public AddVisitorToEntryQueueResult()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToEntryQueueResult;
        }

        public void Init(bool result, int entityID, string pathName, int indexInQueue, int entryID)
        {
            this.result = result;
            this.entityID = entityID;
            this.pathName = pathName;
            this.indexInQueue = indexInQueue;
            this.entryID = entryID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToEntryQueueResult Send(bool result, int entityID, string pathName, int indexInQueue, int entryID)
        {
            var msg = pool.New();
            msg.Init(result, entityID, pathName, indexInQueue, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToEntryQueueResult result={0}, entityID={1}, pathName={2}, indexInQueue={3}, entryID={4}",
                result, entityID, pathName, indexInQueue, entryID);
        }
    }
}

