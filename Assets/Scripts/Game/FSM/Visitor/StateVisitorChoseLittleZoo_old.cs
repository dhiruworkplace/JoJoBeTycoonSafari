///*******************************************************************
//* FileName:     StateVisitorChoseLittleZoo.cs
//* Author:       Fan Zheng Yong
//* Date:         2019-8-9
//* Description:  
//* other:    
//********************************************************************/


//using System.Collections;
//using System.Collections.Generic;
//using UFrame;
//using UnityEngine;
//using Logger;
//using Game.GlobalData;
//using Game.MessageCenter;
//using UFrame.MessageCenter;
//using Game.Path.StraightLine;

//namespace Game
//{
//    /// <summary>
//    /// 选择动物栏逻辑
//    /// 门口排队结束后 或者 在观光位cd结束后 进入
//    /// 如果是在排队结束后进入，这时游客已经走到第一个点上。
//    /// 如果是从观光位进入，这时游客还在观光位上
//    /// 
//    /// 1.知道当前所在建筑位置的基础上，从组概率表中，获得要去的建筑。
//    /// 2.向动物栏模块请求进入
//    /// 3.根据请求返回结果做相应处理
//    /// (1)成功走入等待位, 转下一个状态,等待位 StateVisitorStayWaitSeat
//    /// (2)不成功请求组内其他的动物栏
//    /// (3)不成功请求下一个组
//    /// (4)组内，组外都没有可选的动物，转另一个状态,离开 StateVisitorLeaveNonLittleZoo
//    /// </summary>
//    public class StateVisitorChoseLittleZoo_old : FSMState
//    {
//        bool isToStateVisitorStayWaitSeat = false;

//        bool isToStateVisitorLeaveNonLittleZoo = false;

//        bool isToStateVisitorCrossGroupPath = false;

//        /// <summary>
//        /// 是否到达路的起点
//        /// </summary>
//        bool isArrivedStartOfPath = false;

//        /// <summary>
//        /// 是否到达动物栏
//        /// </summary>
//        bool isArrivedLittleZoo = false;

//        string pathOfGotoLittleZoo = "";

//        /// <summary>
//        /// 等待位的具体位置
//        /// </summary>
//        Vector3 waitPos;

//        /// <summary>
//        /// 本状态申请记录
//        /// key group value申请次数
//        /// </summary>
//        Dictionary<int, int> groupApplyRecord = new Dictionary<int, int>();

//        public StateVisitorChoseLittleZoo_old(int stateName, FSMMachine fsmCtr) :
//            base(stateName, fsmCtr) { }

//        public override void Realse()
//        {
//            groupApplyRecord.Clear();
//            base.Realse();
//        }

//        public override void Enter(int preStateName)
//        {
//            //LogWarp.LogError("StateVisitorChoseLittleZoo.Enter");
//            base.Enter(preStateName);

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} StateVisitorChoseLittleZoo.Enter", entity.entityID);
//            DebugFile.GetInstance().MarkGameObject(entity.mainGameObject, "visitor_{0}_{1}_{2}", entity.entityID, (VisitorState)this.preStateName, (VisitorState)this.stateName);

//            isToStateVisitorStayWaitSeat = false;
//            isToStateVisitorLeaveNonLittleZoo = false;
//            isToStateVisitorCrossGroupPath = false;
//            isArrivedLittleZoo = false;

//            isArrivedStartOfPath = false;
//            if (preStateName == (int)VisitorState.EntryQueue)
//            {
//                isArrivedStartOfPath = true;
//            }

//            entity.indexInWaitQueue = Const.Invalid_Int;
//            entity.littleZooIDWaitQueue = Const.Invalid_Int;
//            entity.isApproveVisitSeat = false;
//            entity.indexInVisitQueue = Const.Invalid_Int;
//            entity.littleZooIDVisitQueue = Const.Invalid_Int;

//            groupApplyRecord.Clear();

//            MessageManager.GetInstance().Regist((int)GameMessageDefine.AddVisitorToLittleZooResult, this.OnAddVisitorToLittleZooResult);
//            MessageManager.GetInstance().Regist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
//            MessageManager.GetInstance().Regist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);

