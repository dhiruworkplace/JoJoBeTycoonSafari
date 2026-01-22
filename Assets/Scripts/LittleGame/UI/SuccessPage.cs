using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LittleGame
{
    public class SuccessPage : UIPage
    {
        private Text rewardGoldText;
        private Text rewardRmbText;
        private Button receiveButton;
        private Button advertButton;

        public SuccessPage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
        {
            uiPath = "UIPrefab/UIGameVictory";
        }

        public override void Active()
        {
            base.Active();
            Init();
            receiveButton = RegistCompent<Button>("down/ReceiveButton");
            advertButton = RegistCompent<Button>("down/AdvertButton");
            rewardGoldText = RegistCompent<Text>("Reward/RewardGold/Text");
            rewardRmbText = RegistCompent<Text>("Reward/RewardRmb/Text");


            Register();
        }

        private void Init()
        {
            ///初始化金币
            ///初始化钻石
        }


        private void Register()
        {
            receiveButton.onClick.AddListener(OnClickReceiveButton);
            advertButton.onClick.AddListener(OnClickAdvertButton);

            EventManager.Add<Reward>(UICompentEventName.SuccessReward, UpdateReWard);
        }


        public override void Hide()
        {
            base.Hide();
            EventManager.Remove<Reward>(UICompentEventName.SuccessReward, UpdateReWard);
        }

        private void OnClickReceiveButton()
        {
            EventManager.Trigger<bool>(ToolEventName.AddReward, false);
            PageMgr.ShowPage<StartPage>();
            PageMgr.ClosePage<SuccessPage>();
        }

        private void OnClickAdvertButton()
        {
            ///需要看广告之类的
            EventManager.Trigger<bool>(ToolEventName.AddReward, true);
        }

        /// <summary>
        /// 更新奖励信息
        /// </summary>
        /// <param name="reward"></param>
        private void UpdateReWard(Reward reward)
        {
            ///播放领取动画
            ///


        }
    }
}