using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Damageable))]
public class Enemy : MonoBehaviour
{
    [Header("Stats (fallback)")]
    public int baseMaxHP = 10;
    public float baseSpeed = 3f;

    [Header("Drops")]
    public EssencePickup essencePrefab;          // обычна€ эссенци€
    [Range(0f, 1f)] public float bonusDropChance = 0.15f;
    public BonusDrop bonusDropPrefab;            // бонусный дроп (PowerUp)

    [Header("UI")]
    public EnemyHealthBar healthBar; // в префабе оставь выключенным (SetActive=false)

    // --- runtime ---
    private Rigidbody _rb;
    private Transform _player;
    private float _speed;
    private Damageable _hp;
    private float _nextContactTime;
    private bool _typedInitApplied = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationZ |
                          RigidbodyConstraints.FreezePositionY;

        var col = GetComponent<CapsuleCollider>();
        col.isTrigger = false;

        _hp = GetComponent<Damageable>();
        tag = "Enemy";
    }

    private void Start()
    {
        // ≈сли InitType не вызывали Ч применим значени€ из конфига/фолбэки.
        if (!_typedInitApplied)
        {
            int maxHp = baseMaxHP;
            if (Game.I && Game.I.config)
            {
                maxHp = Mathf.Max(1, Game.I.config.enemyBaseHP);
                _speed = Mathf.Max(0f, Game.I.config.enemyBaseSpeed);
            }
            else
            {
                _speed = baseSpeed;
            }

            _hp.Initialize(maxHp);
        }

        if (!healthBar) healthBar = GetComponentInChildren<EnemyHealthBar>(true);

        // ѕоказ/обновление бара при уроне
        _hp.OnDamaged += (cur, max, crit) =>
        {
            if (healthBar)
            {
                if (!healthBar.gameObject.activeSelf) healthBar.Show();
                healthBar.SetTarget(cur, max);
            }
        };

        _hp.OnDeath += OnDied;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
    }

    private void FixedUpdate()
    {
        if (Game.I && Game.I.buffs && Game.I.buffs.FreezeActive)
        {
            _rb.velocity = Vector3.zero;
            return;
        }

        if (_player == null) return;

        Vector3 dir = _player.position - transform.position; dir.y = 0f;
        float d = dir.magnitude;
        if (d > 0.001f)
        {
            dir /= d;
            _rb.velocity = dir * _speed;
            transform.forward = Vector3.Slerp(transform.forward, dir, 12f * Time.fixedDeltaTime);
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (!collision.transform.CompareTag("Player")) return;
        if (!(Game.I && Game.I.config && Game.I.lifeTimer)) return;

        float now = Time.time;
        if (now < _nextContactTime) return;

        _nextContactTime = now + Game.I.config.contactDamageCooldown;

        // Shield учитываетс€ в LifeTimer.TakeContactDamage
        Game.I.lifeTimer.TakeContactDamage(Game.I.config.contactDamageSeconds);
    }

    private void OnDied()
    {
        // 1) Ёссенци€ Ч всегда
        if (essencePrefab)
            Instantiate(essencePrefab, transform.position, Quaternion.identity);

        // 2) Ѕонусный дроп Ч по шансу
        if (bonusDropPrefab && Random.value < bonusDropChance)
            Instantiate(bonusDropPrefab, transform.position, Quaternion.identity);

        Game.I?.OnEnemyKilled();
        // ”ничтожение объекта Ч делает Damageable (Destroy в OnDeath).
    }

    // ==========================
    //          API
    // ==========================
    /// <summary>
    /// “ипова€ инициализаци€ параметров врага по глобальному enum EnemyType,
    /// которым пользуетс€ спавнер.
    /// </summary>
    public void InitType(EnemyType kind)
    {
        var cfg = Game.I ? Game.I.config : null;

        int baseHP = cfg ? Mathf.Max(1, cfg.enemyBaseHP) : baseMaxHP;
        float baseSpd = cfg ? Mathf.Max(0f, cfg.enemyBaseSpeed) : baseSpeed;

        int hp = baseHP;
        float spd = baseSpd;
        float bonusChance = bonusDropChance;

        switch (kind)
        {
            case EnemyType.Base:
                // без изменений
                break;

            case EnemyType.Tank:
                // танк: x2 HP, 0.7 скорости, немного выше шанс бонуса
                hp = Mathf.RoundToInt(baseHP * 2f);
                spd = baseSpd * 0.7f;
                bonusChance = Mathf.Max(bonusChance, 0.20f);
                break;

            case EnemyType.Bonus:
                // бонусный: меньше HP, быстрее, высокий шанс бонус-дропа
                hp = Mathf.RoundToInt(baseHP * 0.6f);
                if (hp < 1) hp = 1;
                spd = baseSpd * 1.25f;
                bonusChance = 0.75f;
                break;
        }

        _speed = spd;
        bonusDropChance = bonusChance;
        _hp.Initialize(hp);

        _typedInitApplied = true;
    }
}
