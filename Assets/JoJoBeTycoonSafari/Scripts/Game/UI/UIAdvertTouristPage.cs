using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Config;
using ZooGame;
using ZooGame.MessageCenter;
using UFrame;
using ZooGame.GlobalData;
using System;
using UFrame.MessageCenter;
using System.Numerics;
using Logger;
using UFrame.MiniGame;
using UFrame.BehaviourFloat;
using ZooGame.Path.StraightLine;
using DG.Tweening;

public class UIAdvertTouristPage : UIPage
{
    Text titleText;
    Text wordText;
    Text rewardText;
    Text palyButtonText;
    Button closeButton;
    Button advertPalyButton;
    Image touristIcon;
    string condition;
    int numberVisitor;
    PlayerNumberOfVideosWatched playerNumberOfVideosWatched;

    private BigInteger freeItemRwdCoinQuantity = 0;

    public UIAdvertTouristPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UIAdvertTourist";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        this.RegistAllCompent();
        GetTransPrefabAllTextShow(this.transform);
        playerNumberOfVideosWatched = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched;
        //初始化控件
    }

    private void RegistAllCompent()
    {
        touristIcon = RegistCompent<Image>("UiBgMask/UpButtonGroup/AdvertTipsBg/TouristIcon");
        titleText = RegistCompent<Text>("UiBgMask/UpButtonGroup/AdvertTipsBg/TitleText");
        wordText = RegistCompent<Text>("UiBgMask/UpButtonGroup/AdvertTipsBg/WordText");
        rewardText = RegistCompent<Text>("UiBgMask/UpButtonGroup/AdvertTipsBg/RewardText");
        palyButtonText = RegistCompent<Text>("UiBgMask/UpButtonGroup/AdvertPalyButton/PalyButton_1/PalyButtonText");
        palyButtonText.text = GetL10NString("Ui_Text_27");
        closeButton = AddCompentInChildren<Button>(closeButton, "UiBgMask/UpButtonGroup/AdvertTipsBg/CloseButton");
        closeButton = RegistBtnAndClick("UiBgMask/UpButtonGroup/AdvertTipsBg/CloseButton", OnClickCloseButton);
        advertPalyButton = AddCompentInChildren<Button>(advertPalyButton, "UiBgMask/UpButtonGroup/AdvertPalyButton/PalyButton_1");
        advertPalyButton = RegistBtnAndClick("UiBgMask/UpButtonGroup/AdvertPalyButton/PalyButton_1", OnClickAdvertPalyButton);

        LogWarp.LogError(" 测试   RegistAllCompent");

    }
    void InitCompent()
    {
        LogWarp.LogError(" 测试   InitCompent");
        condition = m_data.ToString();
        playerNumberOfVideosWatched = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched;
        string iconPath = null;
        switch (condition)
        {
            case "TouristButton":
                titleText.text = GetL10NString("Ui_Text_28");
                wordText.text = GetL10NString("Ui_Text_29");
                rewardText.text = string.Format(GetL10NString("Ui_Text_30"), numberVisitor);
                iconPath = Config.globalConfig.getInstace().AdvertAddTourist;
                touristIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
                break;
            case "VisitButton":
                titleText.text = GetL10NString("Ui_Text_31");
                wordText.text = GetL10NString("Ui_Text_32");
                var cell = Config.buffConfig.getInstace().getCell(10);
                rewardText.text = string.Format(GetL10NString("Ui_Text_33"), cell.time);
                iconPath = Config.globalConfig.getInstace().AdvertAddVisit;
                touristIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
                break;
            case "TicketButton":
                titleText.text = GetL10NString("Ui_Text_34");
                wordText.text = GetL10NString("Ui_Text_35");
                var cell1 = Config.buffConfig.getInstace().getCell(12);
                rewardText.text = string.Format(GetL10NString("Ui_Text_36"), cell1.time);
                iconPath = Config.globalConfig.getInstace().AdvertAddTicket;
                touristIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
                break;
            case "FreeItemButton":
                freeItemRwdCoinQuantity = PlayerDataModule.GetFreeItemRwdCoinQuantity();
                titleText.text = GetL10NString("Ui_Text_111");
                wordText.text = GetL10NString("Ui_Text_112");
                rewardText.text = MinerBigInt.ToDisplay(freeItemRwdCoinQuantity);
                iconPath = "UIAtlas/UIAdvertActivity/GoldHeap";
                touristIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 隐藏页面
    /// </summary>
    /// <param name="obj"></param>
    private void OnClickCloseButton(string obj)
    {
        LogWarp.LogError("点击了广告关闭页面");
        //PageMgr.ClosePage<UIAdvertTouristPage>();
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.2f).OnComplete(new TweenCallback(Hide));

    }
    private void OnGetIsLockAdsSucceedBool(bool isBool)
    {
#if TEST_NO_AD_SHOW
        isBool = true;
#endif
        Config.monitorCell cell;

        if (isBool == true)
        {
            switch (condition)
            {
                case "TouristButton":
                    LogWarp.LogError("测试    进入观看广告时间  touristButton ");
                    EntityShip.GetoffVisitor(numberVisitor);   //轮船游客到来
                    GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockVisitorNumberAdsVideoCount += 1;
					GameEventManager.SendEvent("video_finish_getvisitor");

                    break;
                case "VisitButton":
                    BroadcastNum.Send((int)GameMessageDefine.AddBuff, 10, 0, 0);    //动物栏观光时间
                    GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockVisitorExpediteAdsVideoCount += 1;
					GameEventManager.SendEvent("video_finish_animal");

                    break;
                case "TicketButton":
                    BroadcastNum.Send((int)GameMessageDefine.AddBuff, 12, 0, 0);    //售票口时间
                    GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockEntryExpediteAdsVideoCount += 1;
					GameEventManager.SendEvent("video_finish_decticket");

                    break;
                case "FreeItemButton":
                    SetValueOfPlayerData.Send((int)GameMessageDefine.SetCoinOfPlayerData, 0, freeItemRwdCoinQuantity, 0); //贵宾定时广告
                    GameManager.GetInstance().StartCoroutine(FinishMoneyEffect());
                    GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerFreeItemAdsVideoCount += 1;
					GameEventManager.SendEvent("video_finish_viptiming");

                    break;
                default:
                    break;
            }
            playerNumberOfVideosWatched = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched;
            //利用dotweeen做延时操作 防止穿透
            GameObject go = new GameObject();
            go.transform.DOMoveZ(0.1f, 0.3f).OnComplete(new TweenCallback(this.Hide));
        }
        else
        {
            LogWarp.LogError("测试： 视频播放  失败");
        }
    }

    private IEnumerator FinishMoneyEffect()
    {
        yield return new WaitForSeconds(0.1f);
        UIMainPage uIMainPage = PageMgr.GetPage<UIMainPage>();
        uIMainPage.OnMoneyEffect();//飘钱特效
    }

    /// <summary>
    /// 点击播放视频按钮
    /// </summary>
    /// <param name="obj"></param>
    private void OnClickAdvertPalyButton(string obj)
    {
        Config.monitorCell cell;
        switch (condition)
        {
            case "TouristButton":
				GameEventManager.SendEvent("video_start_getvisitor");
                break;
            case "TicketButton":
				GameEventManager.SendEvent("video_start_decticket");
                break;
            case "VisitButton":
				GameEventManager.SendEvent("video_start_animal");
                break;
            case "FreeItemButton":
				GameEventManager.SendEvent("video_start_viptiming");
                break;
            default:
                break;
        }

        //ADManager.ShowRewardedAD(OnGetIsLockAdsSucceedBool);
        //if (Advertisements.Instance.IsRewardVideoAvailable())
        //{
        //    Implementation.Instance.ShowRewardedVideo();
        //    OnGetIsLockAdsSucceedBool(true);
        //}
    }


    /// <summary>
    /// 更新
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();
    }
    /// <summary>
    /// 活跃
    /// </summary>
    public override void Active()
    {
        base.Active();
        numberVisitor = PlayerDataModule.SteameVisitorNameber();
        playerNumberOfVideosWatched = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched;
        InitCompent();
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        base.Hide();
    }


}
