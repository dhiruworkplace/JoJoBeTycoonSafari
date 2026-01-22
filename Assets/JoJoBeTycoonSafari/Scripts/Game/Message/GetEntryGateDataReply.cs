using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class GetEntryGateDataReply : Message
    {
        public int entityID;
        public EntryGate entryGate;

        public static ObjectPool<GetEntryGateDataReply> pool = new ObjectPool<GetEntryGateDataReply>();

        public GetEntryGateDataReply()
        {
            this.messageID = (int)GameMessageDefine.GetEntryGateDataReply;
        }

        public void Init(int entityID, EntryGate entryGate)
        {
            this.entityID = entityID;
            this.entryGate = entryGate;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static GetEntryGateDataReply Send(int entityID, EntryGate entryGate)
        {
            var msg = pool.New();
            msg.Init(entityID, entryGate);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("GetEntryGateDataReply entityID={0}", entityID);
        }
    }
}

