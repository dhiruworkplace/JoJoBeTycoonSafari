using ZooGame;
using ZooGame.GlobalData;
using ZooGame.MessageCenter;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;
using UnityEngine.UI;
using UFrame.MiniGame;
using DG.Tweening;

public class UIParkPage : UIPage
{
    /// <summary>
    /// 当前玩家的金钱
    /// </summary>
    BigInteger coinVal;
    Config.parkingCell parkingCell;
    /// <summary>
    /// 停车场利润等级
    /// </summary>
    int profitLevel;
    /// <summary>
    /// 停车位等级
    /// </summary>
    int parkingSpaceLevel;
    /// <summary>
    /// 流量等级
    /// </summary>
    int enterCarSpawnLevel;
    /// <summary>
    /// 停车场数据
    /// </summary>
    ParkingCenterData parkingCenterData;

    /// <summary>
    /// 当前等级段的最大值
    /// </summary>
    int maxGrade;
    int oldMaxGrade;

    /// <summary>
    /// 策划表配置的最大等级
    /// </summary>
    int parkingProfitMaxGrade;
    int parkingSpaceMaxGrade;
    int parkingEnterCarSpawnMaxGrade;

    /// <summary>
    /// 利润要升级需要消费的钱币
    /// </summary>
    BigInteger consumeProfitCoins;
    /// <summary>
    /// 停车位要升级需要消费的钱币
    /// </summary>
    BigInteger consumeParkingSpaceCoins;
    /// <summary>
    /// 流量要升级需要消费的钱币
    /// </summary>
    BigInteger consumeEnterCarSpawnCoins;


    /// <summary>
    /// 是否扣钱成功是否收到回复
    /// </summary>
    bool isGetCoin=true;

    /// <summary>
    /// 假计时   区分单点和长按
    /// </summary>
    int fakeTime = 0;

    /// <summary>
    /// 判断是否是长按状态
    /// </summary>
    bool isLongPress=false;

    //#region 全局UI控件属性
    Text titleText;    //名称：停车场
    Text tipsText;          //释义语言
    Text scoreNumTest;     //UI的星星收集显示
    int starLevelReached;

    Transform effectNode;  //新手引导节点

    Text lvText;            //等级text

    Text profitCoinsText;      //收益text
    Text profitCoins_Text_2;  
    Text profitCoins_Text_3;  
    Text profitCoins_LvText;
    Button profitCoins_Button;   //升级按钮
    Text profitCoins_Button_NeedGoldNum;       //升级模式需要的金钱
    Text profitCoins_Button_ButtonLvUpText;    //升级模式要升的级数
    Transform profitCoins_EffectNode;

    Text parkingSpaceText;        //数量Text
    Text parkingSpace_Text2; 
    Text parkingSpace_Text3; 
    Text parkingSpace_LvText;
    Button parkingSpace_Button;   //升级按钮
    Text parkingSpace_Button_NeedGoldNum;       //升级模式需要的金钱
    Text parkingSpace_Button_ButtonLvUpText;    //升级模式要升的级数
    Transform parkingSpace_EffectNode;

    Text enterCarSpawnText;       //冷却时间
    Text enterCarSpawn_Text2; 
    Text enterCarSpawn_Text3; 
    Text enterCarSpawn_LvText;
    Button enterCarSpawn_Button;   //升级按钮
    Text enterCarSpawn_Button_NeedGoldNum;       //升级模式需要的金钱
    Text enterCarSpawn_Button_ButtonLvUpText;    //升级模式要升的级数
    Transform enterCarSpawn_EffectNode;

    Slider gradeSlider;
    Image gradeSlider_Image;
    Text gradeSlider_Text;



    Button hideUIButton;

    Text gradeText_2;  //价格标签3
   // #endregion

    public UIParkPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UINewParking";
    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        GetTransPrefabAllTextShow(this.transform);

