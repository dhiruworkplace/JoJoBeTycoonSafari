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
    public class LittleZooBuildinPosRoot : MonoBehaviour
    {
        public bool isYValid = false;
        public void ProtecteRoot()
        {
            ShowPathLines.ProtecteNode(transform);
            for (int i = 0; i < transform.childCount; i++)
            {
                var littleZoo = transform.GetChild(i);
                ShowPathLines.ProtecteNode(littleZoo);
            }
        }
    }
}

