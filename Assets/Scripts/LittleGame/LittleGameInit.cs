using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using Game.GlobalData;

namespace LittleGame
{

    /// <summary>
    /// 用于初始化关卡需要信息
    /// </summary>
    public class LittleGameInit : MonoBehaviour
    {
        public List<GameObject> currentLevelRood;

        public List<GameObject> currentLevelCage;

        public List<GameObject> currentLevelCar;

        public Queue<GameObject> cacheCar;

        private Dictionary<string, gamechapterCell> dataDic;
        //private Dictionary<string,>

        void Start()
        {
            dataDic = gamechapterConfig.getInstace().AllData;
            ///需要根据不同关卡生成的道具 在这里主动调用
            


            cacheCar = new Queue<GameObject>();
        }

        private void LoadNeedLevelMessage()
        {

        }

        /// <summary>
        /// 抛出一个车
        /// </summary>
        /// <returns></returns>
        public GameObject GetCar()
        {
            GameObject car = null;
            if (cacheCar.Count > 0)
            {
                car = cacheCar.Dequeue();
            }
            else
            {
                int id = Random.Range(0, currentLevelCar.Count);
                GameObject carPfb = Instantiate(currentLevelCar[id]);
                car = carPfb;
            }
            car.transform.tag = "Car";
            car.transform.localScale = Vector3.one * 0.6f;
            car.SetActive(true);
            return car;
        }

        /// <summary>
        /// 缓存一个车
        /// </summary>
        /// <param name="car"></param>
        public void SaveCar(GameObject car)
        {
            car.SetActive(false);
            cacheCar.Enqueue(car);
        }

        /// <summary>
        /// 清理上一关的资源
        /// </summary>
        public void ClearLastLevelRes()
        {
            if (currentLevelRood != null)
                Utility.ClearList(currentLevelRood);
            if (currentLevelCage != null)
                Utility.ClearList(currentLevelCage);
            Debug.Log("清理完成");
        }

        /// <summary>
        /// 车子是不一样的 可以复用
        /// </summary>
        public void InitLevelCar()
        {
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            currentLevelCar = new List<GameObject>();
            GameObject pfb = ResourceData.Instance.GetCar();
            currentLevelCar.Add(pfb);
        }


        /// <summary>
        /// 初始化笼子信息
        /// </summary>
        public void InitLevelCage()
        {
            //Debug.Log(LittleController.CurrentLevel.ToString());
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            currentLevelCage = new List<GameObject>();
            for (int i = 0; i < currentLevelData.animalmax; i++)
            {
                GameObject pfb = ResourceData.Instance.GetCage(0);
                currentLevelCage.Add(pfb);
                pfb.AddComponent<Cage>();
            }
        }


        /// <summary>
        /// 初始化关卡道路信息
        /// </summary>
        public void InitLevelRoad()
        {
            ///获取关卡数据,
            ///产生一个随机数
            ///如果数字小于刷新马路的权重值
            ///则刷新当前道路

            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            currentLevelRood = new List<GameObject>();

            for (int i = 0; i < currentLevelData.roadnum; i++)
            {
                int rang;
                rang = Random.Range(0, 10);
                //检查当前应该刷新哪条马路
                if (rang <= currentLevelData.laneweight[0])
                {
                    InitRood(1, currentLevelData);
                }
                else if (rang < (currentLevelData.laneweight[0] + currentLevelData.laneweight[1]))
                {
                    InitRood(2, currentLevelData);
                }
                else if (rang < (currentLevelData.laneweight[0] + currentLevelData.laneweight[1] + currentLevelData.laneweight[2]))
                {
                    InitRood(3, currentLevelData);
                }
                else if (rang < (currentLevelData.laneweight[0] + currentLevelData.laneweight[1] + currentLevelData.laneweight[2]
                                + currentLevelData.laneweight[3]))
                {
                    InitRood(4, currentLevelData);
                }
                else if (rang < (currentLevelData.laneweight[0] + currentLevelData.laneweight[1] + currentLevelData.laneweight[2]
                    + currentLevelData.laneweight[3] + currentLevelData.laneweight[4]))
                {
                    InitRood(5, currentLevelData);
                }
                else
                {
                    InitRood(6, currentLevelData);
                }

            }
        }

        /// <summary>
        /// 初始化单条马路的信息
        /// </summary>
        /// <param name="index"></param>
        private void InitRood(int index, gamechapterCell data)
        {
            
            GameObject roodPrefab = ResourceData.Instance.GetRood(index);
            currentLevelRood.Add(roodPrefab);
            Road rood = roodPrefab.AddComponent<Road>();
            rood.speed = Random.Range(data.speedstar, data.speedend);
            rood.minInterval = data.Intervalstar;
            rood.maxInterval = data.Intervalend;
            rood.needCarNum = index;
            rood.roodPrefab = roodPrefab;
        }

