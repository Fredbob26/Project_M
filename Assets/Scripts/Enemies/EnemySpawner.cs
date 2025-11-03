using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Arena")]
    public BoxCollider arenaBounds;  // если пусто — возьмём из StaticConfig.ArenaBounds
    public Transform parent;

    [Header("SPM (спавны/минуту)")]
    public AnimationCurve spmCurve = AnimationCurve.Linear(0, 20, 300, 80);
    [Tooltip("Нормализация кривой (секунды игры).")] public float curveTimeScale = 1f;

    [Header("Mix & Chances")]
    [Tooltip("Вероятность бонусного врага (0..1).")] public float bonusChance = 0.1f;
    [Tooltip("Через сколько секунд после начала разрешить Танка.")] public float tankUnlockTime = 60f;

    [Header("Boss schedule")]
    [Tooltip("Через сколько секунд впервые вызвать босса.")] public float bossFirstDelay = 300f;
    [Tooltip("Периодичность боссов, сек.")] public float bossPeriod = 300f;

    [Header("Prefabs")]
    public Enemy baseEnemyPrefab;
    public Enemy tankEnemyPrefab;
    public Enemy bonusEnemyPrefab;
    public Enemy bossPrefab;

    float _timerSpawn;
    float _nextBossTime;
    float _t;

    void Start()
    {
        if (arenaBounds == null) arenaBounds = Game.I.config.ArenaBounds;
        if (parent == null) parent = transform;
        _t = 0f;
        _nextBossTime = bossFirstDelay > 0f ? bossFirstDelay : 999999f;
    }

    void Update()
    {
        if (Game.I == null || !Game.I.GameReady) return;

        _t += Time.deltaTime;

        if (bossPrefab && _t >= _nextBossTime)
        {
            SpawnEnemy(bossPrefab, Enemy.EnemyType.Boss);
            _nextBossTime += Mathf.Max(1f, bossPeriod);
        }

        float spm = Mathf.Max(0f, spmCurve.Evaluate(_t / Mathf.Max(0.01f, curveTimeScale)));
        float interval = (spm <= 0.01f) ? 9999f : 60f / spm;

        _timerSpawn += Time.deltaTime;
        if (_timerSpawn >= interval)
        {
            _timerSpawn = 0f;
            SpawnRegular();
        }
    }

    void SpawnRegular()
    {
        bool tankUnlocked = _t >= tankUnlockTime;

        bool isBonus = bonusEnemyPrefab && Random.value < Mathf.Clamp01(bonusChance);
        if (isBonus)
        {
            SpawnEnemy(bonusEnemyPrefab, Enemy.EnemyType.Bonus);
            return;
        }

        if (tankUnlocked && tankEnemyPrefab && Random.value < 0.25f)
            SpawnEnemy(tankEnemyPrefab, Enemy.EnemyType.Tank);
        else if (baseEnemyPrefab)
            SpawnEnemy(baseEnemyPrefab, Enemy.EnemyType.Base);
    }

    void SpawnEnemy(Enemy prefab, Enemy.EnemyType type)
    {
        if (!arenaBounds || !prefab) return;

        Bounds b = arenaBounds.bounds;
        Vector3 pos = new Vector3(
            Random.Range(b.min.x, b.max.x),
            prefab.transform.position.y,
            Random.Range(b.min.z, b.max.z)
        );

        Enemy e = Instantiate(prefab, pos, Quaternion.identity, parent);
        e.InitType(type);
    }
}
