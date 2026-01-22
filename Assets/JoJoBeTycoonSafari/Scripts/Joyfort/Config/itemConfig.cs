using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class itemConfig
	{
		private itemConfig(){ 
		}
		private static itemConfig _inst;
		public static itemConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new itemConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,itemCell> AllData;
		public itemCell getCell(string key){
			itemCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public itemCell getCell(int key){
			itemCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 112;
		private void InitData(){
			this.AllData = new Dictionary<string,itemCell> ();
			this.AllData.Add("1",new itemCell(1,"Diamonds","钻石","UIAtlas/UIItemIcon/Star","",5,"Diamonds_desc","1",1));
			this.AllData.Add("2",new itemCell(1,"Diamonds","10钻石","UIAtlas/UIItemIcon/Star","",5,"Diamonds_desc","10",1));
			this.AllData.Add("3",new itemCell(2,"Gold","现金","UIAtlas/UIItemIcon/Star","",4,"Gold_desc","1",1));
			this.AllData.Add("4",new itemCell(4,"Star","星星","UIAtlas/UIItemIcon/Star","",4,"Gold_desc","1",1));
			this.AllData.Add("500",new itemCell(3,"Double_1","2倍收益","UIAtlas/UIItemIcon/Star","",1,"Double_1_desc","1",2));
			this.AllData.Add("501",new itemCell(3,"Double_2","4倍收益","UIAtlas/UIItemIcon/Star","",1,"Double_2_desc","1",2));
			this.AllData.Add("502",new itemCell(3,"Double_3","10倍收益","UIAtlas/UIItemIcon/Star","",2,"Double_3_desc","1",2));
			this.AllData.Add("503",new itemCell(3,"Double_4","20倍收益","UIAtlas/UIItemIcon/Star","",2,"Double_4_desc","1",2));
			this.AllData.Add("504",new itemCell(3,"Double_5","50倍收益","UIAtlas/UIItemIcon/Star","",3,"Double_5_desc","1",2));
			this.AllData.Add("505",new itemCell(3,"Double_6","100倍收益","UIAtlas/UIItemIcon/Star","",3,"Double_6_desc","1",2));
			this.AllData.Add("506",new itemCell(3,"Double_7","500倍收益","UIAtlas/UIItemIcon/Star","",4,"Double_7_desc","1",2));
			this.AllData.Add("507",new itemCell(3,"Double_8","1000倍收益","UIAtlas/UIItemIcon/Star","",5,"Double_8_desc","1",2));
			this.AllData.Add("3001",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1",1));
			this.AllData.Add("3002",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10",1));
			this.AllData.Add("3003",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100",1));
			this.AllData.Add("3004",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000",1));
			this.AllData.Add("3005",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000",1));
			this.AllData.Add("3006",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000",1));
			this.AllData.Add("3007",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000",1));
			this.AllData.Add("3008",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000",1));
			this.AllData.Add("3009",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000",1));
			this.AllData.Add("3010",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000",1));
			this.AllData.Add("3011",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000",1));
			this.AllData.Add("3012",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000",1));
			this.AllData.Add("3013",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000",1));
			this.AllData.Add("3014",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000",1));
			this.AllData.Add("3015",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000",1));
			this.AllData.Add("3016",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000",1));
			this.AllData.Add("3017",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000",1));
			this.AllData.Add("3018",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000",1));
			this.AllData.Add("3019",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000",1));
			this.AllData.Add("3020",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000",1));
			this.AllData.Add("3021",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000",1));
			this.AllData.Add("3022",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000",1));
			this.AllData.Add("3023",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000",1));
			this.AllData.Add("3024",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000",1));
			this.AllData.Add("3025",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000",1));
			this.AllData.Add("3026",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000",1));
			this.AllData.Add("3027",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000",1));
			this.AllData.Add("3028",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000",1));
			this.AllData.Add("3029",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000",1));
			this.AllData.Add("3030",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000",1));
			this.AllData.Add("3031",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000",1));
			this.AllData.Add("3032",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000",1));
			this.AllData.Add("3033",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000",1));
			this.AllData.Add("3034",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000",1));
			this.AllData.Add("3035",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000",1));
			this.AllData.Add("3036",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000",1));
			this.AllData.Add("3037",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000",1));
			this.AllData.Add("3038",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000",1));
			this.AllData.Add("3039",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000",1));
			this.AllData.Add("3040",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000",1));
			this.AllData.Add("3041",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000",1));
			this.AllData.Add("3042",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000",1));
			this.AllData.Add("3043",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000",1));
			this.AllData.Add("3044",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000",1));
			this.AllData.Add("3045",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000",1));
			this.AllData.Add("3046",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3047",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3048",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3049",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3050",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3051",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3052",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3053",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3054",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3055",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3056",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3057",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3058",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3059",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3060",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3061",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3062",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3063",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3064",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3065",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3066",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3067",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3068",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3069",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3070",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3071",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3072",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3073",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3074",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3075",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3076",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3077",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3078",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3079",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3080",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3081",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3082",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3083",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3084",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3085",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3086",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3087",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3088",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3089",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3090",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3091",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3092",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3093",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3094",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3095",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3096",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3097",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3098",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3099",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
			this.AllData.Add("3100",new itemCell(2,"Gold","现金","UIAtlas/Main/Gold","",4,"Gold_desc","1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",1));
		}
	}
	public class itemCell
	{
		///<summary>
		///道具类型(1 钻石 2 金币 3BUFF道具 4 星星)
		///<summary>
		public readonly int itemtype;
		///<summary>
		///名称翻译
		///<summary>
		public readonly string nametranslate;
		///<summary>
		///名称备注
		///<summary>
		public readonly string nameremarks;
		///<summary>
		///图标
		///<summary>
		public readonly string icon;
		///<summary>
		///模型
		///<summary>
		public readonly string model;
		///<summary>
		///道具品质
		///<summary>
		public readonly int quality;
		///<summary>
		///介绍翻译
		///<summary>
		public readonly string desctranslate;
		///<summary>
		///值
		///<summary>
		public readonly string itemval;
		///<summary>
		///使用方式(1 获得及使用 2 获得不使用)
		///<summary>
		public readonly int use;
		public itemCell(int itemtype,string nametranslate,string nameremarks,string icon,string model,int quality,string desctranslate,string itemval,int use){
			this.itemtype = itemtype;
			this.nametranslate = nametranslate;
			this.nameremarks = nameremarks;
			this.icon = icon;
			this.model = model;
			this.quality = quality;
			this.desctranslate = desctranslate;
			this.itemval = itemval;
			this.use = use;
		}
	}
}