/*******************************************************************
* FileName:     LittleZooBuildinPosEditor.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-17
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZooGame.Tools;
using System.IO;
using System.Text;
using Logger;
using UFrame.Path.StraightLine;

namespace ZooGame.Tools
{
    [CustomEditor(typeof(LittleZooBuildinPosRoot))]
    public class LittleZooBuildinPosEditor : Editor
    {
        private LittleZooBuildinPosRoot littleZooBuildinPos;

        void OnEnable()
        {
            littleZooBuildinPos = target as LittleZooBuildinPosRoot;
        }

        public override void OnInspectorGUI()
        {
            if (target == null || littleZooBuildinPos == null)
            {
                return;
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("刷新"))
            {
                littleZooBuildinPos.ProtecteRoot();
                littleZooBuildinPos.gameObject.SetActive(true);
            }

            if (GUILayout.Button("导出"))
            {
                littleZooBuildinPos.ProtecteRoot();
                littleZooBuildinPos.gameObject.SetActive(false);
                ExportToCSHARP();
                ShowPathLinesEditor.ApplyPrefab(littleZooBuildinPos.gameObject);
                //ShowPathLinesEditor.RemovePrefab(littleZooBuildinPos.gameObject);
            }
        }

        public void ExportToCSHARP()
        {
            string csharpFile = "///本代码由动物栏内置点编辑器自动生成, 请勿手动修改! by : Fan Zheng Yong\r\n" +
                "using System.Collections.Generic;\r\n" +
                "using UFrame.Common;\r\n" +
                "using UnityEngine;\r\n" +
                "namespace ZooGame\r\n" +
                "{\r\n" +
                "    public partial class LittleZooBuildinPosManager : Singleton<LittleZooBuildinPosManager>, ISingleton\r\n" +
                "    {\r\n" +
                "        public void AddAll()\r\n" +
                "        {\r\n";

            for (int i = 0; i < littleZooBuildinPos.transform.childCount; i++)
            {
                var littleZooNode = littleZooBuildinPos.transform.GetChild(i);

                //动物栏ID
                string littleZooID = "littleZoo_" + littleZooNode.name;

                //声明
                string littleZooST = string.Format("\r\n            var {0} = new LittleZooBuildinPos();\r\n", littleZooID);
                csharpFile += littleZooST;

                string setName = string.Format("            {0}.LittleZooID = {1};\r\n", littleZooID, littleZooNode.name);
                csharpFile += setName;

                //string addToMap = string.Format("            AddLittleZooBuildinPos({0});\r\n", littleZooID);
                //csharpFile += addToMap;

                var visitNode = littleZooNode.Find("visit_pos");
                //LogWarp.LogFormat("{0}, {1}, {2}", littleZooNode.name, visitNode.name, visitNode.childCount);
                for (int j = 0; j < visitNode.childCount; j++)
                {
                    //LogWarp.LogFormat("{0}, {1}, {2}", littleZooNode.name, visitNode.name, j);
                    var pos = visitNode.GetChild(j).position;
                    if (!littleZooBuildinPos.isYValid)
                    {
                        pos.y = 0f;
                    }
                    string insert = string.Format("            {0}.visitPosList.Add(new Vector3({1}f, {2}f, {3}f));\r\n",
                        littleZooID, pos.x, pos.y, pos.z);
                    csharpFile += insert;
                }

                var waitNode = littleZooNode.Find("wait_pos");
                for (int j = 0; j < waitNode.childCount; j++)
                {
                    var pos = waitNode.GetChild(j).position;
                    if (!littleZooBuildinPos.isYValid)
                    {
                        pos.y = 0f;
                    }
                    string insert = string.Format("            {0}.waitPosList.Add(new Vector3({1}f, {2}f, {3}f));\r\n",
                        littleZooID, pos.x, pos.y, pos.z);
                    csharpFile += insert;
                }

                var animalNode = littleZooNode.Find("animal_pos");
                for (int j = 0; j < animalNode.childCount; j++)
                {
                    var pos = animalNode.GetChild(j).position;
                    if (!littleZooBuildinPos.isYValid)
                    {
                        pos.y = 0f;
                    }
                    string insert = string.Format("            {0}.animalPosList.Add(new Vector3({1}f, {2}f, {3}f));\r\n",
                        littleZooID, pos.x, pos.y, pos.z);
                    csharpFile += insert;
                }

                string addToMap = string.Format("            AddLittleZooBuildinPos({0});\r\n", littleZooID);
                csharpFile += addToMap;
            }

            csharpFile += "        }\r\n";
            csharpFile += "    }\r\n";
            csharpFile += "}\r\n";

            FileUtil.CreateDir("./EditorConfig", true);
            string filepath = "./EditorConfig/LittleZooBuildinPosManager_AddAll.cs";
            File.WriteAllText(filepath, csharpFile, Encoding.UTF8);
            AssetDatabase.Refresh();
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

}
