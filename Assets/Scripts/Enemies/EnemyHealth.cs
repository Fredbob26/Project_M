using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    public int MaxHealth => maxHealth;          // публичный доступ к максимуму
    public int CurrentHealth { get; private set; } // публичный геттер для текущего HP

    [Header("Drops")]
    [SerializeField] private GameObject essencePrefab;

    public Action onDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (essencePrefab != null)
        {
            Instantiate(essencePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        onDeath?.Invoke();
        Destroy(gameObject);
    }
}