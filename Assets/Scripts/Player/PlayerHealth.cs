using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Life Settings")]
    public float maxLifeTime = 60f;     // максимальное время "жизни"
    public float currentLifeTime;       // текущее время
    public float damageOnHit = 5f;      // сколько времени отнимает враг

    private bool isDead = false;

    void Start()
    {
        currentLifeTime = maxLifeTime;
    }

    void Update()
    {
        if (isDead) return;

        currentLifeTime -= Time.deltaTime;

        if (currentLifeTime <= 0f)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(damageOnHit);
        }
    }

    public void TakeDamage(float amount)
    {
        currentLifeTime -= amount;

        if (currentLifeTime <= 0f && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player is dead!");
        // TODO: тут позже вызовем GameManager для рестарта игры
    }
}