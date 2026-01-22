using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddGroundParkingReply : Message
    {
        /// <summary>
        /// 地面第几组
        /// </summary>
        public int groupID;

        /// <summary>
        /// 组内第几个
        /// </summary>
        public int idx;

        public static ObjectPool<AddGroundParkingReply> pool = new ObjectPool<AddGroundParkingReply>();

        public AddGroundParkingReply()
        {
            this.messageID = (int)GameMessageDefine.AddGroundParkingReply;
        }

        public void Init(int groupID, int idx)
        {
            this.groupID = groupID;
            this.idx = idx;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddGroundParkingReply Send(int groupID, int idx)
        {
            var msg = pool.New();
            msg.Init(groupID, idx);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddGroundParkingReply groupID={0}, idx={1}", groupID, idx);
        }
    }
}
