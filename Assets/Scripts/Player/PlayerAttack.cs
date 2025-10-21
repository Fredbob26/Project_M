using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackCooldown = 1f;
    public float projectileSpeed = 10f;
    public float attackRange = 10f;

    private float attackTimer = 0f;

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            GameObject target = FindClosestEnemy();
            if (target != null)
            {
                Shoot(target);
                attackTimer = attackCooldown;
            }
        }
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= attackRange)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    private void Shoot(GameObject target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = direction * projectileSpeed;
        else
            DebugManager.LogError("Projectile prefab missing Rigidbody!");
    }
}