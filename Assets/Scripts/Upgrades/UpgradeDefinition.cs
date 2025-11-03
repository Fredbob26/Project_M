using UnityEngine;

[System.Serializable]
public class UpgradeDefinition
{
    public UpgradeType type;
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public float valueStep;
}
