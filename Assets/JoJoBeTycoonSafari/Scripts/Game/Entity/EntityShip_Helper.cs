/*******************************************************************
* FileName:     EntityShip_Helper.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-28
* Description:  
* other:    
********************************************************************/


using UFrame;
using UFrame.BehaviourFloat;
using UFrame.EntityFloat;
using Logger;
using System.Collections.Generic;
using UnityEngine;
using UFrame.MessageCenter;
using Game.MessageCenter;
using Game.Path.StraightLine;

namespace Game
{
    public partial class EntityShip : EntityMovable
    {
        /// <summary>
        /// 轮船开过来，下指定数量的游客
        /// </summary>
        /// <param name="maxSpawnVisitorNum"></param>
        public static void GetoffVisitor(int maxSpawnVisitorNum)
        {
            LogWarp.LogError("GetoffVisitor");
            var entity = EntityManager.GetInstance().GetRandomEntity(ResType.Ship, EntityFuncType.Ship) as EntityShip;
            EntityManager.GetInstance().AddToEntityMovables(entity);
            if (entity.followPath == null)
            {
                entity.followPath = new FollowPath();
            }
            entity.moveSpeed = Config.globalConfig.getInstace().ZooShipSpeed;
            var path = PathManager.GetInstance().GetPath(Config.globalConfig.getInstace().AdvertVisitorInto_1);
            entity.position = path[0];
            entity.followPath.Init(entity, path, path[0], 0, entity.moveSpeed, false);
            entity.maxSpawnVisitorNum = maxSpawnVisitorNum;
            entity.visitorGetOffInterval = Math_F.FloatToInt1000(Config.globalConfig.getInstace().ShipVisitorGetOffInterval);
            if (entity.fsmMachine == null)
            {
                entity.fsmMachine = new FSMMachineShip(entity);

                entity.fsmMachine.AddState(new StateShipGoto((int)ShipState.Goto,
                    entity.fsmMachine));
                entity.fsmMachine.AddState(new StateShipGoback((int)ShipState.Goback,
                    entity.fsmMachine));

                entity.fsmMachine.SetDefaultState((int)ShipState.Goto);
            }
            else
            {
                entity.fsmMachine.GotoState((int)ShipState.Goto);
            }
            entity.Active();
        }
    }

}
