using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/FreeItemAdConfig")]
    public class FreeItemAdConfig : ScriptableObject
    {
        [Header("热气球")]
        public GameObject balloonTmpl;
        public Vector3 balloonViewportPos = new Vector3(0.5f, 0.5f, 1f);
        public float balloonShowDuration = 1.2f;
        public AnimationCurve balloonShowAnimCurve;
        public float balloonLeaveDuration = 0.6f;
    }


    
}
