using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;  // перетащи сюда Image "Fill" (или найдём сами)

    [Header("Billboard")]
    [Range(0f, 45f)] public float tiltAngle = 10f;
    public float lerpSpeed = 12f;

    private Camera _cam;
    private float _currentFill = 1f;
    private float _targetFill = 1f;

    private void Awake()
    {
        _cam = Camera.main;

        if (fillImage == null)
        {
            var t = transform.Find("Fill");
            if (t) fillImage = t.GetComponent<Image>();
            if (fillImage == null) fillImage = GetComponentInChildren<Image>(true);
        }

        if (fillImage) fillImage.fillAmount = 1f;
        // Важно: ничего не включаем/выключаем и НЕ меняем позицию.
        // Позицию задаёшь трансформом в префабе.
    }

    /// <summary>Показать бар при первом уроне.</summary>
    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    /// <summary>Обновить целевое значение заливки.</summary>
    public void SetTarget(int current, int max)
    {
        _targetFill = Mathf.Clamp01(max <= 0 ? 1f : (float)current / max);
    }

    private void LateUpdate()
    {
        if (_cam == null) _cam = Camera.main;

        // Только разворот к камере + лёгкий наклон. Позицию НЕ трогаем.
        if (_cam)
        {
            var face = Quaternion.LookRotation(_cam.transform.forward, Vector3.up);
            transform.rotation = face * Quaternion.Euler(tiltAngle, 0f, 0f);
        }

        // Плавная интерполяция заливки
        _currentFill = Mathf.MoveTowards(_currentFill, _targetFill, lerpSpeed * Time.deltaTime);
        if (fillImage) fillImage.fillAmount = _currentFill;
    }
}