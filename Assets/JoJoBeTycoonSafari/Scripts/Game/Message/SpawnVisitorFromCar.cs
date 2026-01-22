using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SpawnVisitorFromCar : Message
    {
        public VisitorStage stage;

        public EntityFuncType funcType;

        public static ObjectPool<SpawnVisitorFromCar> pool = new ObjectPool<SpawnVisitorFromCar>();

        public SpawnVisitorFromCar()
        {
            this.messageID = (int)GameMessageDefine.SpawnVisitorFromCar;
        }

        public void Init(VisitorStage stage, EntityFuncType funcType)
        {
            this.stage = stage;
            this.funcType = funcType;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static SpawnVisitorFromCar Send(VisitorStage stage, EntityFuncType funcType)
        {
            var msg = pool.New();
            msg.Init(stage, funcType);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SpawnVisitorFromCar stage={0}", stage);
        }
    }

}
