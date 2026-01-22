//using Game.GlobalData;
//using System.Collections;
//using System.Collections.Generic;
//using UFrame;
//using UFrame.MessageCenter;
//using UnityEngine;
//namespace Game.MessageCenter
//{
//    public class GetPlayerDataReply : Message
//    {
//        public PlayerData playerData { get; protected set; }

//        public static ObjectPool<GetPlayerDataReply> pool = new ObjectPool<GetPlayerDataReply>();

//        public GetPlayerDataReply()
//        {
//            this.messageID = (int)GameMessageDefine.GetPlayerDataReply;
//        }

//        public void Init(PlayerData playerData)
//        {
//            this.playerData = playerData;
//        }

//        public override void Release()
//        {
//            pool.Delete(this);
//        }

//        public static GetPlayerDataReply Send(PlayerData playerData)
//        {
//            var msg = pool.New();
//            msg.Init(playerData);
//            MessageManager.GetInstance().Send(msg);
//            return msg;
//        }

//        public override string ToString()
//        {
//            return string.Format("GetPlayerDataReply ");
//        }
//    }
//}

