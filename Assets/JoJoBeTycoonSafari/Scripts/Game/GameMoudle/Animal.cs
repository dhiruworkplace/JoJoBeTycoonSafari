using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Animal
    {
        /// <summary>
        /// 动物表主键
        /// </summary>
        public int animalID;

        /// <summary>
        /// 动物等级
        /// </summary>
        public int animalLevel;

        /// <summary>
        /// 动物碎片数量
        /// </summary>
        public int animalPartNum;

        public override string ToString()
        {
            return string.Format("Animal {0}, {1}, {2}", animalID, animalLevel, animalPartNum);
        }
    }
}
