using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.GlobalData
{
    public class LogicTableExitGate
    {
        public List<int> sortExitGateIDs;

        public LogicTableExitGate()
        {
            Init();
        }

        protected void Init()
        {
            sortExitGateIDs = new List<int>();

            foreach (var kv in Config.exitgateConfig.getInstace().AllData)
            {
                int exitGateID;
                if (!int.TryParse(kv.Key, out exitGateID))
                {
                    string e = string.Format("exitGate 表错误，exitGateID 不是数字型 {0}", kv.Key);
                    throw new System.Exception(e);
                }
                sortExitGateIDs.Add(exitGateID);
            }

            sortExitGateIDs.Sort();
        }
    }

}

