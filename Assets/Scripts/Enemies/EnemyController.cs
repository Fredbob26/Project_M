using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Transform player;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            DebugManager.LogError("Player not found in scene!");
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // фиксируем Y
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}