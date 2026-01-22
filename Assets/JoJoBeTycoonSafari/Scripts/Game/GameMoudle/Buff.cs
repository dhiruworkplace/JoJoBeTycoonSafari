using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class Buff : TickBase
    {
        /// <summary>
        /// BUFF表主键
        /// </summary>
        public int buffID;

        ///// <summary>
        ///// 有效时间
        ///// </summary>
        //public double effectiveTimeMS;

        public DoubleCD CD;

        /// <summary>
        /// buff类型
        /// </summary>
        public BuffType buffType;

        /// <summary>
        /// buff值
        /// </summary>
        public float buffVal;

        /// <summary>
        /// 是否可合并 目前只用于收入系数 (系数相加 或者相乘)buff
        /// 合并的前提是 本身couldCombine可合并，并且buffType和buffVal相同
        /// 如果合并 是时间相加，系数不变
        /// </summary>
        public bool couldCombine;

        public void Init(int buffID)
        {
            var cell = Config.buffConfig.getInstace().getCell(buffID);
            var eBuffType = (BuffType)(cell.bufftype);

            if (cell == null)
            {
                string e = string.Format("Buff表中没有这个{0}", buffID);
                throw new System.Exception(e);
            }
            this.buffID = buffID;
            buffType = eBuffType;
            //effectiveTimeMS = (double)(cell.time);
            //CD = new DoubleCD(effectiveTimeMS);
            CD = new DoubleCD((double)(cell.time));
            buffVal = cell.multiple;
            couldCombine = true;
            if (cell.merge == 0)
            {
                couldCombine = false;
            }
        }

        public void Init(int buffID, double leftTime)
        {
            var cell = Config.buffConfig.getInstace().getCell(buffID);
            var eBuffType = (BuffType)(cell.bufftype);

            if (cell == null)
            {
                string e = string.Format("Buff表中没有这个{0}", buffID);
                throw new System.Exception(e);
            }
            this.buffID = buffID;
            buffType = eBuffType;
            //effectiveTimeMS = leftTime;
            //CD = new DoubleCD(effectiveTimeMS);
            CD = new DoubleCD(leftTime);
            buffVal = cell.multiple;
            couldCombine = true;
            if (cell.merge == 0)
            {
                couldCombine = false;
            }
        }

        public void Release()
        {

        }

        public override void Tick(int deltaTimeMS)
        {
            CD.Tick(deltaTimeMS);
            //effectiveTimeMS = CD.cd;
        }

        public bool IsVaild()
        {
            return !CD.IsFinish();
        }

        //public int GetLeftMs()
        //{
        //    if (CD != null)
        //    {
        //        return CD.GetLeft();
        //    }

        //    return 0;
        //}
    }
}

