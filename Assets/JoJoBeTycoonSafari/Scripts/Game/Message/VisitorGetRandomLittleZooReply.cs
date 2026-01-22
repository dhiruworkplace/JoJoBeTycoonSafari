using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class VisitorGetRandomLittleZooReply : Message
    {
        public int entityID;
        public bool result;
        public int groupID;
        public int littleZooID;
        //public int indexInQueue;
        //public Vector3 waitPos;
        public bool isCrossGroup;
        public List<int> crossLittleZooIDs;
        
        public static ObjectPool<VisitorGetRandomLittleZooReply> pool = new ObjectPool<VisitorGetRandomLittleZooReply>();

        public VisitorGetRandomLittleZooReply()
        {
            this.messageID = (int)GameMessageDefine.VisitorGetRandomLittleZooReply;
            crossLittleZooIDs = new List<int>();
        }

        public void Init(int entityID, bool result, int groupID, int littleZooID, bool isCrossGroup, List<int> crossLittleZooIDs)
        {
            this.entityID = entityID;
            this.result = result;
            this.groupID = groupID;
            this.littleZooID = littleZooID;
            //this.indexInQueue = indexInQueue;
            //this.waitPos = waitPos;
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

        public static VisitorGetRandomLittleZooReply Send(int entityID, bool result, 
            int groupID, int littleZooID, bool isCrossGroup, List<int> crossLittleZooIDs)
        {
            var msg = pool.New();
            msg.Init(entityID, result, groupID, littleZooID, isCrossGroup, crossLittleZooIDs);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("VisitorGetRandomLittleZooReply entityID={0}, result={1}, groupID={2}, littleZooID={3}, isCrossGroup={4}",
                entityID, result, groupID, littleZooID, isCrossGroup);
        }
    }
}