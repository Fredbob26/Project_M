using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class AtomBombProjectile : MonoBehaviour
{
    public float lifetime = 8f;

    private Rigidbody _rb;
    private Collider _arena;
    private Vector3 _dir;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var arenaObj = GameObject.FindGameObjectWithTag("LevelBounds");
        if (arenaObj) _arena = arenaObj.GetComponent<Collider>();
    }

    public void Init(float speed)
    {
        _dir = Game.I.player.transform.forward.normalized;
        _rb.velocity = _dir * speed;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // подстраховка, чтобы не замедл€лась
        _rb.velocity = _dir * _rb.velocity.magnitude;
        if (_arena && !_arena.bounds.Contains(transform.position))
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ”ничтожаем врагов и их дочерние хитбоксы
        if (other.CompareTag("Enemy"))
        {
            var dmg = other.GetComponent<IDamageable>() ?? other.GetComponentInParent<IDamageable>();
            if (dmg != null) dmg.ApplyDamage(999999f, false);
        }
    }
}
