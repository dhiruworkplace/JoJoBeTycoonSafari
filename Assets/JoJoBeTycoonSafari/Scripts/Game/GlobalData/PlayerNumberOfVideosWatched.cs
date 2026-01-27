using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZooGame.GlobalData;
[Serializable]
public class PlayerNumberOfVideosWatched 
{
    /// <summary>
    /// 观看离线奖励视频次数
    /// </summary>
    public int playerLockOfflineAdsVideoCount ;
    /// <summary>
    /// 观看增益加倍奖励视频次数
    /// </summary>
    public int playerLockGainDoubleAdsVideoCount ;
    /// <summary>
    /// 观看增加游客奖励视频次数
    /// </summary>
    public int playerLockVisitorNumberAdsVideoCount;
    /// <summary>
    /// 观看观光加速奖励视频次数
    /// </summary>
    public int playerLockVisitorExpediteAdsVideoCount ;
    /// <summary>
    /// 观看售票加速奖励视频次数
    /// </summary>
    public int playerLockEntryExpediteAdsVideoCount ;
    /// <summary>
    /// 观看免费货币奖励视频次数
    /// </summary>
    public int playerLockFreeMoneyAdsVideoCount;
    /// <summary>
    /// 气球广告
    /// </summary>
    public int playerFreeItemAdsVideoCount;
    public PlayerNumberOfVideosWatched()
    {
        playerLockOfflineAdsVideoCount = 0;
        playerLockGainDoubleAdsVideoCount = 0;
        playerLockVisitorExpediteAdsVideoCount = 0;
        playerLockEntryExpediteAdsVideoCount = 0;
        playerLockFreeMoneyAdsVideoCount = 0;
        playerFreeItemAdsVideoCount = 0;
    }
    public void SetResetVideosWatchedData()
    {
        playerLockOfflineAdsVideoCount = 0;
        playerLockGainDoubleAdsVideoCount = 0;
        playerLockVisitorExpediteAdsVideoCount = 0;
        playerLockEntryExpediteAdsVideoCount = 0;
        playerLockFreeMoneyAdsVideoCount = 0;
        playerFreeItemAdsVideoCount = 0;
        GlobalDataManager.GetInstance().playerData.playerZoo.LastLogingDate_Day = System.DateTime.Now.Day;
    }
}
