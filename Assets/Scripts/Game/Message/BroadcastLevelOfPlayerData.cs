//using Game.MessageCenter;
//using System.Collections;
//using System.Collections.Generic;
//using UFrame;
//using UFrame.MessageCenter;
//using UnityEngine;

//namespace Game.MessageCenter
//{
//    public class BroadcastLevelOfPlayerData : Message
//    {
//        public LevelOfPlayerDataType levelOfPlayerDataType;

//        /// <summary>
//        /// 根据LevelOfPlayerDataType有不同的解释
//        /// parking  无用
//        /// liitleZoo  动物栏ID
//        /// Animal 动物ID
//        /// LittleGameLevel 无用
//        /// </summary>
//        public int detailID;

//        /// <summary>
//        /// 设置的变化值
//        /// </summary>
//        public int deltaVal;
        

//        public static ObjectPool<BroadcastLevelOfPlayerData> pool = new ObjectPool<BroadcastLevelOfPlayerData>();

//        public BroadcastLevelOfPlayerData()
//        {
//            this.messageID = (int)GameMessageDefine.BroadcastLevelOfPlayerData;
//        }

//        public void Init(LevelOfPlayerDataType levelOfPlayerDataType, int detailID, int deltaVal)
//        {
//            this.levelOfPlayerDataType = levelOfPlayerDataType;
//            this.detailID = detailID;
//            this.deltaVal = deltaVal;
//        }

//        public override void Release()
//        {
//            pool.Delete(this);
//        }

//        public static BroadcastLevelOfPlayerData Send(LevelOfPlayerDataType levelOfPlayerDataType, int detailID, int deltaVal)
//        {
//            var msg = pool.New();
//            msg.Init(levelOfPlayerDataType, detailID, deltaVal);
//            MessageManager.GetInstance().Send(msg);
//            return msg;
//        }

//        public override string ToString()
//        {
//            return string.Format("BroadcastLevelOfPlayerData levelOfPlayerDataType={0}, detailID={1}, deltaVal={2}",
//                levelOfPlayerDataType, detailID, deltaVal);
//        }

//    }
//}

