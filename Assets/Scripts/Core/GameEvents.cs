using System;

public static class GameEvents
{
    public static Action<int, int> EssencePicked;         // (delta, newBalance)
    public static Action EnemyKilled;
    public static Action<PowerUpType, float> PowerUpActivated; // (type, durationSeconds)
    public static Action<float> PlayerHitContact;         // (secondsLost)
    public static Action<UpgradeType, int, int> UpgradePurchased; // (type, newLevel, price)
}

