using UnityEngine;

public class EssencePickup : MonoBehaviour
{
    [Tooltip("Сколько секунд добавляет эта эссенция к таймеру")]
    public float timeBonus = 2f;

    [Tooltip("Сколько единиц ESSENCE добавляется в баланс (и в статистику подобранного)")]
    public int essenceValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Game.I.AddLife(timeBonus);           // пополняем таймер
        Game.I.AddEssence(essenceValue);     // увеличиваем БАЛАНС и СТАТИСТИКУ

        Destroy(gameObject);
    }
}