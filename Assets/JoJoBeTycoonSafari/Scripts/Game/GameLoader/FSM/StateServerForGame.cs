/*******************************************************************
* FileName:     StateLoadOrgSceneForGame.cs
* Author:       Fan Zheng Yong
* Date:         2019-12-18
* Description:  
* other:    
********************************************************************/


using ZooGame.Path.StraightLine;
using UFrame;
using UFrame.MessageCenter;
using Logger;
using ZooGame.MessageCenter;
using System;
using UnityEngine;
using System.Collections.Generic;
using UFrame.MiniGame;
using ZooGame.GlobalData;

namespace ZooGame
{
    /// <summary>
    /// 服务器相关操作
    /// </summary>
    public class StateServerForGame : FSMState
    {
        bool isToStateLoadOrg = false;

        public StateServerForGame(int stateName, FSMMachine fsmCtr) :
            base(stateName, fsmCtr)
        {
        }

        public override void Enter(int preStateName)
        {
            base.Enter(preStateName);

            isToStateLoadOrg = true;
        }

        public override void Tick(int deltaTimeMS)
        {
        }

        public override void Leave()
        {
            base.Leave();
        }

        public override void AddAllConvertCond()
        {
            AddConvertCond((int)GameLoaderState.LoadOrgScene, ToStateLoadOrg);
        }

        protected bool ToStateLoadOrg()
        {
            return isToStateLoadOrg;
        }
    }
}
