using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIWaitAd : UIPage {
	public static bool isWaitLoadingAd = false;
		
	private Image _rotImg;
	private Text _text;
	private GameObject bgGo;

	public UIWaitAd() : base(UIType.PopUp, UIMode.DoNothing, UICollider.None)
	{
		uiPath = "uiprefab/UIWaitAd";
	}

	public override void Awake (GameObject go)
	{
		base.Awake (go);
		UIWaitAd.isWaitLoadingAd = true;

		this._rotImg = this.RegistCompent<Image> ("bg/rotImg");
		this._text = this.RegistCompent<Text>("titleText");
		this._text.gameObject.SetActive (false);
		this.bgGo = this.transform.Find("bg").gameObject;
	}

	public override void Refresh ()
	{
		base.Refresh ();
		this.bgGo.SetActive (true);
	}

	public override void Hide ()
	{
		base.Hide ();
		UIWaitAd.isWaitLoadingAd = false;
	}

	public void HideAndShowText(){
		this.Hide ();
		PromptText.CreatePromptText("AD_Fail");
	}
}
