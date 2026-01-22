using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class OpenNewLittleZoo : Message
    {
        public int littleZooID;

        public static ObjectPool<OpenNewLittleZoo> pool = new ObjectPool<OpenNewLittleZoo>();

        public OpenNewLittleZoo()
        {
            this.messageID = (int)GameMessageDefine.OpenNewLittleZoo;
        }

        public void Init(int littleZooID)
        {
            this.littleZooID = littleZooID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static OpenNewLittleZoo Send(int littleZooID)
        {
            var msg = pool.New();
            msg.Init(littleZooID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("OpenNewLittleZoo littleZooID={0}", littleZooID);
        }
    }

}
