using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class VisitorGetVisitSeatReply : Message
    {
        public int entityID;

        public bool result;

        public int littleZooID;

        public int idxOfQueue;
        
        public static ObjectPool<VisitorGetVisitSeatReply> pool = new ObjectPool<VisitorGetVisitSeatReply>();

        public VisitorGetVisitSeatReply()
        {
            this.messageID = (int)GameMessageDefine.VisitorGetVisitSeatReply;
        }

        public void Init(int entityID, bool result, int littleZooID, int idxOfQueue)
        {
            this.entityID = entityID;
            this.result = result;
            this.littleZooID = littleZooID;
            this.idxOfQueue = idxOfQueue;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static VisitorGetVisitSeatReply Send(int entityID, bool result, int littleZooID, int idxOfQueue)
        {
            var msg = pool.New();
            msg.Init(entityID, result, littleZooID, idxOfQueue);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("VisitorGetVisitSeatReply entityID={0}, result={1}, littleZooID={2} idxOfQueue={3} ",
                entityID, result, littleZooID, idxOfQueue);
        }
    }
}