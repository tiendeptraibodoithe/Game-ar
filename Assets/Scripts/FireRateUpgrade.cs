using UnityEngine;

[CreateAssetMenu(fileName = "Fire Rate Upgrade", menuName = "Upgrades/Fire Rate Upgrade")]
public class FireRateUpgrade : BaseUpgrade
{
    [Header("Fire Rate Settings")]
    public float fireRateDecrease = 0.1f; // Giảm để bắn nhanh hơn

    public override void ApplyUpgrade()
    {
        GameEvents.Instance.OnFireRateUpgrade?.Invoke(fireRateDecrease);
        Debug.Log($"Fire rate decreased by {fireRateDecrease}");
    }
}