using UnityEngine;

public class EssencePickup : MonoBehaviour
{
    [Tooltip("������� ������ ��������� ��� �������� � �������")]
    public float timeBonus = 2f;

    [Tooltip("������� ������ ESSENCE ����������� � ������ (� ������ � ����������)")]
    public int essenceValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Game.I.AddLife(timeBonus);

        // ������� ����������: +������ � +���������� (���������)
        Game.I.AddEssence(essenceValue);

        // ScoreFrenzy: �������� ������ � ������ (��� ����� ����������)
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
