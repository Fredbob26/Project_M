using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_Text essenceText;
    public Image timeBarFill;

    [Header("Hit Flash")]
    public Image hitFlashImage;

    [Header("Essence Pulse")]
    public float essencePulseScale = 1.25f;
    public float essencePulseSpeed = 6f;

    private int _essence;
    private LifeTimer _timer;
    private Coroutine _flashCR;
    private Coroutine _pulseCR;
    private Vector3 _essenceBaseScale;

    public int CurrentEssence => _essence;

    public void Init(LifeTimer timer)
    {
        _timer = timer;
        _essence = 0;
        UpdateEssence(0);

        if (hitFlashImage)
        {
            var c = hitFlashImage.color; c.a = 0f;
            hitFlashImage.color = c;
            hitFlashImage.raycastTarget = false;
            hitFlashImage.gameObject.SetActive(true);
        }

        if (essenceText)
            _essenceBaseScale = essenceText.rectTransform.localScale;
    }

    public void UpdateEssence(int delta)
    {
        _essence += delta;
        if (_essence < 0) _essence = 0;
        if (essenceText) essenceText.text = $"Essence: {_essence}";
    }

    private void Update()
    {
        if (_timer != null && timeBarFill != null)
            timeBarFill.fillAmount = _timer.Normalized;
    }

    public void FlashHit(float duration, Color color)
    {
        if (!hitFlashImage) return;
        if (_flashCR != null) StopCoroutine(_flashCR);
        _flashCR = StartCoroutine(FlashCR(duration, color));
    }

    private IEnumerator FlashCR(float duration, Color color)
    {
        hitFlashImage.color = color;
        float t = 0f;
        Color start = color;
        Color end = color; end.a = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // флэш можно оставить unscaled Ч он визуально отработает сразу
            float k = Mathf.Clamp01(t / duration);
            hitFlashImage.color = Color.Lerp(start, end, k);
            yield return null;
        }
        hitFlashImage.color = end;
        _flashCR = null;
    }

    // === ѕульс ESSENCE (теперь по deltaTime, чтобы пауза работала) ===
    public void PulseEssenceStart(float duration)
    {
        if (!essenceText) return;
        if (_pulseCR != null) StopCoroutine(_pulseCR);
        _pulseCR = StartCoroutine(PulseCR(duration));
    }

    public void PulseEssenceStop()
    {
        if (!essenceText) return;
        if (_pulseCR != null) StopCoroutine(_pulseCR);
        essenceText.rectTransform.localScale = _essenceBaseScale;
        _pulseCR = null;
    }

    private IEnumerator PulseCR(float duration)
    {
        float t = 0f;
        var rt = essenceText.rectTransform;

        while (t < duration)
        {
            t += Time.deltaTime; // <-- важна€ правка (пауза меню останавливает пульс)
            float s = 1f + (essencePulseScale - 1f) * (0.5f + 0.5f * Mathf.Sin(Time.time * essencePulseSpeed));
            rt.localScale = _essenceBaseScale * s;
            yield return null;
        }

        rt.localScale = _essenceBaseScale;
        _pulseCR = null;
    }
}
