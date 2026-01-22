using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToEntryQueue : Message
    {
        public int entityID;

        public static ObjectPool<AddVisitorToEntryQueue> pool = new ObjectPool<AddVisitorToEntryQueue>();

        public AddVisitorToEntryQueue()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToEntryQueue;
        }

        public void Init(int entityID)
        {
            this.entityID = entityID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToEntryQueue Send(int entityID)
        {
            var msg = pool.New();
            msg.Init(entityID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToEntryQueue entityID={0}", entityID);
        }
    }
}
