using ZooGame.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace ZooGame.MessageCenter
{
    /// <summary>
    /// 购买商品的消息体
    /// </summary>
    public class SetAchievementObjectData : Message
    {
        /// <summary>
        /// 所属ID　　
        /// 动物栏ID ：
        /// 非消耗商品ID：
        /// 消耗品ID：
        /// </summary>
        public int belongID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public int goodsID;
        /// <summary>
        /// 采购数量
        /// </summary>
        public int purchaseQuantity;
        /// <summary>
        /// 花费金额（大数据）
        /// </summary>
        public System.Numerics.BigInteger bigIntCostVal;

        public int littleZooID;
        /// <summary>
        /// 花费金额
        /// </summary>
        public int costVal;

        public static ObjectPool<SetAchievementObjectData> pool = new ObjectPool<SetAchievementObjectData>();
        /// <summary>
        /// 消息Init 初始化
        /// </summary>
        /// <param name="messageID">消息ID</param>
        /// <param name="belongID">商品归属类ID</param>
        /// <param name="goodsID">商品ID</param>
        /// <param name="purchaseQuantity">采购数量</param>
        /// <param name="bigIntCostVal">花费金额（大数据）</param>
        public void Init(int messageID, int belongID,int goodsID,int purchaseQuantity,
            System.Numerics.BigInteger bigIntCostVal,int costVal,int littleZooID)
        {
            this.messageID = messageID;
            this.belongID = belongID;
            this.goodsID = goodsID;
            this.purchaseQuantity = purchaseQuantity;
            this.bigIntCostVal = bigIntCostVal;
            this.costVal = costVal;
            this.littleZooID = littleZooID;
        }

        public override void Release()
        {
            pool.Delete(this);
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="messageID">消息ID</param>
        /// <param name="belongID">商品类型</param>
        /// <param name="goodsID">商品ID</param>
        /// <param name="purchaseQuantity">采购数量</param>
        /// <param name="bigIntCostVal">花费金额(大额数据)</param>
        /// <param name="costVal">花费金额</param>
        /// <returns></returns>
        public static SetAchievementObjectData Send(int messageID, int belongID, int goodsID, 
            int purchaseQuantity, System.Numerics.BigInteger bigIntCostVal, int costVal,int littleZooID)
        {
            var msg = pool.New();
            msg.Init(messageID, belongID, goodsID, purchaseQuantity, bigIntCostVal,costVal, littleZooID);
            MessageManager.GetInstance().Send(msg);
            return msg;
        }

        public override string ToString()
        {
            return string.Format("SetValueOfPlayerData messageID = {0}, belongID={1}, goodsID={2} , " +
                "purchaseQuantity={3} , bigIntCostVal={4}，costVal={5}",
                messageID, belongID, goodsID, purchaseQuantity, bigIntCostVal, costVal);
        }
    }
}
