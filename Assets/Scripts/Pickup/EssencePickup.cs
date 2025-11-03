// Assets/Scripts/Pickup/EssencePickup.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EssencePickup : MonoBehaviour
{
    [Tooltip("Сколько секунд добавляет к таймеру")]
    public float timeBonus = 2f;

    [Tooltip("Сколько ESSENCE добавляет (до множителей)")]
    public int essenceValue = 1;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Pickup");
        tag = "Pickup";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Game.I == null) return;

        // таймер
        Game.I.AddLife(timeBonus);

        // ESSENCE с учётом ScoreFrenzy
        float mul = Game.I.powerUps ? Game.I.powerUps.GetEssenceMul() : 1f;
        int add = Mathf.Max(0, Mathf.RoundToInt(essenceValue * mul));
        Game.I.AddEssence(add);

        Destroy(gameObject);
    }
}
