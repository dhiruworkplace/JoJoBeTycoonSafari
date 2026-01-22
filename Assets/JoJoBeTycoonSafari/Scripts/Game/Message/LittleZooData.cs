using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class LittleZooData : Message
    {
        public int entityID;
        public int littleZooID;

        public static ObjectPool<LittleZooData> pool = new ObjectPool<LittleZooData>();

        public LittleZooData()
        {
            this.messageID = (int)GameMessageDefine.LittleZooData;
        }

        public void Init(int entityID, int littleZooID)
        {
            this.entityID = entityID;
            this.littleZooID = littleZooID;

        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static LittleZooData Send(int entityID, int littleZooID)
        {
            var msg = pool.New();
            msg.Init(entityID, littleZooID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("LittleZooData entityID={0}, littleZooID={1}", entityID, littleZooID);
        }
    }
}

