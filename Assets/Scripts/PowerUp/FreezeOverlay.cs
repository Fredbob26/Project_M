using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FreezeOverlay : MonoBehaviour
{
    public float targetAlpha = 0.5f;
    public float fadeSpeed = 6f;

    private Image _img;
    private Color _c;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _c = _img.color;
        _img.raycastTarget = false;
        SetAlpha(0f);
    }

    private void Update()
    {
        bool active = Game.I && Game.I.buffs && Game.I.buffs.FreezeActive;
        float a = Mathf.MoveTowards(_img.color.a, active ? targetAlpha : 0f, fadeSpeed * Time.deltaTime);
        SetAlpha(a);
    }

    private void SetAlpha(float a)
    {
        _c = _img.color;
        _c.a = a;
        _img.color = _c;
    }
}

