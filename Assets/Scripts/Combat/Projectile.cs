using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 20f;
    public float lifetime = 5f;

    [Header("Crit & FX")]
    public GameObject critEffectPrefab;
    public GameObject critTextPrefab;
    public float critTextYOffset = 2f;

    [Header("Ricochet")]
    public float ricochetSearchRadius = 8f;
    public int maxRicochets = 3;

    private Rigidbody _rb;
    private Vector3 _direction = Vector3.zero;
    private float _damage;
    private int _ricochetCount;
    private bool _inited;
    private Collider _arenaCollider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        // находим границы арены по тегу LevelBounds
        var arenaObj = GameObject.FindGameObjectWithTag("LevelBounds");
        if (arenaObj)
            _arenaCollider = arenaObj.GetComponent<Collider>();

        Debug.Log($"[Projectile] Awake -> Rigidbody ready on {name}");
    }

    public void Init(Vector3 targetPos, float damage)
    {
        _damage = damage;
        _direction = (targetPos - transform.position).normalized;
        _inited = true;
        _ricochetCount = 0;
        Destroy(gameObject, lifetime);

        Debug.Log($"[Projectile] Init -> target:{targetPos} damage:{damage} dir:{_direction}");
    }

    private void FixedUpdate()
    {
        if (!_inited) return;

        _rb.velocity = _direction * speed;

        // Проверяем вылет за границы арены
        if (_arenaCollider != null && !_arenaCollider.bounds.Contains(transform.position))
        {
            Debug.Log("[Projectile] Exited arena bounds -> Destroy()");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_inited) return;

        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (!enemy) return;

            var stats = Game.I?.player?.GetComponent<PlayerStats>();
            bool isCrit = stats != null && Random.value < Mathf.Clamp01(stats.CritChance);

            float dmg = _damage;
            if (isCrit)
            {
                dmg *= Random.Range(1.5f, 2.5f);
                ShowCritEffect(enemy.transform);
            }

            enemy.TakeDamage(dmg);

            if (!(stats != null && TryRicochet(enemy, stats)))
                Destroy(gameObject);
        }
    }

    private void ShowCritEffect(Transform enemy)
    {
        if (critEffectPrefab)
            Instantiate(critEffectPrefab, enemy.position + Vector3.up * 1.2f, Quaternion.identity);

        if (critTextPrefab)
        {
            var txt = Instantiate(critTextPrefab, enemy.position + Vector3.up * critTextYOffset, Quaternion.identity);
            Destroy(txt, 1f);
        }
    }

    private bool TryRicochet(Enemy hitEnemy, PlayerStats stats)
    {
        if (Random.value > Mathf.Clamp01(stats.RicochetChance)) return false;
        if (_ricochetCount >= maxRicochets) return false;

        Collider[] hits = Physics.OverlapSphere(hitEnemy.transform.position, ricochetSearchRadius);
        List<Enemy> candidates = new List<Enemy>(4);
        foreach (var c in hits)
        {
            if (c && c.TryGetComponent<Enemy>(out var e) && e != hitEnemy)
                candidates.Add(e);
        }

        if (candidates.Count == 0) return false;

        Enemy newTarget = candidates[Random.Range(0, candidates.Count)];
        _direction = (newTarget.transform.position - transform.position).normalized;
        _ricochetCount++;
        Debug.Log($"[Projectile] Ricochet -> new target {newTarget.name}");
        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ricochetSearchRadius);
    }
#endif
}