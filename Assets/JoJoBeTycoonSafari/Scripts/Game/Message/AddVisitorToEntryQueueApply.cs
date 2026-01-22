using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToEntryQueueApply : Message
    {
        public int entityID;

        public int entryID;

        public static ObjectPool<AddVisitorToEntryQueueApply> pool = new ObjectPool<AddVisitorToEntryQueueApply>();

        public AddVisitorToEntryQueueApply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToEntryQueueApply;
        }

        public void Init(int entityID, int entryID)
        {
            this.entityID = entityID;
            this.entryID = entryID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToEntryQueueApply Send(int entityID, int entryID)
        {
            var msg = pool.New();
            msg.Init(entityID, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToEntryQueueApply entityID={0}", entityID);
        }
    }
}
