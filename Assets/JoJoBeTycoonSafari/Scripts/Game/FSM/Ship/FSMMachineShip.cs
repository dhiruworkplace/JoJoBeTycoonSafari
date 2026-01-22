/*******************************************************************
* FileName:     FSMMachineShip.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-28
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    public class FSMMachineShip : FSMMachine
    {
        public EntityShip ownerEntity;

        public FSMMachineShip(EntityShip ownerEntity)
        {
            this.ownerEntity = ownerEntity;
        }

        public override void Release()
        {
            ownerEntity = null;
            base.Release();
        }
    }

}
