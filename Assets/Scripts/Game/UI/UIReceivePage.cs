using DG.Tweening;
using Game;
using Game.GlobalData;
using Game.MessageCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;

public class UIReceivePage : UIPage
{
    RawImage showImage;
    Button closeButton;

    PlayerData playerData;                       //获取Data，方便获取动物园
    private Text earningsText;
    private Text goldText;
    private Text diamondText;

    ParticleSystem particleSystem;

    private Transform animalShowCamera;
    private Transform mainCamera;
    private RawImage rawImage;

    public UIReceivePage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UIReceive";
    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        GetTransPrefabAllTextShow(this.transform);

        showImage = RegistCompent<RawImage>("RawImage");
        closeButton = RegistBtnAndClick("ReceiveBg/CloseButton", CloseButtonHideUI);

        //查找选择的相机
        GameObject camera = GlobalDataManager.GetInstance().zooGameSceneData.camera;
        animalShowCamera = camera.transform.Find("AnimalShowCamera");
        mainCamera = camera.transform.Find("main_camera");

        earningsText = RegistCompent<Text>("ReceiveBg/MoneyGroup/Money_1/Text");
        goldText = RegistCompent<Text>("ReceiveBg/MoneyGroup/Money_2/Text");
        diamondText = RegistCompent<Text>("ReceiveBg/MoneyGroup/Money_3/Text");
        particleSystem = RegistCompent<ParticleSystem>("ReceiveBg/Fx_Glow");

    }

    protected void OnBroadcastCoinOfPlayerData(Message obj)
    {
        goldText.text = MinerBigInt.ToDisplay(playerData.playerZoo.coin.ToString());
        earningsText.text = MinerBigInt.ToDisplay(PlayerDataModule.LeaveEarnings()) + GetL10NString("Ui_Text_67");
        Logger.LogWarp.LogError("测试：AAAAAA     " + playerData.playerZoo.coin.ToString());
    }

    /// <summary>
    /// 更新
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();
    }
    /// <summary>
    /// 激活
    /// </summary>
    public override void Active()
    {
        base.Active();
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnBroadcastCoinOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastDiamondOfPlayerData, this.OnBroadcastDiamondOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastStarOfPlayerData, this.OnBroadcastStarOfPlayerData);

        InitComment();
    }

    private void OnBroadcastStarOfPlayerData(Message obj)
    {
        playerData = GlobalDataManager.GetInstance().playerData;
        diamondText.text = playerData.playerZoo.star.ToString();
    }

    protected void OnBroadcastDiamondOfPlayerData(Message msg)
    {
        //playerData = GlobalDataManager.GetInstance().playerData;
        //diamondText.text = playerData.playerZoo.diamond.ToString();

    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnBroadcastCoinOfPlayerData);
        MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButShowPart, "UIMainPage");

        base.Hide();
    }
    /// <summary>
    /// 初始化控件属性
    /// </summary>
    private void InitComment()
    {
        //InitData();
        //对相机的显示隐藏
        mainCamera.gameObject.SetActive(true);
        animalShowCamera.gameObject.SetActive(true);

        playerData = GlobalDataManager.GetInstance().playerData;
        goldText.text = MinerBigInt.ToDisplay(playerData.playerZoo.coin.ToString());
        diamondText.text = playerData.playerZoo.star.ToString();

        earningsText.text = MinerBigInt.ToDisplay(PlayerDataModule.LeaveEarnings()) + GetL10NString("Ui_Text_67");

        particleSystem.Play();
        string zooID = m_data.ToString();
        //删除Plane  对象下面的子对象，添加新的动物预制体
        Transform gameObject = animalShowCamera.transform.Find("Plane");
        int childCount = gameObject.childCount;
        for (int i = 0; i < childCount; i++)
        {
            UnityEngine.Object.Destroy(gameObject.GetChild(i).gameObject);
        }
        var cellRes = Config.resourceConfig.getInstace().getCell(zooID);
        var goPart = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
        goPart.transform.SetParent(animalShowCamera.transform.Find("Plane").transform, false);
        var scale = goPart.transform.localScale;
        goPart.transform.localScale = scale * cellRes.zoomratio;

        Vector3 vector = goPart.transform.position;
        goPart.transform.position = new Vector3(vector.x+ cellRes.Xoffset,vector.y+cellRes.Yoffset,vector.z+ cellRes.Zoffset);
        Animation animation = goPart.GetComponentInChildren<Animation>();
        animalShowCamera.GetComponent<ShowAnimelCamera>().animation = animation;
        animalShowCamera.GetComponent<ShowAnimelCamera>().ShowBool = true;
    }

    private void CloseButtonHideUI(string obj)
    {
        mainCamera.gameObject.SetActive(true);
        animalShowCamera.GetComponent<ShowAnimelCamera>().ShowBool = false;
        animalShowCamera.gameObject.SetActive(false);
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.3f).OnComplete(new TweenCallback(this.Hide));
    }
}
