using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Config;
using Game;
using Game.MessageCenter;
using UFrame;
using Game.GlobalData;
using System;
using UFrame.MessageCenter;
using System.Numerics;
using Logger;
using UFrame.MiniGame;
using UFrame.BehaviourFloat;
using Game.Path.StraightLine;
using DG.Tweening;
using UFrame.EntityFloat;
using UnityEngine.EventSystems;

/// <summary>
/// 主界面
/// </summary>
public class UIMainPage : UIPage
{
    /// <summary>
    /// 小游戏的跳转按钮
    /// </summary>
    private Button littleGameButton;

    private Text earningsText;
    private Text goldText;
    private Text diamondText;

    PlayerData playerData;                       //获取Data，方便获取动物园
    Text txtVisistorNum;
    Text txtMaxVisistorNum;
    Text txtShuttleVisistorNum;
    Button AdvertButton;
    Button touristButton; //观看广告 增加轮船游客
    int touristButton_Time;
    bool touristButton_Bool;
    Button visitButton;     //观看广告  加快动物栏观光速度
    int visitButton_Time;
    bool visitButton_Bool;
    Button ticketButton; //观看广告 加快出口乘车速度
    int ticketButton_Time;
    bool ticketButton_Bool;

    Button strengthenButton;
    Text strengthenButton_Text;
    //int strengthenButton_Time;
    //bool strengthenButton_Bool=true;
    //int InIt

    BigInteger incomeCoinMS;

    Transform celerityVisit;
    Transform celerityTicket;

    /// <summary>
    /// 游戏开始计时，达到时间显示对应的按钮
    /// </summary>
    int isGameBiginTime;
    bool isGameBigin_Tourist;
    bool isGameBigin_Visit;
    bool isGameBigin_Ticket;


