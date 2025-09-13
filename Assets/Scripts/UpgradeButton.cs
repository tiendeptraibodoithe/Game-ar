using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("Upgrade Type")]
    public UpgradeType upgradeType = UpgradeType.AttackSpeed;

    public void OnClickUpgrade()
    {
        if (UpgradeSystem.Instance == null)
        {
            Debug.LogError("UpgradeSystem Instance is null!");
            return;
        }

        // Thực hiện nâng cấp dựa trên loại
        switch (upgradeType)
        {
            case UpgradeType.AttackSpeed:
                UpgradeSystem.Instance.UpgradeAttackSpeed();
                break;
            case UpgradeType.Damage:
                UpgradeSystem.Instance.UpgradeDamage();
                break;
            case UpgradeType.Health:
                UpgradeSystem.Instance.UpgradeHealth();
                break;
            case UpgradeType.DoubleBullet:
                UpgradeSystem.Instance.UpgradeDoubleBullet();
                break;
            case UpgradeType.Shield:
                UpgradeSystem.Instance.UpgradeShield();
                break;
        }

        // Thông báo rằng một nâng cấp đã được chọn
        UpgradeSystem.Instance.OnUpgradeSelected();
    }
}

public enum UpgradeType
{
    AttackSpeed,
    Damage,
    Health,
    Speed,
    DoubleBullet,
    Shield
}