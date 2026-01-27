using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using ZooGame.GlobalData;
using ZooGame;
using UFrame;
using ZooGame.MessageCenter;
using System;
using UFrame.MessageCenter;

namespace LittleGame {

    /// <summary>
    /// 小游戏的逻辑类
    /// </summary>
    public class LittleGameLogic : MonoBehaviour
    {

        #region unity callback

        void Start()
        {
            EventManager.Add(ToolEventName.LittleGameEndSuccess, GameEndSuccess);
            EventManager.Add(ToolEventName.LittleGameEndFail, GameEndFail);
            EventManager.Add<bool>(ToolEventName.AddReward, AddReward);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.GetUnlockAnimalsReply, OnGetUnlockAnimalsReply);
        }



        private void OnDestroy()
        {
            EventManager.Remove(ToolEventName.LittleGameEndSuccess, GameEndSuccess);
            EventManager.Remove(ToolEventName.LittleGameEndFail, GameEndFail);
            EventManager.Remove<bool>(ToolEventName.AddReward, AddReward);
        }

        #endregion

        private void GameEndFail()
        {
            PageMgr.ClosePage<GamePage>();
            PageMgr.ShowPage<FailedPage>();
            //重置小动物数量
            LittleController.Instance.currentAnimalsCount = LittleGameData.LastLevelAnimalsNumber = LittleController.Instance.littleGameInit.GetCurrentLevelAnimalsNum(LittleController.CurrentLevel);
            Debug.Log("当前关卡的小动物数量为 " + LittleController.Instance.currentAnimalsCount);
        }

        private void GameEndSuccess()
        {
            PageMgr.ClosePage<GamePage>();
            PageMgr.ShowPage<SuccessPage>();
            ///获取当前关卡奖励信息
            LittleController.Instance.littleGameInit.InitLevelReWard(LittleController.CurrentLevel);

            Reward reward = new Reward();

            ///获取当前是否首通
            ///当前没有首次通关的设计了
            reward.Coin = LevelData.firstGoldReward;
            reward.RMB = LevelData.RMB;
            

            //通知胜利界面显示奖励信息
            EventManager.Trigger<Reward>(UICompentEventName.SuccessReward, reward);

            MessageManager.GetInstance().Send((int)GameMessageDefine.GetUnlockAnimals);

        }

        private void OnGetUnlockAnimalsReply(Message obj)
        {
            Debug.Log(obj.messageID + "消息ID");
        }

        /// <summary>
        /// 真正增加奖励
        /// </summary>
        /// <param name="isDouble">是否获得了翻倍奖励</param>
        public void AddReward(bool isDouble)
        {


            //保存数据到服务器

            //PlayerZoo.littleLevel = LittleController.CurrentLevel;

            //PlayerZoo.hasAnimalsNum = LittleController.Instance.currentAnimalsCount;
        }


    }
}