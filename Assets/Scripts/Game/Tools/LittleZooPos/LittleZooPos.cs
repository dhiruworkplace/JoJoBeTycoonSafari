/*******************************************************************
* FileName:     LittleZooBuildinPosRoot.cs
* Author:       Fan Zheng Yong
* Date:         2019-8-17
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UFrame.Path.StraightLine;
using UnityEngine;

namespace Game.Tools
{
    public class LittleZooPos : MonoBehaviour
    {
        public void ProtecteRoot()
        {
            ShowPathLines.ProtecteNode(transform);
        }
    }
}

