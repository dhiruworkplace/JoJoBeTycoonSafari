using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using ZooGame.GlobalData;
using Logger;
using UFrame;
using ZooGame.MessageCenter;
using System;
using UFrame.MessageCenter;

namespace ZooGame
{
    /// <summary>
    /// 收集Module层：存储所有和收集有关的数据Data
    /// 数据源：         AchievementDataDic
    /// 监听消息：       GameMessageDefine.SetAchievementObject
    /// 添加实现方法：   OnSetAchievementObjectData
    /// </summary>
    public class AchievementModule : GameModule
    {

        public AchievementModule(int orderID) : base(orderID) { }

        public override void Init()
        {
            //监听收到收集物品消息
            MessageManager.GetInstance().Regist((int)GameMessageDefine.SetAchievementObject, this.OnSetAchievementObjectData);   
        }
        //接受消息进行数据修改
        private void OnSetAchievementObjectData(Message obj)
        {
            var _mag = obj as SetAchievementObjectData;
            LogWarp.LogErrorFormat("测试：收到收集object：  商品归属类ID：{1}，商品ID：{2}， 采购数量：{3}，花费金额：{4}",_mag.belongID,_mag.goodsID,_mag.purchaseQuantity,_mag.bigIntCostVal);

        }

        // Update is called once per frame
        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.SetAchievementObject, this.OnSetAchievementObjectData);

        }
        public override void Tick(int deltaTimeMS)
        {
        }
    }
}
