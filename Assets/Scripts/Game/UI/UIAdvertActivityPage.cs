using DG.Tweening;
using Game;
using Game.GlobalData;
using Game.MessageCenter;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using UnityEngine.UI;

public class UIAdvertActivityPage : UIPage
{

    Text titleText;
    Text tipsText_1;
    Text tipsText_2;
    Text goldText;
    Slider slider;
    Text timeSlider;
    Text rewardNumText;
    Text tipsText;
    Text buttonText;
    Button advertButton;
    Button closeButton;
    Text timeText;
    BigInteger rewardNumCoin;
    public UIAdvertActivityPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None, UITickedMode.Update)
    {
        uiPath = "uiprefab/UIAdvertActivity";
    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        //初始化控件
        this.RegistAllCompent();
        GetTransPrefabAllTextShow(this.transform);
    }

    private void RegistAllCompent()
    {
        titleText = RegistCompent<Text>("UIAdvertUpper/UiBg/TextGroup/TitleText");
        tipsText_1 = RegistCompent<Text>("UIAdvertUpper/UiBg/TextGroup/TipsText_1");
        tipsText_2 = RegistCompent<Text>("UIAdvertUpper/UiBg/TextGroup/TipsText_2");
        goldText = RegistCompent<Text>("UIAdvertUpper/UiBg/TextGroup/GoldText");
        slider = RegistCompent<Slider>("UIAdvertUpper/UiBg/Schedule/Slider");
        timeSlider = RegistCompent<Text>("UIAdvertUpper/UiBg/Schedule/timeSlider");
        rewardNumText = RegistCompent<Text>("UIAdvertLower/UiBg/TextGroup/RewardNumText");
        tipsText = RegistCompent<Text>("UIAdvertLower/UiBg/TextGroup/TipsText");
        buttonText = RegistCompent<Text>("UIAdvertLower/UiBg/TextGroup/AdvertButton/ButtonBg/ButtonText");
        timeText = RegistCompent<Text>("UIAdvertUpper/UiBg/Schedule/TimeText");

        advertButton = AddCompentInChildren<Button>(advertButton, "UIAdvertLower/UiBg/TextGroup/AdvertButton");
        advertButton = RegistBtnAndClick("UIAdvertLower/UiBg/TextGroup/AdvertButton/ButtonBg", OnClickAdvertPalyButton);
        closeButton = AddCompentInChildren<Button>(closeButton, "UIAdvertUpper/UiBg/CloseButton");
        closeButton = RegistBtnAndClick("UIAdvertUpper/UiBg/CloseButton", OnClickCloseButton);

    }

    private void OnClickCloseButton(string obj)
    {
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.2f).OnComplete(new TweenCallback(Hide));
    }
    private void OnGetIsLockAdsSucceedBool(bool isBool)
    {
#if TEST_NO_AD_SHOW
        isBool = true;
#endif
        if (isBool)
        {
            PlayerNumberOfVideosWatched playerNumberOfVideosWatched = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched;

            if (playerNumberOfVideosWatched.playerLockGainDoubleAdsVideoCount < 6)
            {
                GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockGainDoubleAdsVideoCount += 1;
                if (GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockGainDoubleAdsVideoCount == 6)
                {
                    SetValueOfPlayerData.Send((int)GameMessageDefine.SetCoinOfPlayerData,0, rewardNumCoin, 0);
                    GameObject go = new GameObject();
                    go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
                    {
                        UIMainPage uIMainPage = PageMgr.GetPage<UIMainPage>();
                        uIMainPage.OnMoneyEffect();//飘钱特效
                    }));
                    OnClickCloseButton("0");
                }
                BroadcastNum.Send((int)GameMessageDefine.AddBuff, 14, 0, 0);
				GameEventManager.SendEvent("video_finish_doublebonus");
			}
            InitCompent();
        }
        else
        {
            InitCompent();
        }
    }

    private void OnClickAdvertPalyButton(string obj)
    {
        int count = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockOfflineAdsVideoCount;
        int AdvertBUFFTimes = Config.globalConfig.getInstace().AdvertBUFFTimes;
        if ((AdvertBUFFTimes - count) > 0)
        {
			GameEventManager.SendEvent("video_start_doublebonus");
            //ADManager.ShowRewardedAD(OnGetIsLockAdsSucceedBool);

            if (Advertisements.Instance.IsRewardVideoAvailable())
            {
                Implementation.Instance.ShowRewardedVideo();
                OnGetIsLockAdsSucceedBool(true);
            }
        }
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
        MessageManager.GetInstance().Regist((int)GameMessageDefine.AddBuffSucceed, this.OnAddBuffSucceed);

        InitCompent();
    }

    private void OnAddBuffSucceed(Message obj)
    {
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(InitCompent));
    }

    private void InitCompent()
    {
        rewardNumCoin = PlayerDataModule.LeaveEarnings() * Config.globalConfig.getInstace().AdvertGoldReward;
        var buffCell = Config.buffConfig.getInstace().getCell(14);
        int count = GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockGainDoubleAdsVideoCount;
        int AdvertBUFFTimes = Config.globalConfig.getInstace().AdvertBUFFTimes;

        tipsText_1.text = string.Format(GetL10NString("Ui_Text_23"), buffCell.multiple, UFrame.Math_F.IntToFloat_SToHr(buffCell.time));
        tipsText_2.text = string.Format(GetL10NString("Ui_Text_24"), 2);
        goldText.text = string.Format(GetL10NString("Ui_Text_21"), buffCell.multiple);
        rewardNumText.text = MinerBigInt.ToDisplay(rewardNumCoin);
        tipsText.text = string.Format(GetL10NString("Ui_Text_26"), AdvertBUFFTimes - count, count, AdvertBUFFTimes);
        slider.value = count / (float)AdvertBUFFTimes;
    }

    public override void Tick(int deltaTimeMS)
    {
        if (!this.isActive())
        {
            return;
        }
        SetCountDownShow(deltaTimeMS);

    }
    /// <summary>
    /// 设置倒计时功能
    /// </summary>
    protected void SetCountDownShow(int deltaTimeMS)
    {
        var buffList = GlobalDataManager.GetInstance().playerData.playerZoo.buffList;
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].buffID == 14)
            {
                double time = buffList[i].CD.cd;
                int time01 = (int)time / 60;
                int time02 = (int)time % 60;
                int time03 = (int)time01 / 60;
                int time04 = (int)time01 % 60;
                //LogWarp.LogErrorFormat("测试：    istime={0},timeO1={1},tima02={2}", time, time01, time02);
                timeText.text = string.Format("{0}:{1}:{2}", time03, time04, time02);
            }
        }

    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddBuffSucceed, this.OnAddBuffSucceed);

    }
}
