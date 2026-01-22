using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LittleGame
{

    public class StartPage : UIPage
    {
        private Button GameOptionButton1;
        private Button GameOptionButton2;
        private Button GameOptionButton3;
        private Button GameOptionButton4;
        private Button GameOptionButton5;
        private Button ResetButton;
        private Button ReturnButton;

        private Transform loadingTrans;


        public StartPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
        {
            uiPath = "UIPrefab/UIGameCheckpoint";
        }

        public override void Awake(GameObject go)
        {
            base.Awake(go);
            GameOptionButton1 = RegistCompent<Button>("GameOptionGroup/GameOptionButton1/GameStartButton/Button");
            GameOptionButton2 = RegistCompent<Button>("GameOptionGroup/GameOptionButton2/GameStartButton/Button");
            GameOptionButton3 = RegistCompent<Button>("GameOptionGroup/GameOptionButton3/GameStartButton/Button");
            GameOptionButton4 = RegistCompent<Button>("GameOptionGroup/GameOptionButton4/GameStartButton/Button");
            GameOptionButton5 = RegistCompent<Button>("GameOptionGroup/GameOptionButton5/GameStartButton/Button");
            ResetButton = RegistCompent<Button>("ResetButton/ButtonBg");
            ReturnButton = RegistCompent<Button>("ReturnButton");
            loadingTrans = transform.Find("LoadingBG");
        }

        public override void Active()
        {
            base.Active();
            PageMgr.ClosePage<UIMainPage>();
            Regist();
        }

        private void Regist()
        {
            GameOptionButton1.onClick.AddListener(OnClickGameOptionButton1);
            GameOptionButton2.onClick.AddListener(OnClickGameOptionButton2);
            GameOptionButton3.onClick.AddListener(OnClickGameOptionButton3);
            GameOptionButton4.onClick.AddListener(OnClickGameOptionButton4);
            GameOptionButton5.onClick.AddListener(OnClickGameOptionButton5);
            ResetButton.onClick.AddListener(OnClickResetButton);
            ReturnButton.onClick.AddListener(OnClickReturnButton);



            EventManager.Add(ToolEventName.IsInitGameDone, GameInitDone);
        }

        public override void Hide()
        {
            base.Hide();
            EventManager.Remove(ToolEventName.IsInitGameDone, GameInitDone);
            GameOptionButton1.onClick.RemoveListener(OnClickGameOptionButton1);
            GameOptionButton2.onClick.RemoveListener(OnClickGameOptionButton2); 
            GameOptionButton3.onClick.RemoveListener(OnClickGameOptionButton3);
            GameOptionButton4.onClick.RemoveListener(OnClickGameOptionButton4);
            GameOptionButton5.onClick.RemoveListener(OnClickGameOptionButton5);
        }

        int hardLevel;

        private void InitGame(int hardLevel)
        {
            PageMgr.ShowPage<GamePage>();
            loadingTrans.gameObject.SetActive(true);
            transform.SetAsLastSibling();
            this.hardLevel = hardLevel;
            EventManager.Trigger(ToolEventName.LittleGameStart, hardLevel);
        }

        private void OnClickGameOptionButton1()
        {
            InitGame(1);
        }
        private void OnClickGameOptionButton2()
        {
            InitGame(2);
        }
        private void OnClickGameOptionButton3()
        {
            InitGame(3);
        }
        private void OnClickGameOptionButton4()
        {
            InitGame(4);
        }
        private void OnClickGameOptionButton5()
        {
            InitGame(5);
        }
        private void OnClickResetButton()
        {

        }
        private void OnClickReturnButton()
        {
            SceneMgr.Inst.LoadSceneAsync("Loading", () => { });
        }

        /// <summary>
        /// 游戏准备完成之后回调
        /// </summary>
        private void GameInitDone()
        {
            loadingTrans.gameObject.SetActive(false);
            
            PageMgr.ClosePage<StartPage>();
        }
    }
}