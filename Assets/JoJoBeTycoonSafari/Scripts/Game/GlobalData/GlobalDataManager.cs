/*******************************************************************
* FileName:     GlobalDataManager.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-16
* Description:  
* other:    
********************************************************************/


using Logger;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.Common;
using UnityEngine;

namespace ZooGame.GlobalData
{
    public partial class GlobalDataManager : Singleton<GlobalDataManager>, ISingleton
    {
        bool isInit = false;

        public I18N i18n { get; protected set; }

        public LogicTableGroup logicTableGroup { get; protected set; }

        public LogicTableResource logicTableResource { get; protected set; }

        public LogicTableExitGate logicTableExitGate { get; protected set; }

        public LogicTableEntryGate logicTableEntryGate { get; protected set; }

        public LogicTableVisitorAction logicTableVisitorAction { get; protected set; }

        public AnimalAnimation animalAnimation { get; protected set; }

        public ZooGameSceneData zooGameSceneData { get; protected set; }

        public PlayerData playerData = null;

        /// <summary>
        /// 玩家动物栏场景UI类
        /// </summary>
        public LittleSceneUI littleSceneUI;

        private Vector3 sceneForward = Vector3.zero;
        public Vector3 SceneForward {
            get
            {
                float[] v = Config.globalConfig.getInstace().SceneForward;
                sceneForward.x = v[0];
                sceneForward.y = v[1];
                sceneForward.z = v[2];
                return sceneForward;
            }
        }

        public void Init()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            littleSceneUI = new LittleSceneUI();
            InitLogicTable();
            InitLogicRes();
            playerData = PlayerData.Load();
        }

        protected void InitLogicTable()
        {
            if (i18n == null)
            {
                i18n = new I18N();
            }

            if (logicTableGroup == null)
            {
                logicTableGroup = new LogicTableGroup();
            }

            if (logicTableResource == null)
            {
                logicTableResource = new LogicTableResource();
            }

            if (animalAnimation == null)
            {
                animalAnimation = new AnimalAnimation();
            }

            if (logicTableExitGate == null)
            {
                logicTableExitGate = new LogicTableExitGate();
            }

            if (logicTableEntryGate == null)
            {
                logicTableEntryGate = new LogicTableEntryGate();
            }

            if (logicTableVisitorAction == null)
            {
                logicTableVisitorAction = new LogicTableVisitorAction();
            }
        }

        protected void InitLogicRes()
        {
            if (zooGameSceneData == null)
            {
                zooGameSceneData = new ZooGameSceneData();
            }
        }
    }

}
