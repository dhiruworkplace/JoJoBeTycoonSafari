using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class GetUnlockAnimalsReply : Message
    {
        public List<Animal> animals;
        public static ObjectPool<GetUnlockAnimalsReply> pool = new ObjectPool<GetUnlockAnimalsReply>();

        public GetUnlockAnimalsReply()
        {
            this.messageID = (int)GameMessageDefine.GetUnlockAnimalsReply;
            animals = new List<Animal>();
        }

        public void Init(List<Animal> animals)
        {
            this.animals.Clear();
            this.animals.AddRange(animals);
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static GetUnlockAnimalsReply Send(List<Animal> animals)
        {
            var msg = pool.New();
            msg.Init(animals);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("GetUnlockAnimalsReply ");
        }
    }

}
