using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private StaticConfig _cfg;

    public float MoveSpeed { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackDamage { get; private set; }
    public float CritChance { get; private set; }
    public float RicochetChance { get; private set; }

    public void ResetFromConfig(StaticConfig cfg)
    {
        _cfg = cfg;
        MoveSpeed = cfg.baseMoveSpeed;
        AttackSpeed = cfg.baseAttackSpeed;
        AttackDamage = cfg.baseDamage;
        CritChance = cfg.baseCritChance;
        RicochetChance = cfg.baseRicochetChance;
    }

    public void AddMoveSpeed(float amount) => MoveSpeed += amount;
    public void AddAttackSpeed(float amount) => AttackSpeed += amount;
    public void AddDamage(float amount) => AttackDamage += amount;
    public void AddCritChance(float percent) => CritChance = Mathf.Clamp01(CritChance + percent);
    public void AddRicochetChance(float percent) => RicochetChance = Mathf.Clamp01(RicochetChance + percent);

    public void ApplyUpgrade(UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeType.MoveSpeed: AddMoveSpeed(value); break;
            case UpgradeType.AttackSpeed: AddAttackSpeed(value); break;
            case UpgradeType.AttackDamage: AddDamage(value); break;
            case UpgradeType.CritChance: AddCritChance(value); break;
            case UpgradeType.RicochetChance: AddRicochetChance(value); break;
            default:
                Debug.LogWarning($"PlayerStats: неизвестный тип апгрейда {type}");
                break;
        }
    }
}
