using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger;
using UnityEngine.SceneManagement;

namespace LittleGame
{

    public enum PreFabType
    {
        animals,
        road,
        cage,
        car,
        /// <summary>
        /// 终点
        /// </summary>
        destination,
        ///所用场景预制件
        sceneTerrain,
        /// <summary>
        /// 需要场景路面预制件
        /// </summary>
        sceneRoad,
        /// <summary>
        /// 装饰物
        /// </summary>
        ornamental

    }

    /// <summary>
    /// 用于保存加载到内存的数据对象
    /// 未来不需要这个类  应该是使用外部的加载方式加载
    /// 应该是先找需要加载什么再开始加载
    /// </summary>
    public class ResourceData : MonoBehaviour
    {
        private static Dictionary<PreFabType, List<GameObject>> resDic = new Dictionary<PreFabType, List<GameObject>>();

        public static ResourceData Instance;

        Transform resObj;

        //ivate GameObject resGroup;

        private void Awake()
        {
            Instance = this;
            
        }

        void Start()
        {
            if (resObj != null)
            {
                SceneManager.LoadSceneAsync("LittleScene");
                return;
            }
            Load3DPreFabs();
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 加载所有的3D资源
        /// </summary>
        public void Load3DPreFabs()
        {
            resObj = transform;
            StartCoroutine(LoadCar());
        }


        /// <summary>
        /// 随机获取一个车对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetCar()
        {
            if (resDic.ContainsKey(PreFabType.car))
            {
                if (resDic[PreFabType.car].Count < 0)
                {
                    Debug.LogError("车资源没有被加载完成");
                    return null;
                }
                var randNum = Random.Range(0, resDic[PreFabType.car].Count);
                Debug.Log("大小" + resDic[PreFabType.car].Count + " 随机数 " + randNum);
                return Instantiate(resDic[PreFabType.car][randNum]);
            }
            return null;
        }
        /* ///// <summary>
         ///// 随机获取一个树模型
         ///// </summary>
         ///// <returns></returns>
         ////public Tree GetTree()
         ////{
         ////    if (treeList.Count < 0)
         ////    {
         ////        LogWarp.LogError("树资源没有被加载完成");
         ////        return null;
         ////    }
         ////    int index = Random.Range(0, treeList.Count);

         ////    return treeList[index];
         ////}*/


        /// <summary>
        /// 获取场景路面模型
        /// </summary>
        /// <returns></returns>
        public GameObject GetSceneRoad()
        {
            if (resDic.ContainsKey(PreFabType.sceneRoad))
            {
                if (resDic[PreFabType.sceneRoad].Count < 0)
                {
                    LogWarp.LogError("场景路面路资源没有被加载完成");
                    return null;
                }
                return Instantiate(resDic[PreFabType.sceneRoad][0]);
            }
            return null;
        }

        /// <summary>
        /// 获取场景地面模型
        /// </summary>
        /// <returns></returns>
        public GameObject GetSceneTerrain()
        {
            if (resDic.ContainsKey(PreFabType.sceneTerrain))
            {
                if (resDic[PreFabType.sceneTerrain].Count < 0)
                {
                    LogWarp.LogError("场景地面路资源没有被加载完成");
                    return null;
                }
                return Instantiate(resDic[PreFabType.sceneTerrain][0]);
            }
            return null;
        }

        /// <summary>
        /// 获取一个装饰物模型
        /// </summary>
        /// <returns></returns>
        public GameObject GetOrnamental(int index)
        {
            if (resDic.ContainsKey(PreFabType.ornamental))
            {
                if (resDic[PreFabType.ornamental].Count < 0)
                {
                    LogWarp.LogError("路资源没有被加载完成");
                    return null;
                }
                return Instantiate(resDic[PreFabType.ornamental][index]);
            }
            return null;
        }


        /// <summary>
        /// 获取一个路模型
        /// </summary>
        /// <returns></returns>
        public GameObject GetRood(int index)
        {
            if (resDic.ContainsKey(PreFabType.road))
            {
                if (resDic[PreFabType.road].Count < 0)
                {
                    LogWarp.LogError("路资源没有被加载完成");
                    return null;
                }
                return Instantiate(resDic[PreFabType.road][index-1]);
            }
            return null;
        }

        /// <summary>
        /// 获取一个笼子模型
        /// </summary>
        /// <returns></returns>
        public GameObject GetCage(int index)
        {
            if (resDic.ContainsKey(PreFabType.cage))
            {
                if (resDic[PreFabType.cage].Count < 0)
                {
                    LogWarp.LogError("笼子资源没有被加载完成");
                    return null;
                }
                return Instantiate(resDic[PreFabType.cage][index]);
            }
            return null;
        }

        /// <summary>
        /// 获得一个终点模型
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject GetDestination(int index)
        {
            if (resDic.ContainsKey(PreFabType.destination))
            {
                if (resDic[PreFabType.destination].Count < 0)
                {
                    LogWarp.LogError("终点资源没有被加载完成");
                    return null;
                }
                return Instantiate(resDic[PreFabType.destination][index]);
            }
            return null;
        }

        /// <summary>
        /// 随机获取一个小动物模型
        /// </summary>
        /// <returns></returns>
        public GameObject GetAnimal()
        {
            if (resDic.ContainsKey(PreFabType.animals))
            {
                if (resDic[PreFabType.animals].Count < 0)
                {
                    LogWarp.LogError("小动物资源没有被加载完成");
                    return null;
                }
                int index = Random.Range(0, resDic[PreFabType.animals].Count);

                return Instantiate(resDic[PreFabType.animals][index]);
            }
            else
            {
                LogWarp.LogError("小动物资源没有被加载完成");
            }
            return null;
        }

        /// <summary>
        /// 卸载所有资源
        /// </summary>
        public void UnLoadResource()
        {
            foreach (var item in resDic)
            {
                Utility.ClearList(item.Value);
            }

            resDic.Clear();

            //清理缓存
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }


        #region 私有方法
        /// <summary>
        /// 加载车
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadCar()
        {
            GameObject carObj = new GameObject("car");
            carObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            var data = Config.resourceConfig.getInstace().AllData;
            foreach (var item in data)
            {
                if (item.Value.prefabpath.Contains("Car"))
                {
                    Debug.Log("加载了一个");
                    GameObject res = Instantiate(Resources.Load<GameObject>(item.Value.prefabpath));
                    res.transform.SetParent(carObj.transform);
                    res.SetActive(false);
                    list.Add(res);
                }
            }

            resDic.Add(PreFabType.car, list);
            yield return StartCoroutine(LoadAnimals());
        }

        IEnumerator LoadAnimals()
        {
            GameObject animalsObj = new GameObject("animals");
            animalsObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.AnimalResourcePath));
            res.transform.SetParent(animalsObj.transform);
            res.SetActive(false);
            list.Add(res);
            resDic.Add(PreFabType.animals, list);
            yield return StartCoroutine(LoadCage());
        }

