public interface IDamageable
{
    /// <summary>������� ���� (amount >= 0). isCrit � ������ ��� �������/������.</summary>
    void ApplyDamage(float amount, bool isCrit = false);
}
