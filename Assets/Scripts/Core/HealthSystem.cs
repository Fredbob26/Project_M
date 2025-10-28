using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    public int Max => maxHealth;
    public int Current => currentHealth;

    public event Action<int, int, bool> OnDamaged; // (current, max, isCrit)
    public event Action OnDied;
    public event Action<int, int> OnInitialized;

    private bool _initialized;

    public void Init(int max)
    {
        maxHealth = Mathf.Max(1, max);
        currentHealth = maxHealth;
        _initialized = true;
        OnInitialized?.Invoke(currentHealth, maxHealth);
    }

    public void SetMax(int max, bool refill = true)
    {
        maxHealth = Mathf.Max(1, max);
        if (refill) currentHealth = maxHealth;
        OnInitialized?.Invoke(currentHealth, maxHealth);
    }

    public void Apply(int delta, bool isCrit = false)
    {
        if (!_initialized) Init(maxHealth);
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth + delta, 0, maxHealth);
        OnDamaged?.Invoke(currentHealth, maxHealth, isCrit);

        if (currentHealth <= 0)
            OnDied?.Invoke();
    }
}