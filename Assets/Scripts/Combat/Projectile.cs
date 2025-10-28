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

    private Rigidbody _rb;
    private Collider _arena;
    private Vector3 _dir;
    private float _damage;
    private int _ricochets;
    private bool _inited;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        var col = GetComponent<Collider>();
        col.isTrigger = true; // пули — триггер для корректных попаданий по врагам

        var arenaObj = GameObject.FindGameObjectWithTag("LevelBounds");
        if (arenaObj) _arena = arenaObj.GetComponent<Collider>();
    }

    public void Init(Vector3 targetPos, float damage)
    {
        _damage = damage;
        _dir = (targetPos - transform.position).normalized;
        _inited = true;
        _ricochets = 0;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (!_inited) return;

        _rb.velocity = _dir * speed;

        if (_arena && !_arena.bounds.Contains(transform.position))
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_inited) return;

        // IDamageable может висеть на этом же коллайдере...
        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            Apply(dmg, other.transform);
            return;
        }

        // ...или на родителе
        if (other.transform.parent && other.transform.parent.TryGetComponent<IDamageable>(out var parentDmg))
        {
            Apply(parentDmg, other.transform.parent);
            return;
        }
    }

    private void Apply(IDamageable dmg, Transform hitRoot)
    {
        var stats = Game.I?.player?.GetComponent<PlayerStats>();
        bool isCrit = stats ? (Random.value < Mathf.Clamp01(stats.CritChance)) : false;

        // Баффов на урон сейчас нет → множитель = 1
        float finalDamage = _damage * (isCrit ? Random.Range(1.5f, 2.5f) : 1f);
        dmg.ApplyDamage(finalDamage, isCrit);

        // --- РИКОШЕТ ПО ШАНСУ ---
        float chance = stats ? Mathf.Clamp01(stats.RicochetChance) : 0f;
        bool allowedByChance = Random.value < chance;
        bool allowedByCount = _ricochets < maxRicochets;

        if (allowedByChance && allowedByCount && TryFindRicochetTarget(hitRoot, out Vector3 newDir))
        {
            _dir = newDir;  // меняем курс
            _ricochets++;
            return;
        }

        Destroy(gameObject);
    }

    private bool TryFindRicochetTarget(Transform from, out Vector3 newDir)
    {
        newDir = Vector3.zero;

        Collider[] hits = Physics.OverlapSphere(from.position, ricochetSearchRadius);
        List<Transform> candidates = new List<Transform>();

        foreach (var c in hits)
        {
            if (!c) continue;
            if (c.transform == from) continue;

            if (c.TryGetComponent<IDamageable>(out _))
                candidates.Add(c.transform);
            else if (c.transform.parent && c.transform.parent.TryGetComponent<IDamageable>(out _))
                candidates.Add(c.transform.parent);
        }

        if (candidates.Count == 0) return false;

        Transform newTarget = candidates[Random.Range(0, candidates.Count)];
        newDir = (newTarget.position - transform.position).normalized;
        return true;
    }
}