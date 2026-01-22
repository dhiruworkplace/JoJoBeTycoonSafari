using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LittleGame
{
    public class FailedPage : UIPage
    {

        private Button returnButton;
        private Button againButton;

        public FailedPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
        {
            uiPath = "UIPrefab/UIGameFail";
        }

        public override void Awake(GameObject go)
        {
            base.Awake(go);
            returnButton = RegistCompent<Button>("ButtonGroup/ReturnButton/Button");
            againButton = RegistCompent<Button>("ButtonGroup/AgainButton/Button");
        }

        public override void Active()
        {
            base.Active();
            Init();
            Register();
        }

        private void Init()
        {
            ///初始化金币
            ///初始化钻石
        }

        private void Register()
        {
            returnButton.onClick.AddListener(OnClickReturnButton);
            againButton.onClick.AddListener(OnClickAgainButton);
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void OnClickReturnButton()
        {
            PageMgr.ClosePage<FailedPage>();
        }

        private void OnClickAgainButton()
        {
            PageMgr.ClosePage<FailedPage>();
            ///加载关卡数据
            //LittleController.Instance.InitGameData();
        }
    }
}