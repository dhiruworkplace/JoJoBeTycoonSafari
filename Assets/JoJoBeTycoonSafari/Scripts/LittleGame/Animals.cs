using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{

    public class Animals : MonoBehaviour
    {
        public GameObject animalsPrefab;

        public string animalsName;

        public int id;

        private Vector3 pos;

        /// <summary>
        /// 当前小动物是否处于忙的状态下
        /// </summary>
        public bool isBusy = false;

        /// <summary>
        /// 当前的转向  用来让小动物向后向前看
        /// </summary>
        public float dirEuler;

        private void Start()
        {
            EventManager.Add(ToolEventName.NeedCheckSpace, CheckSelfPos);
        }

        /// <summary>
        /// 检查自己的位置
        /// </summary>
        public void CheckSelfPos()
        {
            ///检查自己到达当前终点之后,自己的前面有没有空位.
            ///
            //int selfIndex = 0;
            //for (int i = 0; i < transform.parent.childCount; i++)
            //{
            //    if (transform.parent.GetChild(i) == (transform))
            //    {
            //        selfIndex = i;
            //        break;
            //    }
            //}

            //Debug.Log("自己的位置是" + selfIndex);

            ///自己是第一个 不需要检查
            if (id == 0)
            {
                return;
            }

            //int spaceNum = 0;
            //int forwardIndex = -1;
            //for (int i = 0; i <= id; i++)
            //{
            //    if (transform.parent.GetChild(i).gameObject == gameObject)
            //        continue;
            //    if (!transform.parent.GetChild(i).gameObject.activeInHierarchy)
            //    {
            //        spaceNum++;
            //        forwardIndex = i;
            //        break;
            //    }
            //}

            //if (forwardIndex == -1)
            //{
            //    Debug.Log("不需要补位");
            //    return;
            //}

            ////if (spaceNum == 0)
            ////{
            ////    pos = Vector3.zero;
            ////}
            ////else
            ////{
            //    //if (forwardIndex == -1)
            //    //{
            //    //    Debug.Log("需要补位到第一个");
            //    //    pos = transform.parent.GetChild(0).GetComponent<MoveUtility>().endPos;
            //    //}
            //    Debug.Log(spaceNum + "需要补位到第" + (forwardIndex - 1));
            //    if(LittleController.Instance.currentTurnPos.ContainsKey(transform.parent.GetChild(forwardIndex)))
            //        Debug.Log(LittleController.Instance.currentTurnPos[transform.parent.GetChild(forwardIndex)]);
            //    //var endPos = transform.parent.GetChild(forwardIndex - 1).GetComponent<MoveUtility>().endPos;
            //    //return transform.parent.GetChild(forwardIndex-1).position - (spaceNum * LittleGameDefine.AnimalWidth);
            //    pos = LittleController.Instance.currentTurnPos[transform.parent.GetChild(forwardIndex)];

            //    transform.GetComponent<MoveUtility>().endPos = pos;

            ////}
            ///

            
        }



    }
}
