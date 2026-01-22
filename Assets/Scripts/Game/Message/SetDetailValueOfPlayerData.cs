using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SetDetailValueOfPlayerData : Message
    {
        public int detailVal;

        public int deltaVal;

        public int channelID;
        
        public static ObjectPool<SetDetailValueOfPlayerData> pool = new ObjectPool<SetDetailValueOfPlayerData>();

        public void Init(int messageID, int detailVal, int deltaVal, int channelID)
        {
            this.messageID = messageID;
            this.detailVal = detailVal;
            this.deltaVal = deltaVal;
            this.channelID = channelID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static SetDetailValueOfPlayerData Send(int messageID, int detailVal, int deltaVal, int channelID)
        {
            var msg = pool.New();
            msg.Init(messageID, detailVal, deltaVal, channelID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SetDetailValueOfPlayerData messageID = {0}, detailVal = {1}, deltaVal={2}, channelID={3} ",
                messageID, detailVal, deltaVal, channelID);
        }
    }
}

