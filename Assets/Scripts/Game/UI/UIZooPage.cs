using DG.Tweening;
using Game;
using Game.GlobalData;
using Game.MessageCenter;
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
//using System.Resources;
using UFrame;
using UFrame.MessageCenter;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class UIZooPage : UIPage
{
    string switchButton;//存储修改Ui 名字
    private Dictionary<int, Button> setButtonsDic;//设置UI显示的按钮列表
    private Dictionary<int, Transform> animalTransformData;//UI显示的按钮列表
    private Dictionary<string, int> animalAndID ;//设置UI显示的按钮列表
    private List<string> animalButton = new List<string>();
    private List<Transform> animalCellList;
    private List<int> animalCellID = new List<int>();
  

    Config.groupCell groupCell;
    /// <summary>
    /// 当前动物栏的id
    /// </summary>
    int nameID;
    int oldNameID=0;
    /// <summary>
    /// 语言表   当前动物栏的名字可以从表里获取
    /// </summary>
    Config.translateCell littleZooName;
    /// <summary>
    /// 动物栏Data  信息集合
    /// </summary>
    Config.buildupCell buildUpCell;
    /// <summary>
    /// 玩家的动物数据
    /// </summary>
    PlayerAnimal playerAnimal;

    bool isAnimalUp=false;

    /// <summary>
    /// 动物栏门票等级
    /// </summary>
    int littleZooTicketsLevel;
    /// <summary>
    /// 观光位数量等级
    /// </summary>
    int littleZooVisitorSeatLevel;
    /// <summary>
    /// 观光游客的流量等级
    /// </summary>
    int littleZooEnterVisitorSpawnLevel;
    /// <summary>
    /// 动物栏的动物数量
    /// </summary>
    int zooNumber;
    /// <summary>
    /// 当前要升级的规模参数,   默认为0
    /// </summary>
    int upGradeNumber = 1;
    /// <summary>
    /// 是否可以升级
    /// </summary>
    bool setGradeBool;
    /// <summary>
    /// 门票当前最大等级段
    /// </summary>
    int maxGrade;

    int oldMaxGrade;
    /// <summary>
    /// 策划表配置的最大等级
    /// </summary>
    int TicketsMaxGrade;
    /// <summary>
    /// 观光位数量当前最大等级段
    /// </summary>
    int VisitorSeatMaxGrade;
    /// <summary>
    /// 观光游客的流量当前最大等级段
    /// </summary>
    int EnterVisitorSpawnMaxGrade;
    /// <summary>
    /// 当前金钱
    /// </summary>
    BigInteger coinVal;
    /// <summary>
    /// 记录动物栏利润要升级需要消费的钱币
    /// </summary
    BigInteger ticketsLevelConsumeCoins;
    /// <summary>
    /// 记录动物栏观光位数量要升级需要消费的钱币
    /// </summary
    BigInteger visitorSeatLevelConsumeCoins;
    /// <summary>
    /// 记录动物栏流量等级要升级需要消费的钱币
    /// </summary
    BigInteger EnterVisitorSpawnLevelConsumeCoins;

    /// <summary>
    /// 记录动物要升级需要消费的钱币
    /// </summary
    BigInteger animalConsumeCoins;

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
    bool isLongPress = false;

    int animalLvUpLimit;

    /// <summary>
    /// 判断是否是长按状态
    /// </summary>
    bool isLongPressBuyAnimal = false;

    /// <summary>
    /// 当前动物栏的初始票价
    /// </summary>
    BigInteger pricebase;

    #region 动物栏父类控件字段（共显）
    Button zooCultivateButton;   //切换Cultivate按钮
    Button zooKindButton;        //切换Kind按钮
    GameObject zooCultivateUI;
    GameObject zooKindUI;
    GameObject animalGroup;
    Text titleText;
    Text lvText;
    Text tipsText;          //释义语言
    Text tipsTextAnimalup;  //动物栏动物养成的释义语言
    Text scoreNumTest;     //UI的星星收集显示
    int starLevelReached;
    Button lefeButton;
    Button reghtButton;
    Button hideUIButton;
    #endregion
    //Text lvTextSlider;      //等级（slider）
    //string uplevelString;
    //string levelString;
    #region 动物栏子类控件字段（养成独显）
    Slider LVUpSlider;
    Image LVUpSlider_Image;
    Text LVUpSlider_Text;



    Image animalShow;

    Image iconSlider;
    Text lVUpSliderText;

    Text ticketsText;      //利润text
    Text tickets_Text2;
    Text tickets_Text3;
    Text tickets_LvText;
    Button tickets_Button;
    Text tickets_Button_NeedGoldText;
    Text tickets_Button_buttonLvUpText;
    Transform tickets_EffectNode;

    Text visitorSeatText;        //数量Text
    Text visitorSeat_Text2;
    Text visitorSeat_Text3;
    Text visitorSeat_LvText;
    Button visitorSeat_Button;
    Text visitorSeat_Button_NeedGoldText;
    Text visitorSeat_Button_buttonLvUpText;
    Transform visitorSeat_EffectNode;

    Text visitorSpawnText;       //冷却时间
    Text visitorSpawn_Text2;
    Text visitorSpawn_Text3;
    Text visitorSpawn_LvText;
    Button visitorSpawn_Button;
    Text visitorSpawn_Button_NeedGoldText;
    Text visitorSpawn_Button_buttonLvUpText;
    Transform visitorSpawn_EffectNode;




    Transform effectNode;
   
    Text animalNumber;
    Image buttonIcon_1;
    Image buttonIcon_2;
    Image buttonIcon_3;

    #endregion
    #region 动物养成界面（独显）
    Transform animal_1;
    Transform animal_2;
    Transform animal_3;
    Transform animal_4;
    Transform animal_5;

    #endregion

    List<int> animalShowLevel;
    public UIZooPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        //uiPath = "uiprefab/UIBuild";
        uiPath = "uiprefab/UINewBuild";

    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);

        GetTransPrefabAllTextShow(this.transform);

        //默认开启ZooCultivate
        switchButton = "ZooCultivateButton";
        animalTransformData = new Dictionary<int, Transform>();
        animalAndID = new Dictionary<string, int>();


        //初始化控件  
        this.RegistAllCompent();
        this.RegistZooKindCompent();
        this.RegistAnimalCompent();

        //InitData();
        //this.InitCompent();
        //this.InitCompentZoo();
        //this.InitCompentCultivate();
        

    }

    /// <summary>
    /// 给动物分页分别添加控件
    /// </summary>
    private void RegistAnimalCompent()
    {
        buildUpCell = Config.buildupConfig.getInstace().getCell(m_data.ToString());
        int childCount = animalGroup.transform.childCount;

        float height = animalGroup.GetComponent<RectTransform>().sizeDelta.y;
        
        animalGroup.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2( 20f + 238f * childCount, height);
        /*按钮*/
        animal_1 = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_1");
        Button animal_1_button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_1/BuyButton", BuyPlayerAnimal01);
        animal_2 = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_2");
        Button animal_2_button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_2/BuyButton", BuyPlayerAnimal02);
        animal_3 = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_3");
        Button animal_3_button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_3/BuyButton", BuyPlayerAnimal03);
        animal_4 = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_4");
        Button animal_4_button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_4/BuyButton", BuyPlayerAnimal04);
        animal_5 = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_5");
        Button animal_5_button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup/Animal_5/BuyButton", BuyPlayerAnimal05);

        /*往动物购买按钮中添加长按*/
        animal_1_button.gameObject.AddComponent<RepeatButton>();
        animal_1_button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressBuyAnimal01);//按下。
        animal_1_button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseBuyAnimal);//抬起，调用一次
        animal_2_button.gameObject.AddComponent<RepeatButton>();
        animal_2_button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressBuyAnimal02);//按下。
        animal_2_button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseBuyAnimal);//抬起，调用一次
        animal_3_button.gameObject.AddComponent<RepeatButton>();
        animal_3_button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressBuyAnimal03);//按下。
        animal_3_button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseBuyAnimal);//抬起，调用一次
        animal_4_button.gameObject.AddComponent<RepeatButton>();
        animal_4_button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressBuyAnimal04);//按下。
        animal_4_button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseBuyAnimal);//抬起，调用一次
        animal_5_button.gameObject.AddComponent<RepeatButton>();
        animal_5_button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressBuyAnimal05);//按下。
        animal_5_button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseBuyAnimal);//抬起，调用一次
        animalCellList = new List<Transform>();
        animalCellList.Add(animal_1);
        animalCellList.Add(animal_2);
        animalCellList.Add(animal_3);
        animalCellList.Add(animal_4);
        animalCellList.Add(animal_5);
    }

    /// <summary>
    /// 初始化属性数值  切换动物栏ID时候需要调用
    /// </summary>
    private void InitData()
    {
        if (m_data == null)
        {
            this.Hide();
            return;
        }
        nameID = int.Parse(m_data.ToString());
        buildUpCell = Config.buildupConfig.getInstace().getCell(m_data.ToString());
        animalLvUpLimit = Config.globalConfig.getInstace().AnimalLvUpLimit;
        pricebase = BigInteger.Parse( buildUpCell.pricebase);
        playerAnimal = GlobalDataManager.GetInstance().playerData.playerZoo.playerAnimal;
        littleZooName = Config.translateConfig.getInstace().getCell(buildUpCell.buildname);//根据动物栏的名字ID去语言表获取动物栏的中文显示
        littleZooTicketsLevel = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(nameID).littleZooTicketsLevel;
        littleZooVisitorSeatLevel = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(nameID).littleZooVisitorSeatLevel;
        littleZooEnterVisitorSpawnLevel = GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(nameID).littleZooEnterVisitorSpawnLevel;

        coinVal = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin);//获取玩家现有金币
        ticketsLevelConsumeCoins = LittleZooModule.GetUpGradeConsumption(nameID, littleZooTicketsLevel + 1);//动物栏下一级需要的金钱
        visitorSeatLevelConsumeCoins = LittleZooModule.GetUpGradeVisitorLocationLevelConsumption(nameID, littleZooVisitorSeatLevel + 1);//动物栏下一级需要的金钱
        EnterVisitorSpawnLevelConsumeCoins = LittleZooModule.GetUpGradeEnterVisitorSpawnLevelConsumption(nameID, littleZooEnterVisitorSpawnLevel + 1);//动物栏下一级需要的金钱

        animalConsumeCoins = LittleZooModule.GetAnimalUpLevelPriceFormula(10101);//动物下一级需要的金钱
        //maxGrade = PlayerDataModule.PresentLevelPhases(zooLevel);//获取动物栏等级段最大等级

        int idx = PlayerDataModule.FindLevelRangIndex(buildUpCell.lvshage, littleZooTicketsLevel);
        maxGrade = buildUpCell.lvshage[idx];
        oldMaxGrade = buildUpCell.lvshage[idx-1];

        TicketsMaxGrade = Config.buildupConfig.getInstace().getCell(nameID).lvmax;
        VisitorSeatMaxGrade = Config.buildupConfig.getInstace().getCell(nameID).watchmaxlv;
        EnterVisitorSpawnMaxGrade = Config.buildupConfig.getInstace().getCell(nameID).itemmaxlv;

        starLevelReached = PlayerDataModule.FindLevelRangIndex01(Config.buildupConfig.getInstace().getCell(nameID).lvshage, littleZooTicketsLevel);
        if (littleZooTicketsLevel >= TicketsMaxGrade)
        {
            starLevelReached = PlayerDataModule.FindLevelRangIndex01(Config.buildupConfig.getInstace().getCell(nameID).lvshage, littleZooTicketsLevel);
        }

        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            zooCultivateButton.enabled = false;
            zooKindButton.enabled = false;
        }
        else
        {
            zooCultivateButton.enabled = true;
            zooKindButton.enabled = true;
        }
        

        //刷新数据   修改正确的   animalAndID
        var array = buildUpCell.animalid;
        string[] keys = new string[array.Length];
        
        animalButton.Clear();
        animalButton = new List<string>(keys);

        animalCellID.Clear();
        animalCellID = array.OfType<int>().ToList();
    }


    private void InitAnimalData()
    {
        int childCount = animalGroup.transform.childCount;
        animalTransformData.Clear();
        for (int i = 0; i < childCount; i++)
        {
            int num = buildUpCell.animalid[i];
            animalTransformData.Add(num, animalGroup.transform.GetChild(i));
        }
    }

    /// <summary>
    /// 内部组件的查找赋值
    /// </summary>
    private void RegistAllCompent()
    {
        zooCultivateButton = RegistBtnAndClick("UIPage/ZooCultivateButton", OnClickSetUIButton);
        zooKindButton = RegistBtnAndClick("UIPage/ZooKindButton", OnClickSetUIButton);

        setButtonsDic = new Dictionary<int, Button>();
        setButtonsDic.Add(1, zooCultivateButton);
        setButtonsDic.Add(10, zooKindButton);
        zooCultivateUI = GameObject.Find("UIPage/UiBg2/UIBuild_Animalup");
        zooKindUI = GameObject.Find("UIPage/UiBg2/UIBuild_LvUp");
        animalGroup = GameObject.Find("UIPage/UiBg2/UIBuild_Animalup/ScorllView/AnimalGroup");
        titleText = RegistCompent<Text>("UIPage/TitleGroup/TitleText");
        lvText = RegistCompent<Text>("UIPage/TitleGroup/LvText");
        tipsText = RegistCompent<Text>("UIPage/TitleGroup/TipsText");
        tipsTextAnimalup = RegistCompent<Text>("UIPage/UiBg2/TipsTextAnimalup");
        //GetTransPrefabText(tipsTextAnimalup);
        //lefeButton = RegistBtnAndClick("UIPage/TitleGroup/ArrowButtonLeft", SwitchLittleZooSend);
        //reghtButton = RegistBtnAndClick("UIPage/TitleGroup/ArrowButtonRight", SwitchLittleZooSend);
        animalShow = RegistCompent<Image>("UIPage/AnimalShow");
        hideUIButton = RegistBtnAndClick("UIPage/AnimalShow/Image", HideButtonUI);
    }
    private void RegistZooKindCompent()
    {
        
        /*查找控件*/
        LVUpSlider = RegistCompent<Slider>("UIPage/UiBg2/LvUpSchedule/Schedule/Slider2");
        lVUpSliderText = RegistCompent<Text>("UIPage/UiBg2/LvUpSchedule/Schedule/Text_2");
        LVUpSlider_Image = RegistCompent<Image>("UIPage/UiBg2/LvUpSchedule/Schedule/IconBg/Icon");
        LVUpSlider_Text = RegistCompent<Text>("UIPage/UiBg2/LvUpSchedule/Schedule/IconBg/Num");
        scoreNumTest = RegistCompent<Text>("UIPage/UiBg2/ScoreGroup/ScoreNum");
        //iconSlider = RegistCompent<Image>("UIPage/UiBg2/LvUpSchedule/Schedule/IconBg");
        //iconSlider.gameObject.SetActive(false);
        ticketsText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/Text_1");      //收益text
        //GetTransPrefabText(ticketsText);
        tickets_Text2 = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/TextAll/Text_2");
        tickets_Text3 = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/TextAll/Text_3");
        tickets_LvText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/Level/LvText");
        tickets_Button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/Button", OnClickUpGrade_Tickets);
        tickets_Button.gameObject.AddComponent<RepeatButton>();
        tickets_Button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPress_Tickets);//按下。频繁的调用
        tickets_Button.GetComponent<RepeatButton>().onRelease.AddListener(OnRelease_Tickets);//抬起，调用一次
        tickets_Button_NeedGoldText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/Button/NeedGoldNum");
        tickets_Button_buttonLvUpText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/Button/ButtonLvUpText");
        tickets_EffectNode = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/effectNode");

        visitorSeatText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/Text_1");        //数量Text
        //GetTransPrefabText(visitorSeatText);
        visitorSeat_Text2 = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/TextAll/Text_2");
        visitorSeat_Text3 = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/TextAll/Text_3");
        visitorSeat_LvText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/Level/LvText");
        visitorSeat_Button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/Button", OnClickUpGrade_VisitorSeat);
        visitorSeat_Button.gameObject.AddComponent<RepeatButton>();
        visitorSeat_Button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPress_VisitorSeat);//按下。频繁的调用
        visitorSeat_Button.GetComponent<RepeatButton>().onRelease.AddListener(OnRelease_VisitorSeat);//抬起，调用一次
        visitorSeat_Button_NeedGoldText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/Button/NeedGoldNum");
        visitorSeat_Button_buttonLvUpText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/Button/ButtonLvUpText");
        visitorSeat_EffectNode = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/effectNode");


        visitorSpawnText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/Text_1");       //冷却时间
        //GetTransPrefabText(visitorSpawnText);
        visitorSpawn_Text2 = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/TextAll/Text_2");
        visitorSpawn_Text3 = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/TextAll/Text_3");
        visitorSpawn_LvText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/Level/LvText");
        visitorSpawn_Button = RegistBtnAndClick("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/Button", OnClickUpGrade_VisitorSpawn);
        visitorSpawn_Button.gameObject.AddComponent<RepeatButton>();
        visitorSpawn_Button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPress_VisitorSpawn);//按下。频繁的调用
        visitorSpawn_Button.GetComponent<RepeatButton>().onRelease.AddListener(OnRelease_VisitorSpawn);//抬起，调用一次
        visitorSpawn_Button_NeedGoldText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/Button/NeedGoldNum");
        visitorSpawn_Button_buttonLvUpText = RegistCompent<Text>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/Button/ButtonLvUpText");
        visitorSpawn_EffectNode = RegistCompent<Transform>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/effectNode");



        buttonIcon_1 = RegistCompent<Image>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_1/Button/GoldIcon");
        buttonIcon_2 = RegistCompent<Image>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_2/Button/GoldIcon");
        buttonIcon_3 = RegistCompent<Image>("UIPage/UiBg2/UIBuild_LvUp/ParameterGroup/Parameter/Parameter_3/Button/GoldIcon");

        //新手引导手势组件

    }

    /// <summary>
    /// 01 动物升级—单点
    /// </summary>
    void BuyPlayerAnimal01(string str)
    {
        int animalID = animalCellID[0];

        if (!JudgePressAnimal(animalID))
        {
            LogWarp.LogError("不能升级");
            return;
        }

        //LogWarp.LogError("测试：  按钮点击  动物栏="+nameID+"  动物id="+animalID );


        if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.AlreadyOpen)
        {
            SendSetAnimalLevelMessageManager(animalID);  //发送升级消息

        }
    }
    /// <summary>
    /// 02 动物升级—单点
    /// </summary>
    void BuyPlayerAnimal02(string str)
    {
        int animalID = animalCellID[1];

        if (!JudgePressAnimal(animalID))
        {
            LogWarp.LogError("不能升级");
            return;
        }

        //LogWarp.LogError("测试：  按钮点击  动物栏="+nameID+"  动物id="+zooID );


        if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.AlreadyOpen)
        {
            SendSetAnimalLevelMessageManager(animalID);  //发送升级消息

        }
    }
    /// <summary>
    ///  03 动物升级—单点
    /// </summary>
    void BuyPlayerAnimal03(string str)
    {
        int animalID = animalCellID[2];

        if (!JudgePressAnimal(animalID))
        {
            LogWarp.LogError("不能升级");
            return;
        }

        //LogWarp.LogError("测试：  按钮点击  动物栏="+nameID+"  动物id="+zooID );


        if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.AlreadyOpen)
        {
            SendSetAnimalLevelMessageManager(animalID);  //发送升级消息

        }
    }
    /// <summary>
    ///  04 动物升级—单点
    /// </summary>
    void BuyPlayerAnimal04(string str)
    {
        int animalID = animalCellID[3];

        if (!JudgePressAnimal(animalID))
        {
            LogWarp.LogError("不能升级");
            return;
        }

        //LogWarp.LogError("测试：  按钮点击  动物栏="+nameID+"  动物id="+zooID );


        if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.AlreadyOpen)
        {
            SendSetAnimalLevelMessageManager(animalID);  //发送升级消息

        }
    }
    /// <summary>
    ///  05 动物升级—单点
    /// </summary>
    void BuyPlayerAnimal05(string str)
    {
        int animalID = animalCellID[4];

        if (!JudgePressAnimal(animalID))
        {
            LogWarp.LogError("不能升级");
            return;
        }

        //LogWarp.LogError("测试：  按钮点击  动物栏="+nameID+"  动物id="+zooID );
        if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.AlreadyOpen)
        {
            SendSetAnimalLevelMessageManager(animalID);  //发送升级消息

        }
    }

    /// <summary>
    /// 动物升级成功返回消息
    /// </summary>
    /// <param name="obj"></param>
    private void GetAchievementSetObject(Message obj)
    {
        var _msg = obj as GetAddNewAnimalData;
        isGetCoin = true;
        this.InitData();
        this.InitCompent();
		//LogWarp.LogError("测试： 动物购买成功返回消息    GetAchievementSetObject  ");
		GameEventManager.SendEvent("Buy animal");
	}

    /// <summary>
    /// 控件显示赋值   并切换按钮图片
    /// </summary>
    private void InitCompent()
    {
        titleText.text = GetL10NString(buildUpCell.buildname);
        lvText.text = string.Format(GetL10NString("Ui_Text_2") , GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(nameID).littleZooTicketsLevel);
        string iconPath = buildUpCell.icon;
        animalShow.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
        scoreNumTest.text = starLevelReached + "/" + Config.buildupConfig.getInstace().getCell(nameID).starsum;

        //切换子UI显示页面   
        switch (switchButton)
        {
            case "ZooCultivateButton":

                zooCultivateUI.SetActive(false);
                tipsTextAnimalup.gameObject.SetActive(false);
                tipsText.gameObject.SetActive(true);
                zooKindUI.SetActive(true);
                this.InitCompentZoo();
                break;
            case "ZooKindButton":
                zooCultivateUI.SetActive(true);
                tipsTextAnimalup.gameObject.SetActive(true);
                tipsText.gameObject.SetActive(false);
                zooKindUI.SetActive(false);
                this.InitCompentCultivate();
                break;
            default:
                break;
        }
    }
    private void InitCompentZoo()
    {
   
        //lVUpSlider1.value = AddPercentage(zooLevel+upGradeNumber, levelMax);
        
        if (maxGrade >= TicketsMaxGrade)
        {
            maxGrade = TicketsMaxGrade;
        }
      
        LVUpSlider.value = AddPercentage(littleZooTicketsLevel- oldMaxGrade, maxGrade- oldMaxGrade);
        lVUpSliderText.text = littleZooTicketsLevel.ToString() + "/" + maxGrade.ToString();  //最大等级上限
        tipsText.text = GetL10NString("Ui_Text_8");
        GradeSliderAwardImage();
        //暂时屏蔽
        //if (littleZooTicketsLevel>= buildUpCell.lvanimal[buildUpCell.lvanimal.Length-1])
        //{
        //    iconSlider.gameObject.SetActive(false);
        //}
        //else
        //{
        //    iconSlider.gameObject.SetActive(true);
        //    int idx = LittleZooModule.FindLevelRangIndex(buildUpCell.lvanimal, littleZooTicketsLevel);
        //    if (idx< buildUpCell.animalid.Length)
        //    {
        //        string iconPath = Config.animalupConfig.getInstace().getCell(buildUpCell.animalid[idx]).icon;
        //        iconSlider.transform.Find("Icon").GetComponent<Image>().sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
        //    }
        //}

        tickets_Text2.text = MinerBigInt.ToDisplay(LittleZooModule.GetLittleZooPrice(nameID, littleZooTicketsLevel).ToString()); ;  //价格标签2
        tickets_Text3.text = "+" + MinerBigInt.ToDisplay(LittleZooModule.GetLittleZooPrice(nameID, littleZooTicketsLevel, 1).ToString());  //价格标签3
        tickets_LvText.text =  littleZooTicketsLevel.ToString();

        tickets_Button_NeedGoldText.text = MinerBigInt.ToDisplay(ticketsLevelConsumeCoins.ToString());
        tickets_Button_buttonLvUpText.text = GetL10NString("Ui_Text_7");

        visitorSeat_Text2.text = LittleZooModule.OpenVisitPosNumber(nameID, littleZooVisitorSeatLevel).ToString(); //数量标签2
        visitorSeat_Text3.text = "+" + LittleZooModule.OpenVisitPosNumber(nameID, littleZooVisitorSeatLevel,1).ToString(); //数量标签3
        visitorSeat_Button_NeedGoldText.text = MinerBigInt.ToDisplay(visitorSeatLevelConsumeCoins.ToString());
        visitorSeat_LvText.text =  littleZooVisitorSeatLevel.ToString();
        visitorSeat_Button_buttonLvUpText.text = GetL10NString("Ui_Text_7");

        visitorSpawn_Text2.text = LittleZooModule.GetVisitDurationMS(nameID, littleZooEnterVisitorSpawnLevel).ToString("f1")  + GetL10NString("Ui_Text_67"); //流量标签2
        visitorSpawn_Text3.text = "+" + (LittleZooModule.GetVisitDurationMS(nameID, littleZooEnterVisitorSpawnLevel, 1)).ToString("f1"); //流量标签3
        visitorSpawn_Button_NeedGoldText.text = MinerBigInt.ToDisplay(EnterVisitorSpawnLevelConsumeCoins.ToString());
        visitorSpawn_LvText.text =  littleZooEnterVisitorSpawnLevel.ToString();
        visitorSpawn_Button_buttonLvUpText.text = GetL10NString("Ui_Text_7");

        buttonIcon_1.gameObject.SetActive(true);
        buttonIcon_2.gameObject.SetActive(true);
        buttonIcon_3.gameObject.SetActive(true);

        if (!SetGradeBool_Tickets())
        {
            SwitchButtonUnclickable(tickets_Button, false);
        }
        else
        {
            SwitchButtonUnclickable(tickets_Button, true);
        }

        if (!SetGradeBool_VisitorSeat())
        {
            SwitchButtonUnclickable(visitorSeat_Button, false);
        }
        else
        {
            SwitchButtonUnclickable(visitorSeat_Button, true);
        }
        if (!SetGradeBool_VisitorSpawn())
        {
            SwitchButtonUnclickable(visitorSpawn_Button, false);
        }
        else
        {
            SwitchButtonUnclickable(visitorSpawn_Button, true);
        }

        if (littleZooTicketsLevel >= TicketsMaxGrade)
        {
            setGradeBool = false;
            tickets_Button_NeedGoldText.text = GetL10NString("Ui_Text_47");       //升级模式需要的金钱
            tickets_Button_buttonLvUpText.text = GetL10NString("Ui_Text_46");    //升级模式要升的级数
            lvText.text = Config.buildupConfig.getInstace().getCell(nameID).lvmax.ToString();            //等级text
            tickets_Text3.text = GetL10NString("Ui_Text_47");  //价格变化标签
            buttonIcon_1.gameObject.SetActive(false);
            SwitchButtonUnclickable(tickets_Button, false);

        }
        if (littleZooVisitorSeatLevel >= VisitorSeatMaxGrade)
        {
            setGradeBool = false;
            visitorSeat_Button_NeedGoldText.text = GetL10NString("Ui_Text_47");       //升级模式需要的金钱
            visitorSeat_Button_buttonLvUpText.text = GetL10NString("Ui_Text_46");    //升级模式要升的级数
            visitorSeat_Text3.text = GetL10NString("Ui_Text_47"); //数量变化标签
            SwitchButtonUnclickable(visitorSeat_Button, false);
            buttonIcon_2.gameObject.SetActive(false);
        }
        if (littleZooEnterVisitorSpawnLevel >= EnterVisitorSpawnMaxGrade)
        {
            setGradeBool = false;
            visitorSpawn_Button_NeedGoldText.text = GetL10NString("Ui_Text_47");       //升级模式需要的金钱
            visitorSpawn_Button_buttonLvUpText.text = GetL10NString("Ui_Text_46");    //升级模式要升的级数
            visitorSpawn_Text3.text = GetL10NString("Ui_Text_47"); //速度变化标签
            buttonIcon_3.gameObject.SetActive(false);
            SwitchButtonUnclickable(visitorSpawn_Button, false);
        }

        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {   
            SwitchButtonUnclickable(visitorSeat_Button, false);
            SwitchButtonUnclickable(visitorSpawn_Button, false);
        }
        
    }
    private void InitCompentCultivate()
    {
        bool isShowAnimalCultivate = GlobalDataManager.GetInstance().playerData.playerZoo.isShowAnimalCultivate;
        
        tipsText.text = GetL10NString("Ui_Text_61");
        int childCount = animalGroup.transform.childCount;
        animalCellList.Add(animal_1);
        for (int i = 0; i < childCount; i++)
        {
            int animalID = buildUpCell.animalid[i];
            animalCellID[i] = animalID;
            //Transform[] allChild = animalCellList[i].GetComponentsInChildren<Transform>();
            //foreach (Transform child in allChild)
            //{
            //    child.gameObject.SetActive(true);
            //}
            Text nameText = animalCellList[i].transform.Find("NameText").GetComponent<Text>();
            nameText.text = GetL10NString(Config.animalupConfig.getInstace().getCell(animalID).nametranslate);

            Image iconImage = animalCellList[i].transform.Find("Icon").GetComponent<Image>();
            string iconPath = Config.animalupConfig.getInstace().getCell(animalID).icon;
            iconImage.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
            //动物等级
            int zooLevel = 0;
            Text NameText = animalCellList[i].transform.Find("LvBg/LvNum").GetComponent<Text>();
            zooLevel = playerAnimal.getPlayerAnimalCell(animalID).animalLevel;
            NameText.text = zooLevel.ToString();

            //提示文本
            Text AnimalNumber = animalCellList[i].transform.Find("AnimalNumber").GetComponent<Text>();
            AnimalNumber.text = LittleZooModule.GetAnimalsBuff(zooLevel).ToString()+"%";
            //购买、解锁按钮
            Button buyButton = animalCellList[i].transform.Find("BuyButton").GetComponent<Button>();
            
            ////小锁标识
            Image lockIcon = animalCellList[i].transform.Find("LockIcon").GetComponent<Image>();
            ////购买价格
            if (buyButton == null)
            {
                string e = string.Format("紧急   注意：   buyButton  为null");
                throw new System.Exception(e);
            }
            Text buyText = buyButton.transform.Find("NumText").GetComponent<Text>();
            Text tipsText = animalTransformData[animalID].transform.Find("TipsText").GetComponent<Text>();
            //解锁提示文本
            Text openText = buyButton.transform.Find("OpenText").GetComponent<Text>();
            Image goldImage = buyButton.transform.Find("Gold").GetComponent<Image>();


            if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.NoneOwn)
            {

                AnimalNumber.gameObject.SetActive(true);
                //Ui_Text_62
                string str = GetL10NString("Ui_Text_62");
                tipsText.text = string.Format(str, buildUpCell.lvanimal[i], titleText.text);
                tipsText.gameObject.SetActive(true);
                buyButton.gameObject.SetActive(false);
                lockIcon.gameObject.SetActive(true);
                
            }
            else if (playerAnimal.getPlayerAnimalCell(animalID).animalState == AnimalState.AlreadyOpen)
            {
                AnimalNumber.gameObject.SetActive(true);
                lockIcon.gameObject.SetActive(false);
                openText.gameObject.SetActive(false);
                buyText.text = MinerBigInt.ToDisplay(LittleZooModule.GetAnimalUpLevelPriceFormula(animalID));


                LogWarp.LogErrorFormat("测试  动物购买价格文本{0}   实际{1}",buyText.text,LittleZooModule.GetAnimalUpLevelPriceFormula(animalID));
                tipsText.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(true);
                buyText.gameObject.SetActive(true);
                goldImage.gameObject.SetActive(true);

                if (LittleZooModule.GetAnimalUpLevelPriceFormula(animalID) <= coinVal && zooLevel < animalLvUpLimit)
                {
                    SwitchButtonUnclickable(buyButton, true);
                }
                else if(LittleZooModule.GetAnimalUpLevelPriceFormula(animalID) > coinVal)
                {
                    //buyText.color = Color.white;
                    SwitchButtonUnclickable(buyButton, false);
                }
                if (zooLevel == animalLvUpLimit)
                {
                    openText.text = GetL10NString("Ui_Text_46");
                    openText.gameObject.SetActive(true);
                    buyText.gameObject.SetActive(false);
                    goldImage.gameObject.SetActive(false);
                    AnimalNumber.gameObject.SetActive(true);
                    buyButton.transform.Find("ButtonTipsText").GetComponent<Text>().gameObject.SetActive(false);
                    SwitchButtonUnclickable(buyButton, false);
                }
                //else
                //{
                //    buyButton.transform.Find("ButtonTipsText").GetComponent<Text>().gameObject.SetActive(true);
                //}
                //关于动物培养的显隐
                if (isShowAnimalCultivate)
                {
                    buyButton.gameObject.SetActive(true);
                }
                else
                {
                    buyButton.gameObject.SetActive(false);
                }
            }
        }


    }

    /// <summary>
    /// 返回可以切换动物栏ID  的下标
    /// </summary>
    /// <param name="isID">需要变动下标位数</param>
    /// <returns></returns>
    int VerdictSwitchID(int isID)
    {

        int number = GlobalDataManager.GetInstance().playerData.GetLittleZooIDIndexOfDataIdx(nameID);

        //当前动物栏ID的下表

        int valId = number + isID;//下标加上变换值
        if (valId < 0)
        {
            valId = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas.Count - 1;
        }
        else if (valId > (GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas.Count - 1))
        {
            valId = 0;
        }
        //判断动物栏等级是否为0
        //根据下标获取动物栏ID  判断动物栏的ID是否为0   若为0   累加变换值
        int zooID = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas[valId].littleZooID;
        if (GlobalDataManager.GetInstance().playerData.GetLittleZooModuleData(zooID).littleZooTicketsLevel < 1)
        {
            valId = VerdictSwitchID(++isID);
        }
        return valId;
    }
    /// <summary>
    /// 切换动物栏ID的方法
    /// </summary>
    /// <param name="number">1：增加 0：减小</param>
    void SwitchLittleZooSend(string name)
    {
        int number = 0;
        if (name == lefeButton.name)
        {
            number = -1;
        }
        else if (name == reghtButton.name)
        {
            number = 1;
        }
        //获取动物栏不为0的下标
        int valId = VerdictSwitchID(number);
        //LogWarp.LogError("   切换动物栏 原来是" + GlobalDataManager.GetInstance().playerData.playerZoo.littleZooIDs.IndexOf(nameID));
        int newID = GlobalDataManager.GetInstance().playerData.playerZoo.littleZooModuleDatas[valId].littleZooID;
        m_data = newID;
        nameID = newID;
        buildUpCell = Config.buildupConfig.getInstace().getCell(m_data.ToString());

        

        this.InitData();
        InitAnimalData();

        this.InitCompent();
        //this.InitCompentCultivate();
        //this.InitCompentZoo();
        //LogWarp.Log("   切换动物栏 现在是" + GlobalDataManager.GetInstance().playerData.playerZoo.littleZooIDs.IndexOf(nameID));
        UnityEngine.Vector3 pos = LittleZooPosManager.GetInstance().GetPos(newID);
        ZooCamera.GetInstance().PointAtScreenUpCenter(pos);

    }
    /// <summary>
    /// 设置切换UI界面的按钮点击事件  根据按钮切换UI显隐
    /// </summary>
    /// <param name="button"></param>
    private void OnClickSetUIButton(string name)
    {
        foreach (var item in setButtonsDic.Keys)
        {
            setButtonsDic.TryGetValue(item, out Button value);
            Transform text_Butoon = value.transform.Find("Text");
            Transform image_button01 = value.transform.Find("ButtonBg_1");
            Transform image_button02 = value.transform.Find("ButtonBg_2");
            Transform image_button03 = value.transform.Find("Image");
            if (value.name == name)
            {
                text_Butoon.gameObject.SetActive(true);
                image_button01.gameObject.SetActive(true);
                image_button02.gameObject.SetActive(false);
                image_button03.gameObject.SetActive(true);

                switchButton = name;
            }
            else//切换非选中状态的图片
            {
                text_Butoon.gameObject.SetActive(false);
                image_button01.gameObject.SetActive(true);
                image_button02.gameObject.SetActive(true);
                image_button03.gameObject.SetActive(true);

            }
        }


        //LogWarp.LogError(" 测试    OnClickSetUIButton  ");

        //刷新显示Ui界面
        this.InitCompent();
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
        this.InitData();

        InitAnimalData();
       

        //控件显示赋值
        OnClickSetUIButton("ZooCultivateButton");
        //监听动物栏升级广播
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLittleZooTicketsLevelPlayerData, this.OnGetBroadcastLittleZooTicketsLevelPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData, this.OnGetBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastLittleZooVisitorLocationLevelOfPlayerData, this.OnGetBroadcastLittleZooVisitorLocationLevelOfPlayerData);

        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnGetCoinOfPlayerData);//接受金钱变动的信息
        MessageManager.GetInstance().Regist((int)GameMessageDefine.GetAnimalLevel, this.GetAchievementSetObject);

        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);


        /*  若是新手引导阶段，进入特殊处理方法  */
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            DelayedOperationNewbieGuideStage();
        }
    }

    private void OnOpenOfflineUIPage(Message obj)
    {
        HideButtonUI("");
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
        if (uIGuidePage.procedure == 20)
        {
            //停车场停车位按钮处显示小手点击动画
            effectNode = tickets_Button.transform.Find("effectNode");
            Transform trans = null;
            trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().GuideUiClickEffect).transform;
            trans.SetParent(effectNode, false);
            SimpleParticle particlePlayer = new SimpleParticle();
            particlePlayer.Init(effectNode.gameObject);
            particlePlayer.Play();
        }
        
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
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        isPause = false;
        OnRelease_Tickets();
        OnRelease_VisitorSeat();
        OnRelease_VisitorSpawn();
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLittleZooTicketsLevelPlayerData, this.OnGetBroadcastLittleZooTicketsLevelPlayerData);
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData, this.OnGetBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData);
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastLittleZooVisitorLocationLevelOfPlayerData, this.OnGetBroadcastLittleZooVisitorLocationLevelOfPlayerData);

        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnGetCoinOfPlayerData);//接受金钱变动的信息
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.GetAnimalLevel, this.GetAchievementSetObject);
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);

        MessageString.Send((int)GameMessageDefine.UIMessage_OnClickButShowPart, "UIMainPage");
        UIInteractive.GetInstance().iPage = null;

        DestroyEffectChild();
        base.Hide();

    }

    /// <summary>
    /// 监听玩家数据方法
    /// </summary>
    /// <param name="msg"></param>
    public void OnGetBroadcastLittleZooTicketsLevelPlayerData(Message msg)
    {
        //Logger.LogWarp.LogError("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        //刷新UI显示
        coinVal = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin);
        SwitchButtonUnclickable(tickets_Button, true);
        InitData();
        this.InitCompent();
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
			GameEventManager.SendEvent("TAEvent_process(littleZooTicketsLevel 21");
		}

        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true && littleZooTicketsLevel == 5)
        {   /*新手阶段     显示新手引导UI    步骤应该是  22  */
            DestroyEffectChild();
            SwitchButtonUnclickable(visitorSeat_Button, true);
            SwitchButtonUnclickable(visitorSpawn_Button, true);
            SwitchButtonUnclickable(zooCultivateButton, true);
            SwitchButtonUnclickable(zooKindButton, true);

            this.Hide();
            PageMgr.ShowPage<UIGuidePage>();

        }
        UIButtonEffectSimplePlayer(tickets_EffectNode);
        GameEventManager.SendEvent("littleZooTicketsLevel");
        isGetCoin = true;
    }
    private void OnGetBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData(Message obj)
    {
        //刷新UI显示
        coinVal = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin);
        SwitchButtonUnclickable(visitorSpawn_Button, true);

        InitData();
        this.InitCompent();
        UIButtonEffectSimplePlayer(visitorSpawn_EffectNode);
        var cell = Config.monitorConfig.getInstace().getCell(15);
		GameEventManager.SendEvent("OnGetBroadcastLittleZooEnterVisitorSpawnLevelOfPlayerData");
		isGetCoin = true;
    }

    private void OnGetBroadcastLittleZooVisitorLocationLevelOfPlayerData(Message obj)
    {
        //刷新UI显示
        coinVal = BigInteger.Parse(GlobalDataManager.GetInstance().playerData.playerZoo.coin);
        SwitchButtonUnclickable(visitorSeat_Button, true);
        InitData();
        this.InitCompent();
        UIButtonEffectSimplePlayer(visitorSeat_EffectNode);
        GameEventManager.SendEvent("AppsFlyerEnum.build_ID_attribute_1");
        isGetCoin = true;
    }
    /// <summary>
    /// 监听玩家coin金钱发生改变，是否需要重新计算升级规模
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetCoinOfPlayerData(Message obj)
    {   //旧计算金钱不够，则开始新的计算
        this.InitData();
        this.InitCompent();
    }

    /// <summary>
    /// 点击升级按钮事件
    /// </summary>
    private void OnClickUpGrade_Tickets(string name)
    {
        //isLongPress为true则是长按状态，单点关闭  返回
        if (!JudgePressButton()  )
        {
            LogWarp.LogError("不能升级");
            LogWarp.LogErrorFormat("测试：  不能升级   isGetCoin={0}     SetGradeBool_Tickets={1}", isGetCoin, SetGradeBool_Tickets());

            return;
        }
        if (isLongPress)
        {
            LogWarp.LogError("属于长按状态");
            return;
        }
        /*  新手引导  */
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            if (littleZooTicketsLevel >= 5)
            {
                effectNode.gameObject.SetActive(false);
                return;
            }
        }
        SendSetZooTicketsLevelMessageManager();
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);
          
    }
    /// <summary>
    /// 点击升级按钮事件
    /// </summary>
    private void OnClickUpGrade_VisitorSeat(string name)
    {
        //isLongPress为true则是长按状态，单点关闭  返回
        if (!JudgePressButton01())
        {
            LogWarp.LogError("不能升级JudgePressButton01");
            return;
        }
        if (isLongPress)
        {
            LogWarp.LogError("属于长按状态");
            return;
        }
      
        SendSetZooVisitorSeatLevelMessageManager();

        //lvUpButton.enabled = false;
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);

    }/// <summary>
     /// 点击升级按钮事件
     /// </summary>
    private void OnClickUpGrade_VisitorSpawn(string name)
    {
        //isLongPress为true则是长按状态，单点关闭  返回
        if (!JudgePressButton02())
        {
            LogWarp.LogError("不能升级JudgePressButton02");
            return;
        }
        if (isLongPress)
        {
            LogWarp.LogError("属于长按状态");
            return;
        }
        SendSetZooVisitorSpawnLevelMessageManager();

        //lvUpButton.enabled = false;
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);

    }
    /// <summary>
    /// 动物栏升级消息
    /// </summary>
    private void SendSetZooTicketsLevelMessageManager()
    {
        if (littleZooTicketsLevel >= TicketsMaxGrade)
        {
            SwitchButtonUnclickable(tickets_Button, false);
            return;
        }
        //发送消息       SetValueOfPlayerData  消息体    SetParkingLevelOfPlayerData 设置动物栏升级消息ID，UpGradeNumber  升多少级
        SetDetailValueOfPlayerData.Send((int)GameMessageDefine.SetLittleZooTicketsLevelPlayerData,
                nameID, 1, 0);
    }
    /// <summary>
    /// 动物栏升级消息
    /// </summary>
    private void SendSetZooVisitorSeatLevelMessageManager()
    {
        if (littleZooVisitorSeatLevel >= VisitorSeatMaxGrade)
        {
            SwitchButtonUnclickable(visitorSeat_Button, false);
            return;
        }
        //发送消息       SetValueOfPlayerData  消息体    SetParkingLevelOfPlayerData 设置动物栏升级消息ID，UpGradeNumber  升多少级
        SetDetailValueOfPlayerData.Send((int)GameMessageDefine.SetLittleZooVisitorLocationLevelOfPlayerData,
                nameID, 1, 0);
    }
    /// <summary>
    /// 动物栏升级消息
    /// </summary>
    private void SendSetZooVisitorSpawnLevelMessageManager()
    {
        if (littleZooEnterVisitorSpawnLevel >= EnterVisitorSpawnMaxGrade)
        {
            SwitchButtonUnclickable(visitorSpawn_Button, false);
            return;
        }
        //发送消息       SetValueOfPlayerData  消息体    SetParkingLevelOfPlayerData 设置动物栏升级消息ID，UpGradeNumber  升多少级
        SetDetailValueOfPlayerData.Send((int)GameMessageDefine.SetLittleZooEnterVisitorSpawnLevelOfPlayerData,
                nameID, 1, 0);
    }

    /// <summary>
    /// 动物升级消息
    /// </summary>
    private void SendSetAnimalLevelMessageManager(int animalID)
    {
        if (playerAnimal.getPlayerAnimalCell(animalID).animalLevel == animalLvUpLimit)
        {
            return;
        }
        SetAchievementObjectData.Send((int)GameMessageDefine.SetAchievementObject,
                1, animalID, 1, LittleZooModule.GetAnimalUpLevelPriceFormula(animalID), 0, nameID);
    }

    /// <summary>
    /// 切换选中状态的图片
    /// </summary>
    /// <param name="button"></param>
    void SetSwitchCheckButtonImage(Button button)
    {
        Transform text_Butoon = button.transform.Find("Text");
        Transform image_button01 = button.transform.Find("ButtonBg_1");
        Transform image_button02 = button.transform.Find("ButtonBg_2");
        Transform image_button03 = button.transform.Find("Image");

        text_Butoon.gameObject.SetActive(true);
        image_button01.gameObject.SetActive(false);
        image_button02.gameObject.SetActive(true);
        image_button03.gameObject.SetActive(false);
    }
    /// <summary>
    /// 切换非选中状态的图片
    /// </summary>
    /// <param name="button"></param>
    void SetSwitchOffCheckButtonImage(Button button)
    {
        Transform text_Butoon = button.transform.Find("Text");
        Transform image_button01 = button.transform.Find("ButtonBg_1");
        Transform image_button02 = button.transform.Find("ButtonBg_2");
        Transform image_button03 = button.transform.Find("Image");

        text_Butoon.gameObject.SetActive(false);
        image_button01.gameObject.SetActive(true);
        image_button02.gameObject.SetActive(false);
        image_button03.gameObject.SetActive(true);

    }

    /// <summary>
    /// 判断利润是否可以升级（钱够/等级不超过最大值）
    /// </summary>
    /// <returns></returns>
    private bool SetGradeBool_Tickets()
    {
        if (ticketsLevelConsumeCoins <= coinVal && littleZooTicketsLevel <= TicketsMaxGrade)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断 观光位数量是否可以升级（钱够/等级不超过最大值）
    /// </summary>
    /// <returns></returns>
    private bool SetGradeBool_VisitorSeat()
    {
        if (visitorSeatLevelConsumeCoins <= coinVal && littleZooVisitorSeatLevel <= VisitorSeatMaxGrade)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断观光游客的流量是否可以升级（钱够/等级不超过最大值）
    /// </summary>
    /// <returns></returns>
    private bool SetGradeBool_VisitorSpawn()
    {
        if (EnterVisitorSpawnLevelConsumeCoins <= coinVal && littleZooEnterVisitorSpawnLevel <= EnterVisitorSpawnMaxGrade)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断动物购买是否可以升级（钱够/等级不超过最大值）   等级判断没做
    /// </summary>
    /// <returns></returns>
    private bool SetBuyAnimalGradeBool(int animalID)
    {
        //if (animalConsumeCoins < coinVal && zooLevel <= zooMaxGrade)
        if (LittleZooModule.GetAnimalUpLevelPriceFormula(animalID) <= coinVal )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 动物栏是否可以升级
    /// </summary>
    /// <returns></returns>
    private bool JudgePressButton()
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin && SetGradeBool_Tickets())
        {
            return true;
        }
        //SwitchButtonUnclickable(tickets_Button, false);
        return false;
    }
    /// <summary>
    /// 动物栏观光点是否可以升级
    /// </summary>
    /// <returns></returns>
    private bool JudgePressButton01()
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin && SetGradeBool_VisitorSeat())
        {
            //SwitchButtonUnclickable(visitorSeat_Button, true);
            return true;
        }
        //SwitchButtonUnclickable(visitorSeat_Button, false);
        return false;
    }
    /// <summary>
     /// 动物栏观光流量是否可以升级
     /// </summary>
     /// <returns></returns>
    private bool JudgePressButton02()
    {
        LogWarp.LogErrorFormat("测试：    isGetCoin={0}   SetGradeBool_VisitorSpawn={1}", isGetCoin,SetGradeBool_VisitorSpawn());
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin && SetGradeBool_VisitorSpawn())
        {
            //SwitchButtonUnclickable(visitorSpawn_Button, true);
            return true;
        }
        //SwitchButtonUnclickable(visitorSpawn_Button, false);
        return false;
    }

    /// <summary>
    /// 动物购买是否可以升级
    /// </summary>
    /// <returns></returns>
    private bool JudgePressAnimal(int animalID)
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin && SetBuyAnimalGradeBool(animalID))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPress_Tickets()
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
        //int index = Array.IndexOf(buildUpCell.lvshage, littleZooTicketsLevel + 1);   //若是 数组中含有等级，说明动物栏等级段变化，关闭长按

        //if (index != -1)
        //{
        //    isLongPress = false;
        //    return;
        //}
        if (JudgePressButton())
        {
            SendSetZooTicketsLevelMessageManager();  //发送升级消息
            //isGetCoin = false;
        }
        
    }
    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPress_VisitorSeat()
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
        //int index = Array.IndexOf(buildUpCell.lvmodel, littleZooTicketsLevel + 1);   //若是 数组中含有等级，说明动物栏外观变化，关闭长按

        //if (index != -1)
        //{
        //    isLongPress = false;
        //    return;
        //}

        //LogWarp.LogError("测试：   JudgePressButton() = " + JudgePressButton() + "    index = " + index);
        if (JudgePressButton01())
        {
            SendSetZooVisitorSeatLevelMessageManager();  //发送升级消息
            //isGetCoin = false;
        }

    }
    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPress_VisitorSpawn()
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
        //int index = Array.IndexOf(buildUpCell.lvmodel, littleZooTicketsLevel + 1);   //若是 数组中含有等级，说明动物栏外观变化，关闭长按

        //if (index != -1)
        //{
        //    isLongPress = false;
        //    return;
        //}

        //LogWarp.LogError("测试：   JudgePressButton() = " + JudgePressButton() + "    index = " + index);
        if (JudgePressButton02())
        {
            SendSetZooVisitorSpawnLevelMessageManager();  //发送升级消息
            //isGetCoin = false;
        }
    }

    /// <summary>
    /// 离开长按按钮回调事件
    /// </summary>
    private void OnRelease_Tickets()
    {
        //LogWarp.LogError("测试             离开  按钮         ");
        isLongPress = false;
        tickets_Button.GetComponent<RepeatButton>().isPointerDown = false;
    }

    /// <summary>
    /// 01动物升级长按按钮回调事件
    /// </summary>
    protected void OnLongPressBuyAnimal01()
    {
        if (!RestrictLongPressTime())
        {
            return;
        }
        isLongPressBuyAnimal = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         ");
        if (isGetCoin)
        {
            SendSetAnimalLevelMessageManager(animalCellID[0]);  //发送升级消息
            isGetCoin = false;
        }
    }
    /// <summary>
    /// 02动物升级长按按钮回调事件
    /// </summary>
    protected void OnLongPressBuyAnimal02()
    {
        isLongPressBuyAnimal = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         ");
        if (isGetCoin)
        {
            SendSetAnimalLevelMessageManager(animalCellID[1]);  //发送升级消息
            isGetCoin = false;
        }
    }
    /// <summary>
     /// 03动物升级长按按钮回调事件
     /// </summary>
    protected void OnLongPressBuyAnimal03()
    {
        isLongPressBuyAnimal = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         ");
        if (isGetCoin)
        {
            SendSetAnimalLevelMessageManager(animalCellID[2]);  //发送升级消息
            isGetCoin = false;
        }
    }
    /// <summary>
    /// 04动物升级长按按钮回调事件
    /// </summary>
    protected void OnLongPressBuyAnimal04()
    {
        isLongPressBuyAnimal = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         ");
        if (isGetCoin)
        {
            SendSetAnimalLevelMessageManager(animalCellID[3]);  //发送升级消息
            isGetCoin = false;
        }
    }
    /// <summary>
    /// 05动物升级长按按钮回调事件
    /// </summary>
    protected void OnLongPressBuyAnimal05()
    {
        isLongPressBuyAnimal = true;//进入长按状态
        //LogWarp.LogError("测试             长按  按钮         ");
        if (isGetCoin)
        {
            SendSetAnimalLevelMessageManager(animalCellID[4]);  //发送升级消息
            isGetCoin = false;
        }
    }

    /// <summary>
    /// 动物购买离开长按按钮回调事件
    /// </summary>
    private void OnReleaseBuyAnimal()
    {
        //LogWarp.LogError("测试             离开  按钮         ");
        isLongPressBuyAnimal = false;
        isGetCoin = true;
    }
    /// <summary>
    /// 离开长按按钮回调事件
    /// </summary>
    private void OnRelease_VisitorSeat()
    {
        //LogWarp.LogError("测试             离开  按钮         ");
        isLongPress = false;
        visitorSeat_Button.GetComponent<RepeatButton>().isPointerDown = false;
        isGetCoin = true;
    }


    /// <summary>
    /// 离开长按按钮回调事件
    /// </summary>
    private void OnRelease_VisitorSpawn()
    {
        //LogWarp.LogError("测试             离开  按钮         ");
        isLongPress = false;
        visitorSpawn_Button.GetComponent<RepeatButton>().isPointerDown = false;
        isGetCoin = true;
    }


    /// <summary>
    /// 获取等级段对应的奖励信息
    /// </summary>
    /// <returns></returns>
    private void GradeSliderAwardImage()
    {
        var lvreward = buildUpCell.lvreward;
        int itemID;
        int idx = PlayerDataModule.FindLevelRangIndex(buildUpCell.lvshage, littleZooTicketsLevel);
        if (buildUpCell.lvrewardtype[idx]==2)
        {  //动物
            itemID = lvreward[idx];
            string path = Config.animalupConfig.getInstace().getCell(itemID).icon;
            LVUpSlider_Image.sprite = ResourceManager.LoadSpriteFromPrefab(path);
            LVUpSlider_Text.text = "";
        }
        else if (buildUpCell.lvrewardtype[idx] == 1)
        {
            itemID = lvreward[idx];
            Config.itemCell itemCell = Config.itemConfig.getInstace().getCell(itemID);
            LVUpSlider_Image.sprite = ResourceManager.LoadSpriteFromPrefab(itemCell.icon);
            LVUpSlider_Text.text = MinerBigInt.ToDisplay(itemCell.itemval);
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