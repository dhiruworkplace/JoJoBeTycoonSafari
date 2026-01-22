using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/WorldMapConfig")]
    public class WorldMapConfig : ScriptableObject
    {
        public Material pieceDarkMaterial;
        public Sprite starSprite;
        public Sprite goldSprite;

        [Header("待开启状态")]
        public float unbrowseSymbolAnimDuration = 0.6f;
        public float unbrowseSymbolAnimRange = 30f;
        public AnimationCurve unbrowseSymbolAnimCurve;
        public Vector2 unbrowseBubbleOffset = Vector2.zero;

        [Header("选定地块")]
        public float selectedAnimDuration = 0.3f;
        public Vector2 selectedScale = new Vector2(1.1f, 1.1f);
        public AnimationCurve selectedAnimCurve;
        public Color selectedShadowColor = new Color(0, 0, 0, 0.5f);
        public Vector2 selectedShadowDist = new Vector2(0, -5);

        [Header("当前场景")]
        public Vector2 currPointerOffset = Vector2.zero;
    }
}