    public UIMainPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None, UITickedMode.Update)
    {
        uiPath = "uiprefab/UIMain";
    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        GetTransPrefabAllTextShow(this.transform);
        //strengthenButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().MoneyAccelerateTime);

        //littleGameButton = RegistCompent<Button>("SetMainShow/UpButtonGroup/GameButton");
        earningsText = RegistCompent<Text>("MoneyGroup/Money_1/Text");
        goldText = RegistCompent<Text>("MoneyGroup/Money_2/Text");
        diamondText = RegistCompent<Text>("MoneyGroup/Money_3/Text");

        celerityVisit = RegistCompent<Transform>("SetMainShow/LowerButtonGroup/BuffTimeTips_1");
        celerityTicket = RegistCompent<Transform>("SetMainShow/LowerButtonGroup/BuffTimeTips_2");
        celerityVisit.gameObject.SetActive(false);
        celerityTicket.gameObject.SetActive(false);

        AdvertButton = AddCompentInChildren<Button>(AdvertButton, "SetMainShow/LowerButtonGroup/AdvertButton");
        touristButton = AddCompentInChildren<Button>(touristButton, "SetMainShow/TipsButtonGroup/TouristButton");
        visitButton = AddCompentInChildren<Button>(visitButton, "SetMainShow/TipsButtonGroup/VisitButton");
        ticketButton = AddCompentInChildren<Button>(ticketButton, "SetMainShow/TipsButtonGroup/TicketButton");

        AdvertButton = RegistBtnAndClick("SetMainShow/LowerButtonGroup/AdvertButton", OnClickAdvertButton);
        touristButton = RegistBtnAndClick("SetMainShow/TipsButtonGroup/TouristButton", OnClickAdsButton_TouristButton);
        visitButton = RegistBtnAndClick("SetMainShow/TipsButtonGroup/VisitButton", OnClickAdsButton_VisitButton);
        ticketButton = RegistBtnAndClick("SetMainShow/TipsButtonGroup/TicketButton", OnClickAdsButton_TicketButton);
        strengthenButton = RegistBtnAndClick("SetMainShow/LowerButtonGroup/StrengthenButton", OnClickAdsButton_StrengthenButton);
        strengthenButton_Text = RegistCompent<Text>("SetMainShow/LowerButtonGroup/StrengthenButton/TipsNum");
        strengthenButton_Text.text = Config.globalConfig.getInstace().AnimalupgradingNeed.ToString();
        touristButton.gameObject.SetActive(false);
        visitButton.gameObject.SetActive(false);
        ticketButton.gameObject.SetActive(false);

        txtVisistorNum = RegistCompent<Text>("SetMainShow/Debug/Txt_VisitorNum");
        txtMaxVisistorNum = RegistCompent<Text>("SetMainShow/Debug/Txt_MaxVisitorNum");
        txtShuttleVisistorNum = RegistCompent<Text>("SetMainShow/Debug/Txt_ShuttleVisistorNum");
#if !DEBUG_VISIT
        txtVisistorNum.gameObject.SetActive(false);
        txtMaxVisistorNum.gameObject.SetActive(false);
        txtShuttleVisistorNum.gameObject.SetActive(false);
#endif

        //littleGameButton.onClick.AddListener(OnClickLittleGameButton);
        //#if DEBUG_VISIT
        RegistBtnAndClick("SetMainShow/Debug/Btn_Extend", OnClickTestButton);
        RegistBtnAndClick("SetMainShow/Debug/Btn_Extend01", OnClickTestButton01);

        touristButton_Bool = false;
        visitButton_Bool = false;
        visitButton_Bool = false;

        //Btn_Extend.gameObject.AddComponent<RepeatButton>();
        //Btn_Extend.GetComponent<RepeatButton>().onPress.AddListener(OnTestLongPressButton);//按下。频繁的调用
        //Btn_Extend.GetComponent<RepeatButton>().onRelease.AddListener(OnTestReleaseButton);//抬起，调用一次

        //#endif
#if NO_BIGINT
#else
        Init();
#endif

        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnBroadcastCoinOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.AddBuffSucceed, this.OnAddBuffSucceed);

        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastDiamondOfPlayerData, this.OnBroadcastDiamondOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastStarOfPlayerData, this.OnBroadcastStarOfPlayerData);

        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastVisitorNum, OnBroadcastVisitorNum);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastMaxVisitorNum, OnBroadcastMaxVisitorNum);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastShuttleVisistorNum, OnBroadcastShuttleVisistorNum);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_ActiveButHidePart, OnUIMessage_ActiveButHidePart);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_ActiveButShowPart, OnUIMessage_ActiveButShowPart);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_OnClickButHidePart, OnUIMessage_OnClickButHidePart);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_OnClickButShowPart, OnUIMessage_OnClickButShowPart);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);

    }

    private void OnAddBuffSucceed(Message obj)
    {

        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(Init));
    }

    private void OnBroadcastStarOfPlayerData(Message obj)
    {
        if (GlobalDataManager.GetInstance().playerData.playerZoo.star>=Config.globalConfig.getInstace().AnimalupgradingNeed)
        {
            GlobalDataManager.GetInstance().playerData.playerZoo.isShowAnimalCultivate = true;
            SetHideStrengthenCountdown();
        }
        Init();
    }



    /// <summary>
    /// 接受消息测试是否开启离线UI
    /// </summary>
    /// <param name="obj"></param>
    private void OnOpenOfflineUIPage(Message msg)
    {
        PageMgr.ShowPage<UIOfflineRewardPage>();  //离线UI
    }



    /// <summary>
    /// 观看广告   加倍收益   UIAdvertActivity
    /// </summary>
    /// <param name="obj"></param>
    private void OnClickAdvertButton(string obj)
    {
        LogWarp.Log("UIMain   测试    点击了 OnClickAdvertButton    " + obj);
        PageMgr.ShowPage<UIAdvertActivityPage>(obj);  //加倍收益

    }

    private void OnClickAdsButton_TouristButton(string obj)
    {
        GameObject go = new GameObject();

        /*
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
        {
            PageMgr.ShowPage<UIAdvertTouristPage>(obj);  //增加轮船游客
            touristButton.gameObject.SetActive(false);
            touristButton_Bool = true;
            touristButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().IncreaseTouristCD);
        }));
        */

        //Advertisements.Instance.ShowRewardedVideo(CompleteMethod);

    }


    private void CompleteMethod(bool completed, string advertiser)
    {
        touristButton.gameObject.SetActive(false);
        touristButton_Bool = true;
        touristButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().IncreaseTouristCD);

    }


    private void OnClickAdsButton_TicketButton(string obj)
    {
        GameObject go = new GameObject();

        /*
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
        {
            PageMgr.ShowPage<UIAdvertTouristPage>(obj);  //加快出口乘车速度
            ticketButton.gameObject.SetActive(false);
            ticketButton_Bool = true;
            ticketButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().TicketAccelerateCD);
        }));
        */
        //Advertisements.Instance.ShowRewardedVideo(CompleteMethod1);

    }



    private void CompleteMethod1(bool completed, string advertiser)
    {
        ticketButton.gameObject.SetActive(false);
        ticketButton_Bool = true;
        ticketButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().TicketAccelerateCD);




    }




    private void OnClickAdsButton_VisitButton(string obj)
    {


        GameObject go = new GameObject();

        /*
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
        {
            PageMgr.ShowPage<UIAdvertTouristPage>(obj);  //加快动物栏观光速度
            visitButton.gameObject.SetActive(false);
            visitButton_Bool = true;
            visitButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().VisitAccelerateCD);
        }));
        */

        //Advertisements.Instance.ShowRewardedVideo(CompleteMethod2);

    }



    private void CompleteMethod2(bool completed, string advertiser)
    {
        visitButton.gameObject.SetActive(false);
        visitButton_Bool = true;
        visitButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().VisitAccelerateCD);




    }


    private void OnClickAdsButton_StrengthenButton(string obj)
    {   //免费视频
        //Logger.LogWarp.LogErrorFormat("SSSSSSSSSSSSSSSS");
        //GameObject go = new GameObject();
        //go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
        //{
        //    PageMgr.ShowPage<UIGoldRewardPage>();
        //}));

        //动物培养开启功能
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(delegate
        {
            PageMgr.ShowPage<UIAnimalTipsPage>();
        }));
        //Advertisements.Instance.ShowRewardedVideo(CompleteMethod3);

    }




    private void CompleteMethod3(bool completed, string advertiser)
    {
        PageMgr.ShowPage<UIAnimalTipsPage>();




    }
    public void SetHideStrengthenCountdown()
    {
        //strengthenButton_Bool = true;
        strengthenButton.gameObject.SetActive(false);
        //strengthenButton_Time = Math_F.FloatToInt1000(Config.globalConfig.getInstace().MoneyAccelerateTime);
    }
    private void SetAdvertBuff()
    {
        touristButton.GetComponent<Button>().interactable = true;
    }

    void Init()
    {
        playerData = GlobalDataManager.GetInstance().playerData;
        incomeCoinMS = PlayerDataModule.LeaveEarnings();
        goldText.text = MinerBigInt.ToDisplay(playerData.playerZoo.coin.ToString());
        diamondText.text = playerData.playerZoo.star.ToString();
        //diamondText.text = playerData.playerZoo.diamond.ToString();
        earningsText.text = MinerBigInt.ToDisplay(incomeCoinMS) + GetL10NString("Ui_Text_67");

        //LogWarp.LogErrorFormat("测试： 每分钟收益={0}，相乘buff的值={1},",incomeCoinMS,PlayerDataModule.PlayerRatioCoinInComeMul());
    }

    protected void OnBroadcastCoinOfPlayerData(Message obj)
    {
        playerData = GlobalDataManager.GetInstance().playerData;
        goldText.text = MinerBigInt.ToDisplay(playerData.playerZoo.coin.ToString());
        playerData = GlobalDataManager.GetInstance().playerData;
        incomeCoinMS = PlayerDataModule.LeaveEarnings();
        isBool = true;
    }

    protected void OnBroadcastDiamondOfPlayerData(Message msg)
    {
        //playerData = GlobalDataManager.GetInstance().playerData;
        //diamondText.text = playerData.playerZoo.diamond.ToString();

    }

    protected void OnBroadcastVisitorNum(Message msg)
    {
        var _msg = msg as BroadcastNum;
        txtVisistorNum.text = "Curr Car: " + _msg.currNum.ToString();

    }

    protected void OnBroadcastMaxVisitorNum(Message msg)
    {
        var _msg = msg as BroadcastNum;
        txtMaxVisistorNum.text = "Max Car:" + _msg.currNum.ToString();
    }

    protected void OnBroadcastShuttleVisistorNum(Message msg)
    {
        var _msg = msg as BroadcastNum;
        txtShuttleVisistorNum.text = "ShuttleVisistor:" + _msg.currNum.ToString();
    }

    public override void Active()
    {
        base.Active();
        playerData = GlobalDataManager.GetInstance().playerData;
        isGameBiginTime = 0;
        isGameBigin_Ticket = true;
        isGameBigin_Tourist = true;
        isGameBigin_Visit = true;
        bool isGuide = GlobalDataManager.GetInstance().playerData.playerZoo.isGuide;
        GameEventManager.SendEvent("enter_game");

        if (isGuide == true)
        {
#if NOVICEGUIDE
            PageMgr.ShowPage<UIGuidePage>();  //新手引导页面交互
            LogWarp.Log("NOVICEGUIDE  时候打开 新手引导");
            GameEventManager.SendEvent("first_enter_game");
#endif
        }
        else
        {

            if (playerData.playerZoo.LastLogingDate_Day != System.DateTime.Now.Day)
            {
                playerData.playerZoo.playerNumberOfVideosWatched.SetResetVideosWatchedData();
            }
        }
        ActiveButShowPart();
        //if (GlobalDataManager.GetInstance().playerData.playerZoo.isLoadingShowOffline)
        //{
        //    OnOpenOfflineUIPage(null);
        //    GlobalDataManager.GetInstance().playerData.playerZoo.isLoadingShowOffline = false;
        //}
    }

    protected void OnUIMessage_ActiveButHidePart(Message msg)
    {
        var _msg = msg as MessageString;
        if (_msg.str == this.name)
        {
            this.ActiveButHidePart();
        }
    }
    protected void OnUIMessage_ActiveButShowPart(Message msg)
    {
        var _msg = msg as MessageString;
        if (_msg.str == this.name)
        {
            this.ActiveButShowPart();
        }

    }
    /// <summary>
    /// 需要修改，半分钟刷新每分钟收益
    /// </summary>
    int timerInt = 3000;
    public override void Tick(int deltaTimeMS)
    {
        //设置安卓返回键功能
        SetAndroidQuit();
        /*做buff持续时间的倒计时*/
        SetCountDownShow(deltaTimeMS);
        if (isGameBiginTime>-1)
        {
            isGameBiginTime += deltaTimeMS;
            SetAdsButtonBeginShow(isGameBiginTime);
        }
        //SetStrengthenButtonTimeShow(deltaTimeMS);
        SetAdsButtonShow(deltaTimeMS);
#if NO_BIGINT
#else
        if (timerInt<100)
        {
            Init();
            timerInt = 3000;
        }
        else
        {
            timerInt -= deltaTimeMS;
        }
#endif

    }

    private void SetStrengthenButtonTimeShow(int deltaTimeMS)
    {
        //if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        //{
        //    return;
        //}
        //if ( strengthenButton_Bool == true)
        //{
        //    strengthenButton_Time -= deltaTimeMS;
        //    TimeFormatCalculate_StrengthenButton(strengthenButton_Time);
        //}

    }

    private void SetAdsButtonShow(int deltaTimeMS)
    {
        
        if (touristButton_Bool)
        {
            touristButton_Time -= deltaTimeMS;
            if (touristButton_Time<10)
            {
                touristButton.gameObject.SetActive(true);
                touristButton_Bool = true;
            }
        }
        if (visitButton_Bool)
        {
            visitButton_Time -= deltaTimeMS;
            if (visitButton_Time < 10)
            {
                visitButton.gameObject.SetActive(true);
                visitButton_Bool = true;
            }
        }
        if (ticketButton_Bool)
        {
            ticketButton_Time -= deltaTimeMS;
            if (ticketButton_Time < 10)
            {
                ticketButton.gameObject.SetActive(true);
                ticketButton_Bool = true;
            }
        }
    }

    /// <summary>
    /// 设置开始游戏显示广告按钮时间 
    /// </summary>
    /// <param name="isGameBiginTime"></param>
    private void SetAdsButtonBeginShow(int isGameBiginTime)
    {
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            return;
        }
        if (isGameBiginTime > Math_F.FloatToInt1000(Config.globalConfig.getInstace().IncreaseTouristTime)&&isGameBigin_Tourist==true)
        {
            touristButton.gameObject.SetActive(true);
            isGameBigin_Tourist = false;
        }
        else if (isGameBiginTime > Math_F.FloatToInt1000(Config.globalConfig.getInstace().VisitAccelerateTime)&&isGameBigin_Visit==true)
        {
            visitButton.gameObject.SetActive(true);
            isGameBigin_Visit = false;
        }
        else if (isGameBiginTime > Math_F.FloatToInt1000( Config.globalConfig.getInstace().TicketAccelerateTime)&&isGameBigin_Ticket == true)
        {
            ticketButton.gameObject.SetActive(true);
            isGameBigin_Ticket = false;
            isGameBiginTime = -1;
        }
       

    }

    /// <summary>
    /// 设置安卓返回键功能
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_ANDROID")]
    protected void SetAndroidQuit()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
