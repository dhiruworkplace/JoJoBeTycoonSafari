using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToEntryQueueReply : Message
    {
        public int entityID;
        public int indexInQueue;

        public static ObjectPool<AddVisitorToEntryQueueReply> pool = new ObjectPool<AddVisitorToEntryQueueReply>();

        public AddVisitorToEntryQueueReply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToEntryQueueReply;
        }

        public void Init(int entityID, int indexInQueue)
        {
            this.entityID = entityID;
            this.indexInQueue = indexInQueue;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToEntryQueueReply Send(int entityID, int indexInQueue)
        {
            var msg = pool.New();
            msg.Init(entityID, indexInQueue);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToEntryQueueReply entityID={0}, entryID={1}",
                entityID, indexInQueue);
        }
    }
}

