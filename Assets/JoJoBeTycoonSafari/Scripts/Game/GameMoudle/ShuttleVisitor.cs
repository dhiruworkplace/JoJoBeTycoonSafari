/*******************************************************************
* FileName:     ShuttleVisitor.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-15
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    public class ShuttleVisitor //: IObjectPoolable
    {
        //public static ObjectPool<ShuttleVisitor> pool = new ObjectPool<ShuttleVisitor>();
        public int entityID;
        public EntityFuncType entityFuncType;

        public void Init(int entityID, EntityFuncType entityFuncType)
        {
            this.entityID = entityID;
            this.entityFuncType = entityFuncType;
        }

    }

}

