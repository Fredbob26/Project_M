using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType type;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Game.I && Game.I.buffs)
        {
            Game.I.buffs.Activate(type);
        }
        Destroy(gameObject);
    }
}
