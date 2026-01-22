using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalData
{
    public class ZooGameSceneData
    {
        /// <summary>
        /// 场景的相机父类
        /// </summary>
        public GameObject camera ;

        /// <summary>
        /// 动物栏的父节点
        /// </summary>
        public Transform littleZooParentNode;

        /// <summary>
        /// 停车场地块的父节点
        /// </summary>
        public GameObject ParckingSencePos;

        /// <summary>
        /// 售票口场景挂点数据
        /// </summary>
        public EntryGateSenceData entryGateSenceData;

        public Transform endNode;

        /// <summary>
        /// 额外加载的group
        /// group和动物栏的逻辑不一样
        /// group是有默认加载的情况，也就是场景制作时就有的
        /// </summary>
        Dictionary<int, GameObject> extendLoadGroup;

        Dictionary<int, GameObject> loadLittleZoo;

        public ZooGameSceneData()
        {
            Init();
        }

        protected void Init()
        {
            extendLoadGroup = new Dictionary<int, GameObject>();
            loadLittleZoo = new Dictionary<int, GameObject>();
            entryGateSenceData = new EntryGateSenceData();

        }

        public void Realse()
        {
            foreach(var val in extendLoadGroup.Values)
            {
                GameObject.Destroy(val);
            }
            extendLoadGroup.Clear();

            foreach (var val in loadLittleZoo.Values)
            {
                GameObject.Destroy(val);
            }
            loadLittleZoo.Clear();

            littleZooParentNode = null;
            endNode = null;
        }

        public void AddExtendLoadGroup(int groupID, GameObject go)
        {
            extendLoadGroup.Add(groupID, go);
        }

        public bool IsExtendGroupContains(int groupID)
        {
            return extendLoadGroup.ContainsKey(groupID);
        }

        public int GetExtendLoadGroupCount()
        {
            return extendLoadGroup.Count;
        }

        public void AddLoadLittleZoo(int littleZooID, GameObject go)
        {
            loadLittleZoo.Add(littleZooID, go);
        }

        public void RemoveLoadLittleZoo(int littleZooID)
        {
            GameObject go = null;
            if (!loadLittleZoo.TryGetValue(littleZooID, out go))
            {
                return;
            }

            GameObject.Destroy(go);
            loadLittleZoo.Remove(littleZooID);
        }

        
    }
}

