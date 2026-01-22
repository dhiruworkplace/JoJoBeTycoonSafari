using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalData
{
    public class AnimalAnimation
    {
        public string walkName { get; protected set; }
        public string poseName { get; protected set; }
        public string idleName { get; protected set; }

        public AnimalAnimation()
        {
            Init();
        }

        protected void Init()
        {
            walkName = Config.globalConfig.getInstace().AnimalWalk;
            idleName = Config.globalConfig.getInstace().AnimalIdle;
            poseName = Config.globalConfig.getInstace().AnimalPose;
        }

        public string GetRandomName()
        {
            string randomName = idleName;
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r > 0.5f)
            {
                randomName = poseName;
            }
            return randomName;
        }
    }
}
