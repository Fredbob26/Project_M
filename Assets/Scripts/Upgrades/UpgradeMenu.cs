using UnityEngine;
using System;
using System.Collections.Generic;

public class UpgradeMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;
    public bool IsOpen { get; private set; }

    [Header("Fixed Cards (size = 5)")]
    public UpgradeCardView[] cards = new UpgradeCardView[5];

    private PlayerStats _stats;
    private Action<UpgradeDefinition> _onPick;
    private int _currentPrice = 0;
    private Func<UpgradeDefinition, bool> _canBuy;

    public void Init(PlayerStats stats)
    {
        _stats = stats;
        if (panel)
        {
            panel.SetActive(true);
            Canvas.ForceUpdateCanvases();
            panel.SetActive(false);
        }
        IsOpen = false;
    }

    public void Show(List<UpgradeDefinition> upgrades, Func<UpgradeDefinition, bool> canBuy, Action<UpgradeDefinition> onPick, int price)
    {
        _onPick = onPick;
        _currentPrice = price;
        _canBuy = canBuy;

        if (panel) panel.SetActive(true);
        IsOpen = true;
        Time.timeScale = 0f;

        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            if (!card) continue;

            if (upgrades == null || i >= upgrades.Count || upgrades[i] == null)
            {
                card.gameObject.SetActive(false);
                continue;
            }

            var def = upgrades[i];
            int curLevel = Game.I.upgradeSystem.GetCurrentLevel(def);
            bool capped = Game.I.upgradeSystem.IsCapped(def);
            bool interactable = !capped && (_canBuy == null || _canBuy(def));

            card.gameObject.SetActive(true);
            card.Set(
                def: def,
                currentLevel: curLevel,
                priceValue: _currentPrice,
                interactable: interactable,
                isCapped: capped,
                onClick: () =>
                {
                    _onPick?.Invoke(def);
                    Close();
                });
        }
    }

    public void Close()
    {
        if (panel) panel.SetActive(false);
        IsOpen = false;
        Time.timeScale = 1f;
    }
}