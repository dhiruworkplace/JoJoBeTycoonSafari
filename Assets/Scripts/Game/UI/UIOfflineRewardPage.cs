using DG.Tweening;
using Game;
using Game.GlobalData;
using Game.MessageCenter;
using Logger;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UFrame;
using UFrame.MessageCenter;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;

public class UIOfflineRewardPage : UIPage
{
    public UIOfflineRewardPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UIOfflineReward";
    }
    Text rewardText;
    Button rmbButton;
    Button advertButton;
    Button hideUIButton;
    bool lockAdsBool= false;
    //视频广告翻倍倍数
    int waitingADProfit;
    
    /// <summary>
    /// 离线时间的所有收益（不翻倍）
    /// </summary>
    BigInteger fflineReward;
    BigInteger coinVal;
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        this.RegistAllCompent();
        //this.ShowUICallBack();
        GetTransPrefabAllTextShow(this.transform);
    }


    /// <summary>
    /// 内部组件的查找赋值
    /// </summary>
    private void RegistAllCompent()
    {
        waitingADProfit = Config.globalConfig.getInstace().WaitingADProfit;

        rewardText =   RegistCompent<Text>("UI/TextGroup/RewardText");

        rmbButton    = AddCompentInChildren(rmbButton, "UI/ButtonGroup/RmbButton");
        rmbButton    = RegistBtnAndClick("UI/ButtonGroup/RmbButton", OnClickDiamond);
        advertButton = AddCompentInChildren(advertButton, "UI/ButtonGroup/AdvertButton");
        advertButton = RegistBtnAndClick("UI/ButtonGroup/AdvertButton", OnClickAds);
        Text advertButton_Text = RegistCompent<Text>("UI/ButtonGroup/AdvertButton/ButtonText");
        hideUIButton = AddCompentInChildren(hideUIButton,"UI/TextGroup/CloseButton");
        hideUIButton = RegistBtnAndClick("UI/TextGroup/CloseButton", OnClickHideUI);

        fflineReward = PlayerDataModule.OnGetCalculateOfflineSecondCoin();
        Logger.LogWarp.LogError("测试：   "+ fflineReward+"    "+ waitingADProfit);
        rewardText.text = MinerBigInt.ToDisplay(fflineReward);
        advertButton_Text.text = string.Format(GetL10NString("Ui_Text_21"), waitingADProfit);

        coinVal = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin);//获取玩家现有金币
    }
    /// <summary>
    /// 钻石加倍
    /// </summary>
    /// <param name="name"></param>
    private void OnClickDiamond(string name)
    {
        BigInteger coins = fflineReward;
        LogWarp.Log(" 钻石加3倍");
        /*  发送消息扣钻石  */
    }
    /// <summary>
    /// 广告加倍
    /// </summary>
    /// <param name="name"></param>
    private void OnClickAds(string name)
    {
        GameEventManager.SendEvent("video_start_offline");

        //ADManager.ShowRewardedAD(OnGetIsLockAdsSucceedBool);

        if (Advertisements.Instance.IsRewardVideoAvailable())
        {
            Implementation.Instance.ShowRewardedVideo();
            OnGetIsLockAdsSucceedBool(true);
        }
        
    }



    private void OnGetIsLockAdsSucceedBool(bool isBool)
    {
#if TEST_NO_AD_SHOW
        isBool = true;
#endif
        if (isBool)
        {
            BigInteger coins = fflineReward;
            lockAdsBool = true;
            ActualEarnings(coins * waitingADProfit);
            GlobalDataManager.GetInstance().playerData.playerZoo.playerNumberOfVideosWatched.playerLockOfflineAdsVideoCount += 1;
            GameEventManager.SendEvent("video_finish_offline");
        }
    }

    private void OnClickHideUI(string name )
    {
        BigInteger coins = fflineReward ;
        ActualEarnings(coins);
    }
    /// <summary>
    /// 发送消息离线的收益
    /// </summary>
    /// <param name="bigInteger"></param>
    private void ActualEarnings(BigInteger bigInteger)
    {
        SetValueOfPlayerData.Send((int)GameMessageDefine.SetCoinOfPlayerData,
                0, bigInteger, 0);
        this.HideUICallBack();
    }

    private void HideUICallBack()
    {

        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.2f).OnComplete(new TweenCallback(Hide));
    }
    /// <summary>
    /// 更新:动态修改图片大小
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
        if (lockAdsBool)
        {
            GameObject go = new GameObject();
            go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
            {
                UIMainPage uIMainPage = PageMgr.GetPage<UIMainPage>();
                uIMainPage.OnMoneyEffect();//飘钱特效
            }));
        }
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate {
            UIMainPage uIMainPage = PageMgr.GetPage<UIMainPage>();
            uIMainPage.OnMoneyEffect();//飘钱特效
        }));
        
    }
}
