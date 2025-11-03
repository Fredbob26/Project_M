using UnityEngine;

public class Logger : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.EssencePicked += OnEssencePicked;
        GameEvents.EnemyKilled += OnEnemyKilled;
        GameEvents.PowerUpActivated += OnPowerUpActivated;
        GameEvents.PlayerHitContact += OnPlayerHitContact;
        GameEvents.UpgradePurchased += OnUpgradePurchased;
    }

    private void OnDisable()
    {
        GameEvents.EssencePicked -= OnEssencePicked;
        GameEvents.EnemyKilled -= OnEnemyKilled;
        GameEvents.PowerUpActivated -= OnPowerUpActivated;
        GameEvents.PlayerHitContact -= OnPlayerHitContact;
        GameEvents.UpgradePurchased -= OnUpgradePurchased;
    }

    void OnEssencePicked(int delta, int balance) =>
        Debug.Log($"[EVT] Essence +{delta} → balance={balance}");

    void OnEnemyKilled() =>
        Debug.Log("[EVT] Enemy killed");

    void OnPowerUpActivated(PowerUpType t, float dur) =>
        Debug.Log($"[EVT] PowerUp {t} for {dur:0.##}s");

    void OnPlayerHitContact(float sec) =>
        Debug.Log($"[EVT] Contact hit: -{sec:0.##}s");

    void OnUpgradePurchased(UpgradeType t, int lvl, int price) =>
        Debug.Log($"[EVT] Upgrade {t} -> level {lvl} (price {price})");
}
