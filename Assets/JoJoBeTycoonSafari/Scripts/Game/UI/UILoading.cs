using ZooGame;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
//using Tools;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIPage
{
    private Slider m_Slider;
    private Text m_AlertText;
    private Text m_ValueText;
    private Image m_TitleImage;
    private int m_CurCount;
    private int m_MaxCount;
    private int TableCount;
    private int SoundCount;
    private int EffectCount;
    private int TextureCount;
    private Text m_VersionText;
    private float sliderValue;

    public UILoading() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None, UITickedMode.Update)
    {
        uiPath = "UIPrefab/UILoading";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        this.RegistCompent();
        this.LoadScene();
        GetTransPrefabAllTextShow(this.transform);

    }

    private void LoadSceneTimeOut()
    {
        LoadScene();
    }

    private void RegistCompent()
    {
        m_Slider = this.RegistCompent<Slider>("Slider");
        m_Slider.value = 0;
        m_ValueText = this.RegistCompent<Text>("Slider/FillArea/ValueText");




        m_AlertText = this.RegistCompent<Text>("AlertText");
        m_TitleImage = this.RegistCompent<Image>("title");
        m_VersionText = this.RegistCompent<Text>("versions");
    }

    public override void Refresh()
    {
        base.Refresh();
        m_TitleImage.SetNativeSize();
    }

    public override void Active()
    {
        base.Active();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void SliderValueLoading(float value)
    {
        sliderValue += value;
        if (sliderValue>1)
        {
            sliderValue = 1;
        }
        m_Slider.value = sliderValue;

        int number = (int)(sliderValue * 100f);
        m_ValueText.text = number + "%";
        //Logger.LogWarp.LogError("测试： 场景加载  value="+ sliderValue);
    }

    /*, UITickedMode.Update*/
    public override void Tick(int deltaTimeMS)
    {
        if (!this.isActive())
        {
            return;
        }

        ZooGameLoader.GetInstance().Tick(deltaTimeMS);
    }

    //private void GameLoadSceneFristCallBack()
    //{
    //    float value = 1 / 6f;
    //    SliderValueLoading(value);
    //}

    //private void GameLoadSceneLastCallBack()
    //{
    //    PageMgr.ShowPage<UIMainPage>();
    //    if (LoadingMgr.Inst.debugCamera)
    //    {
    //        PageMgr.ShowPage<UIEditor>();
    //    }

    //    SliderValueLoading(1);
    //    PageMgr.ClosePage(this);
    //}

    //private void PerSceneLoadCallBack(float val)
    //{
    //    SliderValueLoading(val / 6);
    //}

    private void EditorLoadSceneCallBack()
    {
        PageMgr.ClosePage(this);
    }

    private void LoadScene()
    {
        //场景加载流程在ZooGameLoader中实现
        switch(LoadingMgr.Inst.runTimeLoaderType)
        {
            case RunTimeLoaderType.Game:
                ZooGameLoader.GetInstance().Load(null, null, null);
                return;
            case RunTimeLoaderType.Editor:
                ZooGameLoader.GetInstance().Load(EditorLoadSceneCallBack, null);
                return;
            default:
                string e = string.Format("runTimeLoaderType 类型错误{0}", LoadingMgr.Inst.runTimeLoaderType);
                throw new System.Exception(e);
        }
    }
}
