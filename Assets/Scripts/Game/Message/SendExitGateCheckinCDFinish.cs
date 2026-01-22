using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SendExitGateCheckinCDFinish : Message
    {
        public int entityID;
        public int entryID;

        public static ObjectPool<SendExitGateCheckinCDFinish> pool = new ObjectPool<SendExitGateCheckinCDFinish>();

        public SendExitGateCheckinCDFinish()
        {
            this.messageID = (int)GameMessageDefine.SendExitGateCheckinCDFinish;
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

        public static SendExitGateCheckinCDFinish Send(int entityID, int entryID)
        {
            var msg = pool.New();
            msg.Init(entityID, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SendExitGateCheckinCDFinish entityID={1}, entryID={0}", entityID,entryID);
        }
    }

}
