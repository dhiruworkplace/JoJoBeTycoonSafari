using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalData
{
    public class ResourceKeyCell
    {
        public int key;
        public Config.resourceCell cell;
    }

    public class LogicTableResource
    {
        Dictionary<int, List<ResourceKeyCell>> resTypeMap = new Dictionary<int, List<ResourceKeyCell>>();

        public LogicTableResource()
        {
            Init();
        }

        protected void Init()
        {
            foreach (var kv in Config.resourceConfig.getInstace().AllData)
            {
                var cell = kv.Value;
                List<ResourceKeyCell> cellList = null;
                if (!resTypeMap.TryGetValue(cell.restype, out cellList))
                {
                    cellList = new List<ResourceKeyCell>();
                    resTypeMap.Add(cell.restype, cellList);
                }

                var resourceKeyCell = new ResourceKeyCell();
                if (!int.TryParse(kv.Key, out resourceKeyCell.key))
                {
                    string e = string.Format("资源表数据异常 {0}", resourceKeyCell.key);
                    throw new System.Exception(e);
                }
                resourceKeyCell.cell = cell;
                cellList.Add(resourceKeyCell);
            }
        }

        public List<ResourceKeyCell> GetResListByResType(int resType)
        {
            List<ResourceKeyCell> result = null;

            resTypeMap.TryGetValue(resType, out result);

            return result;
        }
    }
}

