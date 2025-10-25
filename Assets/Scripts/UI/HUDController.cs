using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("UI Refs")]
    public TMP_Text essenceText;  // "Essence: N"
    public Image timeBarFill;     // заполняемость полосы времени

    private int _essence;
    private LifeTimer _timer;

    public int CurrentEssence => _essence;

    public void Init(LifeTimer timer)
    {
        _timer = timer;
        _essence = 0;
        UpdateEssence(0);
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
}