using UnityEngine;

public class LevelTitleFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rect;
    public float fadeDuration = 1.5f;
    public float moveDistance = 80f;

    Vector2 startPos;

    void Start()
    {
        startPos = rect.anchoredPosition;
        rect.anchoredPosition = startPos + Vector2.up * moveDistance;
        StartCoroutine(Animate());
    }

    System.Collections.IEnumerator Animate()
    {
        float t = 0;
        canvasGroup.alpha = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;

            canvasGroup.alpha = p;
            rect.anchoredPosition =
                Vector2.Lerp(startPos + Vector2.up * moveDistance, startPos, p);

            yield return null;
        }
    }
}