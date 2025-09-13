using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrades/Base Upgrade")]
public abstract class BaseUpgrade : ScriptableObject
{
    [Header("Upgrade Info")]
    public string upgradeName;
    public string description;
    public Sprite icon;

    public abstract void ApplyUpgrade();
}