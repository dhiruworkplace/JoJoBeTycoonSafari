using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SendExitGateCheckinCDFinishReply : Message
    {
        public int entityID;

        public static ObjectPool<SendExitGateCheckinCDFinishReply> pool = new ObjectPool<SendExitGateCheckinCDFinishReply>();

        public SendExitGateCheckinCDFinishReply()
        {
            this.messageID = (int)GameMessageDefine.SendExitGateCheckinCDFinishReply;
        }

        public void Init(int entityID)
        {
            this.entityID = entityID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static SendExitGateCheckinCDFinishReply Send(int entityID)
        {
            var msg = pool.New();
            msg.Init(entityID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SendExitGateCheckinCDFinishReply entityID={0} ", entityID);
        }
    }

}
