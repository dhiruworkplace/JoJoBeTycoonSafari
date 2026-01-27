using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using ZooGame.GlobalData;
using Logger;
using UFrame.MiniGame;

namespace ZooGame
{
    public partial class ParkingCenter : GameModule
    {
        /// <summary>
        /// 给定一个等级范围数组，等级，返回等级所在范围数组的索引
        /// 等级范围数组 1|50|100|150|200|250|300|350
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int FindLevelRangIndex(int[] levels, int level)
        {
            int idx = UFrame.Const.Invalid_Int;
            for (int j = 0; j < levels.Length; j++)
            {
                if (level <= levels[j])
                {
                    idx = j;
                    break;
                }
            }
            if (idx == UFrame.Const.Invalid_Int)    //若idx超过数组范围则等于最大返回结果
            {
                idx = levels.Length - 1;
            }
            return idx;
        }

        /// <summary>
        /// 加载停车场场景
        /// </summary>
        /// <param name="idx"></param>
        public static void LoadParkingScene( int idx )
        {
            GameObject parckingSencePos = GlobalDataManager.GetInstance().zooGameSceneData.ParckingSencePos;
            if (parckingSencePos != null)
            {
                for (int i = 0; i < parckingSencePos.transform.childCount; i++)
                {
                    GameObject.Destroy(parckingSencePos.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                string e = string.Format("报错：停车场挂点资源对象为  null");
                throw new System.Exception(e);
            }

            var cellBuild = Config.parkingConfig.getInstace().getCell(1);
            //LogWarp.LogError(" 测试：当前停车场模型 cellBuild.openggroup[idx]=" + cellBuild.openggroup[idx]);

            var cellRes = Config.resourceConfig.getInstace().getCell(cellBuild.openggroup[idx]);
            var goLittleZoo = ResourceManager.GetInstance().LoadGameObject(cellRes.prefabpath);
            //LogWarp.LogError(" 测试：当前停车场模型 name=" + goLittleZoo.name);


            goLittleZoo.transform.position = new UnityEngine.Vector3(0, 0, 0);

            goLittleZoo.transform.SetParent(parckingSencePos.transform, false);

            //添加停车场的升级特效
            Transform trans = null;
            trans = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().ParkingUpEffect).transform;
            trans.SetParent(GlobalDataManager.GetInstance().zooGameSceneData.ParckingSencePos.transform, false);

            UFrame.SimpleParticle particlePlayer = new UFrame.SimpleParticle();
            particlePlayer.Init(GlobalDataManager.GetInstance().zooGameSceneData.ParckingSencePos);
            particlePlayer.Play();


        }
    }
}

