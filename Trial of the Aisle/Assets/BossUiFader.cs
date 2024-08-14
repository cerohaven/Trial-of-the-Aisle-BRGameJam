using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUiFader : MonoBehaviour
{
    public CanvasGroup bossBarUI; // Reference to the CanvasGroup controlling the boss bar UI
    public Transform player;      // Reference to the player's transform
    private float maxYPosition;   // The maximum Y position for 15% opacity
    private float minYPosition;   // The minimum Y position for full opacity
    [SerializeField] private BoxCollider2D boxCollider;

    private void Start()
    {
        // Calculate the min and max Y positions based on the collider's bounds
        minYPosition = boxCollider.bounds.min.y;
        maxYPosition = boxCollider.bounds.max.y;

        // Ensure the alpha starts at full opacity (1)
        bossBarUI.alpha = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeBossBar());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            ResetAlpha();
        }
    }

    private IEnumerator FadeBossBar()
    {
        while (true)
        {
            float playerY = player.position.y;

            // Calculate the alpha value based on the player's Y position
            float alpha = Mathf.InverseLerp(minYPosition, maxYPosition, playerY);

            // Adjust the alpha value so that it ranges from almost full (e.g., 0.95) to 0.15 (15% opacity)
            alpha = Mathf.Lerp(0.95f, 0.15f, alpha);

            // Set the CanvasGroup's alpha to fade the UI
            bossBarUI.alpha = alpha;

            yield return null;
        }
    }

    private void ResetAlpha()
    {
        // Reset the alpha to full opacity when the player exits the collider
        bossBarUI.alpha = 1f;
    }
}
