using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class WaitSeatToVisitSeat : Message
    {
        public int entityID;
        public int indexInVisitQueue;
        public int littleZooID;

        public static ObjectPool<WaitSeatToVisitSeat> pool = new ObjectPool<WaitSeatToVisitSeat>();

        public WaitSeatToVisitSeat()
        {
            this.messageID = (int)GameMessageDefine.WaitSeatToVisitSeat;
        }

        public void Init(int entityID, int indexInVisitQueue, int littleZooID)
        {
            this.entityID = entityID;
            this.indexInVisitQueue = indexInVisitQueue;
            this.littleZooID = littleZooID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static WaitSeatToVisitSeat Send(int entityID, int indexInVisitQueue, int littleZooID)
        {
            var msg = pool.New();
            msg.Init(entityID, indexInVisitQueue, littleZooID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("WaitSeatToVisitSeat entityID={0}, indexInVisitQueue={1}, littleZooID={2}", entityID, indexInVisitQueue, littleZooID);
        }
    }
}

