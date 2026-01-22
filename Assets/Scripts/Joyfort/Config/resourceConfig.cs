using UnityEngine;
using System;
using System.Security;
using System.Collections.Generic;
namespace Config
{
	public class resourceConfig
	{
		private resourceConfig(){ 
		}
		private static resourceConfig _inst;
		public static resourceConfig getInstace(){
			if (_inst != null) {
				return _inst;
			}
			_inst = new resourceConfig ();
			_inst.InitData ();
			return _inst;
		}
		public Dictionary<string,resourceCell> AllData;
		public resourceCell getCell(string key){
			resourceCell t = null;
			this.AllData.TryGetValue (key, out t);
			return t;
		}
		public resourceCell getCell(int key){
			resourceCell t = null;
			this.AllData.TryGetValue (key.ToString(), out t);
			return t;
		}
		public readonly int RowNum = 277;
		private void InitData(){
			this.AllData = new Dictionary<string,resourceCell> ();
			this.AllData.Add("1",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_bus_001","",-1f,0f,0f,0f));
			this.AllData.Add("2",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_bus_002","",-1f,0f,0f,0f));
			this.AllData.Add("3",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_bus_003","",-1f,0f,0f,0f));
			this.AllData.Add("4",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_bus_004","",-1f,0f,0f,0f));
			this.AllData.Add("5",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Coupe_01","",-1f,0f,0f,0f));
			this.AllData.Add("6",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Coupe_02","",-1f,0f,0f,0f));
			this.AllData.Add("7",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Coupe_03","",-1f,0f,0f,0f));
			this.AllData.Add("8",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Coupe_04","",-1f,0f,0f,0f));
			this.AllData.Add("9",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Large_01","",-1f,0f,0f,0f));
			this.AllData.Add("10",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Large_02","",-1f,0f,0f,0f));
			this.AllData.Add("11",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Large_03","",-1f,0f,0f,0f));
			this.AllData.Add("12",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Large_04","",-1f,0f,0f,0f));
			this.AllData.Add("13",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Truck_01","",-1f,0f,0f,0f));
			this.AllData.Add("14",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Truck_02","",-1f,0f,0f,0f));
			this.AllData.Add("15",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Truck_03","",-1f,0f,0f,0f));
			this.AllData.Add("16",new resourceCell("小汽车",1,1,"prefabs/Probe/Car_Truck_04","",-1f,0f,0f,0f));
			this.AllData.Add("17",new resourceCell("小汽车",1,1,"prefabs/Probe/che_01","",-1f,0f,0f,0f));
			this.AllData.Add("18",new resourceCell("小汽车",1,1,"prefabs/Probe/che_02","",-1f,0f,0f,0f));
			this.AllData.Add("19",new resourceCell("小汽车",1,1,"prefabs/Probe/che_03","",-1f,0f,0f,0f));
			this.AllData.Add("20",new resourceCell("小汽车",1,1,"prefabs/Probe/che_04","",-1f,0f,0f,0f));
			this.AllData.Add("21",new resourceCell("小汽车",1,1,"prefabs/Probe/che_05","",-1f,0f,0f,0f));
			this.AllData.Add("22",new resourceCell("小汽车",1,1,"prefabs/Probe/che_06","",-1f,0f,0f,0f));
			this.AllData.Add("23",new resourceCell("小汽车",1,1,"prefabs/Probe/che_07","",-1f,0f,0f,0f));
			this.AllData.Add("24",new resourceCell("小汽车",1,1,"prefabs/Probe/che_08","",-1f,0f,0f,0f));
			this.AllData.Add("101",new resourceCell("游客",2,1,"prefabs/people/npc_01","",-1f,0f,0f,0f));
			this.AllData.Add("102",new resourceCell("游客",2,1,"prefabs/people/npc_02","",-1f,0f,0f,0f));
			this.AllData.Add("103",new resourceCell("游客",2,1,"prefabs/people/npc_03","",-1f,0f,0f,0f));
			this.AllData.Add("104",new resourceCell("游客",2,1,"prefabs/people/npc_04","",-1f,0f,0f,0f));
			this.AllData.Add("105",new resourceCell("游客",2,1,"prefabs/people/npc_05","",-1f,0f,0f,0f));
			this.AllData.Add("106",new resourceCell("游客",2,1,"prefabs/people/npc_06","",-1f,0f,0f,0f));
			this.AllData.Add("107",new resourceCell("游客",2,1,"prefabs/people/npc_07","",-1f,0f,0f,0f));
			this.AllData.Add("108",new resourceCell("游客",2,1,"prefabs/people/npc_08","",-1f,0f,0f,0f));
			this.AllData.Add("109",new resourceCell("游客",2,1,"prefabs/people/npc_09","",-1f,0f,0f,0f));
			this.AllData.Add("110",new resourceCell("游客",2,1,"prefabs/people/npc_10","",-1f,0f,0f,0f));
			this.AllData.Add("201",new resourceCell("老虎栏0级",3,1,"prefabs/Probe/xweilan_1001_0","",-1f,0f,0f,0f));
			this.AllData.Add("202",new resourceCell("老虎栏1级",3,1,"prefabs/Probe/xweilan_1001_1","",-1f,0f,0f,0f));
			this.AllData.Add("203",new resourceCell("老虎栏2级",3,1,"prefabs/Probe/xweilan_1001_2","",-1f,0f,0f,0f));
			this.AllData.Add("204",new resourceCell("老虎栏3级",3,1,"prefabs/Probe/xweilan_1001_3","",-1f,0f,0f,0f));
			this.AllData.Add("205",new resourceCell("老虎栏4级",3,1,"prefabs/Probe/xweilan_1001_4","",-1f,0f,0f,0f));
			this.AllData.Add("206",new resourceCell("老虎栏5级",3,1,"prefabs/Probe/xweilan_1001_5","",-1f,0f,0f,0f));
			this.AllData.Add("301",new resourceCell("狼栏0级",3,1,"prefabs/Probe/xweilan_1002_0","",-1f,0f,0f,0f));
			this.AllData.Add("302",new resourceCell("狼栏1级",3,1,"prefabs/Probe/xweilan_1002_1","",-1f,0f,0f,0f));
			this.AllData.Add("303",new resourceCell("狼栏2级",3,1,"prefabs/Probe/xweilan_1002_2","",-1f,0f,0f,0f));
			this.AllData.Add("304",new resourceCell("狼栏3级",3,1,"prefabs/Probe/xweilan_1002_3","",-1f,0f,0f,0f));
			this.AllData.Add("305",new resourceCell("狼栏4级",3,1,"prefabs/Probe/xweilan_1002_4","",-1f,0f,0f,0f));
			this.AllData.Add("306",new resourceCell("狼栏5级",3,1,"prefabs/Probe/xweilan_1002_5","",-1f,0f,0f,0f));
			this.AllData.Add("401",new resourceCell("狮子栏0级",3,1,"prefabs/Probe/xweilan_1003_0","",-1f,0f,0f,0f));
			this.AllData.Add("402",new resourceCell("狮子栏1级",3,1,"prefabs/Probe/xweilan_1003_1","",-1f,0f,0f,0f));
			this.AllData.Add("403",new resourceCell("狮子栏2级",3,1,"prefabs/Probe/xweilan_1003_2","",-1f,0f,0f,0f));
			this.AllData.Add("404",new resourceCell("狮子栏3级",3,1,"prefabs/Probe/xweilan_1003_3","",-1f,0f,0f,0f));
			this.AllData.Add("405",new resourceCell("狮子栏4级",3,1,"prefabs/Probe/xweilan_1003_4","",-1f,0f,0f,0f));
			this.AllData.Add("406",new resourceCell("狮子栏5级",3,1,"prefabs/Probe/xweilan_1003_5","",-1f,0f,0f,0f));
			this.AllData.Add("501",new resourceCell("熊栏0级",3,1,"prefabs/Probe/xweilan_1004_0","",-1f,0f,0f,0f));
			this.AllData.Add("502",new resourceCell("熊栏1级",3,1,"prefabs/Probe/xweilan_1004_1","",-1f,0f,0f,0f));
			this.AllData.Add("503",new resourceCell("熊栏2级",3,1,"prefabs/Probe/xweilan_1004_2","",-1f,0f,0f,0f));
			this.AllData.Add("504",new resourceCell("熊栏3级",3,1,"prefabs/Probe/xweilan_1004_3","",-1f,0f,0f,0f));
			this.AllData.Add("505",new resourceCell("熊栏4级",3,1,"prefabs/Probe/xweilan_1004_4","",-1f,0f,0f,0f));
			this.AllData.Add("506",new resourceCell("熊栏5级",3,1,"prefabs/Probe/xweilan_1004_5","",-1f,0f,0f,0f));
			this.AllData.Add("601",new resourceCell("鳄鱼栏0级",3,1,"prefabs/Probe/xweilan_1005_0","",-1f,0f,0f,0f));
			this.AllData.Add("602",new resourceCell("鳄鱼栏1级",3,1,"prefabs/Probe/xweilan_1005_1","",-1f,0f,0f,0f));
			this.AllData.Add("603",new resourceCell("鳄鱼栏2级",3,1,"prefabs/Probe/xweilan_1005_2","",-1f,0f,0f,0f));
			this.AllData.Add("604",new resourceCell("鳄鱼栏3级",3,1,"prefabs/Probe/xweilan_1005_3","",-1f,0f,0f,0f));
			this.AllData.Add("605",new resourceCell("鳄鱼栏4级",3,1,"prefabs/Probe/xweilan_1005_4","",-1f,0f,0f,0f));
			this.AllData.Add("606",new resourceCell("鳄鱼栏5级",3,1,"prefabs/Probe/xweilan_1005_5","",-1f,0f,0f,0f));
			this.AllData.Add("701",new resourceCell("兽脚龙栏0级",3,1,"prefabs/Probe/weilan_close03","",-1f,0f,0f,0f));
			this.AllData.Add("702",new resourceCell("兽脚龙栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("703",new resourceCell("兽脚龙栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("704",new resourceCell("兽脚龙栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("705",new resourceCell("兽脚龙栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("706",new resourceCell("兽脚龙栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("801",new resourceCell("蜥脚龙栏0级",3,1,"prefabs/Probe/weilan_close03","",-1f,0f,0f,0f));
			this.AllData.Add("802",new resourceCell("蜥脚龙栏1级",3,1,"prefabs/Probe/weilan_1006_1","",-1f,0f,0f,0f));
			this.AllData.Add("803",new resourceCell("蜥脚龙栏2级",3,1,"prefabs/Probe/weilan_1006_2","",-1f,0f,0f,0f));
			this.AllData.Add("804",new resourceCell("蜥脚龙栏3级",3,1,"prefabs/Probe/weilan_1006_3","",-1f,0f,0f,0f));
			this.AllData.Add("805",new resourceCell("蜥脚龙栏4级",3,1,"prefabs/Probe/weilan_1006_4","",-1f,0f,0f,0f));
			this.AllData.Add("806",new resourceCell("蜥脚龙栏5级",3,1,"prefabs/Probe/weilan_1006_5","",-1f,0f,0f,0f));
			this.AllData.Add("901",new resourceCell("角龙栏0级",3,1,"prefabs/Probe/weilan_close03","",-1f,0f,0f,0f));
			this.AllData.Add("902",new resourceCell("角龙栏1级",3,1,"prefabs/Probe/weilan_1007_1","",-1f,0f,0f,0f));
			this.AllData.Add("903",new resourceCell("角龙栏2级",3,1,"prefabs/Probe/weilan_1007_2","",-1f,0f,0f,0f));
			this.AllData.Add("904",new resourceCell("角龙栏3级",3,1,"prefabs/Probe/weilan_1007_3","",-1f,0f,0f,0f));
			this.AllData.Add("905",new resourceCell("角龙栏4级",3,1,"prefabs/Probe/weilan_1007_4","",-1f,0f,0f,0f));
			this.AllData.Add("906",new resourceCell("角龙栏5级",3,1,"prefabs/Probe/weilan_1007_5","",-1f,0f,0f,0f));
			this.AllData.Add("1001",new resourceCell("剑龙栏0级",3,1,"prefabs/Probe/weilan_close03","",-1f,0f,0f,0f));
			this.AllData.Add("1002",new resourceCell("剑龙栏1级",3,1,"prefabs/Probe/weilan_1007_1","",-1f,0f,0f,0f));
			this.AllData.Add("1003",new resourceCell("剑龙栏2级",3,1,"prefabs/Probe/weilan_1007_2","",-1f,0f,0f,0f));
			this.AllData.Add("1004",new resourceCell("剑龙栏3级",3,1,"prefabs/Probe/weilan_1007_3","",-1f,0f,0f,0f));
			this.AllData.Add("1005",new resourceCell("剑龙栏4级",3,1,"prefabs/Probe/weilan_1007_4","",-1f,0f,0f,0f));
			this.AllData.Add("1006",new resourceCell("剑龙栏5级",3,1,"prefabs/Probe/weilan_1007_5","",-1f,0f,0f,0f));
			this.AllData.Add("1101",new resourceCell("甲龙栏0级",3,1,"prefabs/Probe/weilan_close03","",-1f,0f,0f,0f));
			this.AllData.Add("1102",new resourceCell("甲龙栏1级",3,1,"prefabs/Probe/weilan_1006_1","",-1f,0f,0f,0f));
			this.AllData.Add("1103",new resourceCell("甲龙栏2级",3,1,"prefabs/Probe/weilan_1006_2","",-1f,0f,0f,0f));
			this.AllData.Add("1104",new resourceCell("甲龙栏3级",3,1,"prefabs/Probe/weilan_1006_3","",-1f,0f,0f,0f));
			this.AllData.Add("1105",new resourceCell("甲龙栏4级",3,1,"prefabs/Probe/weilan_1006_4","",-1f,0f,0f,0f));
			this.AllData.Add("1106",new resourceCell("甲龙栏5级",3,1,"prefabs/Probe/weilan_1006_5","",-1f,0f,0f,0f));
			this.AllData.Add("1201",new resourceCell("袋鼠栏0级",3,1,"prefabs/Probe/weilan_close01","",-1f,0f,0f,0f));
			this.AllData.Add("1202",new resourceCell("袋鼠栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1203",new resourceCell("袋鼠栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1204",new resourceCell("袋鼠栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1205",new resourceCell("袋鼠栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1206",new resourceCell("袋鼠栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1301",new resourceCell("猴子栏0级",3,1,"prefabs/Probe/weilan_close01","",-1f,0f,0f,0f));
			this.AllData.Add("1302",new resourceCell("猴子栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1303",new resourceCell("猴子栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1304",new resourceCell("猴子栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1305",new resourceCell("猴子栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1306",new resourceCell("猴子栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1401",new resourceCell("鹿栏0级",3,1,"prefabs/Probe/weilan_close01","",-1f,0f,0f,0f));
			this.AllData.Add("1402",new resourceCell("鹿栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1403",new resourceCell("鹿栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1404",new resourceCell("鹿栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1405",new resourceCell("鹿栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1406",new resourceCell("鹿栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1501",new resourceCell("犀牛栏0级",3,1,"prefabs/Probe/weilan_close02","",-1f,0f,0f,0f));
			this.AllData.Add("1502",new resourceCell("犀牛栏1级",3,1,"prefabs/Probe/weilan_lv1_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1503",new resourceCell("犀牛栏2级",3,1,"prefabs/Probe/weilan_lv2_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1504",new resourceCell("犀牛栏3级",3,1,"prefabs/Probe/weilan_lv3_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1505",new resourceCell("犀牛栏4级",3,1,"prefabs/Probe/weilan_lv4_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1506",new resourceCell("犀牛栏5级",3,1,"prefabs/Probe/weilan_lv5_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1601",new resourceCell("大象栏0级",3,1,"prefabs/Probe/weilan_close02","",-1f,0f,0f,0f));
			this.AllData.Add("1602",new resourceCell("大象栏1级",3,1,"prefabs/Probe/weilan_lv1_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1603",new resourceCell("大象栏2级",3,1,"prefabs/Probe/weilan_lv2_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1604",new resourceCell("大象栏3级",3,1,"prefabs/Probe/weilan_lv3_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1605",new resourceCell("大象栏4级",3,1,"prefabs/Probe/weilan_lv4_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1606",new resourceCell("大象栏5级",3,1,"prefabs/Probe/weilan_lv5_01B","",-1f,0f,0f,0f));
			this.AllData.Add("1701",new resourceCell("孔雀栏0级",3,1,"prefabs/Probe/weilan_close01","",-1f,0f,0f,0f));
			this.AllData.Add("1702",new resourceCell("孔雀栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1703",new resourceCell("孔雀栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1704",new resourceCell("孔雀栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1705",new resourceCell("孔雀栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1706",new resourceCell("孔雀栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1801",new resourceCell("鹦鹉栏0级",3,1,"prefabs/Probe/weilan_close01","",-1f,0f,0f,0f));
			this.AllData.Add("1802",new resourceCell("鹦鹉栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1803",new resourceCell("鹦鹉栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1804",new resourceCell("鹦鹉栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1805",new resourceCell("鹦鹉栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1806",new resourceCell("鹦鹉栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1901",new resourceCell("鹰栏0级",3,1,"prefabs/Probe/weilan_close01","",-1f,0f,0f,0f));
			this.AllData.Add("1902",new resourceCell("鹰栏1级",3,1,"prefabs/Probe/weilan_lv1_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1903",new resourceCell("鹰栏2级",3,1,"prefabs/Probe/weilan_lv2_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1904",new resourceCell("鹰栏3级",3,1,"prefabs/Probe/weilan_lv3_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1905",new resourceCell("鹰栏4级",3,1,"prefabs/Probe/weilan_lv4_01A","",-1f,0f,0f,0f));
			this.AllData.Add("1906",new resourceCell("鹰栏5级",3,1,"prefabs/Probe/weilan_lv5_01A","",-1f,0f,0f,0f));
			this.AllData.Add("2001",new resourceCell("鸵鸟栏0级",3,1,"prefabs/Probe/weilan_close02","",-1f,0f,0f,0f));
			this.AllData.Add("2002",new resourceCell("鸵鸟栏1级",3,1,"prefabs/Probe/weilan_lv1_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2003",new resourceCell("鸵鸟栏2级",3,1,"prefabs/Probe/weilan_lv2_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2004",new resourceCell("鸵鸟栏3级",3,1,"prefabs/Probe/weilan_lv3_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2005",new resourceCell("鸵鸟栏4级",3,1,"prefabs/Probe/weilan_lv4_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2006",new resourceCell("鸵鸟栏5级",3,1,"prefabs/Probe/weilan_lv5_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2101",new resourceCell("天鹅栏0级",3,1,"prefabs/Probe/weilan_close02","",-1f,0f,0f,0f));
			this.AllData.Add("2102",new resourceCell("天鹅栏1级",3,1,"prefabs/Probe/weilan_lv1_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2103",new resourceCell("天鹅栏2级",3,1,"prefabs/Probe/weilan_lv2_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2104",new resourceCell("天鹅栏3级",3,1,"prefabs/Probe/weilan_lv3_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2105",new resourceCell("天鹅栏4级",3,1,"prefabs/Probe/weilan_lv4_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2106",new resourceCell("天鹅栏5级",3,1,"prefabs/Probe/weilan_lv5_01B","",-1f,0f,0f,0f));
			this.AllData.Add("2201",new resourceCell("华南虎",4,1,"prefabs/Animal/animal_001b","textures/animal_001b",0.6f,0f,0f,0f));
			this.AllData.Add("2202",new resourceCell("蓝虎",4,1,"prefabs/Animal/animal_001c","textures/animal_001c",0.6f,0f,0f,0f));
			this.AllData.Add("2203",new resourceCell("美洲虎",4,1,"prefabs/Animal/animal_001d","textures/animal_001d",0.6f,0f,0f,0f));
			this.AllData.Add("2204",new resourceCell("袋剑齿虎",4,1,"prefabs/Animal/animal_001e","textures/animal_001e",0.6f,0f,0f,0f));
			this.AllData.Add("2205",new resourceCell("白虎",4,1,"prefabs/Animal/animal_001a","textures/animal_001a",0.6f,0f,0f,0f));
			this.AllData.Add("2301",new resourceCell("土狼",4,1,"prefabs/Animal/animal_008e","textures/animal_008e",0.8f,0f,0f,0f));
			this.AllData.Add("2302",new resourceCell("东加拿大狼",4,1,"prefabs/Animal/animal_008b","textures/animal_008b",0.7f,0f,0f,0f));
			this.AllData.Add("2303",new resourceCell("黑背胡狼",4,1,"prefabs/Animal/animal_008d","textures/animal_008d",0.8f,0f,0f,0f));
			this.AllData.Add("2304",new resourceCell("鬃狼",4,1,"prefabs/Animal/animal_008c","textures/animal_008c",0.8f,0f,0f,0f));
			this.AllData.Add("2305",new resourceCell("苔原狼",4,1,"prefabs/Animal/animal_008a","textures/animal_008a",0.7f,0f,0f,0f));
			this.AllData.Add("2402",new resourceCell("克鲁格狮",4,1,"prefabs/Animal/animal_003b","textures/animal_003b",0.6f,0f,0f,0f));
			this.AllData.Add("2401",new resourceCell("美洲狮",4,1,"prefabs/Animal/animal_003d","textures/animal_003d",0.5f,0f,0f,0f));
			this.AllData.Add("2403",new resourceCell("亚洲狮",4,1,"prefabs/Animal/animal_003a","textures/animal_003a",0.5f,0f,0f,0f));
			this.AllData.Add("2404",new resourceCell("黑色美洲狮",4,1,"prefabs/Animal/animal_003c","textures/animal_003c",0.6f,0f,0f,0f));
			this.AllData.Add("2405",new resourceCell("白狮",4,1,"prefabs/Animal/animal_003e","textures/animal_003e",0.5f,0f,0f,0f));
			this.AllData.Add("2501",new resourceCell("棕熊",4,1,"prefabs/Animal/animal_004a","textures/animal_004a",0.5f,0f,0f,0f));
			this.AllData.Add("2502",new resourceCell("美洲黑熊",4,1,"prefabs/Animal/animal_004b","textures/animal_004b",0.5f,0f,0f,0f));
			this.AllData.Add("2503",new resourceCell("柯莫德熊",4,1,"prefabs/Animal/animal_004c","textures/animal_004c",0.5f,0f,0f,0f));
			this.AllData.Add("2504",new resourceCell("北极熊",4,1,"prefabs/Animal/animal_004e","textures/animal_004e",0.5f,0f,0f,0f));
			this.AllData.Add("2505",new resourceCell("大熊猫",4,1,"prefabs/Animal/animal_004d","textures/animal_004d",0.5f,0f,0f,0f));
			this.AllData.Add("2601",new resourceCell("美洲鳄",4,1,"prefabs/Animal/animal_012a","textures/animal_012a",0.5f,0f,3f,0f));
			this.AllData.Add("2602",new resourceCell("黑凯门鳄",4,1,"prefabs/Animal/animal_012b","textures/animal_012b",0.5f,0f,3f,0f));
			this.AllData.Add("2603",new resourceCell("美国短吻鳄",4,1,"prefabs/Animal/animal_012d","textures/animal_012d",0.5f,0f,3f,0f));
			this.AllData.Add("2604",new resourceCell("尼罗鳄",4,1,"prefabs/Animal/animal_012e","textures/animal_012e",0.5f,0f,3f,0f));
			this.AllData.Add("2605",new resourceCell("奥利诺科鳄",4,1,"prefabs/Animal/animal_012c","textures/animal_012c",0.5f,0f,3f,0f));
			this.AllData.Add("2701",new resourceCell("伶盗龙",4,1,"prefabs/Animal/animal_016a","textures/animal_016a",1f,0f,0f,0f));
			this.AllData.Add("2702",new resourceCell("角鼻龙",4,1,"prefabs/Animal/animal_016a","textures/animal_016a",1f,0f,0f,0f));
			this.AllData.Add("2703",new resourceCell("双冠龙",4,1,"prefabs/Animal/animal_016a","textures/animal_016a",1f,0f,0f,0f));
			this.AllData.Add("2704",new resourceCell("棘龙",4,1,"prefabs/Animal/animal_016a","textures/animal_016a",1f,0f,0f,0f));
			this.AllData.Add("2705",new resourceCell("霸王龙",4,1,"prefabs/Animal/animal_016a","textures/animal_016a",1f,0f,0f,0f));
			this.AllData.Add("2801",new resourceCell("马门溪龙",4,1,"prefabs/Animal/animal_015b","textures/animal_015b",0.1f,0f,0f,0f));
			this.AllData.Add("2802",new resourceCell("梁龙",4,1,"prefabs/Animal/animal_015e","textures/animal_015e",0.1f,0f,0f,0f));
			this.AllData.Add("2803",new resourceCell("雷龙",4,1,"prefabs/Animal/animal_015a","textures/animal_015a",0.1f,0f,0f,0f));
			this.AllData.Add("2804",new resourceCell("峨嵋龙",4,1,"prefabs/Animal/animal_015c","textures/animal_015c",0.1f,0f,0f,0f));
			this.AllData.Add("2805",new resourceCell("地震龙",4,1,"prefabs/Animal/animal_015d","textures/animal_015d",0.1f,0f,0f,0f));
			this.AllData.Add("2901",new resourceCell("尖角龙",4,1,"prefabs/Animal/animal_017b","textures/animal_017b",0.3f,0f,0f,0f));
			this.AllData.Add("2902",new resourceCell("牛角龙",4,1,"prefabs/Animal/animal_017e","textures/animal_017e",0.3f,0f,0f,0f));
			this.AllData.Add("2903",new resourceCell("三角龙",4,1,"prefabs/Animal/animal_017a","textures/animal_017a",0.3f,0f,0f,0f));
			this.AllData.Add("2904",new resourceCell("五角龙",4,1,"prefabs/Animal/animal_017d","textures/animal_017d",0.3f,0f,0f,0f));
			this.AllData.Add("2905",new resourceCell("戟龙",4,1,"prefabs/Animal/animal_017c","textures/animal_017c",0.3f,0f,0f,0f));
			this.AllData.Add("3001",new resourceCell("华阳龙",4,1,"prefabs/Animal/animal_018b","textures/animal_018b",0.3f,0f,0f,0f));
			this.AllData.Add("3002",new resourceCell("巨刺龙",4,1,"prefabs/Animal/animal_018c","textures/animal_018c",0.3f,0f,0f,0f));
			this.AllData.Add("3003",new resourceCell("钉状龙",4,1,"prefabs/Animal/animal_018d","textures/animal_018d",0.3f,0f,0f,0f));
			this.AllData.Add("3004",new resourceCell("沱江龙",4,1,"prefabs/Animal/animal_018e","textures/animal_018e",0.3f,0f,0f,0f));
			this.AllData.Add("3005",new resourceCell("剑龙",4,1,"prefabs/Animal/animal_018a","textures/animal_018a",0.3f,0f,0f,0f));
			this.AllData.Add("3101",new resourceCell("包头龙",4,1,"prefabs/Animal/animal_021b","textures/animal_021b",0.25f,0f,0f,0f));
			this.AllData.Add("3102",new resourceCell("盾甲龙",4,1,"prefabs/Animal/animal_021a","textures/animal_021a",0.25f,0f,0f,0f));
			this.AllData.Add("3103",new resourceCell("森林龙",4,1,"prefabs/Animal/animal_021c","textures/animal_021c",0.25f,0f,0f,0f));
			this.AllData.Add("3104",new resourceCell("埃德蒙顿甲龙",4,1,"prefabs/Animal/animal_021d","textures/animal_021d",0.25f,0f,0f,0f));
			this.AllData.Add("3105",new resourceCell("多刺甲龙",4,1,"prefabs/Animal/animal_021e","textures/animal_021e",0.25f,0f,0f,0f));
			this.AllData.Add("3201",new resourceCell("黑尾袋鼠",4,1,"prefabs/Animal/animal_007b","textures/animal_007b",0.65f,0f,0f,0f));
			this.AllData.Add("3202",new resourceCell("羚大袋鼠",4,1,"prefabs/Animal/animal_007c","textures/animal_007c",0.65f,0f,0f,0f));
			this.AllData.Add("3203",new resourceCell("西部灰袋鼠",4,1,"prefabs/Animal/animal_007a","textures/animal_007a",0.65f,0f,0f,0f));
			this.AllData.Add("3204",new resourceCell("岩大袋鼠",4,1,"prefabs/Animal/animal_007e","textures/animal_007e",0.65f,0f,0f,0f));
			this.AllData.Add("3205",new resourceCell("红大袋鼠",4,1,"prefabs/Animal/animal_007d","textures/animal_007d",0.65f,0f,0f,0f));
			this.AllData.Add("3301",new resourceCell("长尾叶猴",4,1,"prefabs/Animal/animal_013b","textures/animal_013b",0.85f,0f,0f,0f));
			this.AllData.Add("3302",new resourceCell("门岛叶猴",4,1,"prefabs/Animal/animal_013a","textures/animal_013a",0.85f,0f,0f,0f));
			this.AllData.Add("3303",new resourceCell("日本猕猴",4,1,"prefabs/Animal/animal_013c","textures/animal_013c",0.85f,0f,0f,0f));
			this.AllData.Add("3304",new resourceCell("红吼猴",4,1,"prefabs/Animal/animal_013e","textures/animal_013e",0.85f,0f,0f,0f));
			this.AllData.Add("3305",new resourceCell("川金丝猴",4,1,"prefabs/Animal/animal_013d","textures/animal_013d",0.85f,0f,0f,0f));
			this.AllData.Add("3401",new resourceCell("梅花鹿",4,1,"prefabs/Animal/animal_010d","textures/animal_010d",0.6f,0f,0f,0f));
			this.AllData.Add("3402",new resourceCell("泽鹿",4,1,"prefabs/Animal/animal_010e","textures/animal_010e",0.6f,0f,0f,0f));
			this.AllData.Add("3403",new resourceCell("白尾鹿",4,1,"prefabs/Animal/animal_010b","textures/animal_010b",0.6f,0f,0f,0f));
			this.AllData.Add("3404",new resourceCell("白唇鹿",4,1,"prefabs/Animal/animal_010c","textures/animal_010c",0.6f,0f,0f,0f));
			this.AllData.Add("3405",new resourceCell("驼鹿",4,1,"prefabs/Animal/animal_010a","textures/animal_010a",0.6f,0f,0f,0f));
			this.AllData.Add("3501",new resourceCell("印度犀牛",4,1,"prefabs/Animal/animal_006a","textures/animal_006a",0.6f,0f,0f,0f));
			this.AllData.Add("3502",new resourceCell("苏门答腊犀牛",4,1,"prefabs/Animal/animal_006c","textures/animal_006c",0.8f,0f,0f,0f));
			this.AllData.Add("3503",new resourceCell("黑犀牛",4,1,"prefabs/Animal/animal_006d","textures/animal_006d",0.8f,0f,0f,0f));
			this.AllData.Add("3504",new resourceCell("白犀牛",4,1,"prefabs/Animal/animal_006b","textures/animal_006b",0.6f,0f,0f,0f));
			this.AllData.Add("3505",new resourceCell("爪哇犀牛",4,1,"prefabs/Animal/animal_006e","textures/animal_006e",0.8f,0f,0f,0f));
			this.AllData.Add("3601",new resourceCell("普通非洲象",4,1,"prefabs/Animal/animal_002b","textures/animal_002b",0.4f,0f,0f,0f));
			this.AllData.Add("3602",new resourceCell("斯里兰卡象",4,1,"prefabs/Animal/animal_002d","textures/animal_002d",0.4f,0f,0f,0f));
			this.AllData.Add("3603",new resourceCell("婆罗洲象",4,1,"prefabs/Animal/animal_002c","textures/animal_002c",0.4f,0f,0f,0f));
			this.AllData.Add("3604",new resourceCell("亚洲象",4,1,"prefabs/Animal/animal_002e","textures/animal_002e",0.4f,0f,0f,0f));
			this.AllData.Add("3605",new resourceCell("非洲象",4,1,"prefabs/Animal/animal_002a","textures/animal_002a",0.4f,0f,0f,0f));
			this.AllData.Add("3701",new resourceCell("刚果孔雀",4,1,"prefabs/Animal/animal_011e","textures/animal_011e",1f,0f,0f,0f));
			this.AllData.Add("3702",new resourceCell("绿孔雀",4,1,"prefabs/Animal/animal_011b","textures/animal_011b",1f,0f,0f,0f));
			this.AllData.Add("3703",new resourceCell("蓝孔雀",4,1,"prefabs/Animal/animal_011a","textures/animal_011a",1f,0f,0f,0f));
			this.AllData.Add("3704",new resourceCell("黑孔雀",4,1,"prefabs/Animal/animal_011d","textures/animal_011d",1f,0f,0f,0f));
			this.AllData.Add("3705",new resourceCell("白孔雀",4,1,"prefabs/Animal/animal_011c","textures/animal_011c",1f,0f,0f,0f));
			this.AllData.Add("3801",new resourceCell("黑颈鹳",4,1,"prefabs/Animal/animal_019b","textures/animal_019b",0.6f,0f,0f,0f));
			this.AllData.Add("3802",new resourceCell("东方白鹳",4,1,"prefabs/Animal/animal_019c","textures/animal_019c",0.6f,0f,0f,0f));
			this.AllData.Add("3803",new resourceCell("黑鹳",4,1,"prefabs/Animal/animal_019d","textures/animal_019d",0.6f,0f,0f,0f));
			this.AllData.Add("3804",new resourceCell("白鹮鹳",4,1,"prefabs/Animal/animal_019e","textures/animal_019e",0.6f,0f,0f,0f));
			this.AllData.Add("3805",new resourceCell("美洲红鹳",4,1,"prefabs/Animal/animal_019a","textures/animal_019a",0.6f,0f,0f,0f));
			this.AllData.Add("3901",new resourceCell("苍鹰",4,1,"prefabs/Animal/animal_005d","textures/animal_005d",1f,0f,0f,0f));
			this.AllData.Add("3902",new resourceCell("白肩雕",4,1,"prefabs/Animal/animal_005b","textures/animal_005b",1f,0f,0f,0f));
			this.AllData.Add("3903",new resourceCell("虎头海雕",4,1,"prefabs/Animal/animal_005c","textures/animal_005c",1f,0f,0f,0f));
			this.AllData.Add("3904",new resourceCell("菲律宾鹰",4,1,"prefabs/Animal/animal_005e","textures/animal_005e",1f,0f,0f,0f));
			this.AllData.Add("3905",new resourceCell("白头海雕",4,1,"prefabs/Animal/animal_005a","textures/animal_005a",1f,0f,0f,0f));
			this.AllData.Add("4001",new resourceCell("鸵鸟",4,1,"prefabs/Animal/animal_014a","textures/animal_014a",0.6f,0f,0f,0f));
			this.AllData.Add("4002",new resourceCell("南方鹤鸵",4,1,"prefabs/Animal/animal_014e","textures/animal_014e",0.6f,0f,0f,0f));
			this.AllData.Add("4003",new resourceCell("小美洲鸵",4,1,"prefabs/Animal/animal_014c","textures/animal_014c",0.6f,0f,0f,0f));
			this.AllData.Add("4004",new resourceCell("鸸鹋",4,1,"prefabs/Animal/animal_014d","textures/animal_014d",0.6f,0f,0f,0f));
			this.AllData.Add("4005",new resourceCell("大美洲鸵",4,1,"prefabs/Animal/animal_014b","textures/animal_014b",0.6f,0f,0f,0f));
			this.AllData.Add("4101",new resourceCell("黑嘴天鹅",4,1,"prefabs/Animal/animal_020a","textures/animal_020a",1f,0f,0f,0f));
			this.AllData.Add("4102",new resourceCell("黑天鹅",4,1,"prefabs/Animal/animal_020b","textures/animal_020b",1f,0f,0f,0f));
			this.AllData.Add("4103",new resourceCell("黑颈天鹅",4,1,"prefabs/Animal/animal_020d","textures/animal_020d",1f,0f,0f,0f));
			this.AllData.Add("4104",new resourceCell("比尤伊克天鹅",4,1,"prefabs/Animal/animal_020c","textures/animal_020c",1f,0f,0f,0f));
			this.AllData.Add("4105",new resourceCell("大天鹅",4,1,"prefabs/Animal/animal_020e","textures/animal_020e",1f,0f,0f,0f));
			this.AllData.Add("5001",new resourceCell("场景结尾资源",5,1,"prefabs/Probe/end","",-1f,0f,0f,0f));
			this.AllData.Add("5002",new resourceCell("场景组3地块资源",5,1,"prefabs/Probe/kuozhan_03","",-1f,0f,0f,0f));
			this.AllData.Add("5003",new resourceCell("场景组4地块资源",5,1,"prefabs/Probe/kuozhan_04","",-1f,0f,0f,0f));
			this.AllData.Add("5004",new resourceCell("场景组5地块资源",5,1,"prefabs/Probe/kuozhan_01","",-1f,0f,0f,0f));
			this.AllData.Add("5005",new resourceCell("场景组6地块资源",5,1,"prefabs/Probe/kuozhan_02","",-1f,0f,0f,0f));
			this.AllData.Add("5006",new resourceCell("场景组7地块资源",5,1,"prefabs/Probe/kuozhan_05","",-1f,0f,0f,0f));
			this.AllData.Add("5007",new resourceCell("场景组8地块资源",5,1,"prefabs/Probe/kuozhan_06","",-1f,0f,0f,0f));
			this.AllData.Add("5008",new resourceCell("场景默认开启第二组地块",5,1,"prefabs/Probe/kuozhan_07","",-1f,0f,0f,0f));
			this.AllData.Add("6001",new resourceCell("摆渡车",6,1,"prefabs/Probe/Car_bus_001","",-1f,0f,0f,0f));
			this.AllData.Add("7001",new resourceCell("动物展示预制体",7,1,"prefabs/Probe/Animal_1","",-1f,0f,0f,0f));
			this.AllData.Add("8001",new resourceCell("轮船模型资源路径",8,1,"prefabs/Probe/boat","",-1f,0f,0f,0f));
			this.AllData.Add("9001",new resourceCell("停车场第1组",9,1,"prefabs/Probe/parking_lvl_1","",-1f,0f,0f,0f));
			this.AllData.Add("9002",new resourceCell("停车场第2组",9,1,"prefabs/Probe/parking_lvl_2","",-1f,0f,0f,0f));
			this.AllData.Add("9003",new resourceCell("停车场第3组",9,1,"prefabs/Probe/parking_lvl_3","",-1f,0f,0f,0f));
			this.AllData.Add("9004",new resourceCell("停车场第4组",9,1,"prefabs/Probe/parking_lvl_4","",-1f,0f,0f,0f));
			this.AllData.Add("9005",new resourceCell("停车场第5组",9,1,"prefabs/Probe/parking_lvl_5","",-1f,0f,0f,0f));
			this.AllData.Add("9006",new resourceCell("停车场第6组",9,1,"prefabs/Probe/parking_lvl_6","",-1f,0f,0f,0f));
			this.AllData.Add("9007",new resourceCell("停车场第7组",9,1,"prefabs/Probe/parking_lvl_7","",-1f,0f,0f,0f));
			this.AllData.Add("9008",new resourceCell("停车场第8组",9,1,"prefabs/Probe/parking_lvl_8","",-1f,0f,0f,0f));
			this.AllData.Add("9100",new resourceCell("游客照相特效资源",10,1,"prefabs/Effect/Fx_Camera","",-1f,0f,0f,0f));
			this.AllData.Add("9101",new resourceCell("游客招手特效资源",11,1,"prefabs/Effect/Fx_Love","",-1f,0f,0f,0f));
			this.AllData.Add("9201",new resourceCell("动物升级特效",12,1,"prefabs/Effect/Fx_AnimalUp","",-1f,0f,0f,0f));
			this.AllData.Add("9301",new resourceCell("游客观光愤怒表情",13,1,"prefabs/Expression/AngerExpression","",-1f,0f,0f,0f));
		}
	}
	public class resourceCell
	{
		///<summary>
		///备注
		///<summary>
		public readonly string remarks;
		///<summary>
		///随机类型
		///<summary>
		public readonly int restype;
		///<summary>
		///加载类型
		///<summary>
		public readonly int loadtype;
		///<summary>
		///车预制体路径
		///<summary>
		public readonly string prefabpath;
		///<summary>
		///车贴图路径
		///<summary>
		public readonly string texturepath;
		///<summary>
		///缩放比例
		///<summary>
		public readonly float zoomratio;
		///<summary>
		///动物展示X轴偏移
		///<summary>
		public readonly float Xoffset;
		///<summary>
		///动物展示Y轴偏移
		///<summary>
		public readonly float Yoffset;
		///<summary>
		///动物展示Z轴偏移
		///<summary>
		public readonly float Zoffset;
		public resourceCell(string remarks,int restype,int loadtype,string prefabpath,string texturepath,float zoomratio,float Xoffset,float Yoffset,float Zoffset){
			this.remarks = remarks;
			this.restype = restype;
			this.loadtype = loadtype;
			this.prefabpath = prefabpath;
			this.texturepath = texturepath;
			this.zoomratio = zoomratio;
			this.Xoffset = Xoffset;
			this.Yoffset = Yoffset;
			this.Zoffset = Zoffset;
		}
	}
}