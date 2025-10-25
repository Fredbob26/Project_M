using UnityEngine;

[System.Serializable]
public class UpgradeDefinition
{
    public UpgradeType type;
    public string upgradeName;       // ������������ � UpgradeMenu
    [TextArea] public string description;
    public Sprite icon;
    public float valueStep;          // ��� ���������� ��������
}