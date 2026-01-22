using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace LittleGame
{
    /// <summary>
    /// 辅助工具类
    /// </summary>
    public class Utility
    {
        public static int GetARandomIntNumber(int maxNum)
        {
            return Random.Range(1, maxNum);
        }

        /// <summary>
        /// 清理list
        /// </summary>
        /// <param name="targetList"></param>
        public static void ClearList(List<Transform> targetList)
        {
            List<Transform> removeList = new List<Transform>();
            for (int i = 0; i < targetList.Count; i++)
            {
                removeList.Add(targetList[i]);
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                GameObject.Destroy(removeList[i].gameObject);
                targetList.Remove(removeList[i]);
            }
            targetList.Clear();
        }

        /// <summary>
        /// 删除所有的子对象
        /// </summary>
        /// <param name="target">父对象</param>
        public static void DestoryAllChild(Transform target)
        {
            for (int i = 0; i < target.childCount; i++)
            {
                GameObject.Destroy(target.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 清理list
        /// </summary>
        /// <param name="targetList"></param>
        public static void ClearList(List<GameObject> targetList)
        {
            List<GameObject> removeList = new List<GameObject>();
            for (int i = 0; i < targetList.Count; i++)
            {
                removeList.Add(targetList[i]);
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                GameObject.Destroy(removeList[i]);
                targetList.Remove(removeList[i]);
            }
            targetList.Clear();
        }
        /// <summary>
        /// 清理list
        /// </summary>
        /// <param name="targetList"></param>
        public static void ClearList(Queue<Transform> targetList)
        {
            Queue<Transform> removeList = new Queue<Transform>();
            for (int i = 0; i < targetList.Count; i++)
            {
                removeList.Enqueue(targetList.Dequeue());
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                GameObject.Destroy(removeList.Dequeue().gameObject);
            }
            targetList.Clear();
        }


        /// <summary>
        /// 检查当前是第几条马路
        /// </summary>
        /// <param name="road"></param>
        /// <returns></returns>
        public static int CheckCurrentRoadIndex(Road road)
        {
            string roadName = road.name;
            int index = 0;
            if (roadName.Contains("1"))
            {
                index = 1;
            }
            else if (roadName.Contains("2"))
            {
                index = 2;
            }
            else if (roadName.Contains("3"))
            {
                index = 3;
            }
            else if (roadName.Contains("4"))
            {
                index = 4;
            }
            else if (roadName.Contains("5"))
            {
                index = 5;
            }
            else 
            {
                index = 6;
            }

            return index;
        }


        /// <summary>
        /// 转向自己的后方
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action">结束回调</param>
        public static Tweener TurnBack(Transform target,float dir)
        {
            //float yEuler = (target.eulerAngles.y + 180) % 360;
            Tweener tweener = target.DORotate(new Vector3(0, dir, 0), 0.5f);
            //tweener.OnComplete(action);
            return tweener;
            
        }


        public static void DelayledExecute(float time,UnityAction action)
        {
            LittleController.Instance.StartCoroutine("");
        }

    }
}