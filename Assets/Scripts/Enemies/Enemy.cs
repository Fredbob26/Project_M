// Assets/Scripts/Enemies/Enemy.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Damageable))]
public class Enemy : MonoBehaviour
{
    public enum EnemyType { Base, Tank, Bonus, Boss }

    [Header("Type")]
    public EnemyType type = EnemyType.Base;

    [Header("Stats (fallback)")]
    public int baseMaxHP = 10;
    public float baseSpeed = 3.5f;

    [Header("Drops")]
    public EssencePickup essencePrefab;    // Base/Tank/Boss
    public BonusDrop bonusDropPrefab;      // Bonus-only

    [Header("UI")]
    public EnemyHealthBar healthBar;       // держи выключенным в префабе

    NavMeshAgent _agent;
    Transform _player;
    Damageable _hp;
    float _nextContact;

    public void InitType(EnemyType t) => type = t;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _hp = GetComponent<Damageable>();

        var col = GetComponent<CapsuleCollider>();
        if (col) col.isTrigger = true;

        tag = "Enemy";
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;

        int maxHp = baseMaxHP;
        float speed = baseSpeed;

        var cfg = Game.I ? Game.I.config : null;
        if (cfg)
        {
            switch (type)
            {
                case EnemyType.Base:
                    maxHp = cfg.enemyBaseHP;
                    speed = cfg.enemyBaseSpeed;
                    break;
                case EnemyType.Tank:
                    maxHp = Mathf.RoundToInt(cfg.enemyBaseHP * cfg.tankHpMul);
                    speed = cfg.enemyBaseSpeed * cfg.tankSpeedMul;
                    break;
                case EnemyType.Bonus:
                    maxHp = Mathf.RoundToInt(cfg.enemyBaseHP * cfg.bonusHpMul);
                    speed = cfg.enemyBaseSpeed * cfg.bonusSpeedMul;
                    break;
                case EnemyType.Boss:
                    int wave = Game.I ? Game.I.enemiesKilled : 0;
                    maxHp = Mathf.RoundToInt(cfg.bossBaseHP + cfg.bossHpGrowth * wave);
                    speed = cfg.bossSpeed;
                    break;
            }
        }

        _hp.Initialize(maxHp);
        _agent.speed = speed;
        _agent.acceleration = 40f;
        _agent.angularSpeed = 720f;
        _agent.stoppingDistance = 0f;
        _agent.updateRotation = true;

        if (healthBar == null) healthBar = GetComponentInChildren<EnemyHealthBar>(true);
        _hp.OnDamaged += (cur, max, crit) =>
        {
            if (healthBar != null)
            {
                if (!healthBar.gameObject.activeSelf) healthBar.Show();
                healthBar.SetTarget(cur, max);
            }
        };
        _hp.OnDeath += OnDied;
    }

    void Update()
    {
        if (_player == null) return;

        // Freeze — стопаем агента (используем powerUps, а не buffs)
        bool frozen = (Game.I && Game.I.powerUps && Game.I.powerUps.FreezeActive);
        if (frozen)
        {
            if (!_agent.isStopped) _agent.isStopped = true;
            return;
        }
        else if (_agent.isStopped)
        {
            _agent.isStopped = false;
        }

        _agent.SetDestination(_player.position);
    }

    // Контактный урон — коллайдер врага триггерный
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!(Game.I && Game.I.config && Game.I.lifeTimer)) return;

        float cd = Mathf.Max(0.05f, Game.I.config.contactDamageCooldown);
        if (Time.time < _nextContact) return;
        _nextContact = Time.time + cd;

        // Флэш делает LifeTimer (он же учитывает активный щит)
        Game.I.lifeTimer.TakeContactDamage(Game.I.config.contactDamageSeconds);
    }

    void OnDied()
    {
        if (type == EnemyType.Base || type == EnemyType.Tank || type == EnemyType.Boss)
        {
            if (essencePrefab) Instantiate(essencePrefab, transform.position, Quaternion.identity);
        }

        if (type == EnemyType.Bonus)
        {
            if (bonusDropPrefab) Instantiate(bonusDropPrefab, transform.position, Quaternion.identity);
            else if (essencePrefab) Instantiate(essencePrefab, transform.position, Quaternion.identity);
        }

        Game.I?.OnEnemyKilled();
    }
}
