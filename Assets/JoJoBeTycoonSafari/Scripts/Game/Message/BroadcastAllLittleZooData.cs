using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
namespace Game.MessageCenter
{
    public class BroadcastAllLittleZooData : Message
    {
        public Dictionary<int, LittleZoo> littleZooMap;

        public static ObjectPool<BroadcastAllLittleZooData> pool = new ObjectPool<BroadcastAllLittleZooData>();

        public BroadcastAllLittleZooData()
        {
            this.messageID = (int)GameMessageDefine.BroadcastAllLittleZooData;
        }

        public void Init(Dictionary<int, LittleZoo> littleZooMap)
        {
            this.littleZooMap = littleZooMap;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        public static BroadcastAllLittleZooData Send(Dictionary<int, LittleZoo> littleZooMap)
        {
            var msg = pool.New();
            msg.Init(littleZooMap);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("BroadcastAllLittleZooData ");
        }
    }
}

