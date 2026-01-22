using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class BroadcastForwardOneStepInQueue : Message
    {
        //public List<int> entityIDs;

        public int entityID;
        public int entryID;

        public static ObjectPool<BroadcastForwardOneStepInQueue> pool = new ObjectPool<BroadcastForwardOneStepInQueue>();

        public BroadcastForwardOneStepInQueue()
        {
            //entityIDs = new List<int>();
        }

        //public void Init(int messageID, List<int> entityIDs)
        //{
        //    this.messageID = messageID;

        //    this.entityIDs.Clear();
        //    this.entityIDs.AddRange(entityIDs);
        //}


        public void Init(int messageID, int entityID, int entryID)
        {
            this.messageID = messageID;
            this.entityID = entityID;
            this.entryID = entryID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        //public static BroadcastForwardOneStepInQueue Send(int messageID, List<int> entityIDs)
        //{
        //    var msg = pool.New();
        //    msg.Init(messageID, entityIDs);
        //    MessageManager.GetInstance().Send(msg);
        //    return msg;
        //}

        public static BroadcastForwardOneStepInQueue Send(int messageID, int entityID, int entryID)
        {
            var msg = pool.New();
            msg.Init(messageID, entityID, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastForwardOneStepInQueue {0} entityID={1} entryID={2}", 
                (GameMessageDefine)(this.messageID), entityID, entryID);
        }
    }

}
