using ZooGame;
using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UFrame;
using UFrame.Common;
using UFrame.MessageCenter;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInteractive : SingletonMono<UIInteractive>
{
    /// <summary>
    /// 获取Data，方便获取动物园
    /// </summary>
    PlayerData playerData;
    public UIPage iPage;
    Dictionary<string, System.Action> clickCallbacks = new Dictionary<string, System.Action>();
    bool showUI;
    string nameID;
    string needShowID;
    Transform sceneUIButtonPos;
    /// <summary>
    /// 点击动画播放器
    /// </summary>
    SimpleAnimation buildingClickSa;

    public override void Awake()
    {
        base.Awake();

        buildingClickSa = new SimpleAnimation();

        //初始化
        Init();
        showUI = false;
        MessageManager.GetInstance().Regist((int)GameMessageDefine.OpenNewLittleZoo, this.OnOpenNewLittleZoo);
        clickCallbacks.Add(Config.globalConfig.getInstace().ParkingButton, OnClickParking);
        clickCallbacks.Add(Config.globalConfig.getInstace().StationButton, OnClickEntry);
        //Advertisements.Instance.Initialize();
    }
    private void OnOpenNewLittleZoo(Message obj)
    {
        this.Init();
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="gesture"></param>
    public void OnTapUIGB()
    {
        //若没有UI显示界面，显示，否则隐藏Ui界面
        if (iPage == null)
        {
            showUI = false;
        }
        if (showUI == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                showUI = true;
                DisposeUIInteractive(hit.collider.gameObject);
            }
        }
        else
        {
            showUI = false;
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide != true)
            {
                PageMgr.ClosePage(iPage.name);
                MessageString.Send((int)GameMessageDefine.UIMessage_OnClickButShowPart, "UIMainPage");
            }
            //else if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            //{
            //}
            else
            {
                return;
            }

        }
    }
    /// <summary>
    /// 处理养成UI点击交互
    /// </summary>
    /// <param name="gameObject">点击对象</param>
    void DisposeUIInteractive(GameObject gameObject)
    {
        LogWarp.LogError("点击了  " + gameObject.name);
        nameID = gameObject.name;
        Action action = null;
        if (clickCallbacks.TryGetValue(gameObject.name, out action))
        {
            sceneUIButtonPos = gameObject.transform;
            action?.Invoke();
            var anim = gameObject.GetComponentInChildren<Animation>();
            if (anim != null)
            {
                buildingClickSa.Init(anim);
                buildingClickSa.Play(Config.globalConfig.getInstace().BuildClickAnim);
            }
            ZooCamera.GetInstance().PointAtScreenUpCenter(gameObject.transform.position);
            MessageString.Send((int)GameMessageDefine.UIMessage_OnClickButHidePart, "UIMainPage");

        }
        //if (gameObject.name == needShowID)
        //{
        //    ZooCamera.GetInstance().PointAtScreenUpCenter(gameObject.transform.position);
        //    MessageString.Send((int)GameMessageDefine.UIMessage_OnClickButHidePart, "UIMainPage");
        //}
    }

    public void OnClickParking()
    {
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
            if (uIGuidePage ==null)
            {
                string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                throw new System.Exception(e);
            }
            if (uIGuidePage.procedure != 5)
            {
                return;
            }
            else
            {  //取消场景特效  进入场景点击事件
                uIGuidePage.DestroyEffectChild();
				GameEventManager.SendEvent("guild_1");
				uIGuidePage.SetCameraOnClickScene(sceneUIButtonPos);
            }
        }
        PageMgr.ShowPage<UIParkPage>();  //停车场UI交互
        iPage = new UIParkPage();
        //Advertisements.Instance.ShowInterstitial();
    }

    public void OnClickEntry()
    {
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
            if (uIGuidePage == null)
            {
                string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                throw new System.Exception(e);
            }
            if (uIGuidePage.procedure != 12)
            {
                return;
            }
            else
            {  //取消场景特效  进入场景点击事件
                uIGuidePage.DestroyEffectChild();
				GameEventManager.SendEvent("guild_1");
                uIGuidePage.SetCameraOnClickScene(sceneUIButtonPos);
            }
        }
        PageMgr.ShowPage<UIEntryPage>();  //摆渡车UI交互
        iPage = new UIEntryPage();
    }
    public void OnClickZoo()
    {
        //判断当前动物栏的等级是否不为0   为0开启新的动物栏
        int litteZooLevel = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(int.Parse(nameID)).littleZooTicketsLevel;
        if (litteZooLevel > 0)
        {
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
                if (uIGuidePage == null)
                {
                    string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                    throw new System.Exception(e);
                }
                if (uIGuidePage.procedure != 20)
                {
                    return;
                }
                else
                {  //取消场景特效  进入场景点击事件
                    uIGuidePage.DestroyEffectChild();
					GameEventManager.SendEvent("guild_1");
					uIGuidePage.SetCameraOnClickScene(sceneUIButtonPos);
                }
            }

            //显示UI
            PageMgr.ShowPage<UIZooPage>(nameID);  //大象馆UI交互 
            iPage = new UIZooPage();
        }
        else
        {
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
            {
                return;
            }
            PageMgr.ShowPage<UIBuildOpenPage>(nameID);  //开启新的动物园交互
            iPage = new UIBuildOpenPage();
        }
    }
    private void Init()
    {
        //对clickCallbacks字典进行数据填充
        //获取数据
        playerData = GlobalDataManager.GetInstance().playerData;
        foreach (var item in playerData.playerZoo.littleZooModuleDatas)
        {
            LogWarp.Log(item.littleZooID);
            nameID = gameObject.name;
            if (!clickCallbacks.ContainsKey(item.littleZooID.ToString()))
            {
                LogWarp.Log("不包含此  " + item.littleZooID);
                int level = playerData.GetLittleZooModuleData(item.littleZooID).littleZooTicketsLevel;
                clickCallbacks.Add(item.littleZooID.ToString(), OnClickZoo);
                if (level < 1)
                {
                    break;
                }
            }
        }
        LogWarp.Log("获取了最新的动物栏列表数据");
    }

}
