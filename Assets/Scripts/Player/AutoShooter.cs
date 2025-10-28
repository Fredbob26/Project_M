using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(Collider))]
public class AutoShooter : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Combat")]
    public float attackRange = 12f;     // радиус автонаведения в обычном режиме
    public float spawnOffset = 0.5f;    // вынос пули вперёд от firePoint, чтобы не цеплять игрока

    private PlayerStats _stats;
    private Collider _playerCol;
    private float _timer;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _playerCol = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!Game.I || !Game.I.GameReady) return;
        if (_stats == null || projectilePrefab == null || firePoint == null) return;

        _timer += Time.deltaTime;

        // RapidFire: скорость = текущая AttackSpeed * множитель баффа
        float rateMul = (Game.I.buffs ? Game.I.buffs.RapidFireRateMul : 1f);
        float interval = Mathf.Max(0.05f, 1f / Mathf.Max(0.01f, _stats.AttackSpeed * rateMul));

        if (_timer < interval) return;
        _timer = 0f;

        bool rapid = Game.I.buffs && Game.I.buffs.RapidFireActive;

        if (rapid)
        {
            // Если есть цель — стреляем тройным веером в её сторону
            Enemy target = FindClosestEnemy();
            Vector3 baseDir;

            if (target != null)
            {
                baseDir = (target.transform.position - firePoint.position);
                baseDir.y = 0f;
                if (baseDir.sqrMagnitude < 0.0001f) baseDir = transform.forward;
                else baseDir.Normalize();
            }
            else
            {
                // Цели нет — стреляем туда, куда смотрит игрок
                baseDir = transform.forward;
            }

            float spread = Game.I.config.rapidFireSpreadDeg;
            ShootDir(baseDir);
            ShootDir(Quaternion.AngleAxis(+spread, Vector3.up) * baseDir);
            ShootDir(Quaternion.AngleAxis(-spread, Vector3.up) * baseDir);
        }
        else
        {
            // Обычный режим: автонаведение на ближайшего врага в радиусе (одиночный выстрел)
            Enemy target = FindClosestEnemy();
            if (target != null)
                ShootPos(target.transform.position);
        }
    }

    private void ShootPos(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - firePoint.position).normalized;
        Vector3 spawnPos = firePoint.position + dir * spawnOffset;

        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir, Vector3.up));
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(spawnPos + dir, _stats.AttackDamage); // цель как точка на линии полёта

        IgnorePlayerCollision(go);
        Debug.DrawRay(firePoint.position, dir * 2f, Color.red, 0.5f);
    }

    private void ShootDir(Vector3 dir)
    {
        dir = (dir.sqrMagnitude > 0.0001f) ? dir.normalized : transform.forward;
        Vector3 spawnPos = firePoint.position + dir * spawnOffset;

        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir, Vector3.up));
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(spawnPos + dir, _stats.AttackDamage);

        IgnorePlayerCollision(go);
        Debug.DrawRay(firePoint.position, dir * 2f, Color.yellow, 0.5f);
    }

    private void IgnorePlayerCollision(GameObject projectile)
    {
        if (_playerCol == null) _playerCol = GetComponent<Collider>();
        var projCol = projectile.GetComponent<Collider>();
        if (_playerCol && projCol)
            Physics.IgnoreCollision(_playerCol, projCol, true);
    }

    private Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float bestDistSqr = attackRange * attackRange;
        Vector3 playerPos = transform.position;

        foreach (Enemy e in enemies)
        {
            if (!e) continue;
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