#else
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape)) 
#endif
        {
            if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
            {
                return;
            }


            var activePages = PageMgr.GetActivePages();
            if (activePages.Count == 1 && activePages[0].name == "UIMainPage")
            {
                //只有主界面存在,并且点了返回键
                PageMgr.ShowPage<UIQuitGamePage>();
                return;
            }

            foreach (var item in PageMgr.allPages)
            {
                if (item.Key != "UIMainPage")
                {
                    item.Value.Hide();
                }
            }
        }
    }

    /// <summary>
    /// 设置倒计时功能
    /// </summary>
    protected void SetCountDownShow(int deltaTimeMS)
    {
        var buffList = GlobalDataManager.GetInstance().playerData.playerZoo.buffList;
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].buffID ==10)
            {
                double time = buffList[i].CD.cd;
                celerityVisit.gameObject.SetActive(true);
                TimeFormatCalculate_Visit(time);
            }
            else if (buffList[i].buffID == 12)
            {
                double time = buffList[i].CD.cd;
                celerityTicket.gameObject.SetActive(true);
                TimeFormatCalculate_Ticket(time);
            }
            else if (buffList[i].buffID == 14)
            {
            }
        }

        //if (strengthenButton_Bool == true)
        //{
        //    if (strengthenButton_Time<0)
        //    {
        //        return;
        //    }
        //    strengthenButton_Time -= deltaTimeMS;
        //    TimeFormatCalculate_StrengthenButton(strengthenButton_Time);
        //}

    }

    protected void TimeFormatCalculate_Visit(double isTime)
    {
        celerityVisit.Find("Text").GetComponent<Text>().text = Math_F.OnDounbleToFormatTime((int)isTime);
        if (isTime <= 1)
        {
            celerityVisit.gameObject.SetActive(false);
        }
    }
    protected void TimeFormatCalculate_Ticket(double isTime)
    {
        celerityTicket.Find("Text").GetComponent<Text>().text = Math_F.OnDounbleToFormatTime((int)isTime);
        if (isTime <= 1)
        {
            celerityTicket.gameObject.SetActive(false);
        }
    }
    protected void TimeFormatCalculate_StrengthenButton(double isTime)
    {
        //int time01 = (int)isTime / 1000;
        //strengthenButton_Text.text = Math_F.OnDounbleToFormatTime(time01);
        ////LogWarp.LogError("测试：time01     "+ time01);
        //if ( time01 <= 0)
        //{
        //    strengthenButton_Text.text =GetL10NString("Ui_Text_83");
        //    strengthenButton_Bool = false;
        //    strengthenButton.enabled = true;
        //}
        //else
        //{
        //    strengthenButton.enabled = false;
        //}
    }


    protected void OnClickLittleGameButton()
    {
        ZooGameLoader.GetInstance().UnLoad(null);
        SceneMgr.Inst.LoadSceneAsync("Load", () => { });
    }

    protected void OnClickTestButton(string str)
    {
        OnTestAddCoin();
        //OnTestEntryGate();
        //OnTestQuit();
        //OnTestBuff();
        //OnTestShip();
        //OnTestNewLittleZoo();
        //OnTestMoneyEffect();
        //OnTestRotaCamera();
        //OnTestDeactiveAnimal();
        //OnTestAnimalShowUI3D();
        //OnTestLongPressButton();
        //OnTestFormule();

        //OnTestOfflineBuff();
       // OnTestAddItem();
    }

    /// <summary>
    /// 测试Item的消息机制
    /// </summary>
    private void OnTestAddItem()
    {
        MessageInt.Send((int)GameMessageDefine.GetItem, 4);
    }

    private void OnTestOfflineBuff()
    {
        List<int> removeList = new List<int>{
            0,
            1,
            3,
            5,
        };
        List<string> myList = new List<string>
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
        };

        List<string> myList01 = new List<string>();

        for (int i = 0; i < removeList.Count; i++)
        {
            int removeIdx = removeList[i];
            var str = myList[removeIdx];
            myList01.Add(str);
        }
        foreach (var item in myList01)
        {
            myList.Remove(item);
        }
        foreach (var item in myList)
        {
            LogWarp.LogError("测试：       "+item);
        }
    }

    protected void OnClickTestButton01(string str)
    {
        //OnTestBuffReturn();
       // OnTestItemData();
    }

    private void OnTestItemData()
    {
        var pd = GlobalDataManager.GetInstance().playerData;

        Logger.LogWarp.LogErrorFormat("测试: item   钻石diamond={0}，   星星star={1}，   现金coin={2} ",pd.playerZoo.diamond,pd.playerZoo. star,pd.playerZoo.coin);



    }



    /// <summary>
    /// 测试数值
    /// </summary>
    private void OnTestFormule()
    {
        for (int i = 1; i < 50; i++)
        {
            //var number01 = PlayerDataModule.GetAdditionExpect(i);
            //LogWarp.LogErrorFormat("测试数值  加成预期：  等级={0}   value={1}    ", i, number01);
            //var number02 = ParkingCenter.ParkingEnterCarSpawnExpectLevel(i);
            //LogWarp.LogErrorFormat("测试数值  来客速度期望等级：  等级={0}   value={1}    ", i, number02);
            //var number03 = ParkingCenter.ParkingSpaceExpectLevel(i);
            //LogWarp.LogErrorFormat("测试数值  停车位数期望等级：  等级={0}   value={1}    ", i, number03);
            var number04 = ParkingCenter.ParkingProfitExpectLevel(i);
            LogWarp.LogErrorFormat("测试数值  利润提升期望等级等级：  等级={0}   value={1}    ", i, number04);


            //var number05 = LittleZooModule.GetAnimalExpectLevel(i);
            //LogWarp.LogErrorFormat("测试数值  动物期望等级：  等级={0}   value={1}    ", i, number05);


            //var number06 = LittleZooModule.GetAnimalUpLevelPriceFormula( i);
            //LogWarp.LogErrorFormat("测试数值  动物升级消耗价格：  等级={0}   value={1}    ", i, number06);

        }
    }

    private void TestOnAddBuffSucceed(Message obj)
    {
        LogWarp.LogError("测试，buff便换了");

    }

    /// <summary>
    /// 测试:3D动物显示在UI上
    /// </summary>
    private void OnTestAnimalShowUI3D()
    {
        //PageMgr.ShowPage<UIReceivePage>("10101");  //旋转视角UI
        //PageMgr.ClosePage<UIZooPage>();
        ////PageMgr.ClosePage<UIMainPage>();
        //MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButHidePart, "UIMainPage");
    }

    /// <summary>
    /// 测试：动物购买
    /// </summary>
    private void OnTestDeactiveAnimal()
    {
        LogWarp.LogError("测试：：   删除动物");
        // LittleZooModule.DeactiveAnimal(1001, 10101);
        Dictionary<int, EntityMovable> entityMovables = EntityManager.GetInstance().entityMovables;
        EntityManager.GetInstance().RemoveFromEntityMovables(entityMovables[entityMovables.Count - 1]);
    }

    /// <summary>
    /// 测试 建筑升级相机旋转
    /// </summary>
    private void OnTestRotaCamera()
    {
        //int littleZooID = 1001;
        //string path = string.Format("LittleZoo/{0}", littleZooID.ToString());
        ////LogWarp.LogError("测试：   路径   " + path);
        //GlobalDataManager.GetInstance().playerData.playerZoo.BuildShowTransform = GameObject.Find(path).transform;
        //PageMgr.ClosePage<UIZooPage>();
        ////PageMgr.ClosePage<UIMainPage>();
        //MessageString.Send((int)GameMessageDefine.UIMessage_ActiveButHidePart, "UIMainPage");

        //PageMgr.ShowPage<UIBuildShowPage>();  //旋转视角UI
    }


    protected void OnTestAddCoin()
    {
        //点一下,把当前的金币变成2倍
        var pd = GlobalDataManager.GetInstance().playerData;
        var addCoin = BigInteger.Parse(pd.playerZoo.coin);
        SetValueOfPlayerData.Send((int)GameMessageDefine.SetCoinOfPlayerData, 0, addCoin, 0);
    }

    protected void OnTestEntryGate()
    {
        SetDetailValueOfPlayerData.Send((int)GameMessageDefine.SetEntryGatePureLevelOfPlayerData, 0, 1, 0);
        //SetValueOfPlayerData.Send((int)GameMessageDefine.SetEntryGateNumOfPlayerData, 1, 0, 0);
    }

    protected void OnTestQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#else
        Application.Quit();
