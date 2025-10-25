using UnityEngine;

public class EssencePickup : MonoBehaviour
{
    [Tooltip("������� ������ ��������� ��� �������� � �������")]
    public float timeBonus = 2f;

    [Tooltip("������� ������ ESSENCE ����������� � ������ (� � ���������� ������������)")]
    public int essenceValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Game.I.AddLife(timeBonus);           // ��������� ������
        Game.I.AddEssence(essenceValue);     // ����������� ������ � ����������

        Destroy(gameObject);
    }
}