        IEnumerator LoadCage()
        {
            GameObject cageObj = new GameObject("cage");
            cageObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.CageResourcePath));
            res.transform.SetParent(cageObj.transform);
            res.SetActive(false);
            list.Add(res);
            resDic.Add(PreFabType.cage, list);
            yield return StartCoroutine(LoadRood());
        }

        /// <summary>
        /// 加载路
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadRood()
        {
            GameObject roadObj = new GameObject("road");
            roadObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < LittleGameDefine.RoomNumber; i++)
            {
                GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.RoodResourcePath + (i + 1)));
                res.transform.SetParent(roadObj.transform);
                res.SetActive(false);
                list.Add(res);
            }
            resDic.Add(PreFabType.road, list);
            yield return StartCoroutine(LoadSceneTerrainPrefab());

            // ResourceLoadTool.IsLoadResourceDone = true;
        }

        /// <summary>
        /// 加载场景需要地面资源
        /// 应该是分成3部分加载 图片 预制件 然后根据配置表拼接
        /// 当前没有配置表就暂时这么写
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadSceneTerrainPrefab()
        {
            GameObject sceneObj = new GameObject("sceneTerrainObj");
            sceneObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.SceneTerrainResourcePath));
            res.transform.SetParent(sceneObj.transform);
            res.SetActive(false);
            list.Add(res);
            resDic.Add(PreFabType.sceneTerrain, list);
            yield return StartCoroutine(LoadDestination());
        }

        /// <summary>
        /// 加载场景需要马路资源
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadSceneRoadPrefab()
        {
            GameObject sceneObj = new GameObject("SceneRoadObj");
            sceneObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.SceneRoadResourcePath));
            res.transform.SetParent(sceneObj.transform);
            res.SetActive(false);
            list.Add(res);
            resDic.Add(PreFabType.sceneRoad, list);
            yield return StartCoroutine(LoadDestination());
        }


        /// <summary>
        /// 加载终点
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadDestination()
        {
            GameObject destinationObj = new GameObject("destination");
            destinationObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.DestinationResourcePath));
            res.transform.SetParent(destinationObj.transform);
            res.SetActive(false);
            list.Add(res);
            resDic.Add(PreFabType.destination, list);
            yield return StartCoroutine(LoadOrnamental());
        }

        /// <summary>
        /// 加载小游戏场景装饰物
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadOrnamental()
        {
            GameObject ornamentalObj = new GameObject("ornamental");
            ornamentalObj.transform.SetParent(resObj);
            List<GameObject> list = new List<GameObject>();
            int ornamentalNum = 5;
            for (int i = 1; i < ornamentalNum+1; i++)
            {
                GameObject res = Instantiate(Resources.Load<GameObject>(LittleGameDefine.LittleGameOrnamentalPath + i));
                res.transform.SetParent(ornamentalObj.transform);
                res.SetActive(false);
                list.Add(res);
            }
            resDic.Add(PreFabType.ornamental, list);
            yield return StartCoroutine(ChangeScene());
        }


        IEnumerator ChangeScene()
        {
            //SceneManager.LoadScene("LittleScene");
            SceneManager.LoadSceneAsync("LittleScene");
            yield return null;
        }

        #endregion
    }

}