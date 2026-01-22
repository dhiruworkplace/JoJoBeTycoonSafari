using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToExitGateQueueApplyReply : Message
    {
        public bool result;
        public int entityID;
        public string pathName; //刷出人物到入口的路径名称
        public int indexInQueue; //处于排队中的位置
        public int entryID;//入口编号

        public static ObjectPool<AddVisitorToExitGateQueueApplyReply> pool = new ObjectPool<AddVisitorToExitGateQueueApplyReply>();

        public AddVisitorToExitGateQueueApplyReply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToExitGateQueueApplyReply;
        }

        public void Init(bool result, int entityID, string pathName, int indexInQueue, int entryID)
        {
            this.result = result;
            this.entityID = entityID;
            this.pathName = pathName;
            this.indexInQueue = indexInQueue;
            this.entryID = entryID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToExitGateQueueApplyReply Send(bool result, int entityID, string pathName, int indexInQueue, int entryID)
        {
            var msg = pool.New();
            msg.Init(result, entityID, pathName, indexInQueue, entryID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToExitGateQueueApplyReply result={0}, entityID={1}, pathName={2}, indexInQueue={3}, entryID={4}",
                result, entityID, pathName, indexInQueue, entryID);
        }
    }
}

