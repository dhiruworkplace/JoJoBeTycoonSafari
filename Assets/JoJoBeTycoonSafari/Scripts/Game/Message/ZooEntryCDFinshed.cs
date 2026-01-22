using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class ZooEntryCDFinshed : Message
    {
        public int entryID;
        public int entityID;
        public bool isHead; //是不是排第一个

        public static ObjectPool<ZooEntryCDFinshed> pool = new ObjectPool<ZooEntryCDFinshed>();

        public ZooEntryCDFinshed()
        {
            this.messageID = (int)GameMessageDefine.ZooEntryCDFinshed;
        }

        public void Init(int entryID, int entityID, bool isHead)
        {
            this.entryID = entryID;
            this.entityID = entityID;
            this.isHead = isHead;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static ZooEntryCDFinshed Send(int entryID, int entityID, bool isHead)
        {
            var msg = pool.New();
            msg.Init(entryID, entityID, isHead);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("ZooEntryCDFinshed entryID={0}, entityID={1}, isHead={2}", entryID, entityID, isHead);
        }
    }

}
