using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToEntryQueuePlaceHolderReply : Message
    {
        public bool result;
        public int entityID;
        public string pathName; //刷出人物到入口的路径名称
        //public int indexInQueue; //处于排队中的位置
        public int entryID;//入口编号

        public static ObjectPool<AddVisitorToEntryQueuePlaceHolderReply> pool = new ObjectPool<AddVisitorToEntryQueuePlaceHolderReply>();

        public AddVisitorToEntryQueuePlaceHolderReply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToEntryQueuePlaceHolderReply;
        }

        //public void Init(bool result, int entityID, string pathName, int indexInQueue, int entryID)
        public void Init(bool result, int entityID, string pathName, int entryID)
        {
            this.result = result;
            this.entityID = entityID;
            this.pathName = pathName;
            //this.indexInQueue = indexInQueue;
            this.entryID = entryID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToEntryQueuePlaceHolderReply Send(bool result, int entityID, string pathName, int entryID)
        {
            var msg = pool.New();
            msg.Init(result, entityID, pathName, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToEntryQueuePlaceHolderReply result={0}, entityID={1}, pathName={2}, entryID={3}",
                result, entityID, pathName, entryID);
        }
    }
}

