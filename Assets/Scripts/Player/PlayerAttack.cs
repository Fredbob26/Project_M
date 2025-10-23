using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public PlayerStats stats;
    public GameObject projectilePrefab;

    [Header("Targeting")]
    public float targetRange = 25f;
    public LayerMask enemyMask;

    private float cooldownTimer = 0f;
    private float currentCooldown = 1f;

    private bool initialized = false; // защита от двойного вызова

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        if (stats != null)
            stats.OnStatsChanged += OnStatsChanged;

        InitializeCooldown();
    }

    private void OnDisable()
    {
        if (stats != null)
            stats.OnStatsChanged -= OnStatsChanged;
    }

    private void Update()
    {
        if (!initialized || stats == null || projectilePrefab == null) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return;

        Transform target = FindClosestEnemy();
        if (target == null) return;

        Fire(target);
        cooldownTimer = currentCooldown;
    }

    private void InitializeCooldown()
    {
        RecalculateCooldown();
        initialized = true;
    }

    private void OnStatsChanged()
    {
        // Если игрок в игре и система уже инициализирована — пересчитать кулдаун
        if (initialized)
            RecalculateCooldown();
    }

    private void RecalculateCooldown()
    {
        float shotsPerSecond = Mathf.Max(0.1f, stats.attackSpeed);
        currentCooldown = 1f / shotsPerSecond;
    }

    private void Fire(Transform target)
    {
        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = projObj.GetComponent<Projectile>();

        if (proj != null)
            proj.Init(target, stats.attackDamage);
    }

    private Transform FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, targetRange, enemyMask);
        float best = float.MaxValue;
        Transform closest = null;

        foreach (var hit in hits)
        {
            float dist = (hit.transform.position - transform.position).sqrMagnitude;
            if (dist < best)
            {
                best = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }
}