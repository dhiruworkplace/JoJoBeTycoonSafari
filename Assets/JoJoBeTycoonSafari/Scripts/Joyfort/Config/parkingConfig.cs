using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class parkingConfig
	{
		private parkingConfig(){ 
		}
		private static parkingConfig _inst;
		public static parkingConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new parkingConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,parkingCell> AllData;
		public parkingCell getCell(string key){
			parkingCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public parkingCell getCell(int key){
			parkingCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 1;
		private void InitData(){
			this.AllData = new Dictionary<string,parkingCell> ();
			this.AllData.Add("1",new parkingCell("停车场",1,0,4,5,1,163,"1",2,6,2,58,"100",4,"1",3,2000,new int[]{1,2,3,4,5,6,7,8},new string[]{"9001","9002","9003","9004","9005","9006","9007","9008"},new int[]{-1,1,1,-1,1,-1,1,-1},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
		}
	}
	public class parkingCell
	{
		///<summary>
		///停车场名称
		///<summary>
		public readonly string buildname;
		///<summary>
		///所属场景
		///<summary>
		public readonly int scene;
		///<summary>
		///停车费初始
		///<summary>
		public readonly int price;
		///<summary>
		///停车费公式
		///<summary>
		public readonly int priceformula;
		///<summary>
		///停车场来人速度初始
		///<summary>
		public readonly int touristbase;
		///<summary>
		///停车场来人速度公式
		///<summary>
		public readonly int touristformula;
		///<summary>
		///速度最大等级
		///<summary>
		public readonly int touristmaxlv;
		///<summary>
		///停车场来人速度升级消耗初始
		///<summary>
		public readonly string touristcastbase;
		///<summary>
		///停车场来人速度升级公式
		///<summary>
		public readonly int touristcastformula;
		///<summary>
		///停车场最大位置初始
		///<summary>
		public readonly int spacebase;
		///<summary>
		///停车场最大位置公式
		///<summary>
		public readonly int spaceformula;
		///<summary>
		///最大位置等级
		///<summary>
		public readonly int spacemaxlv;
		///<summary>
		///停车场最大位置升级消耗初始
		///<summary>
		public readonly string spaceupcastbase;
		///<summary>
		///停车场最大位置升级消耗公式
		///<summary>
		public readonly int spaceupcastformula;
		///<summary>
		///停车场利润升级初始消耗
		///<summary>
		public readonly string depletebase;
		///<summary>
		///停车场利润升级消耗公式
		///<summary>
		public readonly int depleteformula;
		///<summary>
		///等级上限
		///<summary>
		public readonly int lvmax;
		///<summary>
		///停车场开启等级
		///<summary>
		public readonly int[] openlv;
		///<summary>
		///开启组资源
		///<summary>
		public readonly string[] openggroup;
		///<summary>
		///停车位位置
		///<summary>
		public readonly int[] openseatdir;
		///<summary>
		///等级阶段
		///<summary>
		public readonly int[] lvshage;
		///<summary>
		///奖励类型
		///<summary>
		public readonly int[] lvrewardtype;
		///<summary>
		///奖励道具ID
		///<summary>
		public readonly int[] lvreward;
		///<summary>
		///奖励星星数量
		///<summary>
		public readonly int[] star;
		///<summary>
		///总星数
		///<summary>
		public readonly int starsum;
		public parkingCell(string buildname,int scene,int price,int priceformula,int touristbase,int touristformula,int touristmaxlv,string touristcastbase,int touristcastformula,int spacebase,int spaceformula,int spacemaxlv,string spaceupcastbase,int spaceupcastformula,string depletebase,int depleteformula,int lvmax,int[] openlv,string[] openggroup,int[] openseatdir,int[] lvshage,int[] lvrewardtype,int[] lvreward,int[] star,int starsum){
			this.buildname = buildname;
			this.scene = scene;
			this.price = price;
			this.priceformula = priceformula;
			this.touristbase = touristbase;
			this.touristformula = touristformula;
			this.touristmaxlv = touristmaxlv;
			this.touristcastbase = touristcastbase;
			this.touristcastformula = touristcastformula;
			this.spacebase = spacebase;
			this.spaceformula = spaceformula;
			this.spacemaxlv = spacemaxlv;
			this.spaceupcastbase = spaceupcastbase;
			this.spaceupcastformula = spaceupcastformula;
			this.depletebase = depletebase;
			this.depleteformula = depleteformula;
			this.lvmax = lvmax;
			this.openlv = openlv;
			this.openggroup = openggroup;
			this.openseatdir = openseatdir;
			this.lvshage = lvshage;
			this.lvrewardtype = lvrewardtype;
			this.lvreward = lvreward;
			this.star = star;
			this.starsum = starsum;
		}
	}
}