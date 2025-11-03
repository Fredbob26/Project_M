using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(Collider))]
public class AutoShooter : MonoBehaviour
{
    [Header("Refs")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Combat")]
    public float attackRange = 12f;

    PlayerStats _stats;
    float _timer;

    void Awake() => _stats = GetComponent<PlayerStats>();

    void Update()
    {
        if (Game.I == null || !Game.I.GameReady) return;
        if (!_stats || !projectilePrefab || !firePoint) return;

        _timer += Time.deltaTime;

        float fireMul = Game.I.powerUps?.GetFireRateMul() ?? 1f;
        float interval = Mathf.Max(0.05f, 1f / Mathf.Max(0.01f, _stats.AttackSpeed * fireMul));
        if (_timer < interval) return;

        bool rapid = Game.I.powerUps && Game.I.powerUps.IsActive(PowerUpType.RapidFire);

        Enemy target = FindClosestEnemy();
        if (target != null)
        {
            _timer = 0f;
            ShootTowards(target.transform.position);
        }
        else if (rapid)
        {
            _timer = 0f;
            ShootTowards(firePoint.position + firePoint.forward * 10f);
        }
    }

    void ShootTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - firePoint.position).normalized;
        Vector3 spawnPos = firePoint.position + dir * 0.5f;

        var go = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir, Vector3.up));
        var proj = go.GetComponent<Projectile>();
        if (proj) proj.Init(targetPos, _stats.AttackDamage);

        var playerCol = GetComponent<Collider>();
        var projCol = go.GetComponent<Collider>();
        if (playerCol && projCol) Physics.IgnoreCollision(playerCol, projCol, true);

        if (Game.I.powerUps && Game.I.powerUps.IsActive(PowerUpType.RapidFire))
        {
            float spread = Game.I.config.rapidSpreadDeg;
            FireExtra(dir, spawnPos, spread);
            FireExtra(dir, spawnPos, -spread);
        }
    }

    void FireExtra(Vector3 dir, Vector3 spawnPos, float spreadDeg)
    {
        Quaternion q = Quaternion.AngleAxis(spreadDeg, Vector3.up);
        Vector3 d = (q * dir).normalized;
        var go = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(d, Vector3.up));
        var proj = go.GetComponent<Projectile>();
        if (proj) proj.Init(spawnPos + d * 10f, _stats.AttackDamage);
    }

    Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy best = null;
        float bestDistSqr = attackRange * attackRange;
        Vector3 p = transform.position;

        foreach (var e in enemies)
        {
            Vector3 from = p + Vector3.up;
            Vector3 to = e.transform.position + Vector3.up;
            float d2 = (e.transform.position - p).sqrMagnitude;

            if (d2 < bestDistSqr && HasLOS(from, to))
            {
                best = e;
                bestDistSqr = d2;
            }
        }
        return best;
    }

    bool HasLOS(Vector3 from, Vector3 to)
    {
        int mask = Game.I.config.losBlockMask;
        return !Physics.Linecast(from, to, mask, QueryTriggerInteraction.Ignore);
    }
}
