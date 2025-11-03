// Assets/Scripts/Pickup/BonusDrop.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BonusDrop : MonoBehaviour
{
    [Tooltip("ƒлительность срабатывающего пауэрапа, сек.")]
    [SerializeField] private float duration = 10f;

    private bool _consumed;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Pickup");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_consumed) return;
        if (IsPlayer(other)) Consume();
    }

    private void OnTriggerStay(Collider other)
    {
        if (_consumed) return;
        if (IsPlayer(other)) Consume();
    }

    private bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player")) return true;
        if (other.GetComponentInParent<PlayerStats>() != null) return true;

        if (Game.I && Game.I.player)
        {
            var pt = Game.I.player.transform;
            if (other.transform == pt || other.transform.IsChildOf(pt)) return true;
        }
        return false;
    }

    private void Consume()
    {
        _consumed = true;

        var pum = Game.I ? Game.I.powerUps : null;
        if (pum == null)
        {
            Debug.LogWarning("[Pickup] BonusDrop: PowerUpManager не найден Ч бонус не применЄн.");
            Destroy(gameObject);
            return;
        }

        // ¬ыбираем тип (FireRing включаетс€ флагом в StaticConfig)
        bool includeRing = Game.I && Game.I.config ? Game.I.config.includeFireRingInRandom : true;
        PowerUpType type;
        if (includeRing)
        {
            float r = Random.value;
            type =
                r < 0.20f ? PowerUpType.RapidFire :
                r < 0.40f ? PowerUpType.ScoreFrenzy :
                r < 0.60f ? PowerUpType.Shield :
                r < 0.80f ? PowerUpType.Freeze :
                            PowerUpType.FireRing;
        }
        else
        {
            float r = Random.value;
            type =
                r < 0.25f ? PowerUpType.RapidFire :
                r < 0.50f ? PowerUpType.ScoreFrenzy :
                r < 0.75f ? PowerUpType.Shield :
                            PowerUpType.Freeze;
        }

        // Ћќ√ Ч какой бонус подн€т
        Debug.Log($"[Pickup] BonusDrop -> {type} ({duration:0.#}s)");

        // ¬ажно: просто вызов, Ѕ≈« присваивани€ в var Ч иначе CS0815
        if (type == PowerUpType.FireRing)
            pum.Activate(type, duration);       // менеджер сам отстрелит ринг
        else
            pum.Activate(type, duration);

        Destroy(gameObject);
    }
}