//            //将去哪组逻辑
//            //wouldGotoBuildingGroupID由本状态的上一个状态指定
//            //if (this.preStateName != (int)VisitorState.EntryQueue)
//            if (this.preStateName != (int)VisitorState.EnterZoo)
//            {
//                NextGroup_New(entity, entity.wouldGotoBuildingGroupID, this.groupApplyRecord, false);
//            }
//            else
//            {
//                if (!this.WouldGotoGroupByGotoweight(entity.wouldGotoBuildingGroupID))
//                {
//                    LogWarp.LogFormat("{0} 组goto概率决定不去group {1} 准备离开", entity.entityID, entity.wouldGotoBuildingGroupID);
//                    DebugFile.GetInstance().WriteKeyFile(entity.entityID, 
//                        "{0} 组goto概率决定不去group {1} 准备离开 tickCount={2}", 
//                        entity.entityID, entity.wouldGotoBuildingGroupID, GameManager.GetInstance().tickCount);
//                    PrepareLeaveZoo();
//                    return;
//                }
//                LogWarp.LogFormat("组goto概率决定去group {0}, 提出申请", entity.wouldGotoBuildingGroupID);

//                int endID = GetApplyBuildingID(entity, entity.wouldGotoBuildingGroupID);

//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} ApplyLittleZoo A, tickCount={1}",
//                    entity.entityID, GameManager.GetInstance().tickCount);
//                this.ApplyLittleZoo(entity.wouldGotoBuildingGroupID, endID, entity.entityID);
//            }
//        }

//        public override void Leave()
//        {
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.AddVisitorToLittleZooResult, this.OnAddVisitorToLittleZooResult);
//            MessageManager.GetInstance().UnRegist((int)UFrameBuildinMessage.Arrived, this.OnArrived);
//            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.WaitSeatToVisitSeat, OnWaitSeatToVisitSeat);

//            base.Leave();
//        }

//        public override void AddAllConvertCond()
//        {
//            AddConvertCond((int)VisitorState.StayWaitSeat, this.ToStateVisitorStayWaitSeat);
//            AddConvertCond((int)VisitorState.LeaveNonLittleZoo, this.ToStateVisitorLeaveNonLittleZoo);
//            AddConvertCond((int)VisitorState.CrossGroupPath, ToStateVisitorCrossGroupPath);
//        }

//        public override void Tick(int deltaTimeMS)
//        {
//        }

//        protected bool ToStateVisitorStayWaitSeat()
//        {
//            return this.isToStateVisitorStayWaitSeat;
//        }

//        protected bool ToStateVisitorLeaveNonLittleZoo()
//        {
//            return isToStateVisitorLeaveNonLittleZoo;
//        }

//        protected bool ToStateVisitorCrossGroupPath()
//        {
//            return isToStateVisitorCrossGroupPath;
//        }

//        /// <summary>
//        /// 组概率判定是否去这组
//        /// </summary>
//        /// <param name="groupID"></param>
//        /// <returns></returns>
//        protected bool WouldGotoGroupByGotoweight(int groupID)
//        {
//            float r = Random.Range(0f, 1f);
//            int weight = 0;
//            var cell = Config.groupConfig.getInstace().getCell(groupID);
//            if (null == cell)
//            {
//                string e = string.Format("WouldGotoGroupByGotoweight 没找到组{0}", groupID);
//                throw new System.Exception(e);
//            }
//            weight = cell.gotoweight;

//            return (r * 100) <= weight;
//        }

//        /// <summary>
//        /// 再次访问概率判定是否去这组
//        /// </summary>
//        /// <param name="groupID"></param>
//        /// <returns></returns>
//        protected bool WouldGotoGroupByAgainweight(int groupID)
//        {
//            float r = Random.Range(0f, 1f);
//            int weight = 0;
//            var cell = Config.groupConfig.getInstace().getCell(groupID);
//            if (null == cell)
//            {
//                string e = string.Format("WouldGotoGroupByAgainweight 没找到组{0}", groupID);
//                throw new System.Exception(e);
//            }
//            weight = cell.againweight;

//            return (r * 100) <= weight;
//        }

//        /// <summary>
//        /// 从概率表中去掉游览过的，然后通过概率计算要去哪个动物栏
//        /// </summary>
//        /// <param name="entity"></param>
//        /// <param name="groupID"></param>
//        /// <returns></returns>
//        protected int GetApplyBuildingID(EntityVisitor entity, int groupID)
//        {
//            List<int> visitedLittleZooIDs = null;
//            entity.visitedGroupMap.TryGetValue(groupID, out visitedLittleZooIDs);
            
