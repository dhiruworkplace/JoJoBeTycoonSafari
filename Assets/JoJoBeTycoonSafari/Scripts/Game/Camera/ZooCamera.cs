using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.OrthographicCamera;
using Logger;
using Game.MessageCenter;
using UFrame;
using UFrame.MessageCenter;

namespace Game
{
    public class ZooCamera : FingerCamera
    {
        public Vector3 gameCamLeftUpPos;
        public Vector3 gameCamRightUpPos;

        public List<Vector2> cameraArea = new List<Vector2>();
        
        float offset = 0f;
        public override void Init()
        {
            if (LoadingMgr.Inst.runTimeLoaderType == RunTimeLoaderType.Editor)
            {
                base.SetRange();
            }

            MessageManager.GetInstance().Regist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastOpenNewLittleZoo, OnBroadcastOpenNewLittleZoo);
        }

        public override void OnPinch(PinchGesture gesture)
        {
            if (LoadingMgr.Inst.runTimeLoaderType == RunTimeLoaderType.Game &&  !LoadingMgr.Inst.debugCamera)
            {
                base.OnPinch(gesture);
                return;
            }

            if (gesture.Phase == ContinuousGesturePhase.Started)
            {
                //MessageString.Send((int)UFrameBuildinMessage.CameraDebug, Time.realtimeSinceStartup + " OnPinch true");
                pinching = true;
            }
            else if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
                if (pinching)
                {
                    cacheCam.orthographicSize -= gesture.Delta.Centimeters() * pinchSensitivity;
                }
            }
            else if (gesture.Phase == ContinuousGesturePhase.Ended)
            {
                //if (pinching)
                {
                    pinching = false;
                }
                //MessageString.Send((int)UFrameBuildinMessage.CameraDebug, Time.realtimeSinceStartup + " OnPinch false");
            }
        }

        public override bool CouldMoveTo(Vector2 moveTo)
        {
            if (LoadingMgr.Inst.runTimeLoaderType == RunTimeLoaderType.Editor)
            {
                return base.CouldMoveTo(moveTo);
            }

            return Math_F.IsPointInPolygon(moveTo, cameraArea);
        }

        public override void WhenCannotMoveTo(Vector2 moveTo)
        {
            Vector2 camPos2D = Math_F.Vector3_2D(cacheTrans.position);
            Vector2 tryPos = camPos2D + (moveTo - camPos2D).normalized * 0.5f;
#if UNITY_EDITOR
            Vector3 tryPos3D = cacheTrans.position;
            tryPos3D.x = tryPos.x;
            tryPos3D.z = tryPos.y;
            tryPoint.position = tryPos3D;

            Vector3 moveToPos3D = cacheTrans.position;
            moveToPos3D.x = moveTo.x;
            moveToPos3D.z = moveTo.y;
            moveToPoint.position = moveToPos3D;
#endif
            Vector2 lineStart;
            int idxStart;
            Vector2 lineEnd;
            int idxEnd;
            Vector2 crossPos = Math_F.GetClosestCrossPoint(tryPos, cameraArea, out lineStart, out idxStart, out lineEnd, out idxEnd);
#if UNITY_EDITOR
            LogWarp.LogErrorFormat("WhenCannotMoveTo moveToPoint={0}, crossPos={1}, lineStart={2}, idxStart={3}, lineEnd={4}, idxEnd={5}", 
                moveToPos3D, crossPos, lineStart, idxStart, lineEnd, idxEnd);
#endif
            bool isCrossInLine = false;
            Vector2 crossDir = Vector2.zero; 
            for (int i = 0; i < cameraArea.Count; i++)
            {
                if (Math_F.Approximate2D(crossPos, cameraArea[i], 0.5f))
                {
                    LogWarp.LogErrorFormat("WhenCannotMoveTo crossPos == {0}", i);
                    isCrossInLine = true;
                    crossDir = (cameraArea[(i + 1) % cameraArea.Count] - cameraArea[i]).normalized;
                    break;
                }
            }
            //Debug.LogError("GetClosestCrossPoint");
            //if (Math_F.IsPointInPolygon(crossPos, cameraArea))
            {
                //Debug.LogError("IsPointInPolygon");
                Vector3 camPos3D = cacheTrans.position;
                camPos3D.x = crossPos.x;
                camPos3D.z = crossPos.y;

#if UNITY_EDITOR
                crossPoint.position = camPos3D;
#endif

                switch (dragType)
                {
                    case DragType.Normal:
                        cacheTrans.position = camPos3D;
                        break;
                    case DragType.NormalPlus:   
                        if (!isCrossInLine)
                        {
                            dragSmoothDir = (lineEnd - lineStart).normalized * crossSensitivity;
                            float dot = Vector2.Dot((moveTo - camPos2D).normalized, (lineEnd - lineStart).normalized);
                            if (dot < 0)
                            {
                                dragSmoothDir *= -1f;
                            }
                        }
                        else
                        {
                            dragSmoothDir = crossDir * crossSensitivity;
                        }

                        break;
                    default:
                        string e = string.Format("不支持滑动类型{0}", dragType);
                        throw new System.Exception(e);
                }
            }
        }


        protected void OnLoadZooSceneFinished(Message msg)
        {
            cacheTrans.position = editorInitPos;
            this.cacheCam.orthographicSize = this.maxOrthographicSize;
            dragMoveTo = cacheTrans.position;

            LogWarp.LogFormat("camera {0}", GetLoadGroup());
            if (LoadingMgr.Inst.runTimeLoaderType == RunTimeLoaderType.Game)
            {
                offset = (Config.globalConfig.getInstace().InitMaxGroupNum - GetLoadGroup()) * Config.globalConfig.getInstace().ZooPartResLen;

                gameCamLeftUpPos = editorCamLeftUpPos;
                gameCamLeftUpPos.x += offset;

                gameCamRightUpPos = editorCamRightUpPos;
                gameCamRightUpPos.x += offset;
            }

            cameraArea.Clear();
            cameraArea.Add(Math_F.Vector3_2D(editorCamLeftDownPos));
            cameraArea.Add(Math_F.Vector3_2D(gameCamLeftUpPos));
            cameraArea.Add(Math_F.Vector3_2D(gameCamRightUpPos));
            cameraArea.Add(Math_F.Vector3_2D(editorCamRightDownPos));

#if UNITY_EDITOR
            SetRange();
#else
            GameObject.Find("camer_range").SetActive(false);
#endif

        }

        protected void OnBroadcastOpenNewLittleZoo(Message msg)
        {
            var _msg = msg as BroadcastOpenNewLittleZoo;

            //是否需要加载额外地块
            if (!_msg.isTriggerExtend)
            {
                return;
            }

            WhenExtendScene(_msg.triggerLoadGroupID);
#if UNITY_EDITOR
            SetRange();
#endif
        }

        public int GetLoadGroup()
        {
            return ZooGameLoader.GetInstance().loadGroup.Count;
        }

        public Vector3 GetCameraPos()
        {
            return cacheTrans.position;
        }

        public Camera GetCamera()
        {
            return cacheCam;
        }

        Vector3 hStart;
        Vector3 hEnd;
        Vector3 vStart;
        Vector3 vEnd;
        public override void Update()
        {
            base.Update();
#if UNITY_EDITOR
            hStart = cacheCam.ViewportToWorldPoint(new Vector3(0f, 0.5f, cacheCam.nearClipPlane));
            hEnd = cacheCam.ViewportToWorldPoint(new Vector3(1f, 0.5f, cacheCam.nearClipPlane));

            vStart = cacheCam.ViewportToWorldPoint(new Vector3(0.5f, 0f, cacheCam.nearClipPlane));
            vEnd = cacheCam.ViewportToWorldPoint(new Vector3(0.5f, 1f, cacheCam.nearClipPlane));
#endif
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(hStart, hEnd);
            Gizmos.DrawLine(vStart, vEnd);
        }

        public override void SetRange()
        {
            //GameObject editorPosRange = GameObject.Find("camer_range/editor_pos_range");
            //SetRangPos(editorPosRange, editorCamLeftDownPos, editorCamLeftUpPos, editorCamRightUpPos, editorCamRightDownPos);
            base.SetRange();

            GameObject gamePosRange = GameObject.Find("camer_range/game_pos_range");
            SetRangPos(gamePosRange, editorCamLeftDownPos, gameCamLeftUpPos, gameCamRightUpPos, editorCamRightDownPos);
        }

        protected void WhenExtendScene(int groupID)
        {
            LogWarp.LogFormat("camera {0}", GetLoadGroup());
            if (LoadingMgr.Inst.runTimeLoaderType == RunTimeLoaderType.Game)
            {
                var cell = Config.groupConfig.getInstace().getCell(groupID);
                //gameCamLeftUpPos.x -= Config.globalConfig.getInstace().ZooPartResLen;
                gameCamLeftUpPos.x -= cell.groundsize;
                //gameCamRightUpPos.x -= Config.globalConfig.getInstace().ZooPartResLen;
                gameCamRightUpPos.x -= cell.groundsize;
            }

            cameraArea.Clear();
            cameraArea.Add(Math_F.Vector3_2D(editorCamLeftDownPos));
            cameraArea.Add(Math_F.Vector3_2D(gameCamLeftUpPos));
            cameraArea.Add(Math_F.Vector3_2D(gameCamRightUpPos));
            cameraArea.Add(Math_F.Vector3_2D(editorCamRightDownPos));
        }

        public override bool IsPointOnUI()
        {
            return base.IsPointOnUI();
        }
    }

}

