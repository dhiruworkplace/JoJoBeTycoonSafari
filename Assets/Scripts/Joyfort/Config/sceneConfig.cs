using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class sceneConfig
	{
		private sceneConfig(){ 
		}
		private static sceneConfig _inst;
		public static sceneConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new sceneConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,sceneCell> AllData;
		public sceneCell getCell(string key){
			sceneCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public sceneCell getCell(int key){
			sceneCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 1;
		private void InitData(){
			this.AllData = new Dictionary<string,sceneCell> ();
			this.AllData.Add("1",new sceneCell("海岛",50,"UIAtlas/UIIcon/Tiger","prefabs/Scene/dwy_9",10));
		}
	}
	public class sceneCell
	{
		///<summary>
		///场景名称
		///<summary>
		public readonly string scenename;
		///<summary>
		///开启星级
		///<summary>
		public readonly int openstar;
		///<summary>
		///图标
		///<summary>
		public readonly string icon;
		///<summary>
		///场景资源加载
		///<summary>
		public readonly string resourceid;
		///<summary>
		///翻倍系数
		///<summary>
		public readonly int doublenum;
		public sceneCell(string scenename,int openstar,string icon,string resourceid,int doublenum){
			this.scenename = scenename;
			this.openstar = openstar;
			this.icon = icon;
			this.resourceid = resourceid;
			this.doublenum = doublenum;
		}
	}
}