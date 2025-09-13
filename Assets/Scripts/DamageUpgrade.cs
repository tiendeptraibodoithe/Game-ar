using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Upgrade", menuName = "Upgrades/Damage Upgrade")]
public class DamageUpgrade : BaseUpgrade
{
    [Header("Damage Settings")]
    public float damageIncrease = 5f;

    public override void ApplyUpgrade()
    {
        // Sử dụng event system để áp dụng nâng cấp
        GameEvents.Instance.OnDamageUpgrade?.Invoke(damageIncrease);
        Debug.Log($"Damage increased by {damageIncrease}");
    }
}
