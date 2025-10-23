using System;
using UnityEngine;

public class UpgradeMenuUI : MonoBehaviour
{
    [Header("Slots (plug your UI here)")]
    public UpgradeSlotUI slot1;
    public UpgradeSlotUI slot2;
    public UpgradeSlotUI slot3;

    private Action<UpgradeData> onPick;
    private Action onClose;

    public void Show(UpgradeData[] options, Action<UpgradeData> onPick, Action onClose)
    {
        gameObject.SetActive(true);
        this.onPick = onPick;
        this.onClose = onClose;

        // гарантируем 3 слота (дублируем при необходимости)
        var a = options.Length > 0 ? options[0] : null;
        var b = options.Length > 1 ? options[1] : a;
        var c = options.Length > 2 ? options[2] : b;

        slot1.Setup(a, () => Choose(a));
        slot2.Setup(b, () => Choose(b));
        slot3.Setup(c, () => Choose(c));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        onPick = null;
        onClose = null;
    }

    private void Choose(UpgradeData data)
    {
        onPick?.Invoke(data);
    }

    //  нопка "«акрыть без выбора" Ч опционально
    public void CloseWithoutPick()
    {
        onClose?.Invoke();
    }
}