//            //visitedLittleZooIDs可能为空，没有游览过
//            var wouldGotoWeights = GlobalDataManager.GetInstance().logicTableGroup.GetWouldGotoWeights(groupID, visitedLittleZooIDs);

//            //Dictionary<int, int> weightStartIDMap = null;
//            LittleZooWeight weightStartIDMap = null;
//            GlobalDataManager.GetInstance().logicTableGroup.weightStartIDMaps.TryGetValue(groupID, out weightStartIDMap);
//            if (weightStartIDMap == null)
//            {
//                string e = string.Format("weightStartIDMap 没找到{0}", groupID);
//                throw new System.Exception(e);
//            }

//            //int weight = Math_F.TableProbability(wouldGotoWeights);
//            //int startID = Const.Invalid_Int;
//            //if (!weightStartIDMap.TryGetValue(weight, out startID))
//            //{
//            //    string e = string.Format("startID 没找到{0}", weight);
//            //    throw new System.Exception(e);
//            //}

//            int idx = Const.Invalid_Int;
//            int weight = Math_F.TableProbability(wouldGotoWeights, ref idx);
//            int startID = Const.Invalid_Int;
            
//            startID = weightStartIDMap.GetLittleZooID(idx);

//            return startID;
//        }
        
//        protected void OnAddVisitorToLittleZooResult(Message msg)
//        {
//            var _msg = msg as AddVisitorToLittleZooResult;
//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            if (entity.entityID != _msg.entityID)
//            {
//                return;
//            }

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 收到了{1}, tickCount={2}", 
//                entity.entityID, _msg, GameManager.GetInstance().tickCount);
//            if (!_msg.result)
//            {
//                LogWarp.LogFormat("动物栏{0}, 等待位已经满", _msg.littleZooID);
//                NextGroup_New(entity, entity.wouldGotoBuildingGroupID, this.groupApplyRecord, true);
//                return;
//            }

//            //只要申请成功了，必去等待位，记录
//            entity.indexInWaitQueue = _msg.indexInQueue;
//            entity.littleZooIDWaitQueue = _msg.littleZooID;
//            LogWarp.LogFormat("VisitedRecord {0}, {1}, {2}", entity.entityID, entity.littleZooIDWaitQueue, entity.indexInWaitQueue);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} VisitedRecord  {1}, {2}", entity.entityID, entity.littleZooIDWaitQueue, entity.indexInWaitQueue);
//            VisitedRecord(entity);

//            LogWarp.LogFormat("{0} {1}", entity.stayBuildingID, _msg.littleZooID);
//            if (IsCrossGroup(_msg.littleZooID, entity))
//            {
//                LogWarp.LogFormat("跨组了 {0} {1}", entity.stayBuildingID, _msg.littleZooID);
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} 跨组了 {1} {2}", entity.entityID, entity.stayBuildingID, _msg.littleZooID);
//                isToStateVisitorCrossGroupPath = true;
//                return;
//            }

//            waitPos = _msg.waitPos;

//            this.pathOfGotoLittleZoo = EntityVisitor.GetPath(entity.stayBuildingID, _msg.littleZooID);
//            if (string.IsNullOrEmpty(pathOfGotoLittleZoo))
//            {
//                string e = string.Format("{0} -> {1} 没有找到路!!!!!!!!!!", entity.stayBuildingID, _msg.littleZooID);
//                throw new System.Exception(e);
//            }
//            LogWarp.LogFormat("动物栏{0}, 等待位获得成功, 排号{1}， 路径{2}", _msg.littleZooID, _msg.indexInQueue, pathOfGotoLittleZoo);

//            if (this.isArrivedStartOfPath)
//            {
//                EntityVisitor.GodownPath(entity, pathOfGotoLittleZoo);
//                return;
//            }

//            EntityVisitor.GotoStartOfPath(entity, pathOfGotoLittleZoo);
//        }

//        protected void OnArrived(Message msg)
//        {
//            var _msg = msg as MessageArrived;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;
//            //自己的entity
//            if (_msg.followPath.ownerEntity.entityID != entity.entityID)
//            {
//                return;
//            }

