using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FreezeOverlay : MonoBehaviour
{
    [SerializeField] private Image overlay;
    [SerializeField] private Color color = new Color(0f, 0.55f, 1f, 0.35f);
    [SerializeField] private bool fade = true;
    [SerializeField] private float fadeTime = 0.15f;

    Coroutine _co;

    void Awake()
    {
        if (!overlay) overlay = GetComponentInChildren<Image>(true);
        if (overlay)
        {
            overlay.enabled = false;
            overlay.raycastTarget = false;
        }
    }

    public void Show(float duration)
    {
        if (!overlay) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(Run(duration));
    }

    public void Hide()
    {
        if (!overlay) return;
        if (_co != null) StopCoroutine(_co);
        overlay.enabled = false;
        _co = null;
    }

    IEnumerator Run(float duration)
    {
        overlay.color = new Color(color.r, color.g, color.b, 0f);
        overlay.enabled = true;

        if (fade && fadeTime > 0f)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / fadeTime) * color.a;
                overlay.color = new Color(color.r, color.g, color.b, a);
                yield return null;
            }
        }
        else overlay.color = color;

        float alive = 0f;
        while (alive < duration)
        {
            alive += Time.deltaTime;
            yield return null;
        }

        if (fade && fadeTime > 0f)
        {
            float t = 0f; float startA = overlay.color.a;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(startA, 0f, t / fadeTime);
                overlay.color = new Color(color.r, color.g, color.b, a);
                yield return null;
            }
        }

        overlay.enabled = false;
        _co = null;
    }
}
