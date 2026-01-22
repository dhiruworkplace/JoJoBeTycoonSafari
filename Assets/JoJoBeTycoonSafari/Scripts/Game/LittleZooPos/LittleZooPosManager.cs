using Logger;
using System.Collections;
using System.Collections.Generic;
using UFrame.Common;
using UnityEngine;

namespace Game
{
    public partial class LittleZooPosManager : Singleton<LittleZooPosManager>, ISingleton
    {
        Dictionary<int, Vector3> posMap = new Dictionary<int, Vector3>();

        public void Init()
        {
            AddAll();
        }

        public Vector3 GetPos(int littleZooID)
        {
            Vector3 pos = Vector3.zero;

            posMap.TryGetValue(littleZooID, out pos);

            return pos;
        }
    }
}

