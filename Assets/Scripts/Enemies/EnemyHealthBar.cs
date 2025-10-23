using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Зелёная часть полоски (Image внутри Background).")]
    public Image fillImage;             // Обязательно перетащи Fill
    [Tooltip("Корневой объект бара (обычно Background). Если не указать — возьмёт этот объект.")]
    public GameObject barRoot;          // Можно оставить пустым

    [Header("Behaviour")]
    [Tooltip("Скорость плавного изменения заполнения.")]
    public float fillSmoothSpeed = 10f;
    [Tooltip("Через сколько секунд спрятать бар после восстановления полного ХП.")]
    public float hideDelay = 0.75f;
    [Tooltip("Делает бар лицом к камере.")]
    public bool billboardFaceCamera = true;
    [Tooltip("Поворачивать только по оси Y (для топ-дауна).")]
    public bool onlyYAxis = true;
    [Tooltip("Наклон полоски вверх относительно камеры (в градусах).")]
    [Range(-45f, 45f)]
    public float tiltAngle = 20f;

    private EnemyHealth enemyHealth;
    private Transform cam;
    private float displayedFill = 1f;
    private float lastNotFullTime = -999f; // последний момент, когда ХП было не полным

    void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        if (barRoot == null)
            barRoot = gameObject;
    }

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;

        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
            fillImage.fillAmount = 1f;
        }

        // Прячем бар при старте (появится при первом уроне)
        SetBarVisible(false, true);
    }

    void Update()
    {
        if (enemyHealth == null || fillImage == null) return;

        float targetFill = Mathf.Clamp01(
            (float)enemyHealth.CurrentHealth / Mathf.Max(1, enemyHealth.MaxHealth)
        );

        // Показываем бар, если враг не на фулл хп
        if (targetFill < 1f)
        {
            lastNotFullTime = Time.time;
            if (!barRoot.activeSelf)
                SetBarVisible(true);
        }
        else if (Time.time - lastNotFullTime > hideDelay && barRoot.activeSelf)
        {
            // Прячем, если фулл хп дольше задержки
            SetBarVisible(false);
        }

        // Плавно обновляем полоску
        displayedFill = Mathf.Lerp(displayedFill, targetFill, Time.deltaTime * fillSmoothSpeed);
        fillImage.fillAmount = displayedFill;
    }

    void LateUpdate()
    {
        if (!billboardFaceCamera || cam == null) return;

        if (onlyYAxis)
        {
            // Поворот только по горизонтали
            Vector3 dir = cam.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(-dir.normalized, Vector3.up);
        }
        else
        {
            // Полный билборд
            Vector3 toCam = (transform.position - cam.position).normalized;
            transform.rotation = Quaternion.LookRotation(toCam, Vector3.up);
        }

        // ✨ Добавляем наклон вверх
        transform.Rotate(Vector3.right, -tiltAngle);
    }

    private void SetBarVisible(bool visible, bool instant = false)
    {
        if (barRoot == null) return;

        barRoot.SetActive(visible);

        if (visible && !instant && fillImage != null)
            displayedFill = fillImage.fillAmount;
    }
}