using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SpawnShuttle : Message
    {
        public List<ShuttleVisitor> shuttleVisitorList;
        public static ObjectPool<SpawnShuttle> pool = new ObjectPool<SpawnShuttle>();

        public SpawnShuttle()
        {
            this.messageID = (int)GameMessageDefine.SpawnShuttle;
            shuttleVisitorList = new List<ShuttleVisitor>();
        }

        public void Init(List<ShuttleVisitor> visitorList)
        {
            shuttleVisitorList.Clear();
            shuttleVisitorList.AddRange(visitorList);
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static SpawnShuttle Send(List<ShuttleVisitor> visitorList)
        {
            var msg = pool.New();
            msg.Init(visitorList);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SpawnShuttle ");
        }
    }
}
