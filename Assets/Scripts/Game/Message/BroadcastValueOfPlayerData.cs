using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using System.Numerics;

namespace Game.MessageCenter
{
    public class BroadcastValueOfPlayerData : Message
    {
        /// <summary>
        /// 当前值(已经修改后的)
        /// </summary>
        public int currVal;

        /// <summary>
        /// 变化值(导致这次广播的变化值)
        /// </summary>
        public int deltaVal;

        /// <summary>
        /// 大数据 当前值(已经修改后的)
        /// </summary>
        public System.Numerics.BigInteger bigIntCurrVal;

        /// <summary>
        /// 大数据 变化值(导致这次广播的变化值)
        /// </summary>
        public System.Numerics.BigInteger bigIntDeltaVal;

        public static ObjectPool<BroadcastValueOfPlayerData> pool = new ObjectPool<BroadcastValueOfPlayerData>();

        public void Init(int messageID, int currVal, int deltaVal, System.Numerics.BigInteger bigIntCurrVal, System.Numerics.BigInteger bigIntDeltaVal)
        {
            this.messageID = messageID;
            this.currVal = currVal;
            this.deltaVal = deltaVal;
            this.bigIntCurrVal = bigIntCurrVal;
            this.bigIntDeltaVal = bigIntDeltaVal;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static BroadcastValueOfPlayerData Send(int messageID, int currVal, int deltaVal, System.Numerics.BigInteger bigIntCurrVal, System.Numerics.BigInteger bigIntDeltaVal)
        {
            var msg = pool.New();
            msg.Init(messageID, currVal, deltaVal, bigIntCurrVal, bigIntDeltaVal);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastValueOfPlayerData messageID={0}, currVal={1}, deltaVal={2}, bigIntCurrVal={3}, bigIntDeltaVal={4}",
                messageID, currVal, deltaVal, bigIntCurrVal, bigIntDeltaVal);
        }

    }
}

