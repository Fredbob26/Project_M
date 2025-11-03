using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour
{
    [Serializable]
    public class CardRefs
    {
        public GameObject root;
        public TMP_Text title;
        public TMP_Text desc;
        public Image icon;
        public TMP_Text level;
        public TMP_Text price;
        public Button selectButton;
    }

    [Header("Refs")]
    public GameObject panel;
    public CardRefs[] cards;

    private PlayerStats _stats;
    private Action<UpgradeDefinition> _onPick;
    private int _price = 0;

    public bool IsOpen => panel && panel.activeSelf;

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

    public void Show(List<UpgradeDefinition> upgrades, Func<UpgradeDefinition, bool> canAfford, Action<UpgradeDefinition> onPick, int price)
    {
        if (upgrades == null || upgrades.Count == 0 || cards == null || cards.Length == 0) return;

        _onPick = onPick;
        _price = price;

        Time.timeScale = 0f;
        Game.I?.powerUps?.SetPaused(true);

        if (panel) panel.SetActive(true);

        for (int i = 0; i < cards.Length; i++)
        {
            bool has = i < upgrades.Count;
            var card = cards[i];
            if (!card.root) continue;

            card.root.SetActive(has);
            if (!has) continue;

            var def = upgrades[i];

            if (card.title) card.title.text = def.upgradeName;
            if (card.desc) card.desc.text = def.description;
            if (card.icon) card.icon.sprite = def.icon;

            int curLvl = Game.I.upgradeSystem.GetCurrentLevel(def);
            if (card.level) card.level.text = curLvl > 0 ? ToRoman(curLvl) : "–";
            if (card.price) card.price.text = $"{_price} Essence";

            bool afford = canAfford?.Invoke(def) ?? true;

            if (card.selectButton)
            {
                card.selectButton.interactable = afford;
                card.selectButton.onClick.RemoveAllListeners();
                card.selectButton.onClick.AddListener(() =>
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
        Game.I?.powerUps?.SetPaused(false);
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
