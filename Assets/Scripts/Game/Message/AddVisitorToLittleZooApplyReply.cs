using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToLittleZooApplyReply : Message
    {
        public int entityID;
        public bool result;
        public int groupID;
        public int littleZooID;
        public int indexInQueue;
        public Vector3 waitPos;
        public bool isCrossGroup;
        public List<int> crossLittleZooIDs;
        
        public static ObjectPool<AddVisitorToLittleZooApplyReply> pool = new ObjectPool<AddVisitorToLittleZooApplyReply>();

        public AddVisitorToLittleZooApplyReply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToLittleZooApplyReply;
            crossLittleZooIDs = new List<int>();
        }

        public void Init(int entityID, bool result, int groupID, int littleZooID, int indexInQueue, Vector3 waitPos, bool isCrossGroup, List<int> crossLittleZooIDs)
        {
            this.entityID = entityID;
            this.result = result;
            this.groupID = groupID;
            this.littleZooID = littleZooID;
            this.indexInQueue = indexInQueue;
            this.waitPos = waitPos;
            this.isCrossGroup = isCrossGroup;
            this.crossLittleZooIDs.Clear();
            if (this.isCrossGroup)
            {
                this.crossLittleZooIDs.AddRange(crossLittleZooIDs);
            }
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToLittleZooApplyReply Send(int entityID, bool result, 
            int groupID, int littleZooID, int indexInQueue, 
            Vector3 waitPos, bool isCrossGroup, List<int> crossLittleZooIDs)
        {
            Logger.LogWarp.LogFormat("AddVisitorToLittleZooApplyReply {0}, {1}, {2}, {3}, {4}, {5}", entityID, result, groupID, littleZooID, indexInQueue, waitPos);
            var msg = pool.New();
            msg.Init(entityID, result, groupID, littleZooID, indexInQueue, waitPos, isCrossGroup, crossLittleZooIDs);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToLittleZooApplyReply entityID={0}, result={1}, groupID={2}, littleZooID={3}, indexInQueue={4}, waitPos={5}, isCrossGroup={6}",
                entityID, result, groupID, littleZooID, indexInQueue, waitPos, isCrossGroup);
        }
    }
}