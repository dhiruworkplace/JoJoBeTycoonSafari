using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToExitGateQueueApply : Message
    {
        public int entityID;

        public static ObjectPool<AddVisitorToExitGateQueueApply> pool = new ObjectPool<AddVisitorToExitGateQueueApply>();

        public AddVisitorToExitGateQueueApply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToExitGateQueueApply;
        }

        public void Init(int entityID)
        {
            this.entityID = entityID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToExitGateQueueApply Send(int entityID)
        {
            var msg = pool.New();
            msg.Init(entityID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToExitGateQueueApply entityID={0}", entityID);
        }
    }
}
