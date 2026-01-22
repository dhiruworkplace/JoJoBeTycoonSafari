using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{

    /// <summary>
    /// 数据存储类
    /// </summary>
    public class LittleGameData
    {
        /// <summary>
        /// 当前拥有的金币数量
        /// 这个值不应该是在这里的
        /// </summary>
        public static int CurrentCoin;

        /// <summary>
        /// 上一关的小动物数量
        /// </summary>
        public static int LastLevelAnimalsNumber = 5;
    }


    /// <summary>
    /// 保存需要更新的UI对象名称
    /// </summary>
    public struct UICompentEventName
    {
        public static string CoinText = "CoinText";
        public static string LevelText = "LevelText";
        /// <summary>
        /// 游戏中进度条的数值
        /// </summary>
        public static string ScheduleSlider = "ScheduleSlider";

        /// <summary>
        /// 游戏胜利后的结算奖励
        /// </summary>
        public static string SuccessReward = "SuccessReward";
    }
    /// <summary>
    /// 保存需要的工具事件
    /// </summary>
    public struct ToolEventName
    {
        public static string CarCollideEvent = "CarCollideEvent";
        public static string LittleGameStart = "LittleGameStart";
        public static string LittleGameEndSuccess = "LittleGameEndSuccess";
        public static string LittleGameEndFail = "LittleGameEndFail";
        public static string IsInitGameDone = "IsInitGameDone";
        public static string AddReward = "AddReward";
        /// <summary>
        /// 检查是否需要填补空位
        /// </summary>
        public static string NeedCheckSpace = "NeedCheckSpace";
    }

    /// <summary>
    /// 保存车子在移动的时候需要的起点和终点
    /// </summary>
    public struct CarTargetPosition
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public int dir;
    }

    public struct Reward
    {
        public float Coin;
        /// <summary>
        /// 钻石
        /// </summary>
        public float RMB;

        /// <summary>
        /// 获得奖励的倍数
        /// </summary>
        public int Multiple;

        /// <summary>
        /// 关卡奖励碎片的list
        /// </summary>
        public List<int> DropList;
    }
}