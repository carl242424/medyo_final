using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds a full-screen dark overlay so the scene appears dim. Works even if 2D Lighting isn't active.
/// Attach to any GameObject in the scene (e.g. an empty "DarkOverlay" under LIGHTS).
/// </summary>
public class DarkOverlay : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("How dark the overlay is. 0.5 = half dark, 0.7 = very dark.")]
    public float darkness = 0.65f;

    [Tooltip("Color tint of the overlay (default is black). Slight blue = cooler night.")]
    public Color overlayColor = new Color(0.02f, 0.03f, 0.08f, 1f);

    private static GameObject _overlayRoot;

    private void Start()
    {
        if (_overlayRoot != null)
            return;

        var canvasGo = new GameObject("DarkOverlayCanvas");
        _overlayRoot = canvasGo;

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        var imageGo = new GameObject("OverlayImage");
        imageGo.transform.SetParent(canvasGo.transform, false);

        var rect = imageGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = imageGo.AddComponent<Image>();
        image.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, darkness);
        image.raycastTarget = false;
    }

    private void OnDestroy()
    {
        if (_overlayRoot != null)
        {
            Destroy(_overlayRoot);
            _overlayRoot = null;
        }
    }
}
