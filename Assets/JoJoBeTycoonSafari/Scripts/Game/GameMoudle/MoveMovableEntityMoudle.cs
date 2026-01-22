/*******************************************************************
* FileName:     MoveMovableEntityMoudle.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-9
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MoveMovableEntityMoudle : GameModule
    {
        public MoveMovableEntityMoudle(int orderID) : base(orderID) { }

        public override void Init()
        {
            //this.Run();
        }

        public override void Release()
        {
            this.Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }

            EntityManager.GetInstance().Tick(deltaTimeMS);
        }
    }



}