#endif

    }

    protected void OnTestBuff()
    {
        //Buff测试
        //BroadcastNum.Send((int)GameMessageDefine.AddBuff, 1, 0, 0);
        //BroadcastNum.Send((int)GameMessageDefine.AddBuff, 9, 0, 0);
        //BroadcastNum.Send((int)GameMessageDefine.AddBuff, 10, 0, 0);
        BroadcastNum.Send((int)GameMessageDefine.AddBuff, 1, 0, 0);
        BroadcastNum.Send((int)GameMessageDefine.AddBuff, 2, 0, 0);
        BroadcastNum.Send((int)GameMessageDefine.AddBuff, 5, 0, 0);
        BroadcastNum.Send((int)GameMessageDefine.AddBuff, 14, 0, 0);

    }
    protected void OnTestBuffReturn()
    {
        
        var buffList = GlobalDataManager.GetInstance().playerData.playerZoo.buffList;

        //var offlineBuffList01 = PlayerDataModule.OnGetBuffCoefficient(buffList);

        foreach (var item in buffList)
        {
            LogWarp.LogErrorFormat("测试： offlineBuffList:  buffID={0},  buff使用时间={1}，buff还剩时间={2}， buff类型={3},  buff值={4}     加倍系数{5}",
            item.buffID,    item.CD.org,        item.CD.cd,    item.buffType,  item.buffVal,PlayerDataModule.PlayerRatioCoinInComeMul());
        }



    }



    protected void OnTestShip()
    {
        EntityShip.GetoffVisitor(15);
    }

    protected void OnTestNewLittleZoo()
    {

        //开启新动物栏测试var 
        var littleZooModuleDatas = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas;
        int level = Const.Invalid_Int;
        int littleZooID = Const.Invalid_Int;
        for (int i = 0; i < littleZooModuleDatas.Count; i++)
        {
            littleZooID = littleZooModuleDatas[i].littleZooID;
            level = littleZooModuleDatas[i].littleZooTicketsLevel;
            if (level == 0)
            {
                OpenNewLittleZoo.Send(littleZooID);
                break;
            }
        }
    }


    public void OnMoneyEffect()
    {
        var effTarget = this.transform.Find("MoneyGroup/Money_2/GoldIcon");
        List<object> moneyEffectData = new List<object>();
        moneyEffectData.Clear();
        moneyEffectData.Add((UnityEngine.Vector2)effTarget.position);
        moneyEffectData.Add(MoneyType.Money);
        PageMgr.ShowPage<UIMoneyEffect>(moneyEffectData);
        //Logger.LogWarp.LogError("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
    }

    /// <summary>
    /// 测试长按按钮事件
    /// </summary>
    protected void OnTestLongPressButton()
    {
        LogWarp.LogError("AAAAAAAAAAAAAAAAA    测试长按按钮事件   !");

        if (isBool)
        {
            SetValueOfPlayerData.Send((int)GameMessageDefine.SetParkingProfitLevelOfPlayerData,
                1, 0, 0);
            LogWarp.LogError("AAAAAAAAAAAAAAAAA    成功   停车场等级 =" + GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData.parkingProfitLevel);
            isBool = false;
        }

    }
    bool isBool = true;
    protected void OnTestReleaseButton()
    {
        LogWarp.LogError("BBBBBBBBBBBBBBBBB    测试离开长按按钮事件   !");
        isBool = true;
    }

    protected void ActiveButHidePart()
    {
        this.transform.Find("MoneyGroup").gameObject.SetActive(true);
        this.transform.Find("SetMainShow").GetComponent<CanvasGroup>().alpha = 0;
        touristButton.enabled = false;
        visitButton.enabled = false;
        ticketButton.enabled = false;
        AdvertButton.enabled = false;
        strengthenButton.enabled = false;
    }

    protected void ActiveButShowPart()
    {
        
        this.transform.Find("MoneyGroup").gameObject.SetActive(true);
        this.transform.Find("SetMainShow").GetComponent<CanvasGroup>().alpha = 1;
        touristButton.enabled = true;
        visitButton.enabled = true;
        ticketButton.enabled = true;
        AdvertButton.enabled = true;
        strengthenButton.enabled = true;
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isShowAnimalCultivate)
        {
            strengthenButton.gameObject.SetActive(false);
        }
        //strengthenButton.enabled = true;
    }
    private void OnUIMessage_OnClickButShowPart(Message obj)
    {
        touristButton.enabled = true;
        visitButton.enabled = true;
        ticketButton.enabled = true;
        AdvertButton.enabled = true;
        strengthenButton.enabled = true;
        //touristButton.gameObject.SetActive(true);
        //visitButton.gameObject.SetActive(true);
        //ticketButton.gameObject.SetActive(true);
        //AdvertButton.gameObject.SetActive(true);
    }

    private void OnUIMessage_OnClickButHidePart(Message obj)
    {
        touristButton.enabled = false;
        visitButton.enabled = false;
        ticketButton.enabled = false;
        AdvertButton.enabled = false;
        strengthenButton.enabled = false;
        //touristButton.gameObject.SetActive(false);
        //visitButton.gameObject.SetActive(false);
        //ticketButton.gameObject.SetActive(false);
        //AdvertButton.gameObject.SetActive(false);
    }

    public void OpenGuideTaskPanel()
    {
       
    }
}
