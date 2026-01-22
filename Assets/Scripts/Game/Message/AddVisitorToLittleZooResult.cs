using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToLittleZooResult : Message
    {
        public bool result;
        public int groupID;
        public int indexInQueue;
        public int littleZooID;
        public int entityID;

        /// <summary>
        /// 等待位中的具体地址
        /// </summary>
        public Vector3 waitPos;



        public static ObjectPool<AddVisitorToLittleZooResult> pool = new ObjectPool<AddVisitorToLittleZooResult>();

        public AddVisitorToLittleZooResult()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToLittleZooResult;
        }

        public void Init(bool result, int groupID, int indexInQueue, int littleZooID, int entityID, Vector3 waitPos)
        {
            this.result = result;
            this.indexInQueue = indexInQueue;
            this.littleZooID = littleZooID;
            this.entityID = entityID;
            this.waitPos = waitPos;
            this.groupID = groupID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToLittleZooResult Send(bool result, int groupID, int indexInQueue, int littleZooID, int entityID, Vector3 waitPos)
        {
            var msg = pool.New();
            msg.Init(result, groupID, indexInQueue, littleZooID, entityID, waitPos);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToLittleZooResult groupID={0}, littleZooID={1}, entityID={2}, result={3}, indexInQueue={4}", 
                groupID, littleZooID, entityID, result, indexInQueue);
        }
    }
}