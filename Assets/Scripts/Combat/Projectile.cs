using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 22f;
    public float lifetime = 5f;

    [Header("Ricochet")]
    public float ricochetSearchRadius = 8f;
    public int maxRicochets = 3;

    [Header("VFX (optional, only on crit)")]
    public GameObject critVfxPrefab;
    public GameObject critPopupPrefab;

    private Rigidbody _rb;
    private Collider _arena;
    private Vector3 _dir;
    private float _damage;
    private int _ricochets;
    private bool _inited;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        var col = GetComponent<Collider>();
        col.isTrigger = true; // работаем триггером

        var arenaObj = GameObject.FindGameObjectWithTag("LevelBounds");
        if (arenaObj) _arena = arenaObj.GetComponent<Collider>();

        Destroy(gameObject, lifetime);
    }

    public void Init(Vector3 targetPos, float damage)
    {
        _damage = damage * (Game.I?.powerUps?.GetDamageMul() ?? 1f);
        _dir = (targetPos - transform.position).normalized;
        _ricochets = 0;
        _inited = true;
    }

    void FixedUpdate()
    {
        if (!_inited) return;

        _rb.velocity = _dir * speed;

        if (_arena != null && !_arena.bounds.Contains(transform.position))
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_inited) return;

        // 1) если это препятствие (по маске) — уничтожаем пулю
        int mask = Game.I.config.obstacleMask;
        if (((1 << other.gameObject.layer) & mask) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // 2) попадание по уронопринимающему
        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            Apply(dmg, other.transform);
            return;
        }
        if (other.transform.parent && other.transform.parent.TryGetComponent<IDamageable>(out var parentDmg))
        {
            Apply(parentDmg, other.transform.parent);
            return;
        }
    }

    void Apply(IDamageable dmg, Transform hit)
    {
        var stats = Game.I?.player?.GetComponent<PlayerStats>();
        bool isCrit = stats ? (Random.value < Mathf.Clamp01(stats.CritChance)) : false;

        float finalDamage = _damage * (isCrit ? 2f : 1f);
        dmg.ApplyDamage(finalDamage, isCrit);

        if (isCrit)
        {
            if (critVfxPrefab)
            {
                var v = Instantiate(critVfxPrefab, hit.position, Quaternion.identity);
                Destroy(v, 2f);
            }
            if (critPopupPrefab)
            {
                var p = Instantiate(critPopupPrefab, hit.position + Vector3.up * 0.6f, Quaternion.identity);
                var cp = p.GetComponent<CritPopup>();
                if (cp) cp.SetText("CRIT!");
                Destroy(p, 2f);
            }
        }

        if (TryRicochetFrom(hit)) return;

        Destroy(gameObject);
    }

    bool TryRicochetFrom(Transform hit)
    {
        if (_ricochets >= maxRicochets) return false;

        float chance = Game.I?.player?.GetComponent<PlayerStats>()?.RicochetChance ?? 0f;
        if (Random.value > chance) return false;

        Collider[] hits = Physics.OverlapSphere(hit.position, ricochetSearchRadius);
        var candidates = new List<Transform>();
        foreach (var c in hits)
        {
            if (!c) continue;
            var t = c.transform;
            if (t == hit) continue;

            if (t.TryGetComponent<IDamageable>(out _)) candidates.Add(t);
            else if (t.parent && t.parent.TryGetComponent<IDamageable>(out _)) candidates.Add(t.parent);
        }
        if (candidates.Count == 0) return false;

        Transform newTarget = candidates[Random.Range(0, candidates.Count)];
        _dir = (newTarget.position - transform.position).normalized;
        _ricochets++;
        return true;
    }
}
