using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class groupConfig
	{
		private groupConfig(){ 
		}
		private static groupConfig _inst;
		public static groupConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new groupConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,groupCell> AllData;
		public groupCell getCell(string key){
			groupCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public groupCell getCell(int key){
			groupCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 8;
		private void InitData(){
			this.AllData = new Dictionary<string,groupCell> ();
			this.AllData.Add("1",new groupCell("老虎栏|狼栏|狮子栏",new int[]{1001,1002,1003},1,new int[]{3,4,3},100,100,-1,-1f));
			this.AllData.Add("2",new groupCell("熊栏|鳄鱼栏",new int[]{1004,1005},1,new int[]{5,5},100,100,5008,53f));
			this.AllData.Add("3",new groupCell("袋鼠栏|猴子栏|鹿栏",new int[]{1011,1012,1013},1,new int[]{3,4,3},100,100,5002,53f));
			this.AllData.Add("4",new groupCell("犀牛栏|大象栏",new int[]{1014,1015},1,new int[]{5,5},100,100,5003,53f));
			this.AllData.Add("5",new groupCell("孔雀栏|鹦鹉栏|鹰栏",new int[]{1016,1017,1018},1,new int[]{3,4,3},100,100,5004,53f));
			this.AllData.Add("6",new groupCell("鸵鸟栏|天鹅栏",new int[]{1019,1020},1,new int[]{5,5},100,100,5005,53f));
			this.AllData.Add("7",new groupCell("蜥脚龙栏|角龙栏",new int[]{1007,1008},1,new int[]{5,5},100,100,5006,106f));
			this.AllData.Add("8",new groupCell("剑龙栏|甲龙栏",new int[]{1009,1010},1,new int[]{5,5},100,100,5007,106f));
		}
	}
	public class groupCell
	{
		///<summary>
		///备注
		///<summary>
		public readonly string test;
		///<summary>
		///建筑id
		///<summary>
		public readonly int[] startid;
		///<summary>
		///关联场景id
		///<summary>
		public readonly int scene;
		///<summary>
		///组内随机权重
		///<summary>
		public readonly int[] groupweight;
		///<summary>
		///再次观光概率
		///<summary>
		public readonly int againweight;
		///<summary>
		///前往该组概率
		///<summary>
		public readonly int gotoweight;
		///<summary>
		///资源加载id
		///<summary>
		public readonly int zoopartresID;
		///<summary>
		///地块尺寸
		///<summary>
		public readonly float groundsize;
		public groupCell(string test,int[] startid,int scene,int[] groupweight,int againweight,int gotoweight,int zoopartresID,float groundsize){
			this.test = test;
			this.startid = startid;
			this.scene = scene;
			this.groupweight = groupweight;
			this.againweight = againweight;
			this.gotoweight = gotoweight;
			this.zoopartresID = zoopartresID;
			this.groundsize = groundsize;
		}
	}
}