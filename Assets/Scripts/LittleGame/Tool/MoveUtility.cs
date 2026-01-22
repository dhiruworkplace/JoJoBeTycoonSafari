using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace LittleGame
{

    /// <summary>
    /// 移动辅助类
    /// </summary>
    public class MoveUtility : MonoBehaviour
    {

        /// <summary>
        /// 当前移动的轴
        /// </summary>
        Vector3 axle;

        public float Speed;

        public Vector3 endPos;

        /// <summary>
        /// 记录当前的队列前面是否有空位
        /// </summary>
        public bool needCheckIsHasSpace;

        public bool isPause = false;

        public bool startMove = false;

        public bool isEnd = false;

        UnityAction<GameObject> unityAction;


        private void Start()
        {
            isEnd = false;

            EventManager.Add(ToolEventName.LittleGameEndFail, EndDispose);
            EventManager.Add(ToolEventName.LittleGameEndSuccess, EndDispose);
        }

        /// <summary>
        /// 结束处理
        /// </summary>
        private void EndDispose()
        {
            startMove = false;
            isEnd = true;

            if (tag.Equals("Player"))
            {
                Destroy(gameObject);
            }
            if (tag.Equals("Car"))
            {
                LittleController.Instance.littleGameInit.SaveCar(gameObject);
            }
        }


        private void FixedUpdate()
        {
            if (startMove && !isEnd)
            {
                if (Vector3.Distance(transform.position, endPos) >= 1)
                {
                    transform.position += axle * Speed;
                }
                else
                {
                    transform.position = endPos;
                    startMove = false;
                    unityAction?.Invoke(gameObject);
                }
            }
        }

        public void StartMove(float Speed, Vector3 endPos, UnityAction<GameObject> action, Vector3 axle)
        {
            //if (tag.Equals("Player"))
            //    Debug.Log("开始移动");

            ///开始移动之间检查下自己和终点的距离是不是很近
            ///如果是的话就直接移动过去
            if (Vector3.Distance(transform.position, endPos) <= 1)
            {
                transform.position = endPos;
                action?.Invoke(gameObject);
                return;
            }
            this.Speed = Speed;
            this.endPos = endPos;
            unityAction = action;
            startMove = true;
            this.axle = axle;
        }

        /// <summary>
        /// 停止当前的移动操作
        /// </summary>
        public void StopMove()
        {
            startMove = false;
        }

        private void OnDestroy()
        {
            EventManager.Remove(ToolEventName.LittleGameEndFail, EndDispose);
            EventManager.Remove(ToolEventName.LittleGameEndSuccess, EndDispose);
        }
    }
}