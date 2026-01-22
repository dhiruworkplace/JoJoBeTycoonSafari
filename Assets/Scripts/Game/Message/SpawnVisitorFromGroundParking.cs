using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game.MessageCenter
{
    public class SpawnVisitorFromGroundParking : Message
    {
        public VisitorStage stage;

        public EntityFuncType funcType;

        public int groupID;

        public int idx;

        public static ObjectPool<SpawnVisitorFromGroundParking> pool = new ObjectPool<SpawnVisitorFromGroundParking>();

        public SpawnVisitorFromGroundParking()
        {
            this.messageID = (int)GameMessageDefine.SpawnVisitorFromGroundParking;
        }

        public void Init(VisitorStage stage, EntityFuncType funcType, int groupID, int idx)
        {
            this.stage = stage;
            this.funcType = funcType;
            this.groupID = groupID;
            this.idx = idx;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static SpawnVisitorFromGroundParking Send(VisitorStage stage, EntityFuncType funcType, int groupID, int idx)
        {
            var msg = pool.New();
            msg.Init(stage, funcType, groupID, idx);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SpawnVisitorFromGroundParking stage={0}", stage);
        }
    }

}
