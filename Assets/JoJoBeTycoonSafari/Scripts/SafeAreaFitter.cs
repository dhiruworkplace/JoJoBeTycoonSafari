using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rectTransform;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);
    ScreenOrientation lastOrientation = ScreenOrientation.Portrait;

    // If true, the rect will be fitted to the safe area.
    public bool fitToSafeArea = true;

    // If true, keep preserving current anchorMin/anchorMax pivot offsets
    public bool preserveScale = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // update only when something changes (orientation / safe area)
        if (Screen.safeArea != lastSafeArea || Screen.orientation != lastOrientation)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        if (!fitToSafeArea)
        {
            // If not fitting, remove any previously applied safe area (reset anchors)
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            lastSafeArea = safeArea;
            lastOrientation = Screen.orientation;
            return;
        }

        // convert safeArea pixels to normalized anchor coordinates (0..1)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Apply anchors
        if (preserveScale)
        {
            // Keep sizeDelta/pivot roughly the same - calculate offset and apply
            Vector2 oldAnchorMin = rectTransform.anchorMin;
            Vector2 oldAnchorMax = rectTransform.anchorMax;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            // Optionally preserve anchored position/size by adjusting anchoredPosition/sizeDelta:
            // (This can be more complex depending on layout; many times you can leave it.)
        }
        else
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        lastSafeArea = safeArea;
        lastOrientation = Screen.orientation;
    }
}