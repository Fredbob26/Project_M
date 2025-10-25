using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;
    public Transform container;
    public GameObject upgradeCardPrefab;

    private PlayerStats _stats;
    private Action<UpgradeDefinition> _onPick;
    private int _currentPrice = 0;

    public void Init(PlayerStats stats)
    {
        _stats = stats;
        if (panel)
        {
            panel.SetActive(true);
            Canvas.ForceUpdateCanvases();
            panel.SetActive(false);
        }
    }

    public void Show(List<UpgradeDefinition> upgrades, Action<UpgradeDefinition> onPick, int price)
    {
        _onPick = onPick;
        _currentPrice = price;

        Time.timeScale = 0f;
        if (panel) panel.SetActive(true);

        foreach (Transform child in container) Destroy(child.gameObject);

        foreach (var def in upgrades)
        {
            var card = Instantiate(upgradeCardPrefab, container);

            var title = card.transform.Find("Title")?.GetComponent<TMP_Text>();
            var desc = card.transform.Find("Desc")?.GetComponent<TMP_Text>();
            var icon = card.transform.Find("Icon")?.GetComponent<Image>();
            var level = card.transform.Find("Level")?.GetComponent<TMP_Text>();
            var priceText = card.transform.Find("Price")?.GetComponent<TMP_Text>();
            var btn = card.transform.Find("SelectButton")?.GetComponent<Button>();

            if (title) title.text = def.upgradeName;
            if (desc) desc.text = def.description;
            if (icon) icon.sprite = def.icon;

            int cur = Game.I.upgradeSystem.GetCurrentLevel(def);
            if (level) level.text = cur > 0 ? ToRoman(cur) : "–";
            if (priceText) priceText.text = $"{_currentPrice} Essence";

            if (btn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    _onPick?.Invoke(def);
                    Close();
                });
            }
        }
    }

    public void Close()
    {
        if (panel) panel.SetActive(false);
        Time.timeScale = 1f;
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