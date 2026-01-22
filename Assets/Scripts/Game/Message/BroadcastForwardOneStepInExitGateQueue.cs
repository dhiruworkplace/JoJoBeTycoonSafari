using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class BroadcastForwardOneStepInExitGateQueue : Message
    {
        public List<int> entityIDs;

        public static ObjectPool<BroadcastForwardOneStepInExitGateQueue> pool = new ObjectPool<BroadcastForwardOneStepInExitGateQueue>();

        public BroadcastForwardOneStepInExitGateQueue()
        {
            this.messageID = (int)GameMessageDefine.BroadcastForwardOneStepInExitGateQueue;
            entityIDs = new List<int>();
        }

        public void Init(List<int> entityIDs)
        {
            this.entityIDs.Clear();
            this.entityIDs.AddRange(entityIDs);
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static BroadcastForwardOneStepInExitGateQueue Send(List<int> entityIDs)
        {
            var msg = pool.New();
            msg.Init(entityIDs);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastForwardOneStepInExitGateQueue entityID.Count={0}", entityIDs.Count);
        }
    }

}
