using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class BroadcastDetailValueOfPlayerData : Message
    {
        public int detailVal;

        /// <summary>
        /// 当前值(已经修改后的)
        /// </summary>
        public int currVal;

        /// <summary>
        /// 变化值(导致这次广播的变化值)
        /// </summary>
        public int deltaVal;
        

        public static ObjectPool<BroadcastDetailValueOfPlayerData> pool = new ObjectPool<BroadcastDetailValueOfPlayerData>();


        public void Init(int messageID, int detailVal, int currVal, int deltaVal)
        {
            this.messageID = messageID;
            this.detailVal = detailVal;
            this.currVal = currVal;
            this.deltaVal = deltaVal;
        }

        public override void Release()
        {
            pool.Delete(this);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageID">消息ID</param>
        /// <param name="detailVal">??</param>
        /// <param name="currVal">当前值(已经修改后的)</param>
        /// <param name="deltaVal">ID</param>
        /// <returns></returns>
        public static BroadcastDetailValueOfPlayerData Send(int messageID, int detailVal, int currVal, int deltaVal)
        {
            var msg = pool.New();
            msg.Init(messageID, detailVal, currVal, deltaVal);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastDetailValueOfPlayerData messageID={0}, detailVal= {1}, currVal={2}, deltaVal={3}",
                messageID, detailVal, currVal, deltaVal);
        }

    }
}

