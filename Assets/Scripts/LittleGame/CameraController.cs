using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.Common;

namespace LittleGame 
{

    /// <summary>
    /// 摄像机移动类
    /// </summary>
    public class CameraController : SingletonMono<CameraController> 
    {

        /// <summary>
        /// 摄像机移动速度
        /// </summary>
        public float cameraSpeed = 3f;
        /// <summary>
        /// 当前摄像机的移动目标
        /// </summary>
        private Vector3 targetPos = Vector3.zero;

        public bool isCameraMoving = false;


        /// <summary>
        /// 使摄像机保持相等间距
        /// </summary>
        private float limitDis;
        /// <summary>
        /// 摄像机移动维护坐标
        /// </summary>
        private float zPos;//距离

        private Vector3 cameraDefaultPos;
      
        private void Start() 
        {
            limitDis = LittleController.Instance.gameStartPos.z - transform.position.z;
            cameraDefaultPos = transform.position;
        }

        /// <summary>
        /// 移动摄像机到目标点
        /// </summary>
        /// <param name="targetPos">目标点坐标</param>
        public void MoveCameraToTarget(Vector3 targetPos) 
        {
            //Debug.Log("摄像机移动"+targetPos);
            if (targetPos == Vector3.zero) {
                Logger.LogWarp.LogError( "摄像机移动 输入的参数不合法" + this );
            }
            zPos = transform.position.z;
            isCameraMoving = true;
            this.targetPos = targetPos;
        }
        void LateUpdate() 
        {
            //利用是否移动的状态去判断是否调用移动代码
            if (isCameraMoving == true) 
            {
                MoveCamera();
            }
        }
        /// <summary>
        /// 摄像机插值移动函数块
        /// 如果距离小于0.1，则isCameraMoving为false
        /// </summary>
        private void MoveCamera() 
        {
            zPos = Mathf.Lerp( zPos, Mathf.Abs( targetPos.z - limitDis ), Time.deltaTime * cameraSpeed );
            transform.position = new Vector3( transform.position.x, transform.position.y, zPos );
            if (Mathf.Abs( transform.position.z - ( targetPos.z - limitDis ) ) <= 0.1f) {
                transform.position = new Vector3( transform.position.x, transform.position.y, targetPos.z - limitDis );
                isCameraMoving = false;
                targetPos = Vector3.zero;
            }
            return;
        }
        /// <summary>
        /// 设置摄像机到初始位置
        /// </summary>
        public void SetCameraToDefaultPos() 
        {
            transform.position = cameraDefaultPos;
            isCameraMoving = false;
        }

    }

}