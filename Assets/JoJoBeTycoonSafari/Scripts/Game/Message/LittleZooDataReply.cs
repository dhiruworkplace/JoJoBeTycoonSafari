using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class LittleZooDataReply : Message
    {
        public int entityID;
        public LittleZoo littleZoo;

        public static ObjectPool<LittleZooDataReply> pool = new ObjectPool<LittleZooDataReply>();

        public LittleZooDataReply()
        {
            this.messageID = (int)GameMessageDefine.LittleZooDataReply;
        }

        public void Init(int entityID, LittleZoo littleZoo)
        {
            this.entityID = entityID;
            this.littleZoo = littleZoo;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static LittleZooDataReply Send(int entityID, LittleZoo littleZoo)
        {
            var msg = pool.New();
            msg.Init(entityID, littleZoo);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("LittleZooDataReply entityID={0}, littleZoo={1}", entityID, littleZoo);
        }
    }
}

