/*******************************************************************
* FileName:     VisitorFSMMachine.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-8
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    public class FSMMachineVisitor : FSMMachine
    {
        public EntityVisitor ownerEntity;

        public FSMMachineVisitor(EntityVisitor ownerEntity)
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
