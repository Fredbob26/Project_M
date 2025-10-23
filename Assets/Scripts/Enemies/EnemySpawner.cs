using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 40;
    public float minDistanceFromPlayer = 6f;
    [Range(0f, 1f)] public float backSpawnBias = 0.7f; // шанс спавна за спиной

    [Header("Level Bounds")]
    public BoxCollider levelBounds; // арена, внутри которой спавнятся враги

    private Transform player;
    private float spawnTimer;
    private int currentEnemyCount = 0;

    private void Start()
    {
        // Ищем игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found!");

        // Проверка арены
        if (levelBounds == null)
            Debug.LogError("Level bounds not assigned!");

        // Убеждаемся, что LevelBounds не мешает физике
        if (levelBounds != null && !levelBounds.isTrigger)
        {
            levelBounds.isTrigger = true;
            Debug.LogWarning("LevelBounds collider was not set as Trigger. Fixed automatically.");
        }
    }

    private void Update()
    {
        if (player == null || enemyPrefab == null || levelBounds == null)
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && currentEnemyCount < maxEnemies)
        {
            Vector3 spawnPos = GetSmartSpawnPosition();
            spawnPos = AdjustToGround(spawnPos); // выравниваем по земле
            SpawnEnemy(spawnPos);
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        currentEnemyCount++;

        // визуальный маркер для дебага
        DebugSpawnMarker(position);

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
            enemyHealth.onDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        currentEnemyCount--;
    }

    private Vector3 GetSmartSpawnPosition()
    {
        Bounds b = levelBounds.bounds;
        Vector3 pos = Vector3.zero;
        int safety = 0;

        Vector3 forward = player.forward;
        forward.y = 0f;

        while (safety < 50)
        {
            pos = new Vector3(
                Random.Range(b.min.x, b.max.x),
                0f,
                Random.Range(b.min.z, b.max.z)
            );

            Vector3 toSpawn = (pos - player.position).normalized;
            float dot = Vector3.Dot(forward, toSpawn);

            // Спавн чаще за спиной игрока
            if (Random.value < backSpawnBias && dot > -0.3f)
            {
                safety++;
                continue;
            }

            // Не спавним слишком близко к игроку
            if (Vector3.Distance(player.position, pos) < minDistanceFromPlayer)
            {
                safety++;
                continue;
            }

            break;
        }

        return pos;
    }

    private Vector3 AdjustToGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 10f, Vector3.down, out hit, 30f))
        {
            float heightOffset = 0f;

            // Если у врага есть коллайдер — поднимаем на половину высоты + дополнительный зазор
            if (enemyPrefab.TryGetComponent(out Collider col))
            {
                heightOffset = col.bounds.extents.y + 1f;
            }

            pos.y = hit.point.y + heightOffset;
        }
        else
        {
            pos.y = 1f; // запасной вариант
        }

        return pos;
    }

    // 🧠 Визуализация зоны спавна и игрока
    private void OnDrawGizmos()
    {
        if (levelBounds == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f); // прозрачный зелёный
        Bounds b = levelBounds.bounds;
        Gizmos.DrawCube(b.center, b.size);

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(player.position, 0.5f);
        }
    }

    // 💡 Отладочный маркер (виден 1 секунду в сцене)
    private void DebugSpawnMarker(Vector3 position)
    {
#if UNITY_EDITOR
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.3f;
        marker.GetComponent<Collider>().enabled = false;
        marker.GetComponent<MeshRenderer>().material.color = Color.cyan;
        Destroy(marker, 1f);
#endif
    }
}