        /// <summary>
        /// 获取一个随机的车子刷新时间
        /// </summary>
        /// <returns></returns>
        public float GetRandomCarFlushInterval()
        {
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            return Random.Range(currentLevelData.Intervalstar, currentLevelData.Intervalend);
        }

        /// <summary>
        /// 获取一个随机的车子速度
        /// </summary>
        /// <returns></returns>
        public float GetRandomCarSpeed()
        {
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            return Random.Range(currentLevelData.speedstar, currentLevelData.speedend);
        }

        /// <summary>
        /// 获取可以刷新的笼子的数量
        /// </summary>
        /// <returns></returns>
        public int GetCageNum()
        {
            return dataDic[LittleController.CurrentLevel.ToString()].animalmax;
        }


        /// <summary>
        /// 初始化关卡奖励信息
        /// </summary>
        /// <param name="Level"></param>
        public void InitLevelReWard(int Level)
        {
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            LevelData.firstGoldReward = currentLevelData.firstgoldreward;
            LevelData.repeatGoldReward = currentLevelData.repeatgoldreward;
            LevelData.firstDropWard = currentLevelData.firstdropward;
            LevelData.firstDropNum = currentLevelData.firstdropnum;
            LevelData.repeatDropWard = currentLevelData.repeatdropward;
            LevelData.wardDouble = currentLevelData.warddouble;
            var RMBStr = currentLevelData.diamondreward.Split('|');
            int minNum = 0;
            int maxNum = 0;
            if (!int.TryParse(RMBStr[0], out minNum))
            {
                Debug.LogError(RMBStr[0]+"解析当前钻石数量报错");
            }
            if (!int.TryParse(RMBStr[1], out maxNum))
            {
                Debug.LogError("解析当前钻石数量报错");
            }
            LevelData.RMB = Random.Range(minNum, maxNum);
        }

        /// <summary>
        /// 获取当前拥有小动物的数量
        /// </summary>
        /// <returns></returns>
        //public int GetAnimalsNum() => PlayerZoo.hasAnimalsNum;


        /// <summary>
        /// 获取当前开放最大关卡
        /// </summary>
        /// <returns></returns>
        public int GetMaxLevel() => dataDic.Count;


        /// <summary>
        /// 获取关卡的初始动物数量
        /// </summary>
        /// <returns></returns>
        public int GetAnimalsInitNum()
        {
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            
            return currentLevelData.animalnum;
        }

        /// <summary>
        /// 获取一个小动物的移动速度
        /// </summary>
        /// <returns></returns>
        public float GetAnimalsSpeed()
        {
            return Config.globalConfig.getInstace().LittleGameAnimalSpeed;
        }

        /// <summary>
        /// 获取笼子向上飞的速度
        /// </summary>
        /// <returns></returns>
        public float GetCageFlySpeed()
        {
            return 0.5f;
        }

        /// <summary>
        /// 获取当前关卡场景的地面预制件
        /// </summary>
        /// <param name="level">当前关卡</param>
        /// <returns></returns>
        public GameObject GetSceneTerrain(int level)
        {
            GameObject terrain = null;
            gamechapterCell currentLevelData = dataDic[LittleController.CurrentLevel.ToString()];
            terrain = ResourceData.Instance.GetSceneTerrain();
            terrain.SetActive(true);
            //GameObject sceneRoad = ResourceData.Instance.GetSceneRoad();
            //Vector3 pos = sceneRoad.transform.position;
            //sceneRoad.SetActive(true);
            //sceneRoad.transform.SetParent(terrain.transform);
            //sceneRoad.transform.position = Vector3.forward * 320;
            return terrain;
        }

        /// <summary>
        /// 获取场景路面的宽度
        /// </summary>
        /// <returns></returns>
        public float GetSceneRoadWidth()
        {
            var sceneRoad = GameObject.Find("LevelRes/scene/Little_scene01_a");
            BoxCollider collider;
            if (sceneRoad.gameObject.GetComponent<BoxCollider>() == null)
            {
                collider = sceneRoad.gameObject.AddComponent<BoxCollider>();
            }
            collider = sceneRoad.gameObject.GetComponent<BoxCollider>();
            return collider.size.x;
        }

        /// <summary>
        /// 获取关卡中小动物的数量
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetCurrentLevelAnimalsNum(int level)
        {
            if (dataDic == null)
            {
                dataDic = gamechapterConfig.getInstace().AllData;
            }
            return dataDic[level.ToString()].animalnum;
        }

        public GameObject GetOrnamental(int index)
        {
            return ResourceData.Instance.GetOrnamental(index);
        }
    }
}