using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{
    /// <summary>
    /// 保存马路的数据类
    /// </summary>
    public class Road : MonoBehaviour
    {
        /// <summary>
        /// 当前马路行驶的方向
        /// -1代表左 → 右
        /// 1代表 右 → 左
        /// </summary>
        public int dir;
        /// <summary>
        /// 最小刷新间隔
        /// </summary>
        public float minInterval;

        /// <summary>
        /// 最大刷新间隔
        /// </summary>
        public float maxInterval;

        /// <summary>
        /// 马路上可以跑几辆车
        /// </summary>
        public int needCarNum;

        /// <summary>
        /// 马路的坐标
        /// </summary>
        private Vector3 roodPos;

        public Vector3 RoadPos
        {
            get
            {
                return roodPos;
            }
            set
            {
                roodPos = value;
                transform.position = roodPos;
            }
        }

        /// <summary>
        /// 行驶速度
        /// </summary>
        public float speed;



        public GameObject roodPrefab;
    }
}