using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{
    /// <summary>
    /// 用于存储数据的读取
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void SaveData()
        {


        }


        /// <summary>
        /// 是否首次通关
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsFirstCustoms(int level)
        {
            return false;
        }

    }
}