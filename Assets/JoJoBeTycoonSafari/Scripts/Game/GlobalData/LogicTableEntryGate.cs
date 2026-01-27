using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZooGame.GlobalData
{
    public class LogicTableEntryGate
    {
        public List<int> sortGateIDs;

        public LogicTableEntryGate()
        {
            Init();
        }

        protected void Init()
        {
            sortGateIDs = new List<int>();

            foreach (var kv in Config.ticketConfig.getInstace().AllData)
            {
                int gateID;
                if (!int.TryParse(kv.Key, out gateID))
                {
                    string e = string.Format("ticket 表错误，主键不是数字型 {0}", kv.Key);
                    throw new System.Exception(e);
                }
                sortGateIDs.Add(gateID);
            }

            sortGateIDs.Sort();
        }

    }

}

