using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{
    /// <summary>
    /// 保存关卡信息
    /// </summary>
    public class LevelData
    {
        /// <summary>
        /// 是否首次通关
        /// 1 表示是  其他的数字表示否
        /// </summary>
        public static int isStageCleared = -1;

        /// <summary>
        /// 首次通关金币奖励
        /// </summary>
        public static int firstGoldReward;

        /// <summary>
        /// 复刷金币奖励
        /// </summary>
        public static int repeatGoldReward;

        /// <summary>
        /// 首次通关碎片奖励ID
        /// </summary>
        public static int firstDropWard;

        /// <summary>
        /// 首通碎片数量
        /// </summary>
        public static int firstDropNum;

        /// <summary>
        /// 复刷碎片掉落概率 
        /// 每次掉 1 个
        /// </summary>
        public static int repeatDropWard;

        /// <summary>
        /// 点击翻倍按钮之后,翻倍的数量
        /// </summary>
        public static int wardDouble;

        /// <summary>
        /// 当前关卡的钻石数量
        /// </summary>
        public static int RMB;

        /// <summary>
        /// 计算当前的关卡掉落碎片的list
        /// </summary>
        public void CalculatorDropList()
        {

        }
    }
}