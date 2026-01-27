using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace ZooGame.MessageCenter
{
    public class AddVisitorToLittleZoo : Message
    {
        public int groupID;
        public int littleZooID;
        public int entityID;

        public static ObjectPool<AddVisitorToLittleZoo> pool = new ObjectPool<AddVisitorToLittleZoo>();

        public AddVisitorToLittleZoo()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToLittleZoo;
        }

        public void Init(int groupID, int littleZooID, int entityID)
        {
            this.groupID = groupID;
            this.littleZooID = littleZooID;
            this.entityID = entityID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToLittleZoo Send(int groupID, int littleZooID, int entityID)
        {
            Logger.LogWarp.LogFormat("AddVisitorToLittleZoo {0}, {1}, {2}", entityID, littleZooID, groupID);
            var msg = pool.New();
            msg.Init(groupID, littleZooID, entityID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToLittleZoo groupID={0}, littleZooID={1}, entityID={2}", 
                groupID, littleZooID, entityID);
        }
    }
}