//            //先判断走到起点
//            if (_msg.followPath.isArrivedEnd && !isArrivedStartOfPath)
//            {
//                isArrivedStartOfPath = true;
//                EntityVisitor.GodownPath(entity, pathOfGotoLittleZoo);
//                return;
//            }
            
//            if (_msg.followPath.isArrivedEnd && !isArrivedLittleZoo)
//            {
//                LogWarp.Log("到达动物栏,准备走向等待位");
//                isArrivedLittleZoo = true;

//                EntityVisitor.GotoNextPos(entity, this.waitPos);
//                return;
//            }

//            if (_msg.followPath.isArrivedEnd)
//            {
//                LogWarp.Log("到达动物栏等待位,转下一个状态");
//                isToStateVisitorStayWaitSeat = true;
//            }
//        }

//        /// <summary>
//        /// 在这个状态收到这个消息，被通知走到观光位，说明在去等待位的途中就收到了，
//        /// 直接设置标志位，在下一个状态处理
//        /// </summary>
//        /// <param name="msg"></param>
//        protected void OnWaitSeatToVisitSeat(Message msg)
//        {
//            var _msg = msg as WaitSeatToVisitSeat;

//            var entity = (this.fsmCtr as FSMMachineVisitor).ownerEntity;

//            //不是自己的
//            if (_msg.entityID != entity.entityID)
//            {
//                return;
//            }

//            LogWarp.LogFormat("途中收到去观光位 排位{0}", _msg.indexInVisitQueue);
//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, 
//                "{0}在状态StateVisitorChoseLittleZoo 收到{1}, tickCount={2}",
//                entity.entityID, msg, GameManager.GetInstance().tickCount);

//            entity.isApproveVisitSeat = true;
//            entity.indexInVisitQueue = _msg.indexInVisitQueue;
//            entity.littleZooIDVisitQueue = _msg.littleZooID;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, 
//                "{0}在状态StateVisitorChoseLittleZoo, 收到{1}, entity.followPath.isArrivedEnd={2}, tickCount={3}",
//                entity.entityID, msg, entity.followPath.isArrivedEnd, GameManager.GetInstance().tickCount);

//            if (entity.followPath.isArrivedEnd)
//            {
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0}在状态StateVisitorChoseLittleZoo, 并且处于到达状态 收到{1}",
//                    entity.entityID, msg);
//            }
//        }

//        protected void PrepareLeaveZoo()
//        {
//            LogWarp.Log("PrepareLeaveZoo");
//            isToStateVisitorLeaveNonLittleZoo = true;
//        }

//        protected void ApplyLittleZoo(int groupID, int littleZooID, int entityID)
//        {
//            //发消息
//            var msg = AddVisitorToLittleZoo.Send(groupID, littleZooID, entityID);
//            DebugFile.GetInstance().WriteKeyFile(entityID, "{0} 发送{1}, tickCount={2}",
//                entityID, msg, GameManager.GetInstance().tickCount);

//            //做记录
//            int applyTime = 0;
//            if (this.groupApplyRecord.TryGetValue(groupID, out applyTime))
//            {
//                applyTime++;
//                groupApplyRecord[groupID] = applyTime;

//                return;
//            }

//            this.groupApplyRecord.Add(groupID, 1);
//        }

//        protected void VisitedRecord(EntityVisitor entity)
//        {
//            List<int> littleZooList = null;
//            if (!entity.visitedGroupMap.TryGetValue(entity.wouldGotoBuildingGroupID, out littleZooList))
//            {
//                littleZooList = new List<int>();
//                entity.visitedGroupMap.Add(entity.wouldGotoBuildingGroupID, littleZooList);
//            }
//            if (littleZooList.Contains(entity.littleZooIDWaitQueue))
//            {
//                string e = string.Format("{0} 重复记录观光 {1}", entity.entityID, entity.littleZooIDWaitQueue);
//                throw new System.Exception(e);
//            }
//            littleZooList.Add(entity.littleZooIDWaitQueue);
//        }

//        protected void NextGroup_New(EntityVisitor entity, int groupID, Dictionary<int, int> applyRecord, bool isCheckAgain = false)
//        {
//            int nextGroupID = Const.Invalid_Int;
//            bool isVisitedAll = false;
//            bool isApplyTooMuch = false;
//            bool isAgain = false;
//            while (true)
//            {
//                isVisitedAll = EntityVisitor.IsVisitedAll(entity, groupID);
//                isApplyTooMuch = EntityVisitor.IsApplyTooMuch(groupID, applyRecord);

