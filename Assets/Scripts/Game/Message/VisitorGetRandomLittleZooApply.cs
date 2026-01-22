using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class VisitorGetRandomLittleZooApply : Message
    {
        public int entityID;

        /// <summary>
        /// 是否是首次申请(刚进入动物的那次申请就首次申请)
        /// </summary>
        public bool isFirstApply;
        

        public static ObjectPool<VisitorGetRandomLittleZooApply> pool = new ObjectPool<VisitorGetRandomLittleZooApply>();

        public VisitorGetRandomLittleZooApply()
        {
            this.messageID = (int)GameMessageDefine.VisitorGetRandomLittleZooApply;
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

        public static VisitorGetRandomLittleZooApply Send(int entityID, bool isFirstApply)
        {
            var msg = pool.New();
            msg.Init(entityID, isFirstApply);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("VisitorGetRandomLittleZooApply entityID={0}, isFirstApply={1} ", entityID, isFirstApply);
        }
    }
}