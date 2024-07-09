using UnityEngine;

public class PlayerHealEffect : MonoBehaviour
{
    public Material healMaterial; // Reference to the healing material
    private Material originalMaterial; // Reference to the original material
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    public float healEffectDuration = 1.0f; // Duration of the healing effect

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the original material
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
    }

    // Call this method to apply the heal effect
    public void ApplyHealEffect()
    {
        if (spriteRenderer != null && healMaterial != null)
        {
            // Change to the healing material
            spriteRenderer.material = healMaterial;

            // Revert to the original material after the duration
            Invoke("RevertToOriginalMaterial", healEffectDuration);
        }
    }

    // Revert to the original material
    void RevertToOriginalMaterial()
    {
        if (spriteRenderer != null && originalMaterial != null)
        {
            spriteRenderer.material = originalMaterial;
        }
    }
}
