using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class GetEntryGateDataApply : Message
    {
        public int entityID;
        public int entryID;

        public static ObjectPool<GetEntryGateDataApply> pool = new ObjectPool<GetEntryGateDataApply>();

        public GetEntryGateDataApply()
        {
            this.messageID = (int)GameMessageDefine.GetEntryGateDataApply;
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

        public static GetEntryGateDataApply Send(int entityID, int entryID)
        {
            var msg = pool.New();
            msg.Init(entityID, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("GetEntryGateDataApply entityID={0}, entryID={1}", entityID, entryID);
        }
    }
}

