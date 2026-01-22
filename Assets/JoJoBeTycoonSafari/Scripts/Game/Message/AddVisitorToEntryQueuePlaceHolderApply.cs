using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToEntryQueuePlaceHolderApply : Message
    {
        public int entityID;

        public static ObjectPool<AddVisitorToEntryQueuePlaceHolderApply> pool = new ObjectPool<AddVisitorToEntryQueuePlaceHolderApply>();

        public AddVisitorToEntryQueuePlaceHolderApply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToEntryQueuePlaceHolderApply;
        }

        public void Init(int entityID)
        {
            this.entityID = entityID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToEntryQueuePlaceHolderApply Send(int entityID)
        {
            var msg = pool.New();
            msg.Init(entityID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToEntryQueuePlaceHolderApply entityID={0}", entityID);
        }
    }
}
