using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SetValueOfPlayerData : Message
    {
        public int deltaVal;

        public System.Numerics.BigInteger bigIntDeltaVal;

        public int channelID;

        public static ObjectPool<SetValueOfPlayerData> pool = new ObjectPool<SetValueOfPlayerData>();

        public void Init(int messageID, int deltaVal, System.Numerics.BigInteger bigIntDeltaVal, int channelID)
        {
            this.messageID = messageID;
            this.deltaVal = deltaVal;
            this.channelID = channelID;
            this.bigIntDeltaVal = bigIntDeltaVal;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static SetValueOfPlayerData Send(int messageID, int deltaVal, System.Numerics.BigInteger bigIntDeltaVal, int channelID)
        {
            var msg = pool.New();
            msg.Init(messageID, deltaVal, bigIntDeltaVal, channelID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SetValueOfPlayerData messageID = {0}, deltaVal={1}, channelID={2} , bigIntDeltaVal={3}",
                messageID, deltaVal, channelID, bigIntDeltaVal);
        }
    }
}

