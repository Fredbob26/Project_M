public interface IDamageable
{
    /// <summary>Нанести урон (amount >= 0). isCrit — только для визуала/логики.</summary>
    void ApplyDamage(float amount, bool isCrit = false);
}
