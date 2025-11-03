using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] public int maxHP = 10;
    [SerializeField] private int _hp;

    public int Current => _hp;
    public int Max => maxHP;

    public event Action<int, int, bool> OnDamaged;
    public event Action OnDeath;

    private bool _dead;

    void OnEnable()
    {
        _dead = false;
        _hp = Mathf.Max(1, maxHP);
    }

    public void Initialize(int max)
    {
        maxHP = Mathf.Max(1, max);
        _dead = false;
        _hp = maxHP;
        OnDamaged?.Invoke(_hp, maxHP, false);
    }

    public void ApplyDamage(float amount, bool isCrit = false)
    {
        if (_dead) return;
        int dmg = Mathf.RoundToInt(Mathf.Max(0f, amount));
        if (dmg <= 0) return;

        _hp = Mathf.Clamp(_hp - dmg, 0, maxHP);
        OnDamaged?.Invoke(_hp, maxHP, isCrit);

        if (_hp <= 0 && !_dead)
        {
            _dead = true;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        if (_dead) return;
        if (amount <= 0) return;

        _hp = Mathf.Clamp(_hp + amount, 0, maxHP);
        OnDamaged?.Invoke(_hp, maxHP, false);
    }
}
