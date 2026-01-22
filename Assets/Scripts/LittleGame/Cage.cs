using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{

    /// <summary>
    /// 记录笼子的信息
    /// </summary>
    public class Cage : MonoBehaviour
    {

        private Vector3 pos;

        public Vector3 cagePos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                transform.position = pos;
            }
        }


        public GameObject currentFbs;

    }
}
