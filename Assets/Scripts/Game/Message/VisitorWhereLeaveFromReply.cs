using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class VisitorWhereLeaveFromReply : Message
    {
        public int entityID;
        public bool isFromGround;
        public int groupID;
        public int idx;

        public static ObjectPool<VisitorWhereLeaveFromReply> pool = new ObjectPool<VisitorWhereLeaveFromReply>();

        public VisitorWhereLeaveFromReply()
        {
            this.messageID = (int)GameMessageDefine.VisitorWhereLeaveFromReply;
        }

        public void Init(int entityID, bool isFromGround, int groupID, int idx)
        {
            this.entityID = entityID;
            this.isFromGround = isFromGround;
            this.groupID = groupID;
            this.idx = idx;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static VisitorWhereLeaveFromReply Send(int entityID, bool isFromGround, int groupID, int idx)
        {
            var msg = pool.New();
            msg.Init(entityID, isFromGround, groupID, idx);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("VisitorWhereLeaveFromReply entityID = {0}, isFromGround={1}, groupID={2}, idx={3}", 
                entityID, isFromGround, groupID, idx);
        }
    }

}
