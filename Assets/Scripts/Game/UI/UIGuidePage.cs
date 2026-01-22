using DG.Tweening;
using Game;
using Game.GlobalData;
using Game.MessageCenter;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UFrame;
using UFrame.EntityFloat;
using UFrame.MiniGame;
using UFrame.OrthographicCamera;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIGuidePage : UIPage
{
    /// <summary>
    /// 对话的按钮    即下一步
    /// </summary>
    private Button dialogBoxButton;
    /// <summary>
    /// 对话的文本
    /// </summary>
    private Text dialogText;
    /// <summary>
    /// NPC的名字
    /// </summary>
    private Text npcNameText;
    /// <summary>
    /// 引导页面的遮罩
    /// </summary>
    private MaskableGraphic OpenTouch;
    /// <summary>
    /// 对话按钮的遮罩
    /// </summary>
    private MaskableGraphic dialogButtonMaskableGraphic;

    private GameObject mainMesh;

    private Button uibg;

    /// <summary>
    /// 当前引导步骤
    /// </summary>
    public int procedure;
    /// <summary>
    /// 相机跟随需要的对象
    /// </summary>
    public EntityMovable entity;

    /// <summary>
    /// 场景特效节点
    /// </summary>
    private Transform effectNode;

    public int number;
    PlayerZoo playerZoo;

    public UIGuidePage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None, UITickedMode.Update)
    {
        uiPath = "uiprefab/UIGuide";
    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        this.RegistCompent();
        GetTransPrefabAllTextShow(this.transform);
    }

    /// <summary>
    /// 活跃
    /// </summary>
    public override void Active()
    {
        base.Active();
        MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButHidePart, "UIMainPage");
        if (GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(1001).littleZooTicketsLevel >= 5&& procedure==0)
        {
            GlobalDataManager.GetInstance().playerData.playerZoo.isGuide = false;
            Hide();
        }
        dialogBoxButton.enabled = true;
        uibg.enabled = true;

        OnClickDialogBoxButton();
        number = 0;
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == false)
        {
            ZooCamera.GetInstance().dragMoveTo = ZooCamera.GetInstance().editorInitPos;
            ZooCamera.GetInstance().cacheTrans.position = ZooCamera.GetInstance().editorInitPos;
        }
        base.Hide();
    }
    /// <summary>
    /// 内部组件的查找赋值
    /// </summary>
    private void RegistCompent()
    {
        //dialogBoxButton = RegistBtnAndClick("GameObject/UiBg/MainMesh/GuideGroup", OnClickDialogBoxButton);
        //uibg = transform.Find("GameObject/UiBg").GetComponent<Button>();
        //uibg.onClick.AddListener(delegate
        //{
        //    OnClickDialogBoxButton("uibg");
        //    BtnScaleAnim(dialogBoxButton.gameObject, 1.1f, 0.95f);
        //});

        dialogBoxButton = transform.Find("GameObject/UiBg/MainMesh/GuideGroup").GetComponent<Button>();
        uibg = transform.Find("GameObject/UiBg").GetComponent<Button>();
        dialogBoxButton.onClick.AddListener(OnClickDialogBoxButton);
        uibg.onClick.AddListener(OnClickDialogBoxButton);

        dialogText = RegistCompent<Text>("GameObject/UiBg/MainMesh/GuideGroup/GuideText");
        npcNameText = RegistCompent<Text>("GameObject/UiBg/MainMesh/GuideGroup/NpcNameBg/NpcNameText");
        npcNameText.text = this.AcquireData(0);
        dialogButtonMaskableGraphic = RegistCompent<MaskableGraphic>("GameObject/UiBg/MainMesh/GuideGroup");

        mainMesh = transform.Find("GameObject/UiBg/MainMesh").gameObject;
        //默认进入步骤第一步
        procedure = 0;
        //游戏暂停
        GameManager.GetInstance().Pause(true);
    }

    /// <summary>
    /// 获取引导页面的点击事件
    /// </summary>
    /// <param name="obj"></param>
    public void OnClickDialogBoxButton()
    {

        Debug.LogError("新手阶段：  OnClickDialogBoxButton   步骤 = "+ procedure );

        switch (procedure)
        {
            case 0:
                procedure = 1;
                this.InitCommint(procedure);        //文本刷新
				GameEventManager.SendEvent("guild_" + procedure);
                break;
            case 1:
                procedure = 2;
                this.InitCommint(procedure);        //文本刷新
				GameEventManager.SendEvent("guild_" + procedure);
				break;
            case 2:
                procedure = 3;
                GameManager.GetInstance().Pause(false);
				GameEventManager.SendEvent("guild_" + procedure);
				SetCameraToFollow();                //设置相机跟随
                DelayedHide();                      //隐藏新手引导界面
                break;
            case 3:
                procedure = 4;
				GameEventManager.SendEvent("guild_" + procedure);                  //本阶段到达了停车场 游戏暂停
				GameManager.GetInstance().Pause(true);
                this.InitCommint(procedure);        //文本刷新
                break;
            case 4:
                procedure = 5;
				GameEventManager.SendEvent("guild_" + procedure);                //本阶段需要取消文本  添加场景UI特效
                playerZoo = GlobalDataManager.GetInstance().playerData.playerZoo;
                if (playerZoo.parkingCenterData.parkingSpaceLevel>1)
                {
                    Logger.LogWarp.LogError("新手阶段：    "+ playerZoo.parkingCenterData.parkingSpaceLevel);
                    OnClickDialogBoxButton();
                }
                else
                {
                    Logger.LogWarp.LogError("新手阶段：   添加场景UI特效    隐藏新手引导界面");
                    this.SetSceneAnimateGameObject(procedure);//添加场景UI特效
                    DelayedHide();                      //隐藏新手引导界面
                }

                break;
            case 5:
                procedure = 7;
				GameEventManager.SendEvent("guild_" + procedure);                     //本阶段显示文本
                this.InitCommint(procedure);
                break;

            case 7:
                procedure = 8;
				GameEventManager.SendEvent("guild_" + procedure);                     //本阶段隐藏新手UI  显示停车场UI
				playerZoo = GlobalDataManager.GetInstance().playerData.playerZoo;
                if (playerZoo.parkingCenterData.parkingEnterCarSpawnLevel >= 5)
                {
                    OnClickDialogBoxButton();
                    DestroyEffectChild();
                }
                else
                {
                    //this.SetSceneAnimateGameObject(procedure);//添加场景UI特效
                    DelayedHide();                      //隐藏新手引导界面
                    PageMgr.ShowPage<UIParkPage>();     //显示停车场UI
                }
                break;
            case 8:
                procedure = 9;
				GameEventManager.SendEvent("guild_" + procedure);                    //本阶段显示文本  并取消游戏暂停  开启相机跟随
				this.InitCommint(procedure);                 //文本刷新
                GameManager.GetInstance().Pause(false);
                GameObject go = new GameObject();
                go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(SetCameraToFollow));             //延时设置相机跟随

                break;
            case 9:                                 //本阶段隐藏新手引 导  监听是否到达售票口
                procedure = 10;
				GameEventManager.SendEvent("guild_" + procedure);
                DelayedHide();                      //隐藏新手引导界面
                break;
            case 10:
                procedure = 11;
				GameEventManager.SendEvent("guild_" + procedure);                    //本阶段显示新手引导  游戏暂停
                this.InitCommint(procedure);                 //文本刷新
                GameManager.GetInstance().Pause(true);

                break;
            case 11:
                procedure = 12;                      //本阶段隐藏新手引导  添加场景UI特效
				GameEventManager.SendEvent("guild_" + procedure);
                playerZoo = GlobalDataManager.GetInstance().playerData.playerZoo;
                if (playerZoo.entryTicketsLevel > 4)
                {
                    OnClickDialogBoxButton();
                    DestroyEffectChild();
                }
                else
                {
                    DelayedHide();                      //隐藏新手引导界面
                    this.SetSceneAnimateGameObject(procedure);//添加场景UI特效
                }
                break;
            case 12:
                procedure = 14;                 //本阶段显示新手引导  游戏运行
                this.InitCommint(procedure);                 //文本刷新
				GameEventManager.SendEvent("guild_" + procedure);
				GameManager.GetInstance().Pause(false); //取消游戏暂停
                SetCameraToFollow();

                break;
            case 14:
                procedure = 15;
				GameEventManager.SendEvent("guild_" + procedure);                 //本阶段隐藏新手引导  游戏运行 等待游戏购票完成
				DelayedHide();                      //隐藏新手引导界面
                break;
            case 15:
                procedure = 16;
				GameEventManager.SendEvent("guild_" + procedure);                    //本阶段显示新手引导  游戏运行
				this.InitCommint(procedure);                 //文本刷新
                break;
            case 16:
                procedure = 17;
				GameEventManager.SendEvent("guild_" + procedure);                  //本阶段隐藏新手引导  游戏运行  监听到达观光
				DelayedHide();                      //隐藏新手引导界面
                break;
            case 17:
                procedure = 18;
				GameEventManager.SendEvent("guild_" + procedure);                  //本阶段显示新手引导  游戏暂停  
				this.InitCommint(procedure);                 //文本刷新
                GameManager.GetInstance().Pause(true); //游戏暂停
                break;
            case 18:
                procedure = 19;
				GameEventManager.SendEvent("guild_" + procedure);                  //本阶段显示新手引导    
				this.InitCommint(procedure);                 //文本刷新
                break;
            case 19:
                procedure = 20;
				GameEventManager.SendEvent("guild_" + procedure);                  //本阶段隐藏新手引导  动物栏场景动画  
				DelayedHide();                      //隐藏新手引导界面

                if (GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(1001).littleZooTicketsLevel >= 5)
                {
                    OnClickDialogBoxButton();
                    DestroyEffectChild();
                }
                else
                {
                    DelayedHide();                      //隐藏新手引导界面
                    this.SetSceneAnimateGameObject(procedure);//添加场景UI特效
                }

                break;
            case 20:
                procedure = 23;
				GameEventManager.SendEvent("guild_" + procedure);//本阶段显示新手引导  游戏继续  
				this.InitCommint(procedure);                 //文本刷新
                GameManager.GetInstance().Pause(false);
                SetCameraToFollow();                //设置相机跟随
                break;
            case 23:
                procedure = 24;
				GameEventManager.SendEvent("guild_" + procedure);                  //本阶段显示新手引导  
                this.InitCommint(procedure);                 //文本刷新
                break;
            case 24:
                procedure = 25;
				GameEventManager.SendEvent("guild_" + procedure);                //本阶段显示新手引导   
				this.InitCommint(procedure);                 //文本刷新				
                break;
            case 25:
                procedure = 26;
				GameEventManager.SendEvent("guild_" + procedure);                    //本阶段隐藏新手引导  游戏继续  
                SetNewGuideOver();
                break;
            default:
                break;
        }
    }
	
    /// <summary>
    /// 设置相机跟随
    /// </summary>
    public void SetCameraToFollow()
    {
        if (entity != null)
        {
            TraceCamera.GetInstance().BeginTrace(entity.GetTrans());
        }
        else
        {   //若为空  等待下一秒
            GameObject go = new GameObject();
            go.transform.DOMoveZ(0.1f, 1f).OnComplete(new TweenCallback(SetCameraToFollow));
        }
    }

    /// <summary>
    /// 设置点击场景Ui后的相机偏移
    /// </summary>
    /// <param name="transform"></param>
    public void SetCameraOnClickScene(Transform transform)
    {
        GameObject gameObject = new GameObject();
        //gameObject.transform.position = transform.position + new Vector3(-40, -40, 50);
        TraceCamera.GetInstance().FinishTrace();
        ZooCamera.GetInstance().PointAtScreenUpCenter(transform.position);

    }

    /// <summary>
    /// 控件显示（无参数）
    /// </summary>
    private void InitCommint(int atProcedure)
    {
        dialogText.text = this.AcquireData(atProcedure);
        dialogText.GetComponent<TyperTest>().TyperEffect();
        if (atProcedure ==1)
        {
            dialogText.text = this.AcquireData(atProcedure);
        }

    }
    /// <summary>
    /// 控件显示（有参数）
    /// </summary>
    /// <param name="str">插入的值</param>
    private void InitCommint(int atProcedure, string str)
    {
        //string str1 = this.AcquireData(atProcedure);
        //string str2 = string.Format(str1, str);
        //dialogText.text = str2;
        //dialogText.GetComponent<TyperTest>().TyperEffect();
    }


    /// <summary>
    /// 通过步骤的ID去获取对应的文本并返回
    /// </summary>
    /// <param name="number">对应的ID</param>
    /// <returns></returns>
    private string AcquireData(int number)
    {
        //LogWarp.LogError("测试：  当前需要读的文本步骤为 " + number);
        string string1 = Config.guideConfig.getInstace().getCell(number).guidetext;
        ///再从translate语言表里读取里读取
        string string2 = GlobalDataManager.GetInstance().i18n.GetL10N(string1);
        return string2;
    }
    /// <summary>
    /// 延时隐藏本UI，防止穿透
    /// </summary>
    private void DelayedHide()
    {
        dialogBoxButton.enabled = false;
        uibg.enabled = false;
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(Hide));
    }
    /// <summary>
    /// 给场景中添加特效
    /// </summary>
    public void SetSceneAnimateGameObject(int number)
    {
        DestroyEffectChild();
        switch (number)
        {
            case 5:
                effectNode = GameObject.Find(Config.globalConfig.getInstace().GuideParking).transform;
                break;
            case 12:
                effectNode = GameObject.Find(Config.globalConfig.getInstace().GuideTicket).transform;
                break;
            case 20:
                effectNode = GameObject.Find(Config.globalConfig.getInstace().GuideBuild).transform;
                break;
            default:
                break;
        }
        Transform trans = null;
        if (effectNode.childCount == 0)
        {
            trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().GuideSceneClickEffect).transform;
            trans.SetParent(effectNode, false);
        }
        SimpleParticle particlePlayer = new SimpleParticle();
        particlePlayer.Init(effectNode.gameObject);
        particlePlayer.Play();
    }

    /// <summary>
    /// 清除节点下的特效
    /// </summary>
    public void DestroyEffectChild()
    {
        /*  清除场景特效  */
        if (effectNode != null)
        {
            for (int i = 0; i < effectNode.childCount; i++)
            {
                GameObject.Destroy(effectNode.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// 设置新手引导结束
    /// </summary>
    private void SetNewGuideOver()
    {
        TraceCamera.GetInstance().FinishTrace();
        GlobalDataManager.GetInstance().playerData.playerZoo.isGuide = false;
        //关闭页面
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 1f).OnComplete(new TweenCallback(Hide));

        Image image = transform.Find("Image").GetComponent<Image>();
        image.DOFade(80, 200);//透明度改变
        MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButShowPart, "UIMainPage");
    }
}
