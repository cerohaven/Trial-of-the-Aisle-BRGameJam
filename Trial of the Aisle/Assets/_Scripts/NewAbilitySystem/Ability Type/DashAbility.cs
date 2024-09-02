using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/General/Dash")]
public class DashAbility : Ability
{
    public float dashForce = 200f;
    public float dodgeCooldown = 2f;
    public float dodgeTime = 0.75f;
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.5f;
    public int ghostNumber = 3;
    public AnimationCurve accelerationCurve;
    public float ghostInterval = 0.05f; // Time interval between ghost instances

    private float lastDodgeTime;

    private bool isDodging = false;

    private void OnEnable()
    {
        // Initialize lastDodgeTime so that the cooldown can start correctly from the beginning
        lastDodgeTime = Time.time - dodgeCooldown;
    }

    public override void Activate(GameObject owner)
    {
        Debug.Log("Dash ability activated");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("DashAbility: Player GameObject not found in the scene with tag 'Player'.");
            return;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("DashAbility: PlayerController component is missing on the player GameObject.");
            return;
        }

        if (isDodging)
        {
            Debug.LogWarning("DashAbility: Already dashing.");
            return;
        }

        float timeSinceLastDodge = Time.time - lastDodgeTime;
        if (timeSinceLastDodge < dodgeCooldown)
        {
            float cooldownRemaining = dodgeCooldown - timeSinceLastDodge;
            Debug.LogWarning($"DashAbility: Cooldown not complete. Time remaining: {cooldownRemaining:F2} seconds");
            return;
        }

        lastDodgeTime = Time.time;  // Set last dodge time here
        Debug.Log($"Dash started at time: {lastDodgeTime}. Cooldown will end at: {lastDodgeTime + dodgeCooldown}");

        playerController.StartCoroutine(DodgeRoutine(playerController, player));
    }

    private IEnumerator DodgeRoutine(PlayerController playerController, GameObject player)
    {
        Debug.Log("DodgeRoutine started");
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
            Debug.Log("Screen shake triggered");
        }
        else
        {
            Debug.LogWarning("DashAbility: ScreenShake component is missing.");
        }

        PlayerGhostTrail ghostTrail = playerController.GetComponent<PlayerGhostTrail>();
        Vector2 dodgeDirection = GameManager.Instance.PlayerInputHandler.ReadMovementValue().normalized;
        Debug.Log("Dodge Direction: " + dodgeDirection);

        if (dodgeDirection == Vector2.zero)
        {
            Debug.LogWarning("DashAbility: Dodge direction is zero, no force will be applied.");
            isDodging = false;
            yield break;
        }

        if (accelerationCurve == null)
        {
            Debug.LogError("DashAbility: AccelerationCurve is not assigned.");
            isDodging = false;
            yield break;
        }

        // Apply a single force impulse in the dodge direction
        float initialForce = dashForce * accelerationCurve.Evaluate(0f);
        rb.AddForce(dodgeDirection * initialForce, ForceMode2D.Impulse);
        Debug.Log("Applied force: " + dodgeDirection * initialForce);

        // Create ghost trail with a reduced frequency
        float elapsedTime = 0f;
        float ghostTimer = 0f;

        while (elapsedTime < dodgeTime)
        {
            elapsedTime += Time.deltaTime;
            ghostTimer += Time.deltaTime;

            // Create ghost effect at intervals
            if (ghostTrail != null && ghostTimer >= ghostInterval)
            {
                ghostTrail.CreateGhost();
                ghostTimer = 0f;
                Debug.Log("Ghost trail created");
            }

            yield return null;
        }

        rb.velocity = Vector2.zero; // Stop the player's velocity after the dash
        Debug.Log("Dash completed, resetting velocity");

        PlayDashAnimation(dashAnimationTransform);

        isDodging = false; // Reset dashing state only after everything completes
        Debug.Log("DodgeRoutine completed, isDodging reset to false");
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
