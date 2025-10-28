using UnityEngine;

public class EssencePickup : MonoBehaviour
{
    [Tooltip("Сколько секунд добавляет эта эссенция к таймеру")]
    public float timeBonus = 2f;

    [Tooltip("Сколько единиц ESSENCE добавляется в баланс (и базово в статистику)")]
    public int essenceValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Game.I.AddLife(timeBonus);

        // Базовое пополнение: +баланс и +статистика (подобрано)
        Game.I.AddEssence(essenceValue);

        // ScoreFrenzy: добавить ТОЛЬКО в баланс (без роста статистики)
        var buffs = Game.I ? Game.I.buffs : null;
        if (buffs && buffs.ScoreFrenzyActive)
        {
            float mult = Mathf.Max(1f, Game.I.config.scoreFrenzyMultiplier);
            int extra = Mathf.RoundToInt(essenceValue * (mult - 1f));
            if (extra > 0) Game.I.AddBalanceOnly(extra);
        }

        Destroy(gameObject);
    }
}
