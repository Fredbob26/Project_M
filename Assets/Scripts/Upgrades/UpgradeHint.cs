// Assets/Scripts/Upgrades/UpgradeHint.cs
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHint : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Объект хинта (текст/иконка). Если не задан, используется этот же GameObject.")]
    [SerializeField] private GameObject hintRoot;

    [Tooltip("Скорость мигания при доступном апгрейде.")]
    [SerializeField] private float blinkSpeed = 4f;

    private UpgradeFlow _flow;
    private UpgradeMenu _menu;
    private CanvasGroup _cg;

    private void Awake()
    {
        // Если не назначили — считаем корнем сам объект с этим скриптом
        if (hintRoot == null)
            hintRoot = gameObject;

        // Хинт ДОЛЖЕН быть активен в сцене,
        // видимость мы регулируем только через CanvasGroup.alpha
        if (!hintRoot.activeSelf)
            hintRoot.SetActive(true);

        _cg = hintRoot.GetComponent<CanvasGroup>();
        if (_cg == null)
            _cg = hintRoot.AddComponent<CanvasGroup>();

        // По умолчанию — полностью прозрачный (невидимый)
        _cg.alpha = 0f;

        _flow = FindObjectOfType<UpgradeFlow>(true);
        _menu = FindObjectOfType<UpgradeMenu>(true);
    }

    private void Update()
    {
        if (hintRoot == null || _cg == null)
            return;

        // Игры ещё нет / не готова — хинт невидим
        if (Game.I == null || !Game.I.GameReady)
        {
            SetAlpha(0f);
            return;
        }

        // Перестраховка: вдруг Flow/Menu пересоздали
        if (_flow == null) _flow = FindObjectOfType<UpgradeFlow>(true);
        if (_menu == null) _menu = FindObjectOfType<UpgradeMenu>(true);

        if (_flow == null || _menu == null)
        {
            SetAlpha(0f);
            return;
        }

        // Если меню уже открыто — хинт скрыт
        if (_menu.IsOpen)
        {
            SetAlpha(0f);
            return;
        }

        // Проверяем, можно ли открыть меню и есть ли реальные офферы
        List<UpgradeDefinition> offers;
        bool canOpen = _flow.CanOpenNow(out offers) && offers != null && offers.Count > 0;

        if (!canOpen)
        {
            // Недоступно — хинт просто прозрачный
            SetAlpha(0f);
        }
        else
        {
            // Доступно — мигание по unscaledTime (работает даже если Time.timeScale=0 при паузе)
            float a = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * blinkSpeed);
            SetAlpha(a);
        }
    }

    private void SetAlpha(float a)
    {
        _cg.alpha = Mathf.Clamp01(a);
    }
}
