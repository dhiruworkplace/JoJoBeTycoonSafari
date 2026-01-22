using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class BroadcastOpenNewLittleZoo : Message
    {
        public int littleZooID;

        public bool isTriggerExtend;

        public int triggerLoadGroupID;

        public List<int> triggerLoadLittleZooIDs;

        public static ObjectPool<BroadcastOpenNewLittleZoo> pool = new ObjectPool<BroadcastOpenNewLittleZoo>();

        public BroadcastOpenNewLittleZoo()
        {
            this.messageID = (int)GameMessageDefine.BroadcastOpenNewLittleZoo;
            triggerLoadLittleZooIDs = new List<int>();
        }

        public void Init(int littleZooID, bool isTriggerExtend, int triggerLoadGroupID, List<int> triggerLoadLittleZooIDs)
        {
            this.littleZooID = littleZooID;
            this.isTriggerExtend = isTriggerExtend;
            this.triggerLoadGroupID = triggerLoadGroupID;
            this.triggerLoadLittleZooIDs.Clear();
            this.triggerLoadLittleZooIDs.AddRange(triggerLoadLittleZooIDs);
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static BroadcastOpenNewLittleZoo Send(int littleZooID, bool isTriggerExtend, int triggerLoadGroupID, List<int> triggerLoadLittleZooIDs)
        {
            var msg = pool.New();
            msg.Init(littleZooID, isTriggerExtend, triggerLoadGroupID, triggerLoadLittleZooIDs);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastOpenNewLittleZoo littleZooID={0}, isTriggerExtend={1}, triggerLoadGroupID={2}",
                littleZooID, isTriggerExtend, triggerLoadGroupID);
        }
    }

}
