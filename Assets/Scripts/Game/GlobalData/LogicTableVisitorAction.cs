using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalData
{
    public class LogicTableVisitorAction
    {
        public List<Config.npcactionCell> actionList { get; protected set; }

        public LogicTableVisitorAction()
        {
            Init();
        }

        protected void Init()
        {
            actionList = new List<Config.npcactionCell>();
            foreach (var kv in Config.npcactionConfig.getInstace().AllData)
            {
                actionList.Add(kv.Value);
            }
        }
    }
}
