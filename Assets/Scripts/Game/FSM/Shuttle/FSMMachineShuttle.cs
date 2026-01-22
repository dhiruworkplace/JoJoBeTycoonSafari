/*******************************************************************
* FileName:     FSMMachineShuttle.cs
* Author:       Fan Zheng Yong
* Date:         2019-9-27
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    public class FSMMachineShuttle : FSMMachine
    {
        public EntityShuttle ownerEntity;

        public FSMMachineShuttle(EntityShuttle ownerEntity)
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
