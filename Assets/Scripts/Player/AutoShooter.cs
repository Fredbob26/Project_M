using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(Collider))]
public class AutoShooter : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Combat")]
    public float attackRange = 12f;

    private PlayerStats _stats;
    private float _timer;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (!Game.I || !Game.I.GameReady) return;
        if (_stats == null || projectilePrefab == null || firePoint == null) return;

        _timer += Time.deltaTime;

        // Интервал выстрела = 1 / AttackSpeed (с ограничением)
        float interval = Mathf.Max(0.05f, 1f / Mathf.Max(0.01f, _stats.AttackSpeed));

        if (_timer >= interval)
        {
            Enemy target = FindClosestEnemy();
            if (target != null)
            {
                _timer = 0f;
                Shoot(target.transform.position);
            }
        }
    }

    private void Shoot(Vector3 targetPos)
    {
        // Рассчитываем направление к цели
        Vector3 dir = (targetPos - firePoint.position).normalized;

        // Создаём пулю чуть впереди дула, чтобы не пересекалась с коллайдером игрока
        Vector3 spawnPos = firePoint.position + dir * 0.5f;
        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir, Vector3.up));

        // Инициализируем снаряд
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Init(targetPos, _stats.AttackDamage);
        }

        // Игнорируем столкновение между игроком и пулей
        Collider playerCol = GetComponent<Collider>();
        Collider projCol = go.GetComponent<Collider>();
        if (playerCol && projCol)
        {
            Physics.IgnoreCollision(playerCol, projCol, true);
            Debug.Log($"[AutoShooter] Ignoring collision between {playerCol.name} and {projCol.name}");
        }

        // Отладочный луч
        Debug.DrawRay(firePoint.position, dir * 2f, Color.red, 1f);
    }

    private Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float bestDistSqr = attackRange * attackRange;
        Vector3 playerPos = transform.position;

        foreach (Enemy e in enemies)
        {
            float dist = (e.transform.position - playerPos).sqrMagnitude;
            if (dist < bestDistSqr)
            {
                bestDistSqr = dist;
                closest = e;
            }
        }

        return closest;
    }
}