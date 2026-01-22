using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class EntryGateCheckInCDFinshedApply : Message
    {
        public int entityID;
        public int entryID;

        public static ObjectPool<EntryGateCheckInCDFinshedApply> pool = new ObjectPool<EntryGateCheckInCDFinshedApply>();

        public EntryGateCheckInCDFinshedApply()
        {
            this.messageID = (int)GameMessageDefine.EntryGateCheckInCDFinshedApply;
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

        public static EntryGateCheckInCDFinshedApply Send(int entityID, int entryID)
        {
            var msg = pool.New();
            msg.Init(entityID, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("EntryGateCheckInCDFinshedApply entityID={1}, entryID={0}", entityID,entryID);
        }
    }

}
