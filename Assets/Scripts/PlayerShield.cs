using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public int damageOnHit = 10;      // Mỗi lần enemy chạm -> enemy mất máu
    public float shieldRadius = 2f;   // Bán kính của shield

    [Header("Visual Settings")]
    public Color shieldColor = Color.cyan;          // Màu của shield
    private Color baseShieldColor;
    public Color flashColor;    // Màu khi shield bị chạm
    public Material shieldMaterial;                 // Material cho shield (optional)
    public bool showVisualShield = true;            // Bật/tắt hiển thị shield
    public float shieldAlpha = 0.3f;               // Độ trong suốt của shield (0-1)

    private SphereCollider shieldCollider;
    private GameObject shieldVisual;                // GameObject hiển thị shield
    private Renderer shieldRenderer;

    void Start()
    {
        // Collider của shield
        shieldCollider = GetComponent<SphereCollider>();
        shieldCollider.isTrigger = true;
        shieldCollider.radius = shieldRadius;

        baseShieldColor = shieldColor;

        // Tạo visual cho shield
        CreateShieldVisual();
    }

    void CreateShieldVisual()
    {
        if (!showVisualShield) return;

        // Tạo một sphere để hiển thị shield
        shieldVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shieldVisual.name = "ShieldVisual";
        shieldVisual.transform.SetParent(transform);
        shieldVisual.transform.localPosition = Vector3.zero;
        shieldVisual.transform.localScale = Vector3.one * (shieldRadius * 2);

        // Xóa collider của visual sphere (chỉ dùng để hiển thị)
        Collider visualCollider = shieldVisual.GetComponent<Collider>();
        if (visualCollider != null)
            DestroyImmediate(visualCollider);

        // Lấy renderer và cấu hình material
        shieldRenderer = shieldVisual.GetComponent<Renderer>();

        if (shieldMaterial != null)
        {
            shieldRenderer.material = shieldMaterial;
        }
        else
        {
            // Tạo material mặc định với màu trong suốt
            Material defaultMaterial = new Material(Shader.Find("Standard"));
            defaultMaterial.SetFloat("_Mode", 3); // Transparent mode
            defaultMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            defaultMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            defaultMaterial.SetInt("_ZWrite", 0);
            defaultMaterial.DisableKeyword("_ALPHATEST_ON");
            defaultMaterial.EnableKeyword("_ALPHABLEND_ON");
            defaultMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            defaultMaterial.renderQueue = 3000;

            shieldRenderer.material = defaultMaterial;
        }

        // Đặt màu cho shield
        UpdateShieldColor();
    }

    void UpdateShieldColor()
    {
        if (shieldRenderer != null)
        {
            Color finalColor = baseShieldColor;
            finalColor.a = shieldAlpha;
            shieldRenderer.material.color = finalColor;
        }
    }

    void UpdateShieldVisualSize()
    {
        if (shieldVisual != null)
        {
            shieldVisual.transform.localScale = Vector3.one * (shieldRadius * 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemyHealth = other.GetComponent<Enemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageOnHit);
                Debug.Log($"Enemy chạm shield! Enemy mất {damageOnHit} máu.");

                // Tạo hiệu ứng khi shield được kích hoạt
                StartCoroutine(ShieldHitEffect());
            }
        }
    }

    // Hiệu ứng khi shield bị chạm
    System.Collections.IEnumerator ShieldHitEffect()
    {
        if (shieldRenderer == null) yield break;

        // Flash trắng
        shieldRenderer.material.color = flashColor;
        yield return new WaitForSeconds(0.1f);

        // Trở về màu gốc (không lấy từ biến shieldColor nữa)
        UpdateShieldColor();
    }

    // ====== HÀM NÂNG CẤP ======
    // Tăng damage shield
    public void UpgradeShieldDamage(int amount)
    {
        damageOnHit += amount;
        Debug.Log($"Shield damage upgraded! Damage = {damageOnHit}");
    }

    // Tăng độ rộng shield
    public void UpgradeShieldSize(float amount)
    {
        shieldRadius += amount;
        if (shieldCollider != null)
        {
            shieldCollider.radius = shieldRadius;
        }

        // Cập nhật visual
        UpdateShieldVisualSize();

        Debug.Log($"Shield size upgraded! Radius = {shieldRadius}");
    }

    // ====== HÀM VISUAL CONTROLS ======
    public void SetShieldColor(Color newColor)
    {
        shieldColor = newColor;
        UpdateShieldColor();
    }

    public void SetShieldAlpha(float alpha)
    {
        shieldAlpha = Mathf.Clamp01(alpha);
        UpdateShieldColor();
    }

    public void ToggleShieldVisual(bool show)
    {
        showVisualShield = show;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(show);
        }
    }

    void OnDestroy()
    {
        if (shieldVisual != null)
        {
            DestroyImmediate(shieldVisual);
        }
    }

    // Để debug trong Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = shieldColor;
        Gizmos.DrawWireSphere(transform.position, shieldRadius);
    }
}