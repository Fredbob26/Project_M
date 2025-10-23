using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade", fileName = "Upgrade_")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public UpgradeType type;
    public float value = 0.1f; // ��� %, ����� ��� 0.1 (=10%) ��� ��� ������� ����� (������/���/����)
    public int minWave = 1;    // ���� ���������� ��������� �� ������ ������
}
