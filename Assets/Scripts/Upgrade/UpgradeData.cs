using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade", fileName = "Upgrade_")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public UpgradeType type;
    public float value = 0.1f; // для %, храни как 0.1 (=10%) или как плоское число (радиус/сек/урон)
    public int minWave = 1;    // если пригодится отсеивать на ранних этапах
}
