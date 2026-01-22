using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class ticketConfig
	{
		private ticketConfig(){ 
		}
		private static ticketConfig _inst;
		public static ticketConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new ticketConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,ticketCell> AllData;
		public ticketCell getCell(string key){
			ticketCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public ticketCell getCell(int key){
			ticketCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 8;
		private void InitData(){
			this.AllData = new Dictionary<string,ticketCell> ();
			this.AllData.Add("0",new ticketCell("1号售票口",1,1,"2",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into1",8,1,9,8,2000,"TestDaMen/1","damen_02/1",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},1.6f,1,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("1",new ticketCell("2号售票口",1,1,"150",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into2",8,2,9,8,2000,"TestDaMen/2","damen_02/2",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},1.6f,10,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("2",new ticketCell("3号售票口",1,1,"46000",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into3",8,3,9,8,2000,"TestDaMen/3","damen_02/3",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},2.4f,50,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("3",new ticketCell("4号售票口",1,1,"6000000",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into4",8,4,9,8,2000,"TestDaMen/4","damen_02/4",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},2.4f,150,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("4",new ticketCell("5号售票口",1,1,"260000000000",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into5",8,5,9,8,2000,"TestDaMen/5","damen_02/5",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},3.2f,300,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("5",new ticketCell("6号售票口",1,1,"63000000000000",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into6",8,6,9,8,2000,"TestDaMen/6","damen_02/6",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},3.2f,450,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("6",new ticketCell("7号售票口",1,1,"110000000000000000",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into7",8,7,9,8,2000,"TestDaMen/7","damen_02/7",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},4f,600,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
			this.AllData.Add("7",new ticketCell("8号售票口",1,1,"180000000000000000000",20,"10","1",13,6,"8",136,"1",9,"path_touristwalk_into8",8,8,9,8,2000,"TestDaMen/8","damen_02/8",new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000,1200,1400,1600,1800,2000},new int[]{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},new int[]{3002,3003,3004,3006,3009,3012,3014,3016,3018,3023,3027,3031,3036,3040,3044,3048,3057,3065,3073,3082,3090},4f,800,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},20));
		}
	}
	public class ticketCell
	{
		///<summary>
		///名称
		///<summary>
		public readonly string name;
		///<summary>
		///所属场景id
		///<summary>
		public readonly int scene;
		///<summary>
		///开启条件
		///<summary>
		public readonly int open;
		///<summary>
		///开启参数
		///<summary>
		public readonly string number;
		///<summary>
		///票价基础
		///<summary>
		public readonly int pricebase;
		///<summary>
		///票价公式
		///<summary>
		public readonly string priceupformula;
		///<summary>
		///升级消耗初始
		///<summary>
		public readonly string depbase;
		///<summary>
		///升级消耗公式
		///<summary>
		public readonly int depformula;
		///<summary>
		///售票速度
		///<summary>
		public readonly int speedbase;
		///<summary>
		///售票速度公式
		///<summary>
		public readonly string speedupformula;
		///<summary>
		///速度最大等级
		///<summary>
		public readonly int speedmaxlv;
		///<summary>
		///售票口速度升级初始消耗
		///<summary>
		public readonly string lvupcastbase;
		///<summary>
		///售票速度升级消耗公式
		///<summary>
		public readonly int lvupcastformula;
		///<summary>
		///进口路线
		///<summary>
		public readonly string touristwalkinto;
		///<summary>
		///最大排队数量
		///<summary>
		public readonly int maxnumofperqueue;
		///<summary>
		///售票口数量
		///<summary>
		public readonly int space;
		///<summary>
		///售票口数量公式
		///<summary>
		public readonly int spaceformula;
		///<summary>
		///售票口最大等级
		///<summary>
		public readonly int spacemaxlv;
		///<summary>
		///等级上限
		///<summary>
		public readonly int lvmax;
		///<summary>
		///场景UI路径
		///<summary>
		public readonly string gameobjectpath;
		///<summary>
		///禁止牌预制体路径
		///<summary>
		public readonly string prohibitroute;
		///<summary>
		///等级阶段
		///<summary>
		public readonly int[] lvshage;
		///<summary>
		///奖励类型废弃
		///<summary>
		public readonly int[] lvrewardtype;
		///<summary>
		///奖励物品ID
		///<summary>
		public readonly int[] lvreward;
		///<summary>
		///等级系数
		///<summary>
		public readonly float lvratio;
		///<summary>
		///等级系数
		///<summary>
		public readonly int lvratio1;
		///<summary>
		///奖励星星数量
		///<summary>
		public readonly int[] star;
		///<summary>
		///总星数
		///<summary>
		public readonly int starsum;
		public ticketCell(string name,int scene,int open,string number,int pricebase,string priceupformula,string depbase,int depformula,int speedbase,string speedupformula,int speedmaxlv,string lvupcastbase,int lvupcastformula,string touristwalkinto,int maxnumofperqueue,int space,int spaceformula,int spacemaxlv,int lvmax,string gameobjectpath,string prohibitroute,int[] lvshage,int[] lvrewardtype,int[] lvreward,float lvratio,int lvratio1,int[] star,int starsum){
			this.name = name;
			this.scene = scene;
			this.open = open;
			this.number = number;
			this.pricebase = pricebase;
			this.priceupformula = priceupformula;
			this.depbase = depbase;
			this.depformula = depformula;
			this.speedbase = speedbase;
			this.speedupformula = speedupformula;
			this.speedmaxlv = speedmaxlv;
			this.lvupcastbase = lvupcastbase;
			this.lvupcastformula = lvupcastformula;
			this.touristwalkinto = touristwalkinto;
			this.maxnumofperqueue = maxnumofperqueue;
			this.space = space;
			this.spaceformula = spaceformula;
			this.spacemaxlv = spacemaxlv;
			this.lvmax = lvmax;
			this.gameobjectpath = gameobjectpath;
			this.prohibitroute = prohibitroute;
			this.lvshage = lvshage;
			this.lvrewardtype = lvrewardtype;
			this.lvreward = lvreward;
			this.lvratio = lvratio;
			this.lvratio1 = lvratio1;
			this.star = star;
			this.starsum = starsum;
		}
	}
}