/*******************************************************************
* FileName:     GameMoudle.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-6
* Description:  
* other:    
********************************************************************/


using UFrame;

namespace ZooGame
{
    public abstract class GameModule : TickBase
    {
        public GameModule(int orderID)
        {
            this.orderID = orderID;
        }
        public int orderID = 0;
        public abstract void Init();
        public abstract void Release();
    }
}

