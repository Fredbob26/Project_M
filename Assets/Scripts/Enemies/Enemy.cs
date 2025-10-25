using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    public int maxHP = 10;
    public EssencePickup essencePrefab;

    private int _hp;
    private Rigidbody _rb;
    private Transform _player;
    private float _baseSpeed;
    private float _nextContactTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Твои рабочие настройки:
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationZ |
                          RigidbodyConstraints.FreezePositionY;

        var col = GetComponent<CapsuleCollider>();
        col.isTrigger = false;

        gameObject.layer = LayerMask.NameToLayer("Enemy");
        tag = "Enemy";
    }

    private void Start()
    {
        _hp = Mathf.Max(1, Game.I.config.enemyBaseHP);
        _baseSpeed = Mathf.Max(0f, Game.I.config.enemyBaseSpeed);

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
    }

    private void FixedUpdate()
    {
        if (!_player) return;
        Vector3 dir = (_player.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        _rb.velocity = dir * _baseSpeed;
        if (dir.sqrMagnitude > 0.0001f) transform.forward = dir;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.transform.CompareTag("Player")) return;
        float now = Time.time;
        if (now < _nextContactTime) return;

        _nextContactTime = now + Game.I.config.contactDamageCooldown;
        Game.I.lifeTimer.TakeContactDamage(Game.I.config.contactDamageSeconds);
    }

    public void TakeDamage(float dmg)
    {
        _hp -= Mathf.RoundToInt(dmg);
        if (_hp <= 0) Die();
    }

    private void Die()
    {
        if (essencePrefab)
            Instantiate(essencePrefab, transform.position, Quaternion.identity);

        Game.I.OnEnemyKilled();
        Destroy(gameObject);
    }
}