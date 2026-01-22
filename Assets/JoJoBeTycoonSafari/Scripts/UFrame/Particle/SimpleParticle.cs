/*******************************************************************
* FileName:     SimpleParticle.cs
* Author:       Fan Zheng Yong
* Date:         2019-10-22
* Description:  
* other:    
********************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFrame
{
    public class SimpleParticle
    {
        ParticleSystem[] particles;

        public bool isInit { get; protected set; }

        public void Init(GameObject owner)
        {
            if (particles == null || particles.Length == 0)
            {
                particles = owner.GetComponentsInChildren<ParticleSystem>();
            }

            isInit = true;
        }

        public void UnInit()
        {
            isInit = false;
            particles = null;
        }

        public void Play()
        {
            for(int i = 0, iMax = particles.Length; i < iMax; i++)
            {
                particles[i].Play();
            }
        }

        public void Pause()
        {
            for (int i = 0, iMax = particles.Length; i < iMax; i++)
            {
                particles[i].Pause();
            }
        }

        public void Stop()
        {
            for (int i = 0, iMax = particles.Length; i < iMax; i++)
            {
                particles[i].Stop();
            }
        }

        public bool IsPlaying()
        {
            for(int i = 0; i < particles.Length; i++)
            {
                if (particles[i].isPlaying)
                {
                    return true;
                }
            }

            return false;
        }

        public void Release()
        {
            Stop();
            particles = null;
        }
    }

}
