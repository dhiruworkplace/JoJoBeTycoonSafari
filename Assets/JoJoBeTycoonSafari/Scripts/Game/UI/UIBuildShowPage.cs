using DG.Tweening;
using ZooGame;
using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildShowPage : UIPage
{
    public Transform zooBuildShow;
    private GameObject BuildUpEffect;
    private Transform rotationCamera;
    private Transform mainCamera;
    //Text titleText;
    //Text titleText1;
    //Text explainText1;
    //Text explainText;
    Button buttonHide;   //背景按钮点击事件
    Text buttonText;
    //Image Icon;
    Transform effectNode;
    public UIBuildShowPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None, UITickedMode.Update)
    {
        uiPath = "UIPrefab/UIBuildShow";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        GetTransPrefabAllTextShow(this.transform);
        GameObject camera = GlobalDataManager.GetInstance().zooGameSceneData.camera;
        //查找选择的相机
        rotationCamera = camera.transform.Find("RotationCamera");
        mainCamera = camera.transform.Find("main_camera");
        buttonHide = AddCompentInChildren<Button>(buttonHide, "UIBg/UIShowGroup/OkButton");
        buttonHide = RegistBtnAndClick("UIBg/UIShowGroup/OkButton", HideUI);
        buttonText = RegistCompent<Text>("UIBg/UIShowGroup/OkButton/ButtonText");
        //GetTransPrefabText(buttonText);

        //titleText = RegistCompent<Text>("UIBg/UIShowGroup/NameText");
        //explainText = RegistCompent<Text>("UIBg/UIShowGroup/Text");
        //titleText1 = RegistCompent<Text>("UIBg/UIShowGroup/NameText2");
        //explainText1 = RegistCompent<Text>("UIBg/UIShowGroup/Text2");
        //Icon = RegistCompent<Image>("UIBg/UIShowGroup/Icon");

        //新手引导手势组件
        effectNode = RegistCompent<Transform>("UIBg/UIShowGroup/OkButton/effectNode");
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            Transform trans = null;
            trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().GuideUiClickEffect).transform;
            trans.SetParent(effectNode, false);
            UFrame.SimpleParticle particlePlayer = new UFrame.SimpleParticle();
            particlePlayer.Init(effectNode.gameObject);
            particlePlayer.Play();
            UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
            if (uIGuidePage == null)
            {
                string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                throw new System.Exception(e);
            }
			GameEventManager.SendEvent("guild_1");
        }
    }
    private void InitCompent()
    {
        rotationCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        BuildUpEffect.SetActive(true);
        BuildUpEffect.transform.SetParent(zooBuildShow, false);
        Config.buildupCell buildUpCell = Config.buildupConfig.getInstace().getCell(m_data.ToString());
        string nameStr = GetL10NString(buildUpCell.buildname);
        string iconPath = buildUpCell.icon;
        string str = GetL10NString("Ui_Text_48");
        //titleText.text = nameStr;
        //explainText.text = str;
        //titleText1.text = nameStr;
        //explainText1.text = str;
        //Icon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
    }
    public override void Refresh()
    {
        base.Refresh();
    }

    public override void Active()
    {
        base.Active();
        zooBuildShow = transform.Find("UIBg/Effect");

        if (BuildUpEffect == null)
        {
            BuildUpEffect = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().BuildUpEffect);
        }
        InitCompent();
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            GameManager.GetInstance().Pause(false); //游戏暂停
        }

    }
    private void HideUI(string str)
    {
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.2f).OnComplete(new TweenCallback(this.Hide));
    }
    public override void Hide()
    {
        DestroyEffectChild();
        rotationCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        BuildUpEffect.SetActive(false);
        //PageMgr.ShowPage<UIMainPage>(); 
        MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButShowPart, "UIMainPage");
       
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide != true)
        {
            PageMgr.ShowPage<UIZooPage>(m_data);
            UIInteractive.GetInstance().iPage = new UIZooPage();

            base.Hide();
        }
        else
        {
            base.Hide();
            PageMgr.ShowPage<UIGuidePage>();
            return;
        }  
    }
    int timeTick=5000;
    public override void Tick(int deltaTimeMS)
    {
        if (!this.isActive())
        {
            return;
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
        {
            timeTick -= deltaTimeMS;
            if (timeTick<100)
            {
                Hide();
            }
        }

    }


    /// <summary>
    /// 清除节点下的特效
    /// </summary>
    private void DestroyEffectChild()
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
}
