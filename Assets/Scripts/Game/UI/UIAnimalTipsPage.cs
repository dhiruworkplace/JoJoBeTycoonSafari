using DG.Tweening;
using Game.GlobalData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimalTipsPage : UIPage
{



    Text rewardText;
    Button confirmButton;
    Button closeButton;
    public UIAnimalTipsPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "uiprefab/UIAnimalTips";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        GetTransPrefabAllTextShow(this.transform);

        rewardText = RegistCompent<Text>("UpButtonGroup/GameObject/RewardText");
        confirmButton = RegistBtnAndClick("UpButtonGroup/AdvertPalyButton", OnClickConfirmButton);
        closeButton = AddCompentInChildren<Button>(closeButton, "UpButtonGroup/CloseButton");
        closeButton =   RegistBtnAndClick("UpButtonGroup/CloseButton", OnClickCloseButton);


    }

    private void OnClickCloseButton(string obj)
    {
        GameObject go = new GameObject();
        go.transform.DOMoveZ(0.1f, 0.2f).OnComplete(new TweenCallback(Hide));
    }

    private void OnClickConfirmButton(string obj)
    {
        OnClickCloseButton("");
    }

    public override void Active()
    {
        base.Active();
        int ruleStarNumber = Config.globalConfig.getInstace().AnimalupgradingNeed;
        rewardText.text = ruleStarNumber.ToString();

    }


    public override void Refresh()
    {
        base.Refresh();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