        //初始化控件
        this.RegistAllCompent(); 
    }
    
    /// <summary>
    /// 内部组件的查找
    /// </summary>
    private void RegistAllCompent()
    {
        titleText = RegistCompent<Text>("UIParking_LvUp/UiBg/TitleGroup/TitleText");
        //当前等级
        lvText = RegistCompent<Text>("UIParking_LvUp/UiBg/TitleGroup/LvText");
        tipsText = RegistCompent<Text>("UIParking_LvUp/UiBg/TitleGroup/TipsText");
        //等级进度条控件
        gradeSlider = RegistCompent<Slider>("UIParking_LvUp/UiBg/LvUpSchedule/Schedule/Slider2");
        gradeText_2 = RegistCompent<Text>("UIParking_LvUp/UiBg/LvUpSchedule/Schedule/Text_2");
        gradeSlider_Image = RegistCompent<Image>("UIParking_LvUp/UiBg/LvUpSchedule/IconBg/Icon");
        gradeSlider_Text = RegistCompent<Text>("UIParking_LvUp/UiBg/LvUpSchedule/IconBg/Num");
        scoreNumTest = RegistCompent<Text>("UIParking_LvUp/UiBg/ScoreGroup/ScoreNum");

        /* 利润text */
        profitCoinsText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/Text_1");      
        //当前停车场价格
        profitCoins_Text_2 = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/TextAll/Text_2");
        //升级后的价格
        profitCoins_Text_3 = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/TextAll/Text_3");
        profitCoins_LvText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/Level/LvText");
        profitCoins_EffectNode = RegistCompent<Transform>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/effectNode");
        //升级按钮
        profitCoins_Button = RegistBtnAndClick("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/LvUpButton", OnClickUpGrade_ProfitCoins);
        profitCoins_Button.gameObject.AddComponent<RepeatButton>();
        profitCoins_Button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressButton_ProfitCoins);//按下。频繁的调用
        profitCoins_Button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次

        //升级需要的金币
        profitCoins_Button_NeedGoldNum = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/LvUpButton/NeedGoldNum");
        //可以升级的级数
        profitCoins_Button_ButtonLvUpText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_1/LvUpButton/ButtonLvUpText");

        hideUIButton = RegistBtnAndClick("UIParking_LvUp/UiBg/BgImage/BgImage", HideButtonUI);

        /* 数量Text */
        parkingSpaceText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/Text_1");        
        //当前的停车场数量
        parkingSpace_Text2 = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/TextAll/Text_2");
        //升级后的停车场数量
        parkingSpace_Text3 = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/TextAll/Text_3");
        parkingSpace_LvText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/Level/LvText");
        parkingSpace_EffectNode = RegistCompent<Transform>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/effectNode");

        //升级按钮
        parkingSpace_Button = RegistBtnAndClick("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/LvUpButton", OnClickUpGrade_ParkingSpace);
        parkingSpace_Button.gameObject.AddComponent<RepeatButton>();
        parkingSpace_Button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressButton_ParkingSpace);//按下。频繁的调用
        parkingSpace_Button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次
        //升级需要的金币
        parkingSpace_Button_NeedGoldNum = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/LvUpButton/NeedGoldNum");
        //可以升级的级数
        parkingSpace_Button_ButtonLvUpText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_2/LvUpButton/ButtonLvUpText");



        /* 冷却时间 */
        enterCarSpawnText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/Text_1");      
        //当前停车场的流量
        enterCarSpawn_Text2 = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/TextAll/Text_2");
        //升级后的停车场流量
        enterCarSpawn_Text3 = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/TextAll/Text_3");
        enterCarSpawn_LvText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/Level/LvText");
        enterCarSpawn_EffectNode = RegistCompent<Transform>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/effectNode");
        //升级按钮
        enterCarSpawn_Button = RegistBtnAndClick("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/LvUpButton", OnClickUpGrade_EnterCarSpawn);
        enterCarSpawn_Button.gameObject.AddComponent<RepeatButton>();
        enterCarSpawn_Button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressButton_EnterCarSpawn);//按下。频繁的调用
        enterCarSpawn_Button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次
        //升级需要的金币
        enterCarSpawn_Button_NeedGoldNum = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/LvUpButton/NeedGoldNum");
        //可以升级的级数
        enterCarSpawn_Button_ButtonLvUpText = RegistCompent<Text>("UIParking_LvUp/UiBg/ParameterGroup/Parameter/Parameter_3/LvUpButton/ButtonLvUpText");

    }
    /// <summary>
    /// 初始化属性数值
    /// </summary>
    private void InitData( )
    {
        //LogWarp.LogError(" 测试：   InitData   ");
        parkingCell = Config.parkingConfig.getInstace().getCell(1);
        parkingProfitMaxGrade = parkingCell.lvmax;
        parkingSpaceMaxGrade = parkingCell.spacemaxlv;
        parkingEnterCarSpawnMaxGrade = parkingCell.touristmaxlv;

        InitCoin();

        int idx = PlayerDataModule.FindLevelRangIndex(Config.parkingConfig.getInstace().getCell(1).lvshage, profitLevel);
        maxGrade = parkingCell.lvshage[idx];
        oldMaxGrade = parkingCell.lvshage[idx - 1];
        starLevelReached = PlayerDataModule.FindLevelRangIndex01(Config.parkingConfig.getInstace().getCell(1).lvshage, profitLevel);
        if (profitLevel >= parkingProfitMaxGrade)
        {
            starLevelReached = PlayerDataModule.FindLevelRangIndex01(Config.parkingConfig.getInstace().getCell(1).lvshage, profitLevel);
        }

        InitCompent();
    }

    private void InitCoin()
    {
        //获取玩家停车场等级       获取玩家现有金币
        parkingCenterData = GlobalDataManager.GetInstance().playerData.playerZoo.parkingCenterData;

        profitLevel = parkingCenterData.parkingProfitLevel;
        parkingSpaceLevel = parkingCenterData.parkingSpaceLevel;
        enterCarSpawnLevel = parkingCenterData.parkingEnterCarSpawnLevel;

        coinVal = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin);
        consumeProfitCoins = ParkingCenter.GetUpGradeParkingProfitConsumption(profitLevel);
        consumeParkingSpaceCoins = ParkingCenter.GetUpGradeNumberConsumption(parkingSpaceLevel);
        consumeEnterCarSpawnCoins = ParkingCenter.GetUpGradeEnterCarSpawnConsumption(enterCarSpawnLevel);
    }

    /// <summary>
    /// 控件显示赋值
    /// </summary>
    private void InitCompent()
    {
        //LogWarp.LogError(" 测试：   InitCompent   ");

        if (maxGrade >= parkingProfitMaxGrade)
        {
            maxGrade = parkingProfitMaxGrade;
        }
        lvText.text = string.Format(GetL10NString("Ui_Text_2"), profitLevel.ToString());
        gradeSlider.value = AddPercentage(profitLevel - oldMaxGrade, maxGrade- oldMaxGrade);
        gradeText_2.text = profitLevel.ToString() + "/" + maxGrade.ToString();  //最大等级上限
        //获取UI image =  
        Config.itemCell itemCell = GradeSliderAwardImage();
        gradeSlider_Image.sprite = ResourceManager.LoadSpriteFromPrefab(itemCell.icon);
        gradeSlider_Text.text =MinerBigInt.ToDisplay(  itemCell.itemval);
        scoreNumTest.text = starLevelReached + "/" + Config.parkingConfig.getInstace().getCell(1).starsum;
        profitCoins_Text_2.text = ParkingCenter.GetParkingProfit(profitLevel).ToString()+"%";//a.ToString("#0.0")
        profitCoins_Text_3.text = "+" + ParkingCenter.GetParkingProfit(profitLevel, 1).ToString()+"%";
        profitCoins_LvText.text = profitLevel.ToString();
        parkingSpace_Text2.text = ParkingCenter.GetParkingSpace(parkingSpaceLevel).ToString();
        parkingSpace_Text3.text = "+" + (ParkingCenter.GetParkingSpace(parkingSpaceLevel, 1)).ToString();
        parkingSpace_LvText.text =  parkingSpaceLevel.ToString();
        enterCarSpawn_Text2.text = ParkingCenter.GetParkingEnterCarSpawn(enterCarSpawnLevel).ToString()+ GetL10NString("Ui_Text_67");
        enterCarSpawn_Text3.text = "+" + ParkingCenter.GetParkingEnterCarSpawn(enterCarSpawnLevel, 1).ToString();
        enterCarSpawn_LvText.text = enterCarSpawnLevel.ToString();

        profitCoins_Button_NeedGoldNum.text = MinerBigInt.ToDisplay(consumeProfitCoins).ToString();       //升级模式需要的金钱
        profitCoins_Button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

        parkingSpace_Button_NeedGoldNum.text = MinerBigInt.ToDisplay(consumeParkingSpaceCoins).ToString();       //升级模式需要的金钱
        parkingSpace_Button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

        enterCarSpawn_Button_NeedGoldNum.text = MinerBigInt.ToDisplay(consumeEnterCarSpawnCoins).ToString();       //升级模式需要的金钱
        enterCarSpawn_Button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

        SetGradeBool_Profit();
        SetGradeBool_ParkingSpace();
        SetGradeBool_EnterCarSpawn();
        if (profitLevel >= parkingProfitMaxGrade)
        {
            profitCoins_Button_NeedGoldNum.text = GetL10NString("Ui_Text_47");       //升级模式需要的金钱
            profitCoins_Button_ButtonLvUpText.text = GetL10NString("Ui_Text_46");    //升级模式要升的级数
            lvText.text = Config.parkingConfig.getInstace().getCell(1).lvmax.ToString();//等级text
            profitCoins_Text_3.text = GetL10NString("Ui_Text_47");  //价格变化标签
            SwitchButtonUnclickable(profitCoins_Button, false);
        }
        if (parkingSpaceLevel >= parkingSpaceMaxGrade)
        {
            parkingSpace_Button_NeedGoldNum.text = GetL10NString("Ui_Text_47");       //升级模式需要的金钱
            parkingSpace_Button_ButtonLvUpText.text = GetL10NString("Ui_Text_46");    //升级模式要升的级数
            parkingSpace_Text3.text = GetL10NString("Ui_Text_47"); //数量变化标签
            SwitchButtonUnclickable(parkingSpace_Button, false);
        }
        if (enterCarSpawnLevel >= parkingEnterCarSpawnMaxGrade)
        {
            enterCarSpawn_Button_NeedGoldNum.text = GetL10NString("Ui_Text_47");       //升级模式需要的金钱
            enterCarSpawn_Button_ButtonLvUpText.text = GetL10NString("Ui_Text_46");    //升级模式要升的级数
            enterCarSpawn_Text3.text = GetL10NString("Ui_Text_47"); //速度变化标签
            SwitchButtonUnclickable(enterCarSpawn_Button, false);
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
        {
            UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
            if (uIGuidePage == null)
            {
                string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
                throw new System.Exception(e);
            }
            if (uIGuidePage.procedure == 5)
            {
                SwitchButtonUnclickable(parkingSpace_Button, true);
                SwitchButtonUnclickable(profitCoins_Button, false);
                SwitchButtonUnclickable(enterCarSpawn_Button, false);
            }
            else if (uIGuidePage.procedure == 8)
            {
                SwitchButtonUnclickable(parkingSpace_Button, false);
                SwitchButtonUnclickable(profitCoins_Button, false);
                SwitchButtonUnclickable(enterCarSpawn_Button, true);
            }
        }
        //else
        //{
        //    SwitchButtonUnclickable(parkingSpace_Button, true);
        //    SwitchButtonUnclickable(profitCoins_Button, true);
        //    SwitchButtonUnclickable(enterCarSpawn_Button, true);
        //}

    }

    /// <summary>
    /// 点击利润按钮事件
    /// </summary>
    public void OnClickUpGrade_ProfitCoins(string str)
    {
        //isLongPress为true则是长按状态，单点关闭  返回
        if (!JudgePressButton_Profit()&& isLongPress)
        {
            LogWarp.LogError("不能升级");
            return;
        }
        /*  新手引导  */
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            if (profitLevel >= 5)
            {
                return;
            }
        }
        SendSetParkingLevelMessageManager();
        isGetCoin = false;  //设置等待回复状态
        //upGradeButton.enabled = false; //设置按钮不能点击
        ///播放音乐
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);

        //LogWarp.LogError("测试             单点  按钮         ");
    }
    /// <summary>
    /// 点击停车场数量按钮事件
    /// </summary>
    public void OnClickUpGrade_ParkingSpace(string str)
    {
        if (!JudgePressButton_ParkingSpace() && isLongPress)
        {
            LogWarp.LogError("不能升级");
            return;
        }
        SendSetParkingSpaceLevelMessageManager();
        isGetCoin = false;  //设置等待回复状态
        ///播放音乐
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);
    }
    /// <summary>
    /// 点击流量按钮事件
    /// </summary>
    public void OnClickUpGrade_EnterCarSpawn(string str)
    {
        if (!JudgePressButton_EnterCarSpawn() && isLongPress)
        {
            LogWarp.LogError("不能升级");
            return;
        }
        SendSetParkingCoolingLevelMessageManager();
        isGetCoin = false;  //设置等待回复状态
        ///播放音乐
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);
    }

    private void SendSetParkingLevelMessageManager()
    {
        if (profitLevel >= parkingProfitMaxGrade)
        {
            SwitchButtonUnclickable(profitCoins_Button, false);
            return;
        }
        //发送消息       SetValueOfPlayerData  消息体   
        SetValueOfPlayerData.Send((int)GameMessageDefine.SetParkingProfitLevelOfPlayerData,
        1, 0, 0);
    }
    private void SendSetParkingSpaceLevelMessageManager()
    {
        if (parkingSpaceLevel >= parkingSpaceMaxGrade)
        {
            //parkingSpace_Button.enabled = false;
            SwitchButtonUnclickable(parkingSpace_Button, false);
            //parkingSpace_Button_ButtonLvUpText.color = Color.red;
            return;
        }
        SetValueOfPlayerData.Send((int)GameMessageDefine.SetParkingSpaceLevelOfPlayerData,
        1, 0, 0);
    }
    private void SendSetParkingCoolingLevelMessageManager()
    {
        if (enterCarSpawnLevel >= parkingEnterCarSpawnMaxGrade)
        {
            SwitchButtonUnclickable(enterCarSpawn_Button, false);
            return;
        }

        SetValueOfPlayerData.Send((int)GameMessageDefine.SetParkingEnterCarSpawnLevelOfPlayerData,
        1, 0, 0);
    }

    /// <summary>
    /// 监听广播停车场等级
    /// </summary>
    /// <param name="msg"></param>
    protected void OnGetBroadcastParkingProfitLevelOfPlayerData(Message msg)
    {
        this.InitData();
        UIButtonEffectSimplePlayer(profitCoins_EffectNode);
		GameEventManager.SendEvent("AppsFlyerEnum.parking_attribute_lv");
        isGetCoin = true;

    }

    /// <summary>
    /// 监听广播停车场停车位数量等级
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetBroadcastParkingSpaceLevelOfPlayerData(Message obj)
    {
        this.InitData();
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            GameEventManager.SendEvent("parkingSpaceLevel");
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true && parkingSpaceLevel == 2)
        {   /*新手阶段   隐藏停车场Ui  显示新手引导UI    步骤应该是  7    */
            DestroyEffectChild();
            this.Hide();

            PageMgr.ShowPage<UIGuidePage>();
        }
        UIButtonEffectSimplePlayer(parkingSpace_EffectNode);
        GameEventManager.SendEvent("parking_attribute_lv");
        isGetCoin = true;
    }
    /// <summary>
    /// 监听广播停车场来客数量等级
    /// </summary>
    private void OnGetBroadcastParkingEnterCarSpawnLevelOfPlayerData(Message obj)
    {
        this.InitData();
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            GameEventManager.SendEvent("enterCarSpawnLevel");
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true && enterCarSpawnLevel >= 5)
        {   /*新手阶段   隐藏停车场Ui  显示新手引导UI    步骤应该是  9   */
            DestroyEffectChild();
            SwitchButtonUnclickable(parkingSpace_Button, true);
            SwitchButtonUnclickable(profitCoins_Button, true);
            SwitchButtonUnclickable(enterCarSpawn_Button, true);

            this.Hide();
            PageMgr.ShowPage<UIGuidePage>();
            Logger.LogWarp.LogErrorFormat("AAAAAAAAAAAAAAAA");

        }
        UIButtonEffectSimplePlayer(enterCarSpawn_EffectNode);
        GameEventManager.SendEvent("parking_attribute_lv");
        isGetCoin = true;
    }

    /// <summary>
    /// 监听玩家coin金钱发生改变，是否需要重新计算升级规模
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetCoinOfPlayerData(Message obj)
    {   //旧计算金钱不够，则开始新的计算
        this.InitData();
    }
    /// <summary>
    /// 更新数据  页面显示
    /// </summary> 
    public override void Refresh()
    {
        base.Refresh();
        //this.InitData();
        //this.InitCompent();
    }
    /// <summary>
    /// 添加按钮事件
    /// </summary>
    public override void Active()
    {
        base.Active();
        this.InitData();//初始化属性数值
        //注册监听消息     
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnGetCoinOfPlayerData);//接受金钱变动的信息
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastParkingProfitLevelOfPlayerData, this.OnGetBroadcastParkingProfitLevelOfPlayerData);//接收停车场的利润等级的广播
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastParkingSpaceLevelOfPlayerData, this.OnGetBroadcastParkingSpaceLevelOfPlayerData);//接收停车场的位置数量等级的广播
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastParkingEnterCarSpawnLevelOfPlayerData, this.OnGetBroadcastParkingEnterCarSpawnLevelOfPlayerData);//接收停车场的来客流量等级的广播
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);

        /*  若是新手引导阶段，进入特殊处理方法  */
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            DelayedOperationNewbieGuideStage();
        }
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        OnReleaseButton();
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastParkingProfitLevelOfPlayerData, this.OnGetBroadcastParkingProfitLevelOfPlayerData);//接收停车场的利润等级的广播
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastParkingSpaceLevelOfPlayerData, this.OnGetBroadcastParkingSpaceLevelOfPlayerData);//接收停车场的位置数量等级的广播
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastParkingEnterCarSpawnLevelOfPlayerData, this.OnGetBroadcastParkingEnterCarSpawnLevelOfPlayerData);//接收停车场的来客流量等级的广播
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnGetCoinOfPlayerData);//接受金钱变动的信息
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);

        MessageString.Send((int)GameMessageDefine.UIMessage_OnClickButShowPart, "UIMainPage");
        UIInteractive.GetInstance().iPage = null;

    }

    private void OnOpenOfflineUIPage(Message obj)
    {
        HideButtonUI("");
    }

    private void HideButtonUI(string str)
    {
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            return;
        }
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.1f).OnComplete(new TweenCallback(Hide));
    }

    /// <summary>
    /// 判断是否可以升级（钱够/等级不超过最大值）
    /// </summary>
    /// <returns></returns>
    private bool SetGradeBool_Profit()
    {
        InitCoin();
        if (consumeProfitCoins <= coinVal && profitLevel < parkingProfitMaxGrade)
        {
            SwitchButtonUnclickable(profitCoins_Button, true);

            return true;
        }
        else
        {
            SwitchButtonUnclickable(profitCoins_Button, false);

            return false;
        }
    }
    /// <summary>
    /// 判断是否可以升级（钱够/等级不超过最大值）
    /// </summary>
    /// <returns></returns>
    private bool SetGradeBool_ParkingSpace()
    {
        InitCoin();
        //LogWarp.LogErrorFormat("测试：{0}  {1}   {2}   {3}    ",consumeParkingSpaceCoins , coinVal ,parkingSpaceLevel ,Config.parkingConfig.getInstace().getCell(1).spacemaxlv);
        if (consumeParkingSpaceCoins <= coinVal && parkingSpaceLevel < parkingSpaceMaxGrade)
        {
            SwitchButtonUnclickable(parkingSpace_Button, true);

            return true;
        }
        else
        {
            SwitchButtonUnclickable(parkingSpace_Button, false);
            return false;
        }
    }
    /// <summary>
     /// 判断是否可以升级（钱够/等级不超过最大值）
     /// </summary>
     /// <returns></returns>
    private bool SetGradeBool_EnterCarSpawn()
    {
        InitCoin();
        if (consumeEnterCarSpawnCoins <= coinVal && enterCarSpawnLevel < parkingEnterCarSpawnMaxGrade)
        {
            SwitchButtonUnclickable(enterCarSpawn_Button, true);
            return true;
        }
        else
        {
            SwitchButtonUnclickable(enterCarSpawn_Button, false);
            return false;
        }
    }
    /// <summary>
    /// 是否可以升级
    /// </summary>
    /// <returns></returns>
    private bool JudgePressButton_Profit()
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin&& SetGradeBool_Profit())
        {
            SwitchButtonUnclickable(profitCoins_Button, true);
            return true;
        }
        SwitchButtonUnclickable(profitCoins_Button, false);
        return false;
    }
    private bool JudgePressButton_ParkingSpace()
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin && SetGradeBool_ParkingSpace())
        {
            SwitchButtonUnclickable(parkingSpace_Button, true);
            return true;
        }
        SwitchButtonUnclickable(parkingSpace_Button, false);
        return false;
    }
    private bool JudgePressButton_EnterCarSpawn()
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级

        if (isGetCoin && SetGradeBool_EnterCarSpawn())
        {
            SwitchButtonUnclickable(enterCarSpawn_Button, true);
            return true;
        }
        SwitchButtonUnclickable(enterCarSpawn_Button, false);

        return false;
    }
    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPressButton_ProfitCoins()
    {
        if (!RestrictLongPressTime())
        {
            return;
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
        {
            return;
        }
        isLongPress = true;//进入长按状态
        if ( JudgePressButton_Profit() )
        {
            SendSetParkingLevelMessageManager();  //发送升级消息
            isGetCoin = false;
        }
    }

    /// <summary>
    /// 离开长按按钮回调事件
    /// </summary>
    private void OnReleaseButton()
    {
        //LogWarp.LogError("测试             离开  按钮         ");
        isLongPress = false;
        fakeTime = 0;
    }
    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPressButton_ParkingSpace()
    {
        if (!RestrictLongPressTime())
        {
            return;
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
        {
            return;
        }
        isLongPress = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         "+fakeTime);
        if (JudgePressButton_ParkingSpace())
        {
            SendSetParkingSpaceLevelMessageManager();  //发送升级消息
            isGetCoin = false;
        }
    }

    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPressButton_EnterCarSpawn()
    {
        if (!RestrictLongPressTime())
        {
            return;
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
        {
            return;
        }
        isLongPress = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         "+fakeTime);
        if (JudgePressButton_EnterCarSpawn())
        {
            SendSetParkingCoolingLevelMessageManager();  //发送升级消息
            isGetCoin = false;
        }
    }

    /// <summary>
    /// 争对新手引导阶段做些操作
    /// </summary>
    private void DelayedOperationNewbieGuideStage()
    {
        //根据新手引导阶段的步骤显示对应的特效和隐藏对应的按钮点击事件
        UIGuidePage uIGuidePage = PageMgr.GetPage<UIGuidePage>();
        if (uIGuidePage == null)
        {
            string e = string.Format("新手引导界面  PageMgr.allPages里 UIGuidePage   为空");
            throw new System.Exception(e);
        }
        if (uIGuidePage.procedure == 5 && parkingSpaceLevel < 2)
        {
            //停车场停车位按钮处显示小手点击动画
            effectNode = parkingSpace_Button.transform.Find("effectNode");
            Transform trans = null;
            trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().GuideUiClickEffect).transform;
            trans.SetParent(effectNode, false);
            SimpleParticle particlePlayer = new SimpleParticle();
            particlePlayer.Init(effectNode.gameObject);
            particlePlayer.Play();
        }
        else if (uIGuidePage.procedure == 8&& enterCarSpawnLevel<6)
        {
            effectNode = enterCarSpawn_Button.transform.Find("effectNode");
            Transform trans = null;
            trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().GuideUiClickEffect).transform;
            trans.SetParent(effectNode, false);
            SimpleParticle particlePlayer = new SimpleParticle();
            particlePlayer.Init(effectNode.gameObject);
            particlePlayer.Play();
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

    /// <summary>
    /// 获取等级段对应的奖励信息
    /// </summary>
    /// <returns></returns>
    private Config.itemCell GradeSliderAwardImage() {
        var lvreward = Config.parkingConfig.getInstace().getCell(1).lvreward;

        int idex = PlayerDataModule.FindLevelRangIndex(Config.parkingConfig.getInstace().getCell(1).lvshage, profitLevel);
        int itemID = lvreward[idex];

        Config.itemCell itemCell = Config.itemConfig.getInstace().getCell(itemID );

       // Logger.LogWarp.LogErrorFormat("测试： 等级={0}，等级段对应的奖励={1}", profitLevel,itemID);
        return itemCell;
    }
}

