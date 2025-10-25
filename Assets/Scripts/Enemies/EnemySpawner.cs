using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab;
    public Transform parent;
    public float spawnInterval = 1.5f;

    [Header("Spawn Area")]
    public BoxCollider arenaBounds;
    [Tooltip("Высота, на которой спавнятся враги (например, 0.5f)")]
    public float spawnHeight = 0.5f;

    private float _timer;

    private void Start()
    {
        if (arenaBounds == null)
            arenaBounds = Game.I.config.ArenaBounds;

        if (parent == null)
            parent = transform;
    }

    private void Update()
    {
        if (!Game.I.GameReady) return;

        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (!arenaBounds || !enemyPrefab) return;

        Bounds b = arenaBounds.bounds;

        Vector3 pos = new Vector3(
            Random.Range(b.min.x, b.max.x),
            spawnHeight, // ← вот тут используем твой параметр
            Random.Range(b.min.z, b.max.z)
        );

        Instantiate(enemyPrefab, pos, Quaternion.identity, parent);
    }
}