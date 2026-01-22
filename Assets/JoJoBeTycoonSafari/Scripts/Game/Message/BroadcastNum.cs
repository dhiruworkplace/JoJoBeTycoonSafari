using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;
using System.Numerics;

namespace Game.MessageCenter
{
    public class BroadcastNum : Message
    {
        public int currNum;
        public float fCurrNum;
        public BigInteger bigCurrNum;

        public static ObjectPool<BroadcastNum> pool = new ObjectPool<BroadcastNum>();

        public BroadcastNum()
        {
            //this.messageID = (int)GameMessageDefine.ZooEntryCDFinshed;
        }

        public void Init(int messageID, int currNum, float fCurrNum, BigInteger bigCurrNum)
        {
            this.messageID = messageID;
            this.currNum = currNum;
            this.fCurrNum = fCurrNum;
            this.bigCurrNum = bigCurrNum;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static BroadcastNum Send(int messageID, int currNum, float fCurrNum, BigInteger bigCurrNum)
        {
            var msg = pool.New();
            msg.Init(messageID, currNum, fCurrNum, bigCurrNum);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastNum messageID={0}, currNum={1}, fCurrNum={2}, bigCurrNum={3}", messageID, currNum, fCurrNum, bigCurrNum.ToString());
        }
    }

}
