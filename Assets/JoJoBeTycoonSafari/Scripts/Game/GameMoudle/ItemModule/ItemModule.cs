/*******************************************************************
* FileName:     ItemModule.cs
* Author:       Fan Zheng Yong
* Date:         2019-12-13
* Description:  
* other:    
********************************************************************/


using Game.GlobalData;
using Game.MessageCenter;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game
{
    public class ItemModule : GameModule
    {
        Dictionary<int, System.Action<int, Config.itemCell>> getItemCallbacks = new Dictionary<int, System.Action<int, Config.itemCell>>();
        Dictionary<int, System.Action<int, Config.itemCell>> useItemCallbacks = new Dictionary<int, System.Action<int, Config.itemCell>>();
        PlayerData playerData;
        public ItemModule(int orderID) : base(orderID) { }

        public override void Init()
        {
            MessageManager.GetInstance().Regist((int)GameMessageDefine.GetItem, OnGetItem);
            MessageManager.GetInstance().Regist((int)GameMessageDefine.UseItem, OnUseItem);

            playerData = GlobalDataManager.GetInstance().playerData;

            //获得道具
            getItemCallbacks.Add((int)(ItemType.Diamond), OnGetItemDiamond);
            getItemCallbacks.Add((int)(ItemType.Coin), OnGetItemCoin);
            getItemCallbacks.Add((int)(ItemType.Buff), OnGetItemBuff);
            getItemCallbacks.Add((int)(ItemType.Star), OnGetItemStar);

            //使用道具
            useItemCallbacks.Add((int)(ItemType.Diamond), OnUseItemDiamond);
            useItemCallbacks.Add((int)(ItemType.Coin), OnUseItemCoin);
            useItemCallbacks.Add((int)(ItemType.Buff), OnUseItemBuff);
            useItemCallbacks.Add((int)(ItemType.Star), OnUseItemStar);
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.GetItem, OnGetItem);
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.UseItem, OnUseItem);

            getItemCallbacks.Clear();
            useItemCallbacks.Clear();
            Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }
        }

        protected void OnGetItem(Message msg)
        {
            var _msg = msg as MessageInt;
            var itemCell = Config.itemConfig.getInstace().getCell(_msg.val);

            if (itemCell == null)
            {
#if UNITY_EDITOR
                string e = string.Format("没有这个道具 {0}", _msg.val);
                throw new System.Exception(e);
#endif
                return;
            }

            System.Action<int, Config.itemCell> callback = null;
            getItemCallbacks.TryGetValue(itemCell.itemtype, out callback);
            callback?.Invoke(_msg.val, itemCell);
        }

        protected void OnUseItem(Message msg)
        {
            var _msg = msg as MessageInt;
            var itemCell = Config.itemConfig.getInstace().getCell(_msg.val);
            if (itemCell == null)
            {
#if UNITY_EDITOR
                string e = string.Format("没有这个道具 {0}", _msg.val);
                throw new System.Exception(e);
#endif
                return;
            }

            if (itemCell.use != (int)ItemUse.Use_Effective)
            {
#if UNITY_EDITOR
                string e = string.Format("道具使用类型不正确 {0}", _msg.val);
                throw new System.Exception(e);
#endif
                return;
            }

            var items = playerData.playerZoo.itemList;
            //Item item = null;
            int itemID = Const.Invalid_Int;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == _msg.val)
                {
                    itemID = items[i];
                    break;
                }
            }

            if (itemID == Const.Invalid_Int)
            {
                string e = string.Format("用户没有这个道具 {0}", _msg.val);
#if UNITY_EDITOR
                throw new System.Exception(e);
#endif
                return;
            }

            //用户数据移除这个道具
            items.Remove(itemID);

            //使用道具
            System.Action<int, Config.itemCell> callback = null;
            useItemCallbacks.TryGetValue(itemCell.itemtype, out callback);
            callback?.Invoke(_msg.val, itemCell);

        }

        #region 获得道具回调
        protected void OnGetItemDiamond(int itemID, Config.itemCell itemCell)
        {
            int diamond = 0;
            if (!int.TryParse(itemCell.itemval, out diamond))
            {
#if UNITY_EDITOR
                string e = string.Format("Diamond 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            ItemUse eItemUse = (ItemUse)(itemCell.use);
            switch (eItemUse)
            {
                case ItemUse.Get_Effective:
                    SetValueOfPlayerData.Send((int)GameMessageDefine.SetDiamondOfPlayerData, diamond, 0, 0);
                    break;
                case ItemUse.Use_Effective:
                    //var item = ItemCellToItem(itemID, itemCell);
                    playerData.playerZoo.itemList.Add(itemID);
                    break;
                default:
#if UNITY_EDITOR
                    string e = string.Format("没有这种道具使用类型 {0}, {1}", itemID, itemCell.use);
                    throw new System.Exception(e);
#endif
                    return;
            }
        }

        protected void OnGetItemCoin(int itemID, Config.itemCell itemCell)
        {
            BigInteger coin = 0;
            if (!BigInteger.TryParse(itemCell.itemval, out coin))
            {
#if UNITY_EDITOR
                string e = string.Format("Coin 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            ItemUse eItemUse = (ItemUse)(itemCell.use);
            switch (eItemUse)
            {
                case ItemUse.Get_Effective:
                    SetValueOfPlayerData.Send((int)GameMessageDefine.SetCoinOfPlayerData, 0, coin, 0);
                    break;
                case ItemUse.Use_Effective:
                    //var item = ItemCellToItem(itemID, itemCell);
                    playerData.playerZoo.itemList.Add(itemID);
                    break;
                default:
#if UNITY_EDITOR
                    string e = string.Format("没有这种道具使用类型 {0}, {1}", itemID, itemCell.use);
                    throw new System.Exception(e);
#endif
                    return;
            }
        }

        protected void OnGetItemBuff(int itemID, Config.itemCell itemCell)
        {
            int buffID = 0;
            if (!int.TryParse(itemCell.itemval, out buffID))
            {
#if UNITY_EDITOR
                string e = string.Format("buffID 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            ItemUse eItemUse = (ItemUse)(itemCell.use);
            switch (eItemUse)
            {
                case ItemUse.Get_Effective:
                    BroadcastNum.Send((int)GameMessageDefine.AddBuff, buffID, 0, 0);
                    break;
                case ItemUse.Use_Effective:
                    //var item = ItemCellToItem(itemID, itemCell);
                    playerData.playerZoo.itemList.Add(itemID);
                    break;
                default:
#if UNITY_EDITOR
                    string e = string.Format("没有这种道具使用类型 {0}, {1}", itemID, itemCell.use);
                    throw new System.Exception(e);
#endif
                    return;

            }
        }

        protected void OnGetItemStar(int itemID, Config.itemCell itemCell)
        {
            int star = 0;
            if (!int.TryParse(itemCell.itemval, out star))
            {
#if UNITY_EDITOR
                string e = string.Format("Star 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            ItemUse eItemUse = (ItemUse)(itemCell.use);
            switch (eItemUse)
            {
                case ItemUse.Get_Effective:
                    SetValueOfPlayerData.Send((int)GameMessageDefine.SetStarOfPlayerData, star, 0, 0);
                    break;
                case ItemUse.Use_Effective:
                    //var item = ItemCellToItem(itemID, itemCell);
                    playerData.playerZoo.itemList.Add(itemID);
                    break;
                default:
#if UNITY_EDITOR
                    string e = string.Format("没有这种道具使用类型 {0}, {1}", itemID, itemCell.use);
                    throw new System.Exception(e);
#endif
                    return;
            }
        }
        #endregion

        #region 使用道具回调
        protected void OnUseItemDiamond(int itemID, Config.itemCell itemCell)
        {
            int diamond = 0;
            if (!int.TryParse(itemCell.itemval, out diamond))
            {
#if UNITY_EDITOR
                string e = string.Format("Diamond 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            SetValueOfPlayerData.Send((int)GameMessageDefine.SetDiamondOfPlayerData, diamond, 0, 0);
        }

        protected void OnUseItemCoin(int itemID, Config.itemCell itemCell)
        {
            BigInteger coin = 0;
            if (!BigInteger.TryParse(itemCell.itemval, out coin))
            {
#if UNITY_EDITOR
                string e = string.Format("Coin 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            SetValueOfPlayerData.Send((int)GameMessageDefine.SetCoinOfPlayerData, 0, coin, 0);
        }

        protected void OnUseItemBuff(int itemID, Config.itemCell itemCell)
        {
            int buffID = 0;
            if (!int.TryParse(itemCell.itemval, out buffID))
            {
#if UNITY_EDITOR
                string e = string.Format("buffID 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            BroadcastNum.Send((int)GameMessageDefine.AddBuff, buffID, 0, 0);
        }

        protected void OnUseItemStar(int itemID, Config.itemCell itemCell)
        {
            int star = 0;
            if (!int.TryParse(itemCell.itemval, out star))
            {
#if UNITY_EDITOR
                string e = string.Format("Star 数量不是数字型{0}, {1}", itemID, itemCell.itemval);
                throw new System.Exception(e);
#endif
                return;
            }

            SetValueOfPlayerData.Send((int)GameMessageDefine.SetStarOfPlayerData, star, 0, 0);
        }
        #endregion

        public static Item ItemCellToItem(int itemID, Config.itemCell cellCell)
        {
            var item = new Item();
            item.itemID = itemID;
            item.itemType = cellCell.itemtype;
            item.itemVal = cellCell.itemval;
            item.itemUse = cellCell.use;
            return item;
        }
    }
}
