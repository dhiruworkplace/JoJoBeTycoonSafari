
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class GetAddNewAnimalData : Message
    {
        public int animalID;
        public int littleZooID;

        public static ObjectPool<GetAddNewAnimalData> pool = new ObjectPool<GetAddNewAnimalData>();

        public GetAddNewAnimalData()
        {
            this.messageID = (int)GameMessageDefine.LittleZooData;
        }

        public void Init(int messageID, int animalID, int littleZooID)
        {
            this.animalID = animalID;
            this.littleZooID = littleZooID;
            this.messageID = messageID;

        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static GetAddNewAnimalData Send( int messageID , int animalID, int littleZooID)
        {
            var msg = pool.New();
            msg.Init( messageID, animalID,littleZooID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("LittleZooData entityID={0}, littleZooID={1}", animalID, littleZooID);
        }
    }
}


