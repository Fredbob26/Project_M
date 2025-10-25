using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int maxHP = 10;
    private int _hp;

    public System.Action OnDeath;

    private void OnEnable()
    {
        _hp = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        _hp -= dmg;
        if (_hp <= 0) Die();
    }

    void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
