using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UFrame.Path.StraightLine;

namespace Game.Tools
{
    [CustomEditor(typeof(ZooCamera))]
    public class ZooCameraEditor : Editor
    {
        private ZooCamera zooCamera;

        void OnEnable()
        {
            zooCamera = target as ZooCamera;
        }

        public override void OnInspectorGUI()
        {
            if (target == null || zooCamera == null)
            {
                return;
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("记录左下角"))
            {
                zooCamera.editorCamLeftDownPos = zooCamera.GetCameraPos();
                Save();
            }

            if (GUILayout.Button("记录左上角"))
            {
                zooCamera.editorCamLeftUpPos = zooCamera.GetCameraPos();
                Save();
            }

            if (GUILayout.Button("记录右上角"))
            {
                zooCamera.editorCamRightUpPos = zooCamera.GetCameraPos();
                Save();
            }

            if (GUILayout.Button("记录右下角"))
            {
                zooCamera.editorCamRightDownPos = zooCamera.GetCameraPos();
                Save();
            }



            if (GUILayout.Button("记录最远拉升"))
            {
                zooCamera.maxOrthographicSize = zooCamera.GetCamera().orthographicSize;
                Save();
            }

            if (GUILayout.Button("记录最近拉升"))
            {
                zooCamera.minOrthographicSize = zooCamera.GetCamera().orthographicSize;
                Save();
            }

            if (GUILayout.Button("记录进入正对"))
            {
                zooCamera.editorInitPos = zooCamera.GetCameraPos();
                Save();
            }
        }

        protected void Save()
        {
            ShowPathLinesEditor.ApplyPrefab(zooCamera.gameObject);
        }
    }
}

