using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class AddVisitorToLittleZooApply : Message
    {
        public int entityID;

        /// <summary>
        /// 是否是首次申请(刚进入动物的那次申请就首次申请)
        /// </summary>
        public bool isFirstApply;
        

        public static ObjectPool<AddVisitorToLittleZooApply> pool = new ObjectPool<AddVisitorToLittleZooApply>();

        public AddVisitorToLittleZooApply()
        {
            this.messageID = (int)GameMessageDefine.AddVisitorToLittleZooApply;
        }

        public void Init(int entityID, bool isFirstApply)
        {
            this.entityID = entityID;
            this.isFirstApply = isFirstApply;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static AddVisitorToLittleZooApply Send(int entityID, bool isFirstApply)
        {
            Logger.LogWarp.LogFormat("AddVisitorToLittleZooApply {0}, {1} ", entityID, isFirstApply);
            var msg = pool.New();
            msg.Init(entityID, isFirstApply);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("AddVisitorToLittleZooApply entityID={0}, isFirstApply={1} ", entityID, isFirstApply);
        }
    }
}