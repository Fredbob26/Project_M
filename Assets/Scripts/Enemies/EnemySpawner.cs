using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs by type")]
    public Enemy baseEnemyPrefab;
    public Enemy tankEnemyPrefab;
    public Enemy bonusEnemyPrefab;
    public Transform parent;

    [Header("Bounds")]
    public BoxCollider arenaBounds;          // если не задан, возьмём по тегу LevelBounds

    [Header("SPM (spawns per minute)")]
    public float initialSPM = 20f;           // стартовые спавны/мин
    public float spmGrowthPerMinute = 6f;    // рост спавнов/мин каждый реальный мин.

    [Header("Spawn height")]
    [Tooltip("Добавляется к найденной высоте земли")]
    public float spawnHeight = 0.8f;

    [Tooltip("Слой(и) земли для Raycast")]
    public LayerMask groundMask = ~0;

    [Header("Distance from player")]
    public float minDistanceFromPlayer = 2.5f;

    [Header("Enemy Type Chances")]
    [Tooltip("Шанс бонусного врага (0..1)")]
    public float bonusEnemyChance = 0.05f;

    [Tooltip("Шанс танка после разблокировки (0..1), применяется если рулетка не попала в Bonus")]
    public float tankEnemyChance = 0.20f;

    [Tooltip("С какой минуты включать шанс танка")]
    public float tankUnlockMinute = 1.0f;

    // runtime
    private float _startTime;
    private float _nextSpawnAt;
    private Transform _player;

    private void Awake()
    {
        if (!arenaBounds)
        {
            var lb = GameObject.FindGameObjectWithTag("LevelBounds");
            if (lb) arenaBounds = lb.GetComponent<BoxCollider>();
        }
        if (!parent) parent = transform;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
    }

    private void OnEnable()
    {
        _startTime = Time.time;
        ScheduleNextSpawn(Time.time);
    }

    private void Update()
    {
        if (!Game.I || !Game.I.GameReady) return;
        if (!arenaBounds) return;

        if (Time.time >= _nextSpawnAt)
        {
            SpawnOne();
            ScheduleNextSpawn(_nextSpawnAt); // отталкиваемся от предыдущей точки — без «залпов»
        }
    }

    private float GetCurrentSPM(float now)
    {
        float minutes = Mathf.Max(0f, (now - _startTime) / 60f);
        return Mathf.Max(0.1f, initialSPM + spmGrowthPerMinute * minutes);
    }

    private void ScheduleNextSpawn(float anchorTime)
    {
        float spm = GetCurrentSPM(anchorTime);
        float interval = 60f / spm;
        _nextSpawnAt = anchorTime + interval;
    }

    private void SpawnOne()
    {
        var prefab = PickPrefab(out var type);

        if (!prefab) return;

        Bounds b = arenaBounds.bounds;

        // Сэмплим XZ
        Vector3 pos = new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.max.y + 5f, // стартовая высота для Raycast сверху
            Random.Range(b.min.z, b.max.z)
        );

        // Держим дистанцию от игрока
        if (_player && minDistanceFromPlayer > 0f)
        {
            float minSqr = minDistanceFromPlayer * minDistanceFromPlayer;
            int tries = 8;
            while (tries-- > 0 && (pos - _player.position).sqrMagnitude < minSqr)
            {
                pos.x = Random.Range(b.min.x, b.max.x);
                pos.z = Random.Range(b.min.z, b.max.z);
            }
        }

        // Ищем землю лучом вниз
        float rayLen = Mathf.Max(10f, b.size.y + 10f);
        Vector3 rayOrigin = new Vector3(pos.x, b.max.y + 5f, pos.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLen, groundMask, QueryTriggerInteraction.Ignore))
        {
            pos.y = hit.point.y + spawnHeight;
        }
        else
        {
            pos.y = b.min.y + spawnHeight;
        }

        var inst = Instantiate(prefab, pos, Quaternion.identity, parent);
        // На случай если в варианте префаба тип не совпадает с рулеткой — синхронизируем:
        var e = inst.GetComponent<Enemy>();
        if (e) e.InitType(type);
    }

    private Enemy PickPrefab(out EnemyType type)
    {
        type = RollType();

        switch (type)
        {
            case EnemyType.Bonus:
                if (bonusEnemyPrefab) return bonusEnemyPrefab;
                break;
            case EnemyType.Tank:
                if (tankEnemyPrefab) return tankEnemyPrefab;
                break;
        }
        // Fallback — базовый
        type = EnemyType.Base;
        return baseEnemyPrefab;
    }

    private EnemyType RollType()
    {
        float minutes = Mathf.Max(0f, (Time.time - _startTime) / 60f);

        // Сначала шанс бонусного
        if (Random.value < Mathf.Clamp01(bonusEnemyChance))
            return EnemyType.Bonus;

        // Затем шанс танка (после разблокировки)
        if (minutes >= tankUnlockMinute && Random.value < Mathf.Clamp01(tankEnemyChance))
            return EnemyType.Tank;

        return EnemyType.Base;
    }
}