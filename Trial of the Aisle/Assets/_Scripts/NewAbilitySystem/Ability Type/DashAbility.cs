using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/General/Dash")]
public class DashAbility : Ability
{
    [SerializeField] private GameObject player; // Serialized reference to the player GameObject
    public float dashForce = 30f;
    public float dodgeCooldown = 1.5f;
    public float dodgeTime = 0.3f;
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.5f;
    public int ghostNumber = 8;
    public AnimationCurve accelerationCurve;

    private float lastDodgeTime = -5f;
    private bool isDodging = false;

    public override void Activate(GameObject owner)
    {
        Debug.Log("Dash ability activated");

        if (player == null)
        {
            Debug.LogError("DashAbility: Player GameObject reference is not assigned.");
            return;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("DashAbility: PlayerController component is missing on the assigned player GameObject.");
            return;
        }

        if (isDodging || Time.time - lastDodgeTime < dodgeCooldown)
        {
            Debug.LogWarning("DashAbility: Cooldown not complete or already dashing.");
            return;
        }

        lastDodgeTime = Time.time; // Start cooldown after this point
        playerController.StartCoroutine(DodgeRoutine(playerController));
    }

    private IEnumerator DodgeRoutine(PlayerController playerController)
    {
        isDodging = true;

        Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("DashAbility: Rigidbody2D component is missing on the PlayerController.");
            isDodging = false;
            yield break;
        }

        Transform dashAnimationTransform = player.transform.Find("Dash Animation");
        if (dashAnimationTransform == null)
        {
            Debug.LogError("DashAbility: Dash Animation child object is missing.");
            isDodging = false;
            yield break;
        }

        ScreenShake cameraShake = dashAnimationTransform.Find("Screenshake")?.GetComponent<ScreenShake>();
        if (cameraShake != null)
        {
            cameraShake.Shake(shakeDuration, shakeStrength);
        }
        else
        {
            Debug.LogWarning("DashAbility: ScreenShake component is missing.");
        }

        PlayerGhostTrail ghostTrail = playerController.GetComponent<PlayerGhostTrail>();
        Vector2 dodgeDirection = GameManager.Instance.PlayerInputHandler.ReadMovementValue().normalized;
        Debug.Log("Dodge Direction: " + dodgeDirection);

        if (accelerationCurve == null)
        {
            Debug.LogError("DashAbility: AccelerationCurve is not assigned.");
            isDodging = false;
            yield break;
        }

        // Dash and create ghost trail simultaneously
        float elapsedTime = 0f;
        while (elapsedTime < dodgeTime)
        {
            float currentForce = dashForce * accelerationCurve.Evaluate(elapsedTime / dodgeTime);
            rb.velocity = dodgeDirection * currentForce;

            // Create ghost effect during dash
            if (ghostTrail != null && elapsedTime < dodgeTime)
            {
                ghostTrail.CreateGhost();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero; // Stop the player's velocity after the dash
        Debug.Log("Dash completed, resetting velocity");

        PlayDashAnimation(dashAnimationTransform);

        isDodging = false; // Reset dashing state only after everything completes
    }

    private void PlayDashAnimation(Transform dashAnimationTransform)
    {
        Animator dashAnim = dashAnimationTransform.Find("Animation")?.GetComponent<Animator>();
        if (dashAnim != null)
        {
            dashAnim.Play("Base Layer.Dash", 0, 0.25f);
            Debug.Log("Playing dash animation");
        }
        else
        {
            Debug.LogWarning("DashAbility: Animator component not found.");
        }

        ParticleSystem dustParticles = dashAnimationTransform.Find("Dash Particles")?.GetComponent<ParticleSystem>();
        if (dustParticles != null)
        {
            dustParticles.Play();
            Debug.Log("Playing dust particle effects");
        }
        else
        {
            Debug.LogWarning("DashAbility: Dust particle system not found.");
        }
    }
}
