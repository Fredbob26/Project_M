using UnityEngine;

public class BonusDrop : MonoBehaviour
{
    [Header("Randomize from this list. If empty — use all types.")]
    public PowerUpType[] pool;

    private static readonly PowerUpType[] AllTypes = new[]
    {
        PowerUpType.RapidFire,
        PowerUpType.Shield,
        PowerUpType.Freeze,
        PowerUpType.ScoreFrenzy
    };

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Game.I || !Game.I.buffs) { Destroy(gameObject); return; }

        var pick = GetRandomType();
        Game.I.buffs.Activate(pick);
        Debug.Log($"[BonusDrop] Picked {pick}");

        Destroy(gameObject);
    }

    private PowerUpType GetRandomType()
    {
        var src = (pool != null && pool.Length > 0) ? pool : AllTypes;
        int idx = Random.Range(0, src.Length);
        return src[idx];
    }
}

