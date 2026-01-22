using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class buildupConfig
	{
		private buildupConfig(){ 
		}
		private static buildupConfig _inst;
		public static buildupConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new buildupConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,buildupCell> AllData;
		public buildupCell getCell(string key){
			buildupCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public buildupCell getCell(int key){
			buildupCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 21;
		private void InitData(){
			this.AllData = new Dictionary<string,buildupCell> ();
			this.AllData.Add("999",new buildupCell("Build_1",1,1,"10",4,"5",3,1000,0,7,10,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Tiger",0,"0",new int[]{10502,10502,10502,10502,10502},new int[]{1000},new int[]{201},0f,new float[]{0f,0f,0f},"path_1001_out",1,new int[]{1000},new int[]{1000},new int[]{1000},new int[]{1000},-1,new int[]{1},1,0));
			this.AllData.Add("1000",new buildupCell("Build_2",2,1,"10",10,"5",14,1000,0,7,10,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Tiger",0,"0",new int[]{10502,10502,10502,10502,10502},new int[]{1000},new int[]{202},0f,new float[]{0f,0f,0f},"path_1001_out",1,new int[]{1000},new int[]{1000},new int[]{1000},new int[]{1000},-1,new int[]{1},1,0));
			this.AllData.Add("1001",new buildupCell("Build_3",3,1,"20",6,"1",5,1000,1,7,15,"1",24,4,14,41,"1",26,"UIAtlas/UIIcon/Tiger",0,"0",new int[]{10101,10102,10103,10104,10105},new int[]{0,10,50,150,250,350},new int[]{201,202,203,204,205,206},10f,new float[]{0f,0f,4f},"path_1001_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10101,3003,10102,3005,10103,3009,10104,3013,10105,3018,3021,3024,3027,3030,3033,3036},0,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,0));
			this.AllData.Add("1002",new buildupCell("Build_4",3,1,"280",6,"1",5,1000,1,7,15,"1",24,4,14,41,"1",26,"UIAtlas/UIIcon/Wolf",0,"6026",new int[]{10201,10202,10203,10204,10205},new int[]{0,10,50,150,250,350},new int[]{301,302,303,304,305,306},10f,new float[]{0f,0f,4f},"path_1002_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10201,3005,10202,3007,10203,3011,10204,3014,10205,3019,3022,3025,3028,3031,3035,3038},40,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,6));
			this.AllData.Add("1003",new buildupCell("Build_5",3,1,"4191",6,"1",5,1000,1,7,15,"1",24,4,14,41,"1",26,"UIAtlas/UIIcon/Lion",0,"286014",new int[]{10302,10301,10303,10304,10305},new int[]{0,10,50,150,250,350},new int[]{401,402,403,404,405,406},10f,new float[]{0f,0f,4f},"path_1003_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10302,3007,10301,3008,10303,3012,10304,3016,10305,3020,3023,3027,3030,3033,3036,3039},80,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,12));
			this.AllData.Add("1004",new buildupCell("Build_6",3,1,"3636857",6,"1",5,1000,1,7,15,"1",24,4,14,41,"1",26,"UIAtlas/UIIcon/Bear",0,"713775720",new int[]{10401,10402,10403,10404,10405},new int[]{0,10,50,150,250,350},new int[]{501,502,503,504,505,506},10f,new float[]{0f,0f,4f},"path_1004_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10401,3011,10402,3012,10403,3016,10404,3019,10405,3023,3027,3030,3033,3036,3039,3042},180,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,25));
			this.AllData.Add("1005",new buildupCell("Build_7",3,1,"815508357",6,"1",5,1000,1,7,15,"1",24,4,14,41,"1",26,"UIAtlas/UIIcon/Crocodile",0,"1568519813952",new int[]{10501,10502,10503,10504,10505},new int[]{0,10,50,150,250,350},new int[]{601,602,603,604,605,606},10f,new float[]{0f,0f,4f},"path_1005_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10501,3014,10502,3015,10503,3018,10504,3021,10505,3026,3029,3032,3035,3038,3041,3044},260,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,36));
			this.AllData.Add("1007",new buildupCell("Build_9",3,1,"6699111667126580000000000000000000000",6,"1",5,1000,1,7,15,"1",24,3,14,41,"1",26,"UIAtlas/UIIcon/Dinosaur2",0,"398096276032003000000000000000000000000000",new int[]{10705,10702,10703,10704,10701},new int[]{0,10,50,150,250,350},new int[]{801,802,803,804,805,806},10f,new float[]{0f,0f,4f},"path_1007_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10705,3043,10702,3044,10703,3047,10704,3050,10701,3055,3058,3061,3064,3067,3070,3072},1210,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,200));
			this.AllData.Add("1008",new buildupCell("Build_10",3,1,"1502171201996010000000000000000000000000",6,"1",5,1000,1,7,15,"1",24,3,14,41,"1",26,"UIAtlas/UIIcon/Dinosaur3",0,"106471648560166000000000000000000000000000000",new int[]{10801,10802,10803,10804,10805},new int[]{0,10,50,150,250,350},new int[]{901,902,903,904,905,906},10f,new float[]{0f,0f,4f},"path_1008_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10801,3045,10802,3047,10803,3050,10804,3053,10805,3057,3060,3063,3066,3069,3072,3075},1290,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,215));
			this.AllData.Add("1009",new buildupCell("Build_11",3,1,"336838439517164000000000000000000000000000",6,"1",5,1000,1,7,15,"1",24,3,14,41,"1",26,"UIAtlas/UIIcon/Dinosaur4",0,"28184405522608800000000000000000000000000000000",new int[]{10901,10902,10903,10904,10905},new int[]{0,10,50,150,250,350},new int[]{1001,1002,1003,1004,1005,1006},10f,new float[]{0f,0f,4f},"path_1009_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{10901,3048,10902,3049,10903,3052,10904,3055,10905,3059,3062,3065,3068,3071,3074,3077},1370,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,230));
			this.AllData.Add("1010",new buildupCell("Build_12",3,1,"75530761197923400000000000000000000000000000",6,"1",5,1000,1,7,15,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Dinosaur5",0,"7415285422728450000000000000000000000000000000000",new int[]{11001,11002,11003,11004,11005},new int[]{0,10,50,150,250,350},new int[]{1101,1102,1103,1104,1105,1106},10f,new float[]{0f,0f,4f},"path_1010_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11001,3050,11002,3051,11003,3054,11004,3057,11005,3062,3065,3068,3071,3074,3077,3080},1450,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,249));
			this.AllData.Add("1011",new buildupCell("Build_13",3,1,"312137664344366",6,"1",5,1000,1,7,15,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Kangaroo",0,"1378364919651530000",new int[]{11101,11102,11103,11104,11105},new int[]{0,10,50,150,250,350},new int[]{1201,1202,1203,1204,1205,1206},10f,new float[]{0f,0f,4f},"path_1011_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11101,3020,11102,3021,11103,3024,11104,3027,11105,3032,3035,3038,3041,3044,3047,3050},450,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,61));
			this.AllData.Add("1012",new buildupCell("Build_14",3,1,"69991998004344600",6,"1",5,1000,1,7,15,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Monkey",0,"458611119634075000000",new int[]{11201,11202,11203,11204,11205},new int[]{0,10,50,150,250,350},new int[]{1301,1302,1303,1304,1305,1306},10f,new float[]{0f,0f,4f},"path_1012_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11201,3022,11202,3023,11203,3027,11204,3030,11205,3034,3037,3040,3043,3046,3049,3052},530,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,70));
			this.AllData.Add("1013",new buildupCell("Build_15",3,1,"15694612807877900000",6,"1",5,1000,1,7,15,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Deer",0,"146517277885880000000000",new int[]{11301,11302,11303,11304,11305},new int[]{0,10,50,150,250,350},new int[]{1401,1402,1403,1404,1405,1406},10f,new float[]{0f,0f,4f},"path_1013_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11301,3025,11302,3026,11303,3029,11304,3032,11305,3037,3040,3043,3046,3049,3052,3055},610,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,83));
			this.AllData.Add("1014",new buildupCell("Build_16",3,1,"3519271891251260000000",6,"1",5,1000,1,7,15,"1",24,2,14,41,"1",26,"UIAtlas/UIIcon/Rhinoceros",0,"44415184691081400000000000",new int[]{11401,11402,11403,11404,11405},new int[]{0,10,50,150,250,350},new int[]{1501,1502,1503,1504,1505,1506},10f,new float[]{0f,0f,4f},"path_1014_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11401,3027,11402,3028,11403,3031,11404,3035,11405,3039,3042,3045,3048,3051,3054,3057},690,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,94));
			this.AllData.Add("1015",new buildupCell("Build_17",3,1,"203929231157221000000000",6,"1",5,1000,1,7,15,"1",24,1,14,41,"1",26,"UIAtlas/UIIcon/Elephant",0,"3289931203958840000000000000",new int[]{11501,11502,11503,11504,11505},new int[]{0,10,50,150,250,350},new int[]{1601,1602,1603,1604,1605,1606},10f,new float[]{0f,0f,4f},"path_1015_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11501,3029,11502,3030,11503,3033,11504,3036,11505,3041,3044,3047,3050,3053,3056,3059},750,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,105));
			this.AllData.Add("1016",new buildupCell("Build_18",3,1,"45727946257830000000000000",6,"1",5,1000,1,7,15,"1",24,1,14,41,"1",26,"UIAtlas/UIIcon/Peacock",0,"957081641536545000000000000000",new int[]{11601,11602,11603,11604,11605},new int[]{0,10,50,150,250,350},new int[]{1701,1702,1703,1704,1705,1706},10f,new float[]{0f,0f,4f},"path_1016_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11601,3031,11602,3033,11603,3036,11604,3039,11605,3043,3046,3049,3052,3055,3058,3061},830,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,118));
			this.AllData.Add("1017",new buildupCell("Build_19",3,1,"2649771092122820000000000000",6,"1",5,1000,1,7,15,"1",24,1,14,41,"1",26,"UIAtlas/UIIcon/Stork",0,"68905192292309900000000000000000",new int[]{11701,11702,11703,11704,11705},new int[]{0,10,50,150,250,350},new int[]{1801,1802,1803,1804,1805,1806},10f,new float[]{0f,0f,4f},"path_1017_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11701,3033,11702,3035,11703,3038,11704,3041,11705,3045,3048,3051,3054,3057,3060,3063},890,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,133));
			this.AllData.Add("1018",new buildupCell("Build_20",3,1,"594169798064544000000000000000",6,"1",5,1000,1,7,15,"1",24,1,14,41,"1",26,"UIAtlas/UIIcon/Eagle",0,"19364382428137200000000000000000000",new int[]{11801,11802,11803,11804,11805},new int[]{0,10,50,150,250,350},new int[]{1901,1902,1903,1904,1905,1906},10f,new float[]{0f,0f,4f},"path_1018_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11801,3036,11802,3037,11803,3040,11804,3043,11805,3047,3050,3053,3056,3059,3062,3065},970,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,148));
			this.AllData.Add("1019",new buildupCell("Build_21",3,1,"133233300786458000000000000000000",6,"1",5,1000,1,7,15,"1",24,1,14,41,"1",26,"UIAtlas/UIIcon/Ostrich",0,"5362827132310250000000000000000000000",new int[]{11901,11902,11903,11904,11905},new int[]{0,10,50,150,250,350},new int[]{2001,2002,2003,2004,2005,2006},10f,new float[]{0f,0f,4f},"path_1019_out",1,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{11901,3038,11902,3039,11903,3042,11904,3045,11905,3050,3053,3056,3059,3062,3065,3068},1050,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,166));
			this.AllData.Add("1020",new buildupCell("Build_22",3,1,"29875487606871700000000000000000000",6,"1",5,1000,1,7,15,"1",24,1,14,41,"1",26,"UIAtlas/UIIcon/Swan",0,"1465267022543010000000000000000000000000",new int[]{12001,12002,12003,12004,12005},new int[]{0,10,50,150,250,350},new int[]{2101,2102,2103,2104,2105,2106},10f,new float[]{0f,0f,4f},"path_1020_out",0,new int[]{1,25,100,200,300},new int[]{0,10,25,50,100,150,200,250,300,400,500,600,700,800,900,1000},new int[]{2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1},new int[]{12001,3041,12002,3042,12003,3045,12004,3048,12005,3052,3055,3058,3061,3064,3067,3070},1130,new int[]{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},15,183));
		}
	}
	public class buildupCell
	{
		///<summary>
		///动物栏名称
		///<summary>
		public readonly string buildname;
		///<summary>
		///建筑类型
		///<summary>
		public readonly int buildtype;
		///<summary>
		///所属场景
		///<summary>
		public readonly int scene;
		///<summary>
		///动物栏产出初始
		///<summary>
		public readonly string pricebase;
		///<summary>
		///动物栏产出公式
		///<summary>
		public readonly int priceformula;
		///<summary>
		///动物栏升级消耗初始
		///<summary>
		public readonly string castbase;
		///<summary>
		///动物栏升级消耗公式
		///<summary>
		public readonly int castformula;
		///<summary>
		///等级上限
		///<summary>
		public readonly int lvmax;
		///<summary>
		///观光点数初始
		///<summary>
		public readonly int watchbase;
		///<summary>
		///观光点数公式
		///<summary>
		public readonly int watchformula;
		///<summary>
		///观光点最大等级
		///<summary>
		public readonly int watchmaxlv;
		///<summary>
		///观光点数升级消耗初始
		///<summary>
		public readonly string watchupbase;
		///<summary>
		///观光点数升级消耗公式
		///<summary>
		public readonly int watchupformula;
		///<summary>
		///观光速度初始
		///<summary>
		public readonly int timebase;
		///<summary>
		///观光速度公式
		///<summary>
		public readonly int timeformula;
		///<summary>
		///观光速度最大等级
		///<summary>
		public readonly int itemmaxlv;
		///<summary>
		///观光速度升级消耗初始
		///<summary>
		public readonly string timeupbase;
		///<summary>
		///观光速度升级消耗公式
		///<summary>
		public readonly int timeupformula;
		///<summary>
		///图标
		///<summary>
		public readonly string icon;
		///<summary>
		///动物栏开启条件
		///<summary>
		public readonly int open;
		///<summary>
		///条件参数
		///<summary>
		public readonly string number;
		///<summary>
		///包含动物ID
		///<summary>
		public readonly int[] animalid;
		///<summary>
		///模型升级所需建筑等级
		///<summary>
		public readonly int[] lvmodel;
		///<summary>
		///资源加载id
		///<summary>
		public readonly int[] loadresource;
		///<summary>
		///动物栏半径
		///<summary>
		public readonly float animalwanderradius;
		///<summary>
		///动物栏中心点偏移
		///<summary>
		public readonly float[] animalwanderoffset;
		///<summary>
		///动物栏离开路线
		///<summary>
		public readonly string outpath;
		///<summary>
		///路线左右类型
		///<summary>
		public readonly int pathtype;
		///<summary>
		///开启新动物所需建筑等级
		///<summary>
		public readonly int[] lvanimal;
		///<summary>
		///等级阶段
		///<summary>
		public readonly int[] lvshage;
		///<summary>
		///奖励类型
		///<summary>
		public readonly int[] lvrewardtype;
		///<summary>
		///奖励类型
		///<summary>
		public readonly int[] lvreward;
		///<summary>
		///等级系数
		///<summary>
		public readonly int lvcoefficient;
		///<summary>
		///奖励星星数
		///<summary>
		public readonly int[] star;
		///<summary>
		///奖励星星总数
		///<summary>
		public readonly int starsum;
		///<summary>
		///开启需要星星数
		///<summary>
		public readonly int needstar;
		public buildupCell(string buildname,int buildtype,int scene,string pricebase,int priceformula,string castbase,int castformula,int lvmax,int watchbase,int watchformula,int watchmaxlv,string watchupbase,int watchupformula,int timebase,int timeformula,int itemmaxlv,string timeupbase,int timeupformula,string icon,int open,string number,int[] animalid,int[] lvmodel,int[] loadresource,float animalwanderradius,float[] animalwanderoffset,string outpath,int pathtype,int[] lvanimal,int[] lvshage,int[] lvrewardtype,int[] lvreward,int lvcoefficient,int[] star,int starsum,int needstar){
			this.buildname = buildname;
			this.buildtype = buildtype;
			this.scene = scene;
			this.pricebase = pricebase;
			this.priceformula = priceformula;
			this.castbase = castbase;
			this.castformula = castformula;
			this.lvmax = lvmax;
			this.watchbase = watchbase;
			this.watchformula = watchformula;
			this.watchmaxlv = watchmaxlv;
			this.watchupbase = watchupbase;
			this.watchupformula = watchupformula;
			this.timebase = timebase;
			this.timeformula = timeformula;
			this.itemmaxlv = itemmaxlv;
			this.timeupbase = timeupbase;
			this.timeupformula = timeupformula;
			this.icon = icon;
			this.open = open;
			this.number = number;
			this.animalid = animalid;
			this.lvmodel = lvmodel;
			this.loadresource = loadresource;
			this.animalwanderradius = animalwanderradius;
			this.animalwanderoffset = animalwanderoffset;
			this.outpath = outpath;
			this.pathtype = pathtype;
			this.lvanimal = lvanimal;
			this.lvshage = lvshage;
			this.lvrewardtype = lvrewardtype;
			this.lvreward = lvreward;
			this.lvcoefficient = lvcoefficient;
			this.star = star;
			this.starsum = starsum;
			this.needstar = needstar;
		}
	}
}