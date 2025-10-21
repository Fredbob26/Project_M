using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public float spawnDistance = 10f;

    private float spawnTimer = 0f;
    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            DebugManager.LogError("Player not found in scene!");
    }

    private void Update()
    {
        if (player == null) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            DebugManager.LogError("Enemy prefab not assigned in EnemySpawner!");
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnDistance;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}