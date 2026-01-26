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
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIEntryPage : UIPage
{
    BigInteger coinVal;
    int entryTicketsLevel;

    BigInteger consumeCoins;                            //记录要升级需要消费的钱币
    int maxGrade;//最大等级
    int oldMaxGrade;

    private List<EntryGateData> entryGateList = new List<EntryGateData>();

    PlayerData playerData;

    /// <summary>
    /// 策划表配置的最大等级
    /// </summary>
    int entryMaxGrade;

    /// <summary>
    /// 是否扣钱成功是否收到回复
    /// </summary>
    bool isGetCoin = true;

    /// <summary>
    /// 假计时   区分单点和长按
    /// </summary>
    int fakeTime = 0;

    /// <summary>
    /// 判断是否是长按状态
    /// </summary>
    bool isLongPress = false;

    Config.ticketCell ticketCell;
    #region 全局UI控件属性
    Button hideUIButton;
    Text titleText;         //名称
    Text tipsText;          //释义语言
    Text lvText;            //等级text

    Transform allEntryCell;//所有的售票口父类

    Transform effectNode;   //新手引导手势节点
    Slider gradeSlider;
    Image gradeSlider_Image;
    Text gradeSlider_Text;

    Text gradeText;  //价格标签3

    Text scoreNumTest;     //UI的星星收集显示
    int starLevelReached;
    #endregion
    public UIEntryPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UINewTicket";
    }
    public override void Awake(GameObject go)
    {
        base.Awake(go);
        //初始化控件
        this.RegistAllCompent();
        GetTransPrefabAllTextShow(this.transform);
    }

    #region 内部组件查找
    /// <summary>
    /// 内部组件的查找
    /// </summary>
    private void RegistAllCompent()
    {
        /*  若是新手引导阶段，进入特殊处理方法  */

        titleText = RegistCompent<Text>("UIFerryCar_LvUp/TitleGroup/TitleText");
        //GetTransPrefabText(titleText);
        tipsText = RegistCompent<Text>("UIFerryCar_LvUp/TitleGroup/TipsText");
        //GetTransPrefabText(tipsText);
        //当前等级
        lvText = RegistCompent<Text>("UIFerryCar_LvUp/TitleGroup/LvText");

        hideUIButton = RegistBtnAndClick("UIFerryCar_LvUp/Image/UIImage", HideButtonUI);

        //等级进度条控件
        //gradeSlider1 = RegistCompent<Slider>("LvUpSchedule/Schedule/Slider1");
        gradeSlider = RegistCompent<Slider>("UIFerryCar_LvUp/LvUpSchedule/Schedule/Slider2");
        gradeSlider_Image = RegistCompent<Image>("UIFerryCar_LvUp/LvUpSchedule/Schedule/IconBg/Icon");
        gradeSlider_Text = RegistCompent<Text>("UIFerryCar_LvUp/LvUpSchedule/Schedule/IconBg/Num");
        gradeText = RegistCompent<Text>("UIFerryCar_LvUp/LvUpSchedule/Schedule/Text_2");

        scoreNumTest = RegistCompent<Text>("UIFerryCar_LvUp/ScoreGroup/ScoreNum");

        allEntryCell = RegistCompent<Transform>("UIFerryCar_LvUp/ParameterGroup/Parameter/ScorllView/AnimalGroup");
        RegistInitEveryCompent();
        actions.Add(OnLongPressButton1);
        actions.Add(OnLongPressButton2);
        actions.Add(OnLongPressButton3);
        actions.Add(OnLongPressButton4);
        actions.Add(OnLongPressButton5);
        actions.Add(OnLongPressButton6);
        actions.Add(OnLongPressButton7);
        actions.Add(OnLongPressButton8);
    }
    /// <summary>
    /// 初始化所有的售票口UI为未开启状态
    /// </summary>
    private void RegistInitEveryCompent()
    {
        string path = "UIFerryCar_LvUp/ParameterGroup/Parameter/ScorllView/AnimalGroup/{0}/{1}";
        entryGateList = GlobalDataManager.GetInstance().playerData.playerZoo.entryGateList;

        for (int i = 0; i < allEntryCell.childCount; i++)
        {

            string name = allEntryCell.GetChild(i).name;
            Text nameText = RegistCompent<Text>(string.Format(path, name, "Text_1"));
            //GetTransPrefabText(nameText);
            nameText.text = "未开启";

            Text Text_2 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_2"));
            //GetTransPrefabText(Text_2);
            Text Text_3 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_3"));
            Text LvText = RegistCompent<Text>(string.Format(path, name, "level/LvText"));
            Text serialText = RegistCompent<Text>(string.Format(path, name, "ID/LvText"));
            //GetTransPrefabText(Text_3);
            Button button = RegistCompent<Button>(string.Format(path, name, "Button"));
            Text button_Text_2 = button.transform.Find("Image/NeedGoldNum").GetComponent<Text>();
            Text button_Text_3 = button.transform.Find("ButtonLvUpText").GetComponent<Text>();
            button.gameObject.SetActive(false);

            if (i == 0)
            {

                nameText.text = GetL10NString("Ui_Text_13");
                Text_2.text = MinerBigInt.ToDisplay(EntryGateModule.GetEntryPrice(entryTicketsLevel));
                Text_3.text = MinerBigInt.ToDisplay(EntryGateModule.GetEntryPrice(entryTicketsLevel, 1));
                LvText.text =  entryTicketsLevel.ToString();
                serialText.gameObject.SetActive(false);
                button = RegistCompent<Button>(string.Format(path, name, "Button"));
                button.onClick.AddListener(OnClickUpGradeTickets);
                if (button.gameObject.GetComponent<RepeatButton>() == null)
                {
                    button.gameObject.AddComponent<RepeatButton>();
                    button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressButton);//按下。频繁的调用
                    button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次
                }
                button_Text_2.text = MinerBigInt.ToDisplay(EntryGateModule.GetUpGradeConsumption(entryTicketsLevel));
                button_Text_3.text = GetL10NString("Ui_Text_7");
                button.gameObject.SetActive(true);
            }

        }

    }
    #endregion

    #region UI页面刷新
    /// <summary>
    /// 初始化属性数值
    /// </summary>
    private void InitData()
    {
        //LogWarp.LogError("测试 ：  InitData ");
        playerData = GlobalDataManager.GetInstance().playerData;
        //获取玩家出口等级
        entryTicketsLevel = playerData.playerZoo.entryTicketsLevel;

        //获取玩家现有金币
        coinVal = BigInteger.Parse(playerData.playerZoo.coin);
        consumeCoins = EntryGateModule.GetUpGradeConsumption(entryTicketsLevel);//下一级需要的金钱

        ticketCell = Config.ticketConfig.getInstace().getCell(0);
        int idx = PlayerDataModule.FindLevelRangIndex(ticketCell.lvshage, entryTicketsLevel);
        maxGrade = ticketCell.lvshage[idx];
        oldMaxGrade = ticketCell.lvshage[idx-1];
        entryMaxGrade = ticketCell.lvmax;
        starLevelReached = PlayerDataModule.FindLevelRangIndex01(Config.ticketConfig.getInstace().getCell(1).lvshage, entryTicketsLevel);
        if (entryTicketsLevel >= entryMaxGrade)
        {
            starLevelReached = PlayerDataModule.FindLevelRangIndex01(Config.ticketConfig.getInstace().getCell(1).lvshage, entryTicketsLevel);
        }
        entryGateList = playerData.playerZoo.entryGateList;

        InitCompent();

    }
    /// <summary>
    /// 控件显示赋值
    /// </summary>
    private void InitCompent()
    {
        InitEveryCompent();

        if (maxGrade >= entryMaxGrade)
        {
            maxGrade = entryMaxGrade;
        }
        lvText.text = string.Format(GetL10NString("Ui_Text_2"), playerData.playerZoo.entryTicketsLevel.ToString());            //等级text
        scoreNumTest.text = starLevelReached + "/" + Config.ticketConfig.getInstace().getCell(0).starsum;
        gradeSlider.value = AddPercentage(entryTicketsLevel - oldMaxGrade, maxGrade - oldMaxGrade);
        gradeText.text = entryTicketsLevel.ToString() + "/" + maxGrade.ToString();  //最大等级上限
        Config.itemCell itemCell = GradeSliderAwardImage();
        gradeSlider_Image.sprite = ResourceManager.LoadSpriteFromPrefab(itemCell.icon);
        gradeSlider_Text.text = MinerBigInt.ToDisplay(itemCell.itemval);
    }
    /// <summary>
    /// 根据数据源显示正确的UI
    /// </summary>
    private void InitEveryCompent()
    {
        string path = "UIFerryCar_LvUp/ParameterGroup/Parameter/ScorllView/AnimalGroup/{0}/{1}";
        if (entryGateList.Count <= 0)
        {
            entryGateList = playerData.playerZoo.entryGateList;
        }
        float width = allEntryCell.GetComponent<RectTransform>().sizeDelta.x;
        int count = entryGateList.Count;
        if (count==8)
        {
            count = count - 1;
            allEntryCell.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(width, 220f + 125f * count);
        }
        else
        {
            allEntryCell.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(width, 220f + 125f * count);
        }
        for (int i = 0; i < allEntryCell.childCount; i++)
        {
            /*查找所有的用的上的对象*/
            string name = allEntryCell.GetChild(i).name;
            Text nameText = RegistCompent<Text>(string.Format(path, name, "Text_1"));
            Image imageIcon = RegistCompent<Image>(string.Format(path, name, "Icon"));
            Text Text_2 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_2"));
            Text Text_3 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_3"));
            Text LvText = RegistCompent<Text>(string.Format(path, name, "level/LvText"));
            Text serialText = RegistCompent<Text>(string.Format(path, name, "ID/LvText"));
            Button button = RegistCompent<Button>(string.Format(path, name, "Button"));
            Text button_NeedGoldNum = button.transform.Find("Image/NeedGoldNum").GetComponent<Text>();
            Text button_ButtonLvUpText = button.transform.Find("ButtonLvUpText").GetComponent<Text>();
            button.gameObject.SetActive(false);

            if (i == 0)
            {
                /* 第一个cell的  门票价格 显示 */
                nameText.text = GetL10NString("Ui_Text_13");
                Text_2.text = MinerBigInt.ToDisplay(EntryGateModule.GetEntryPrice(entryTicketsLevel).ToString());
                Text_3.text = "+" + MinerBigInt.ToDisplay(EntryGateModule.GetEntryPrice(entryTicketsLevel, 1).ToString());
                LvText.text = entryTicketsLevel.ToString();
                allEntryCell.GetChild(i).Find("ID").gameObject.SetActive(false);

                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    OnClickUpGradeTickets();
                    BtnScaleAnim(button.gameObject, 1.1f, 0.95f);
                });
                if (button.gameObject.GetComponent<RepeatButton>() == null)
                {
                    button.gameObject.AddComponent<RepeatButton>();//需要长按
                    button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressButton);//按下。频繁的调用
                    button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次
                }

                button_NeedGoldNum.text = MinerBigInt.ToDisplay(EntryGateModule.GetUpGradeConsumption(entryTicketsLevel).ToString());
                button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

                //判断是否钱够
                var coin = EntryGateModule.GetUpGradeConsumption(entryTicketsLevel);
                if (coin > coinVal)
                {
                    SwitchButtonUnclickable(button, false);
                }
                else
                {
                    SwitchButtonUnclickable(button, true);
                }

                //判断是否是最大值
                var max_Level = Config.ticketConfig.getInstace().getCell(i).lvmax;
                if (entryTicketsLevel >= max_Level)
                {
                    button_ButtonLvUpText.text = GetL10NString("Ui_Text_46");
                    button_NeedGoldNum.gameObject.SetActive(false);
                    Text_3.gameObject.SetActive(false);
                    SwitchButtonUnclickable(button, false);
                }
                if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide && effectNode == null)
                {
                    //停车场停车位按钮处显示小手点击动画
                    effectNode = button.transform.Find("effectNode");
                    Transform trans = null;
                    trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().GuideUiClickEffect).transform;
                    trans.SetParent(effectNode, false);
                    SimpleParticle particlePlayer = new SimpleParticle();
                    particlePlayer.Init(effectNode.gameObject);
                    particlePlayer.Play();
                }
            }
            else
            {
                if (entryGateList.Count + 1 > i)
                {
                    /* 正常单售票口开启后的ui显示 */
                    nameText.text = GetL10NString("Ui_Text_14");

                    //var cell = Config.ticketConfig.getInstace().getCell(entryGateList[i - 1].entryID);
                    Text_2.text = EntryGateModule.GetCheckinSpeed(entryGateList[i - 1].entryID, entryGateList[i - 1].level).ToString()+GetL10NString("Ui_Text_67");
                    Text_3.text = "+" + EntryGateModule.GetCheckinSpeed(entryGateList[i - 1].entryID, entryGateList[i - 1].level, 1).ToString("f1");
                    LvText.text =  entryGateList[i - 1].level.ToString();
                    serialText.text = string.Format(GetL10NString("Ui_Text_70"), i);
                    string iconPath = Config.globalConfig.getInstace().LvUpTicketIcon;
                    imageIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
                    button.gameObject.SetActive(true);
                    button.onClick.RemoveAllListeners();
                    int isID = entryGateList[i - 1].entryID;

                    button.onClick.AddListener(delegate
                    {
                        BtnScaleAnim(button.gameObject, 1.1f, 0.95f);
                        OnClickUpGradeEntry(isID);
                    });
                    if (button.gameObject.GetComponent<RepeatButton>() == null)
                    {
                        button.gameObject.AddComponent<RepeatButton>();//需要长按
                        button.GetComponent<RepeatButton>().onPress.AddListener(actions[i - 1]);//按下。频繁的调用
                        button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次
                    }
                    button_NeedGoldNum.text = MinerBigInt.ToDisplay(EntryGateModule.GetUpGradeCheckinSpeedConsumption(isID, entryGateList[isID].level).ToString());
                    //LogWarp.LogErrorFormat("测试：   {0}    {1}  {2}", isID, entryGateList[isID].level,button_NeedGoldNum.text);
                    button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

                    //判断是否钱够
                    var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(entryGateList[i - 1].entryID, entryGateList[i - 1].level);
                    if (coin > coinVal)
                    {
                        SwitchButtonUnclickable(button, false);
                    }
                    else
                    {
                        SwitchButtonUnclickable(button, true);
                        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
                        {
                            SwitchButtonUnclickable(button, false);
                        }
                    }

                    //判断是否是最大值
                    var max_Level = Config.ticketConfig.getInstace().getCell(i - 1).speedmaxlv;
                    if (entryGateList[i - 1].level >= max_Level)
                    {
                        button_ButtonLvUpText.text = GetL10NString("Ui_Text_46");
                        button_NeedGoldNum.gameObject.SetActive(false);
                        Text_3.gameObject.SetActive(false);
                        SwitchButtonUnclickable(button, false);

                    }
                    
                   

                }
                else if (entryGateList.Count + 1 == i)
                {
                    /**** 下一个待开的售票口ui显示 ****/
                    nameText.text = GetL10NString("Ui_Text_45");
                    Text_2.gameObject.SetActive(false);
                    Text_3.gameObject.SetActive(false);
                    LvText.text =  "0";
                    serialText.text = string.Format(GetL10NString("Ui_Text_70"), i);

                    string iconPath = Config.globalConfig.getInstace().AddTicketIcon;
                    imageIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);

                    button.gameObject.SetActive(true);
                    button.onClick.RemoveAllListeners();
                    /*不需要长按*/
                    button.onClick.AddListener(delegate
                    {
                        OnClickOpenNewEntry();
                        BtnScaleAnim(button.gameObject, 1.1f, 0.95f);
                    });
                    var price = Config.ticketConfig.getInstace().getCell(i - 1).number;
                    button_NeedGoldNum.text = MinerBigInt.ToDisplay(BigInteger.Parse(price));
                    button_ButtonLvUpText.text = GetL10NString("Ui_Text_68");
                    BigInteger coin = BigInteger.Parse(price);
                    //判断是否钱够
                    if (coin > coinVal)
                    {
                        SwitchButtonUnclickable(button, false);
                    }
                    else
                    {
                        SwitchButtonUnclickable(button, true);
                        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
                        {
                            SwitchButtonUnclickable(button, false);
                        }
                    }
                    
                   
                }
                else if (entryGateList.Count + 1 < i)
                { /**** 不在列表中的隐藏 ****/
                    allEntryCell.GetChild(i).gameObject.SetActive(false);
                    nameText.text = "&&&&&&&";
                    Text_2.text = "AAAAAA";
                    //Text_3.text = EntryGateModule.GetEntryPrice( entryGateList[i].level, 1).ToString();
                    Text_3.text = "BBB";
                }

            }
        }

    }

    /// <summary>
    /// 恢复新手阶段对按钮的限制
    /// </summary>
    private void InitNewGuideEveryCompent()
    {
        string path = "UIFerryCar_LvUp/ParameterGroup/Parameter/ScorllView/AnimalGroup/{0}/{1}";
        if (entryGateList.Count <= 0)
        {
            entryGateList =playerData.playerZoo.entryGateList;
        }

        for (int i = 0; i < allEntryCell.childCount; i++)
        {
            /*查找所有的用的上的对象*/
            string name = allEntryCell.GetChild(i).name;
            Button button = RegistCompent<Button>(string.Format(path, name, "Button"));
            button.gameObject.SetActive(false);

            if (i == 0 || i == 1 || i == 2)
            {
                if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide)
                {
                    SwitchButtonUnclickable(button, false);
                }
                
            }
        }

    }

    /// <summary>
    /// 根据ID显示某一个售票口正常的UI
    /// </summary>
    /// <param name="entryID"></param>
    private void InitAloneEntryCompent(int entryID)
    {
        string path = "UIFerryCar_LvUp/ParameterGroup/Parameter/ScorllView/AnimalGroup/{0}/{1}";

        entryGateList = playerData.playerZoo.entryGateList;

        /*查找所有的用的上的对象*/
        string name = allEntryCell.GetChild(entryID + 1).name;
        Text nameText = RegistCompent<Text>(string.Format(path, name, "Text_1"));
        Image imageIcon = RegistCompent<Image>(string.Format(path, name, "Icon"));
        Text Text_2 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_2"));
        Text Text_3 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_3"));
        Text LvText = RegistCompent<Text>(string.Format(path, name, "level/LvText"));
        Text serialText = RegistCompent<Text>(string.Format(path, name, "ID/LvText"));
        Button button = RegistCompent<Button>(string.Format(path, name, "Button"));
        Text button_NeedGoldNum = button.transform.Find("Image/NeedGoldNum").GetComponent<Text>();
        Text button_ButtonLvUpText = button.transform.Find("ButtonLvUpText").GetComponent<Text>();

        /* 正常单售票口开启后的ui显示 */
        nameText.text = GetL10NString("Ui_Text_14");

        //var cell = Config.ticketConfig.getInstace().getCell(entryID);
        Text_2.text = EntryGateModule.GetCheckinSpeed(entryID, entryGateList[entryID].level).ToString() + GetL10NString("Ui_Text_67");
        Text_3.text = "+" + EntryGateModule.GetCheckinSpeed(entryID, entryGateList[entryID].level, 1).ToString("f1");
        LvText.text =  entryGateList[entryID].level.ToString();
        serialText.text = string.Format(GetL10NString("Ui_Text_70"), entryID + 1);
        var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(entryID, entryGateList[entryID].level);

        button_NeedGoldNum.text = MinerBigInt.ToDisplay(coin).ToString();
        button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

        //判断是否钱够
        if (coin > coinVal)
        {
            SwitchButtonUnclickable(button, false);
        }
        else
        {
            SwitchButtonUnclickable(button, true);
        }
        //判断是否是最大值
        var max_Level = Config.ticketConfig.getInstace().getCell(entryID).speedmaxlv;
        LogWarp.LogErrorFormat("测试：   entryGateList[entryID].level={0}    max_Level={1}  ", entryGateList[entryID].level, max_Level);
        if (entryGateList[entryID].level >= max_Level)
        {
            button_ButtonLvUpText.text = GetL10NString("Ui_Text_46");
            button_NeedGoldNum.gameObject.SetActive(false);
            Text_3.gameObject.SetActive(false);
            SwitchButtonUnclickable(button, false);
        }
    }
    /// <summary>
    /// 根据ID开启某个的售票UI逻辑
    /// </summary>
    /// <param name="entryID"></param>
    private void InitOpenNewEntryCompent(int entryID)
    {
        LogWarp.LogError("aaaaaaaaaaaaa " + entryID);
        float width = allEntryCell.GetComponent<RectTransform>().sizeDelta.x;
        int cellHeight = entryGateList.Count;
        if (entryID == 7)
        {
            cellHeight = entryGateList.Count - 1;
            allEntryCell.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(width, 220f + 125f * cellHeight);

        }
        else
        {
            allEntryCell.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(width, 220f + 125f * cellHeight);

        }
        string path = "UIFerryCar_LvUp/ParameterGroup/Parameter/ScorllView/AnimalGroup/{0}/{1}";

        entryGateList = playerData.playerZoo.entryGateList;

        for (int i = 0; i < allEntryCell.childCount; i++)
        {
            allEntryCell.GetChild(i).gameObject.SetActive(true);

            /*查找所有的用的上的对象*/
            string name = allEntryCell.GetChild(i).name;
            Text nameText = RegistCompent<Text>(string.Format(path, name, "Text_1"));
            Image imageIcon = RegistCompent<Image>(string.Format(path, name, "Icon"));
            Text Text_2 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_2"));
            Text Text_3 = RegistCompent<Text>(string.Format(path, name, "TextAll/Text_3"));
            Text LvText = RegistCompent<Text>(string.Format(path, name, "level/LvText"));
            Text serialText = RegistCompent<Text>(string.Format(path, name, "ID/LvText"));
            Button button = RegistCompent<Button>(string.Format(path, name, "Button"));
            Text button_NeedGoldNum = button.transform.Find("Image/NeedGoldNum").GetComponent<Text>();
            Text button_ButtonLvUpText = button.transform.Find("ButtonLvUpText").GetComponent<Text>();
            //button.gameObject.SetActive(false);

            if (i == entryID + 1)
            {
                /* 正常单售票口开启后的ui显示 */
                nameText.text = GetL10NString("Ui_Text_14");
                var cell = Config.ticketConfig.getInstace().getCell(entryGateList[i - 1].entryID);
                Text_2.text = EntryGateModule.GetCheckinSpeed(entryGateList[i - 1].entryID, entryGateList[i - 1].level).ToString() + GetL10NString("Ui_Text_67");
                Text_3.text = "+" + EntryGateModule.GetCheckinSpeed(entryGateList[i - 1].entryID, entryGateList[i - 1].level, 1).ToString("f1");
                LvText.text = entryGateList[i - 1].level.ToString();
                serialText.text = string.Format(GetL10NString("Ui_Text_70"), i);

                Text_2.gameObject.SetActive(true);
                Text_3.gameObject.SetActive(true);
                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();
                int isID = entryGateList[i - 1].entryID;
                string iconPath = Config.globalConfig.getInstace().LvUpTicketIcon;
                imageIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);
                button.onClick.AddListener(delegate
                {
                    OnClickUpGradeEntry(isID);
                    BtnScaleAnim(button.gameObject, 1.1f, 0.95f);
                });

                //button.GetComponent<RepeatButton>().onPress.AddListener(OnLongPressButton);//按下。频繁的调用
                //button.GetComponent<RepeatButton>().onRelease.AddListener(OnReleaseButton);//抬起，调用一次
                button_NeedGoldNum.text = MinerBigInt.ToDisplay(EntryGateModule.GetUpGradeCheckinSpeedConsumption(isID, entryGateList[isID].level).ToString());
                button_ButtonLvUpText.text = GetL10NString("Ui_Text_7");

            }
            else if (i == entryID + 2)
            {
                nameText.text = GetL10NString("Ui_Text_45");
                Text_2.gameObject.SetActive(false);
                Text_3.gameObject.SetActive(false);
                LvText.text = "0";
                serialText.text = string.Format(GetL10NString("Ui_Text_70"), i);

                string iconPath = Config.globalConfig.getInstace().AddTicketIcon;
                imageIcon.sprite = ResourceManager.LoadSpriteFromPrefab(iconPath);

                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    OnClickOpenNewEntry();
                    BtnScaleAnim(button.gameObject, 1.1f, 0.95f);
                });
                var parce = Config.ticketConfig.getInstace().getCell(i - 1).number;
                button_NeedGoldNum.text = MinerBigInt.ToDisplay(BigInteger.Parse(parce));
                button_ButtonLvUpText.text = GetL10NString("Ui_Text_68");
            }
            else if (i > entryID + 2)
            {
                allEntryCell.GetChild(i).gameObject.SetActive(false);
            }


        }
    }
    #endregion

    #region 按钮升级事件
    /// <summary>
    /// 点击升级门票按钮事件
    /// </summary>
    public void OnClickUpGradeTickets()
    {
        //isLongPress为true则是长按状态，单点关闭  返回
        if (!JudgePressButton() && isLongPress)
        {
            LogWarp.LogError("不能升级     " + (!JudgePressButton()) + "    " + isLongPress);
            return;
        }

        /*  新手引导  */
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            if (entryTicketsLevel >= 5)
            {
                return;
            }
        }
        SendSetTicketsLevelMessageManager();

        isGetCoin = false;  //设置等待回复状态
        //upGradeButton.enabled = false; //设置按钮不能点击
        ///播放音乐
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);

        //LogWarp.LogError("测试             单点  按钮         ");
    }
    private void SendSetTicketsLevelMessageManager()
    {
        if (entryTicketsLevel >= entryMaxGrade)
        {
            return;
        }
        if (!SetGradeBool())
        {
            return;
        }

        //LogWarp.LogErrorFormat("测试：  consumeCoins={0} , coinVal={1} , entryTicketsLevel={2} , entryMaxGrade={3}", consumeCoins, coinVal, entryTicketsLevel, entryMaxGrade);

        //发送消息       SetValueOfPlayerData  消息体    SetParkingLevelOfPlayerData 设置停车场消息ID，UpGradeNumber  升多少级
        SetValueOfPlayerData.Send((int)GameMessageDefine.SetEntryGateLevelOfPlayerData,
               1, 0, 0);
    }

    /// <summary>
    /// 点击升级单个售票口事件
    /// </summary>
    public void OnClickUpGradeEntry(int ID)
    {
        //isLongPress为true则是长按状态，单点关闭  返回
        if (isLongPress)
        {
            LogWarp.LogError("不能升级");
            return;
        }
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                    ///播放音乐
                    string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
                    SoundManager.GetInstance().PlaySound(btnSoundPath);
                }
            }
            else
            {
                LogWarp.LogError(speedmaxlv);
            }
        }



    }
    private void SendEntryGatePureLevelMessageManager(int id)
    {
        //没有对钱进行判断和传值
        SetDetailValueOfPlayerData.Send((int)GameMessageDefine.SetEntryGatePureLevelOfPlayerData, id, 1, 0);
    }

    /// <summary>
    /// 点击开启新的售票口事件
    /// </summary>
    public void OnClickOpenNewEntry()
    {
        //判断是否可以开启动物栏


        //获取下一个应该开启的ID
        int ID = entryGateList[entryGateList.Count - 1].entryID;


        SetValueOfPlayerData.Send((int)GameMessageDefine.SetEntryGateNumOfPlayerData,
               1, 0, ID + 1);

        isGetCoin = false;  //设置等待回复状态
        //upGradeButton.enabled = false; //设置按钮不能点击
        ///播放音乐
        string btnSoundPath = Config.globalConfig.getInstace().BuildUpButtonMusic;
        SoundManager.GetInstance().PlaySound(btnSoundPath);

        LogWarp.LogError("测试             单点  按钮         ");
    }
    #endregion

    #region 监听消息
    /// <summary>
    /// 监听玩家coin金钱发生改变，是否需要重新计算升级规模
    /// </summary>
    /// <param name="obj"></param>
    protected void OnGetCoinOfPlayerData(Message obj)
    {   //旧计算金钱不够，则开始新的计算
        this.InitData();
        InitEveryCompent();
        //LogWarp.LogError("售票口的 金钱监听  coinVal=" + coinVal+ "  consumeCoins ="+ consumeCoins);
    }
    /// <summary>
    /// 监听售票口的门票等级
    /// </summary>
    /// <param name="msg"></param>
    protected void OnGetBroadcastEntryGateLevelOfPlayerData(Message msg)
    {
        //刷新售票口的UI显示
        this.InitData();
        //earnings_Button.enabled = true;
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true)
        {
            GameEventManager.SendEvent("entryTicketsLevel");
        }
        if (GlobalDataManager.GetInstance().playerData.playerZoo.isGuide == true && entryTicketsLevel >= 5)
        {   /*新手阶段   隐藏停车场Ui  显示新手引导UI    步骤应该是  14    */
            DestroyEffectChild();
            InitNewGuideEveryCompent();
            this.Hide();
            PageMgr.ShowPage<UIGuidePage>();
        }
        GameEventManager.SendEvent("ticket_attribute_lv" + entryTicketsLevel);
        isGetCoin = true;

    }
    /// <summary>
    /// 监听某个售票口的等级变换
    /// </summary>
    /// <param name="obj"></param>
    protected void OnGetBroadcastEntryGatePureLevelOfPlayerData(Message obj)
    {
        var _msg = obj as SetDetailValueOfPlayerData;
        InitAloneEntryCompent(_msg.detailVal);
        int level = entryGateList[_msg.detailVal].level;

        GameEventManager.SendEvent("ticket_attribute_lv");
        isGetCoin = true;
    }
    /// <summary>
    /// 监听售票口开启成功的消息
    /// </summary>
    /// <param name="obj"></param>
    protected void OnGetBroadcastEntryGateNumOfPlayerData(Message obj)
    {
        var _msg = obj as SetValueOfPlayerData;
        LogWarp.LogError("测试  开启的下标：" + _msg.channelID);
        InitOpenNewEntryCompent(_msg.channelID);
        GameEventManager.SendEvent("ticket_attribute_lv"+ playerData.GetEntryGateIDIndexOfDataIdx(_msg.channelID).level);
        isGetCoin = true;
    }
    #endregion
	
    /// <summary>
    /// 更新
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();
        //this.InitData();
        //this.InitCompent();
    }
    /// <summary>
    /// 活跃
    /// 添加按钮事件
    /// </summary>
    public override void Active()
    {
        base.Active();
        //upGradeButton.enabled = true;
        //初始化属性数值
        this.InitData();
        InitEveryCompent();
        //注册监听消息     
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnGetCoinOfPlayerData);//接受金钱变动的信息
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastEntryGateLevelOfPlayerData, this.OnGetBroadcastEntryGateLevelOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastEntryGatePureLevelOfPlayerData, OnGetBroadcastEntryGatePureLevelOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.BroadcastEntryGateNumOfPlayerData, this.OnGetBroadcastEntryGateNumOfPlayerData);
        MessageManager.GetInstance().Regist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);

    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        OnReleaseButton();
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastEntryGateLevelOfPlayerData, this.OnGetBroadcastEntryGateLevelOfPlayerData);
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastCoinOfPlayerData, this.OnGetCoinOfPlayerData);//接受金钱变动的信息
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastEntryGatePureLevelOfPlayerData, OnGetBroadcastEntryGatePureLevelOfPlayerData);
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.BroadcastEntryGateNumOfPlayerData, this.OnGetBroadcastEntryGateNumOfPlayerData);
        MessageManager.GetInstance().UnRegist((int)GameMessageDefine.UIMessage_OpenOfflinePage, OnOpenOfflineUIPage);

        MessageString.Send((int)GameMessageDefine.UIMessage_OnClickButShowPart, "UIMainPage");
        UIInteractive.GetInstance().iPage = null;
        DestroyEffectChild();

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
    private bool SetGradeBool()
    {
        if (consumeCoins <= coinVal && entryTicketsLevel <entryMaxGrade)
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 是否可以升级
    /// </summary>
    /// <returns></returns>
    private bool JudgePressButton()
    {
        //第一个  是否扣钱成功   第二  判断是否可以升级
        if (isGetCoin && SetGradeBool())
        {
            //earnings_Button.enabled = true;

            return true;
        }
        // upGradeButton.enabled = false;

        return false;
    }

    /// <summary>
    /// 长按按钮回调事件
    /// </summary>
    protected void OnLongPressButton()
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
        if (JudgePressButton())
        {
            SendSetTicketsLevelMessageManager();  //发送升级消息
            isGetCoin = false;
        }
    }

    protected void OnLongPressButton1()
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
        int ID = 0;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
            else
            {
                LogWarp.LogError(speedmaxlv);
            }
        }

    }

    protected void OnLongPressButton2()
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
        int ID = 1;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
        }
    }

    protected void OnLongPressButton3()
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
        int ID = 2;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
        }
    }

    protected void OnLongPressButton4()
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
        int ID = 3;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
        }
    }

    protected void OnLongPressButton5()
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
        int ID = 4;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
        }
    }
    /// <summary>
    /// 长按按钮回调事件0
    /// </summary>
    protected void OnLongPressButton6()
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
        int ID = 5;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
        }
    }

    protected void OnLongPressButton7()
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
        int ID = 6;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);
                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
        }
    }

    protected void OnLongPressButton8()
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
        int ID = 7;
        if (isGetCoin)
        {
            var speedmaxlv = Config.ticketConfig.getInstace().getCell(ID).speedmaxlv;
            if (entryGateList[ID].level < speedmaxlv)
            {
                var coin = EntryGateModule.GetUpGradeCheckinSpeedConsumption(ID, entryGateList[ID].level);

                if (coin <= coinVal)
                {
                    SendEntryGatePureLevelMessageManager(ID);  //发送升级消息
                    isGetCoin = false;
                }
            }
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

    protected List<UnityAction> actions = new List<UnityAction>();

    /// <summary>
    /// 获取等级段对应的奖励信息
    /// </summary>
    /// <returns></returns>
    private Config.itemCell GradeSliderAwardImage()
    {
        var lvreward = ticketCell.lvreward;

        int idx = PlayerDataModule.FindLevelRangIndex(ticketCell.lvshage, entryTicketsLevel);
        int itemID = lvreward[idx];

        Config.itemCell itemCell = Config.itemConfig.getInstace().getCell(itemID);

        //Logger.LogWarp.LogErrorFormat("测试： 等级={0}，等级段对应的奖励={1}", entryTicketsLevel, itemID);
        return itemCell;
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


