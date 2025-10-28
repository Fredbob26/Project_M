using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeCardView : MonoBehaviour
{
    [Header("Refs")]
    public Image icon;
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text level;
    public TMP_Text price;
    public Button selectButton;

    private void Reset()
    {
        icon = transform.Find("Icon")?.GetComponent<Image>();
        title = transform.Find("Title")?.GetComponent<TMP_Text>();
        desc = transform.Find("Desc")?.GetComponent<TMP_Text>();
        level = transform.Find("Level")?.GetComponent<TMP_Text>();
        price = transform.Find("Price")?.GetComponent<TMP_Text>();
        selectButton = transform.Find("SelectButton")?.GetComponent<Button>();
    }

    public void Set(
        UpgradeDefinition def,
        int currentLevel,
        int priceValue,
        bool interactable,
        bool isCapped,
        Action onClick)
    {
        if (icon) icon.sprite = def.icon;
        if (title) title.text = def.upgradeName;
        if (desc) desc.text = def.description;
        if (level) level.text = currentLevel > 0 ? ToRoman(currentLevel) : "–";

        if (price)
        {
            if (isCapped) price.text = "MAX";
            else if (!interactable) price.text = $"{priceValue} (no money)";
            else price.text = $"{priceValue} Essence";
        }

        if (selectButton)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.interactable = interactable && !isCapped;
            if (selectButton.interactable && onClick != null)
                selectButton.onClick.AddListener(() => onClick());
        }
    }

    private string ToRoman(int n)
    {
        if (n <= 0) return "–";
        var map = new (int, string)[] {
            (1000,"M"),(900,"CM"),(500,"D"),(400,"CD"),
            (100,"C"),(90,"XC"),(50,"L"),(40,"XL"),
            (10,"X"),(9,"IX"),(5,"V"),(4,"IV"),(1,"I")
        };
        var s = "";
        foreach (var (v, sym) in map)
            while (n >= v) { s += sym; n -= v; }
        return s;
    }
}