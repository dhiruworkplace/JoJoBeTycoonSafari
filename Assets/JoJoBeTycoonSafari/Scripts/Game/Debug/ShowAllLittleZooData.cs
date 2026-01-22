using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game
{
    public class ShowAllLittleZooData : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastAllLittleZooData, this.OnBroadcastAllLittleZooData);
        }

        private void OnDestroy()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastAllLittleZooData, this.OnBroadcastAllLittleZooData);
        }

        protected void OnBroadcastAllLittleZooData(Message msg)
        {
            var _msg = msg as BroadcastAllLittleZooData;

            foreach(var kv in _msg.littleZooMap)
            {
                int littleZooID = kv.Key;
                var littleZooData = kv.Value;

                var visitPos = transform.Find(littleZooID + "/visit_pos");
                for(int i = 0; i < littleZooData.visitQueue.Count; i++)
                {
                    var pos = visitPos.GetChild(i);
                    pos.name = littleZooData.visitQueue[i].ToString();
                }

                var waitPos = transform.Find(littleZooID + "/wait_pos");
                for (int i = 0; i < littleZooData.waitQueue.Count; i++)
                {
                    var pos = waitPos.GetChild(i);
                    pos.name = littleZooData.waitQueue[i].ToString();
                }
            }

        }
#endif
    }
}

