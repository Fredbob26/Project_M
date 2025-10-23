using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Button pickButton;

    private Action onPick;

    public void Setup(UpgradeData data, Action onPick)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        this.onPick = onPick;

        if (icon) icon.sprite = data.icon;
        if (title) title.text = data.upgradeName;
        if (desc) desc.text = data.description;

        if (pickButton)
        {
            pickButton.onClick.RemoveAllListeners();
            pickButton.onClick.AddListener(() => this.onPick?.Invoke());
        }
    }
}
