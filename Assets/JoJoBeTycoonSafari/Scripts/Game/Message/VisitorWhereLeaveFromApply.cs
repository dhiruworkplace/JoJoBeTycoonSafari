using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class VisitorWhereLeaveFromApply : Message
    {
        public int entityID;

        public static ObjectPool<VisitorWhereLeaveFromApply> pool = new ObjectPool<VisitorWhereLeaveFromApply>();

        public VisitorWhereLeaveFromApply()
        {
            this.messageID = (int)GameMessageDefine.VisitorWhereLeaveFromApply;
        }

        public void Init(int entityID)
        {
            this.entityID = entityID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static VisitorWhereLeaveFromApply Send(int entityID)
        {
            var msg = pool.New();
            msg.Init(entityID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("VisitorWhereLeaveFromReply entityID={0}", entityID);
        }
    }

}
