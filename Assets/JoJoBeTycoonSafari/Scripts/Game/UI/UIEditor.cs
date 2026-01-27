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

/// <summary>
/// 编辑器
/// </summary>
public class UIEditor : UIPage
{
    InputField ipt_dragSensitivity;
    InputField ipt_CrossSensitivity;
    InputField ipt_dragSmoothDurationMS;
    InputField ipt_minPinch;
    InputField ipt_maxPinch;
    Text txt_Log;
    //Button btn_OK;
    //Button btn_Clean;

    public UIEditor() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UIEditor";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.CameraDebug, OnCameraDebug);
        Init();
    }

    void Init()
    {
        ipt_dragSensitivity = RegistCompent<InputField>("Editor/ipt_DragSensitivity");
        ipt_CrossSensitivity = RegistCompent<InputField>("Editor/ipt_CrossSensitivity");
        ipt_dragSmoothDurationMS = RegistCompent<InputField>("Editor/ipt_dragSmoothDurationMS");
        ipt_minPinch = RegistCompent<InputField>("Editor/ipt_minPinch");
        ipt_maxPinch = RegistCompent<InputField>("Editor/ipt_maxPinch");
        txt_Log = RegistCompent<Text>("Editor/txt_Log");

        ipt_dragSensitivity.text = ZooCamera.GetInstance().dragSensitivity.ToString();
        ipt_CrossSensitivity.text = ZooCamera.GetInstance().crossSensitivity.ToString();
        ipt_dragSmoothDurationMS.text = ZooCamera.GetInstance().dragSmoothDurationMS.ToString();
        ipt_minPinch.text = ZooCamera.GetInstance().minOrthographicSize.ToString();
        ipt_maxPinch.text = ZooCamera.GetInstance().maxOrthographicSize.ToString();

        RegistBtnAndClick("Editor/btn_OK", OnClick_BtnOK);
        RegistBtnAndClick("Editor/btn_Clean", OnClick_BtnClean);
        RegistBtnAndClick("Editor/btn_Min", OnClick_BtnMin);
        RegistBtnAndClick("Editor/btn_Max", OnClick_BtnMax);
    }


    protected void OnClick_BtnOK(String goName)
    {
        float dragSensitivity;
        float.TryParse(ipt_dragSensitivity.text, out dragSensitivity);
        ZooCamera.GetInstance().dragSensitivity = dragSensitivity;
        //Debug.LogError(dragSensitivity);

        float crossSensitivity;
        float.TryParse(ipt_CrossSensitivity.text, out crossSensitivity);
        ZooCamera.GetInstance().crossSensitivity = crossSensitivity;

        int dragSmoothDurationMS;
        int.TryParse(ipt_dragSmoothDurationMS.text, out dragSmoothDurationMS);
        ZooCamera.GetInstance().dragSmoothDurationMS = dragSmoothDurationMS;

        //float minOrthographicSize;
        //float.TryParse(ipt_minPinch.text, out minOrthographicSize);
        //ZooCamera.GetInstance().minOrthographicSize = minOrthographicSize;

        //float maxOrthographicSize;
        //float.TryParse(ipt_maxPinch.text, out maxOrthographicSize);
        //ZooCamera.GetInstance().maxOrthographicSize = maxOrthographicSize;
    }

    protected void OnClick_BtnClean(String goName)
    {
        txt_Log.text = "";
    }

    protected void OnClick_BtnMin(string goName)
    {
        ZooCamera.GetInstance().minOrthographicSize = (ZooCamera.GetInstance() as ZooCamera).GetCamera().orthographicSize;
        ipt_minPinch.text = ZooCamera.GetInstance().minOrthographicSize.ToString();
    }

    protected void OnClick_BtnMax(string goName)
    {
        ZooCamera.GetInstance().maxOrthographicSize = (ZooCamera.GetInstance() as ZooCamera).GetCamera().orthographicSize;
        ipt_maxPinch.text = ZooCamera.GetInstance().maxOrthographicSize.ToString();
    }

    protected void OnCameraDebug(Message msg)
    {
        var _msg = msg as MessageString;

        //ipt_Log.text += "\n" + _msg.str;
        txt_Log.text += string.Format("\n{0}", _msg.str);
    }











}
