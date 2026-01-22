using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{

    /// <summary>
    /// 游戏数据
    /// </summary>
    public class LittleGameDefine
    {
        /// <summary>
        /// 小动物的宽度
        /// </summary>
        public static Vector3 AnimalWidth = new Vector3(0, 0, 2f);

        /// <summary>
        /// 马路的宽度
        /// </summary>
        public static float RoadWidth = 1.5f;

        /// <summary>
        /// 笼子的宽度
        /// </summary>
        public static int CageWidth = 6;

        /// <summary>
        /// 马路的数量 涉及到资源的加载
        /// </summary>
        public const int RoomNumber = 6;

        /// <summary>
        /// 默认小动物数量
        /// </summary>
        public const int DefaultAnimalsNum = 5;

        /// <summary>
        /// 游戏过程中可以拥有的最大小动物数量
        /// </summary>
        public const int MaxAnimalsNum = 25;

        /// <summary>
        /// 装饰物的数量 3个装饰物 + 空
        /// </summary>
        public const int OrnamentalNum = 4;

        #region 加载资源路径

        public static string CarResourcePath = "LittleRes/Car_Large_03";

        public static string RoodResourcePath = "prefabs/Little_scene_prefabs/chedao_0";

        public static string CageResourcePath = "prefabs/Probe/longzi_01";

        public static string AnimalResourcePath = "prefabs/Animal/cattle";

        public static string TreeResourcePath = "prefabs/Tree/";

        /// <summary>
        /// 终点的资源路径
        /// </summary>
        public static string DestinationResourcePath = "prefabs/Probe/zhongdian01";

        /// <summary>
        /// 需要地面场景的路径
        /// </summary>
        public static string SceneTerrainResourcePath = "prefabs/Little_scene_prefabs/Little_scene01";

        /// <summary>
        /// 需要路面场景的路径
        /// </summary>
        public static string SceneRoadResourcePath = "prefabs/Probe/lu_01";

        /// <summary>
        /// 小游戏需要材质球路径
        /// </summary>
        public static string LittleGameTexturePath = "LittleGameTexture/";

        /// <summary>
        /// 小游戏需要装饰物路径
        /// </summary>
        public static string LittleGameOrnamentalPath = "prefabs/Little_scene_prefabs/Little_scene01_00";


        #endregion
    }

}