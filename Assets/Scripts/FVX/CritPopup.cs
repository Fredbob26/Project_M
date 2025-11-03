using UnityEngine;
using TMPro;

public class CritPopup : MonoBehaviour
{
    [Header("Timing")]
    public float lifetime = 0.9f;

    [Header("Motion")]
    public float riseSpeed = 1.2f;
    public float horizontalJitter = 0.2f;

    [Header("Scale")]
    public Vector2 startScale = new(1.00f, 1.00f);
    public Vector2 endScale = new(0.80f, 0.80f);

    private TMP_Text _tmp;
    private float _t;
    private Color _baseColor;
    private Vector3 _drift;

    public void SetText(string s)
    {
        if (!_tmp) _tmp = GetComponentInChildren<TMP_Text>(true);
        if (_tmp) _tmp.text = s;
    }

    void Awake()
    {
        _tmp = GetComponentInChildren<TMP_Text>(true);
        if (_tmp) _baseColor = _tmp.color;

        // лёгкий случайный дрейф
        _drift = new Vector3(
            Random.Range(-horizontalJitter, horizontalJitter),
            0f,
            Random.Range(-horizontalJitter, horizontalJitter)
        );

        transform.localScale = new Vector3(startScale.x, startScale.y, 1f);

        // Жёсткая страховка
        Destroy(gameObject, lifetime + 0.25f);
    }

    void Update()
    {
        _t += Time.unscaledDeltaTime;
        float k = Mathf.Clamp01(_t / lifetime);

        transform.position += Vector3.up * riseSpeed * Time.unscaledDeltaTime + _drift * Time.unscaledDeltaTime;

        if (Camera.main)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        float s = Mathf.Lerp(startScale.x, endScale.x, k);
        transform.localScale = new Vector3(s, s, 1f);

        if (_tmp)
        {
            var c = _tmp.color;
            c.a = Mathf.Lerp(_baseColor.a, 0f, k);
            _tmp.color = c;
        }

        if (_t >= lifetime) Destroy(gameObject);
    }
}
