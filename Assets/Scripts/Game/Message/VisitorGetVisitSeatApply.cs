using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class VisitorGetVisitSeatApply : Message
    {
        public int entityID;

        public int groupID;

        public int littleZooID;
        
        public static ObjectPool<VisitorGetVisitSeatApply> pool = new ObjectPool<VisitorGetVisitSeatApply>();

        public VisitorGetVisitSeatApply()
        {
            this.messageID = (int)GameMessageDefine.VisitorGetVisitSeatApply;
        }

        public void Init(int entityID, int groupID, int littleZooID)
        {
            this.entityID = entityID;
            this.groupID = groupID;
            this.littleZooID = littleZooID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static VisitorGetVisitSeatApply Send(int entityID, int groupID, int littleZooID)
        {
            var msg = pool.New();
            msg.Init(entityID, groupID, littleZooID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("VisitorGetVisitSeatApply entityID={0}, groupID={1} littleZooID={2} ",
                entityID, groupID, littleZooID);
        }
    }
}