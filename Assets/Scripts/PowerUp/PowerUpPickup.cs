using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpPickup : MonoBehaviour
{
    [Header("Настройка пауэрапа")]
    public PowerUpType type = PowerUpType.RapidFire;

    [Tooltip("Длительность эффекта, сек.")]
    [SerializeField] private float duration = 10f;

    private bool _consumed;
    private Collider _col;
    private Renderer _rend;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        if (_col) _col.isTrigger = true;
        _rend = GetComponent<Renderer>();
        gameObject.layer = LayerMask.NameToLayer("Pickup");
        tag = "Pickup";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_consumed) return;
        if (!other.CompareTag("Player") && other.GetComponentInParent<PlayerStats>() == null) return;
        Consume();
    }

    private void Consume()
    {
        _consumed = true;
        if (_col) _col.enabled = false;
        if (_rend) _rend.enabled = false;

        var mgr = Game.I ? Game.I.powerUps : null;
        if (mgr == null) mgr = FindObjectOfType<PowerUpManager>();

        if (mgr != null)
        {
            mgr.Activate(type, duration);
            Debug.Log($"[Pickup] PowerUpPickup picked -> {type} ({duration:0.##}s)");
        }
        else
        {
            Debug.LogWarning("[PowerUpPickup] PowerUpManager не найден — активация не выполнена.");
        }

        Destroy(gameObject, 0.01f);
    }
}
