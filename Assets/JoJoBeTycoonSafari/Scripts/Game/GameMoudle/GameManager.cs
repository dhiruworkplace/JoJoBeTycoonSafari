/*******************************************************************
* FileName:     GameManager.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-8
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.Common;
using UFrame;
using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using UFrame.MessageCenter;
using Logger;
using System;

namespace ZooGame
{
    public class GameManagerTick : TickBase
    {
        public Dictionary<string, UIPage> tickedPages = new Dictionary<string, UIPage>();
        public override void Tick(int deltaTimeMS)
        {
            //MessageManager不受暂停和停止限制
            MessageManager.GetInstance().Tick();

            if (!this.CouldRun())
            {
                return;
            }

            GameModuleManager.GetInstance().Tick(deltaTimeMS);

            foreach(var val in tickedPages.Values)
            {
                if (val != null)
                {
                    val.Tick(deltaTimeMS);
                }
            }
        }
    }

    public class GameManager : SingletonMono<GameManager>
    {
        GameManagerTick tickObj;

        static int moduleOrderID = 0;

        public int tickCount;

        public override void Awake()
        {
            base.Awake();
            Init();
            this.Run();
        }

        public void Start()
        {
            var pd = GlobalDataManager.GetInstance().playerData;
            //if (pd.isFirst)
            //{
            //    ThirdPartTA.Identify();
            //    ThirdPartTA.StartTrack();
            //    ThirdPartTA.Track(TAEventsMonitorEnum.register);
            //}
            //else
            //{
            //    ThirdPartTA.StartTrack();
            //}

            //ThirdPartTA.TrackAppInstall();
            //每次登录写last_login_time
            //Logger.LogWarp.Log("ThirdPartTA.UserSet.last_login_time");
            //var taParam = new Dictionary<string, object>();
            //taParam.Add("last_login_time", DateTime.Now);
            //ThirdPartTA.UserSet(taParam);

            Logger.LogWarp.Log("LoadingMgr.Inst.isRunning = true");
            LoadingMgr.Inst.isRunning = true;
        }

        public void Update()
        {
            this.Tick(Math_F.FloatToInt1000(Time.deltaTime));
        }

        public void Init()
        {
            MessageManager.GetInstance().SetCallbackNotFoundMessage(this.OnNotFoundMessage);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_AddToTick, OnUIMessage_AddToTick);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_RemoveFromTick, OnUIMessage_RemoveFromTick);

            InitGlobaData();
            InitModule();
            PageMgr.SetButtonSound(Config.globalConfig.getInstace().UiButtonSoynd);
            this.tickObj = new GameManagerTick();
#if UNITY_EDITOR
            tickCount = 0;
#endif
        }

        public void Release()
        {
#if UNITY_EDITOR
            tickCount = 0;
#endif
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.LoadZooSceneFinished, OnLoadZooSceneFinished);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.UIMessage_AddToTick, OnUIMessage_AddToTick);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.UIMessage_RemoveFromTick, OnUIMessage_RemoveFromTick);

            UnLoadModule();
            tickObj.tickedPages.Clear();
            this.Stop();
        }

        public void Run()
        {
            tickObj.Run();
        }

        public void Stop()
        {
            tickObj.Stop();
        }
        public void Pause()
        {
            tickObj.Pause();
        }
        public void Pause(bool isPause)
        {
            tickObj.isPause = isPause;
        }

        public void Tick(int deltaTimeMS)
        {
#if UNITY_EDITOR
            tickCount++;
#endif
            tickObj.Tick(deltaTimeMS);
        }

        protected void AddPageToTick(UIPage page)
        {
            tickObj.tickedPages.Add(page.name, page);
        }

        protected void RemovePageFromTick(string pageName)
        {
            tickObj.tickedPages.Remove(pageName);
        }

        protected void InitModule()
        {
            //
            //玩家数据维护
            GameModuleManager.GetInstance().AddMoudle(new PlayerDataModule(moduleOrderID++));
            //动物数据维护
            GameModuleManager.GetInstance().AddMoudle(new AnimalModule(moduleOrderID++));

            //生成
            GameModuleManager.GetInstance().AddMoudle(new ParkingCenter(moduleOrderID++));
            GameModuleManager.GetInstance().AddMoudle(new SpawnModule(moduleOrderID++));

            //决策模块
            //大门
            GameModuleManager.GetInstance().AddMoudle(new EntryGateModule(moduleOrderID++));

            //动物栏
            GameModuleManager.GetInstance().AddMoudle(new LittleZooModule(moduleOrderID++));

            //Buff
            GameModuleManager.GetInstance().AddMoudle(new BuffModule(moduleOrderID++));

            //道具
            GameModuleManager.GetInstance().AddMoudle(new ItemModule(moduleOrderID++));

            //移动
            GameModuleManager.GetInstance().AddMoudle(new MoveMovableEntityMoudle(moduleOrderID++));

            //杂项模块
            GameModuleManager.GetInstance().AddMoudle(new MiscModule(moduleOrderID++));


            GameModuleManager.GetInstance().Stop();
        }

        protected void UnLoadModule()
        {
            GameModuleManager.GetInstance().Realse();
        }

        protected void InitGlobaData()
        {
            GlobalDataManager.GetInstance().Init();
        }

        protected void OnLoadZooSceneFinished(Message msg)
        {
            GameModuleManager.GetInstance().Run();
        }

        protected void OnUIMessage_AddToTick(Message msg)
        {
            var _msg = msg as UIMessage_AddToTick;

            tickObj.tickedPages.Add(_msg.page.name, _msg.page);
        }

        protected void OnUIMessage_RemoveFromTick(Message msg)
        {
            var _msg = msg as MessageString;
            RemovePageFromTick(_msg.str);
        }

        protected void OnNotFoundMessage(int messageID)
        {
            if (messageID < 10001)
            {
                //Logger.LogWarp.LogErrorFormat("消息未注册  {0}", (UFrameBuildinMessage)messageID);
                return;
            }

            //Logger.LogWarp.LogErrorFormat("消息未注册  {0}  {1}", (GameMessageDefine)messageID, (int)(GameMessageDefine)messageID);
        }
    }

}
