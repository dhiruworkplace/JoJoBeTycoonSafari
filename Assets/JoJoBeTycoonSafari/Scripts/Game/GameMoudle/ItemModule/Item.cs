using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Item
    {
        /// <summary>
        /// 道具唯一id，对应item表的主键
        /// </summary>
        public int itemID;

        /// <summary>
        /// 道具类型，对应item表的itemtype
        /// </summary>
        public int itemType;

        /// <summary>
        /// 道具使用方式，对应item表的use
        /// </summary>
        public int itemUse;

        /// <summary>
        /// 道具值，对应item表的itemval
        /// </summary>
        public string itemVal;

        public Item()
        {
            itemID = 0;
            itemType = 0;
            itemUse = 0;
            itemVal = "0";
        }
    }
}