//                if (isCheckAgain)
//                {
//                    isAgain = WouldGotoGroupByAgainweight(groupID);
//                    if (!isVisitedAll && !isApplyTooMuch && isAgain)
//                    {
//                        break;
//                    }
//                }
//                else
//                {
//                    if (!isVisitedAll && !isApplyTooMuch)
//                    {
//                        break;
//                    }
//                }

//                LogWarp.LogFormat("这次申请次数过多或者，都游览过了{0}", groupID);
//                bool isLastGroupID = false;
//                nextGroupID = GlobalDataManager.GetInstance().logicTableGroup.GetNextGroupID_New(groupID, out isLastGroupID);
//                if (nextGroupID == Const.Invalid_Int)
//                {
//                    string e = string.Format("取下一组异常 {0}!!!!!", groupID);
//                    throw new System.Exception(e);
//                }

//                if (isLastGroupID)
//                {
//                    DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} isLastGroupID == true 准备离开 tickCount ={1}", 
//                        entity.entityID, GameManager.GetInstance().tickCount);
//                    PrepareLeaveZoo();
//                    return;
//                }

//                groupID = nextGroupID;
//            }

//            if (!this.WouldGotoGroupByGotoweight(groupID))
//            {
//                LogWarp.LogFormat("组goto概率决定不去group {0}, 准备离开", groupID);
//                DebugFile.GetInstance().WriteKeyFile(entity.entityID, 
//                    "组goto概率决定不去group {0}, 准备离开 tickCount={1}", 
//                    groupID, GameManager.GetInstance().tickCount);
//                PrepareLeaveZoo();
//                return;
//            }

//            int endID = GetApplyBuildingID(entity, groupID);
//            entity.wouldGotoBuildingGroupID = groupID;

//            DebugFile.GetInstance().WriteKeyFile(entity.entityID, "{0} ApplyLittleZoo B, tickCount={1}",
//                entity.entityID, GameManager.GetInstance().tickCount);
//            this.ApplyLittleZoo(groupID, endID, entity.entityID);
//        }

//        /// <summary>
//        /// 判定要去的路是不是跨组
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        protected bool IsCrossGroup(int littleZooID, EntityVisitor entity)
//        {
//            var sortedLittleZooIDs = GlobalDataManager.GetInstance().logicTableGroup.sortedLittleZooIDs;
//            int stayGroupID = Const.Invalid_Int;
//            int gotoGroupID = Const.Invalid_Int;

//            if (entity.stayBuildingID == 1000)
//            {
//                stayGroupID = LogicTableGroup.EntryGroupID;
//            }

//            foreach (var kv in sortedLittleZooIDs)
//            {
//                if (kv.Value.Contains(entity.stayBuildingID) && stayGroupID != LogicTableGroup.EntryGroupID)
//                {
//                    stayGroupID = kv.Key;
//                }

//                if (kv.Value.Contains(littleZooID))
//                {
//                    gotoGroupID = kv.Key;
//                }

//                if (stayGroupID != Const.Invalid_Int && gotoGroupID != Const.Invalid_Int)
//                {
//                    break;
//                }
//            }

//            if (gotoGroupID == Const.Invalid_Int || stayGroupID == Const.Invalid_Int)
//            {
//                string e = string.Format("group id 异常{0} , {1}", gotoGroupID, stayGroupID);
//                throw new System.Exception(e);
//            }

//            //起点
//            if (stayGroupID == LogicTableGroup.EntryGroupID)
//            {
//                //如果是起点，下一组是是第一组
//                if (gotoGroupID != GlobalDataManager.GetInstance().logicTableGroup.sortedGroupID[0])
//                {
//                    return true;
//                }
//                return false;
//            }
//            else
//            {
//                bool isLastGroupID = false;
//                int nextGroupID = GlobalDataManager.GetInstance().logicTableGroup.GetNextGroupID_New(stayGroupID, out isLastGroupID);
//                if (gotoGroupID != nextGroupID && gotoGroupID != stayGroupID)
//                {
//                    return true;
//                }

//                return false;
//            }
//        }
//    }
//}

