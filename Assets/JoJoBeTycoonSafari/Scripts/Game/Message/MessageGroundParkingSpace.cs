using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class MessageGroundParkingSpace : Message
    {
        public int groupID;
        public int idx;

        public static ObjectPool<MessageGroundParkingSpace> pool = new ObjectPool<MessageGroundParkingSpace>();

        public MessageGroundParkingSpace()
        {
        }

        public void Init(int messageID, int groupID, int idx)
        {
            this.messageID = messageID;
            this.groupID = groupID;
            this.idx = idx;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static MessageGroundParkingSpace Send(int messageID, int groupID, int idx)
        {
            var msg = pool.New();
            msg.Init(messageID, groupID, idx);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("MessageGroundParkingSpace messageID={0}, groupID={1}, idx={2}", messageID, groupID, idx);
        }
    }

}
