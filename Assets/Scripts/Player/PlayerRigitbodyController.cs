using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerStats))]
public class PlayerRigidbodyController : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerStats _stats;
    private StaticConfig _cfg;
    private Vector3 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _stats = GetComponent<PlayerStats>();
        _cfg = Game.I != null ? Game.I.config : null;
        if (_cfg == null) Debug.LogWarning("PlayerRigidbodyController: Game.I.config is null");
    }

    private void Update()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        float baseSpeed = _cfg != null ? _cfg.baseMoveSpeed : 7f;
        float speed = _stats != null ? _stats.MoveSpeed : baseSpeed;
        Vector3 target = _rb.position + _input * (speed * Time.fixedDeltaTime);

        // Ограничение по аренe (если есть)
        var arena = _cfg != null ? _cfg.ArenaBounds : null;
        if (arena != null)
        {
            var b = arena.bounds;
            target.x = Mathf.Clamp(target.x, b.min.x, b.max.x);
            target.z = Mathf.Clamp(target.z, b.min.z, b.max.z);
        }

        _rb.MovePosition(target);
        if (_input.sqrMagnitude > 0.001f) transform.forward = _input;
    }
}