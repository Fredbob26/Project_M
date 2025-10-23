using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float moveSpeed = 5f;
    public float attackSpeed = 1f;               // выстрелов в секунду
    public int attackDamage = 1;
    public float pickupRadius = 2.5f;
    public float timerBonusPerEssence = 3f;
    public int maxHealth = 1;

    public event Action OnStatsChanged;

    public void ApplyUpgrade(UpgradeData data)
    {
        switch (data.type)
        {
            case UpgradeType.AttackDamage:
                attackDamage += Mathf.RoundToInt(data.value);
                break;
            case UpgradeType.AttackSpeed:
                attackSpeed *= (1f + data.value); // 0.15 = +15%
                break;
            case UpgradeType.MoveSpeed:
                moveSpeed *= (1f + data.value);
                break;
            case UpgradeType.PickupRadius:
                pickupRadius += data.value;
                break;
            case UpgradeType.TimerBonus:
                timerBonusPerEssence += data.value;
                break;
            case UpgradeType.MaxHealth:
                maxHealth += Mathf.RoundToInt(data.value);
                break;
        }

        OnStatsChanged?.Invoke();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            OnStatsChanged?.Invoke();
    }
}
