using Game.GlobalData;
using Logger;
using System.Collections;
using System.Collections.Generic;
using UFrame.MiniGame;
using UnityEngine;
namespace Game
{
    public class SceneUIArray
    {   
        public GameObject VisitSeatGB;//场景CD
        public GameObject SceneFlutterGB;//场景飘字
    }

    public class LittleSceneUI 
    {
        GameObject littleZooMountPoint;//场景动物栏悬挂节点
        Dictionary<int, Dictionary<int, SceneUIArray>> zooSceneUIDic;//场景动物栏悬挂节点对象的存储字典

        GameObject ticketMountPoint;//场景售票口悬挂节点
        Dictionary<int, SceneUIArray> entrySceneUIDic;//场景售票口悬挂节点对象的存储字典

        public LittleSceneUI()
        {
            zooSceneUIDic = new Dictionary<int, Dictionary<int, SceneUIArray>>();
            entrySceneUIDic = new Dictionary<int,  SceneUIArray>();

        }

        /// <summary>
        /// 获取对应动物栏ID对应的的观光位下的UI显示对象
        /// </summary>
        /// <param name="littleZooID">动物栏ID</param>
        /// <param name="idx">观光位下标</param>
        /// <param name="ID">类型</param>
        /// <returns></returns>
        public GameObject GetLittleZooVisitSeatGameObject(int littleZooID, int idx, int ID)
        {
            GameObject visitSeatGB = null;
            GameObject sceneFlutterGB = null;

            //判断是否有littleZooID的key值，没有则添加
            if (!zooSceneUIDic.ContainsKey(littleZooID))
            {
                Dictionary<int, SceneUIArray> dic = new Dictionary<int, SceneUIArray>();
                zooSceneUIDic.Add(littleZooID,dic);
            }
            Dictionary<int, SceneUIArray> dic2 = zooSceneUIDic[littleZooID];
            //判断是否有idx的key值，没有则添加
            if (!dic2.ContainsKey(idx))
            {
                //生成对应的字典和GameObject 
                string visitSeatPath = Config.globalConfig.getInstace().VisitSeatCDGameObject;
                string sceneFlutterTextPath = Config.globalConfig.getInstace().SceneFlutterTextGameObject;
                var buildinPos = LittleZooBuildinPosManager.GetInstance().GetLittleZooBuildinPos(littleZooID);
                Vector3 vector = buildinPos.visitPosList[idx];
                

                if (littleZooMountPoint == null)
                {
                    littleZooMountPoint = GameObject.Find(Config.globalConfig.getInstace().BuildVisitEffect);
                    LogWarp.LogError("测试：只获取一次 littleZooMountPoint 对象");
                }
                visitSeatGB = SetLoadGameObject(vector, idx, visitSeatPath, littleZooMountPoint.transform);             
                float scale = Config.globalConfig.getInstace().CdTimeZoom;
                visitSeatGB.transform.localScale = new Vector3(scale, scale, scale);

                sceneFlutterGB = SetLoadGameObject(vector, idx, sceneFlutterTextPath, littleZooMountPoint.transform);
                SceneUIArray littleUIArray = new SceneUIArray()
                {
                    VisitSeatGB = visitSeatGB,
                    SceneFlutterGB = sceneFlutterGB
                };
                zooSceneUIDic[littleZooID][idx] = littleUIArray;
            }

            switch (ID)
            {
                case 0:
                    return zooSceneUIDic[littleZooID][idx].VisitSeatGB;
                case 1:
                    return zooSceneUIDic[littleZooID][idx].SceneFlutterGB;
                default:
                    LogWarp.LogError("场景观光位UI：获取类型不对 = "+ID);
                    return null;
            }
        }

        /// <summary>
        /// 获取对应下标的售票口的UI显示对象
        /// </summary>
        /// <param name="vector">售票口下标位置</param>
        /// <param name="idx">售票口下标</param>
        /// <param name="ID">类型</param>
        /// <returns></returns>
        public GameObject GetEntryUISceneGameObject(int idx, int ID)
        {

            GameObject visitSeatGB = null;
            GameObject sceneFlutterGB = null;

            //判断是否有idx的key值，没有则添加
            if (!entrySceneUIDic.ContainsKey(idx))
            {
                //生成对应的字典和GameObject 
                string visitSeatPath = Config.globalConfig.getInstace().VisitSeatCDGameObject;
                string sceneFlutterTextPath = Config.globalConfig.getInstace().SceneFlutterTextGameObject;
                if (ticketMountPoint == null)
                {
                    ticketMountPoint = GameObject.Find(Config.globalConfig.getInstace().TicketsQueueEffect);
                }

                //售票口列表
                Vector3 vector = GlobalDataManager.GetInstance().zooGameSceneData.entryGateSenceData.entryGatesVector[idx];
                //LogWarp.LogError("测试：vector对象  "+ vector);
                visitSeatGB = SetLoadGameObject(vector, idx, visitSeatPath, ticketMountPoint.transform);

                float scale = Config.globalConfig.getInstace().CdTimeZoom;
                visitSeatGB.transform.localScale = new Vector3(scale, scale, scale);

                sceneFlutterGB = SetLoadGameObject(vector, idx, sceneFlutterTextPath, ticketMountPoint.transform);
                Vector3 vector1 = sceneFlutterGB.transform.position;
                sceneFlutterGB.transform.position = new Vector3(vector1.x+10,vector1.y+15,vector1.z+10);
                SceneUIArray littleUIArray = new SceneUIArray()
                {
                    VisitSeatGB = visitSeatGB,
                    SceneFlutterGB = sceneFlutterGB
                };

                entrySceneUIDic[idx] = littleUIArray;
            }

            switch (ID)
            {
                case 0:
                    //LogWarp.LogError("测试：       idx      " + entrySceneUIDic[idx].VisitSeatGB);
                    return entrySceneUIDic[idx].VisitSeatGB;
                case 1:
                    return entrySceneUIDic[idx].SceneFlutterGB;
                default:
                    LogWarp.LogError("场景UI显示：获取类型不对 = " + ID);
                    return null;
            }
        }




        /// <summary>
        /// 生成对应的观光点CD和飘字
        /// </summary>
        /// <param name="littleZooID"></param>
        /// <param name="idx"></param>
        GameObject SetLoadGameObject(Vector3 vector, int idx, string path,Transform transform)
        {
            
            GameObject gameObjectPrefabs = ResourceManager.GetInstance().LoadGameObject(path);//克隆对象
            gameObjectPrefabs.name = gameObjectPrefabs.name + "_" + idx.ToString();
            gameObjectPrefabs.transform.position = vector; //附上位置
            gameObjectPrefabs.transform.SetParent(transform, true);  //设置父节点
            
            return gameObjectPrefabs;
        }





       

    }
}
