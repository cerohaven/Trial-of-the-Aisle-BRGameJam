using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[CreateAssetMenu(fileName = "RaycastAbility", menuName = "Abilities/General/Raycast Ability")]
public class RaycastAbility : Ability
{
    public GameObject moonbeamPrefab; // Reference to the Moonbeam prefab with LineRenderer and particle effects
    public float abilityDuration = 2f; // Duration of the ability's effect
    public float splashDuration = 1f; // Duration of the splash effect
    public float damageInterval = 0.5f; // Time between each damage tick
    public float effectRange = 2f; // Radius of the CircleCollider2D's effective area
    public float beamOffset = 0.5f; // Offset to shorten the beam tip

    private EntityHealth entityHealth; // The SO responsible for changing health
    private ChangeHealth changeHealthAmount; // Amount of damage to apply

    private GameObject shootEffectInstance; // Store a reference to the shoot effect instance
    private ParticleSystem splashEffectInstance; // Store a reference to the splash effect instance

    public override void Activate(GameObject owner)
    {
        // Instantiate the moonbeam prefab
        GameObject moonbeamInstance = Instantiate(moonbeamPrefab, owner.transform.position, Quaternion.identity);

        entityHealth = GameManager.Instance.BossTransform.GetComponent<EntityHealth>();

        // Play the shoot effect and make it follow the player
        PlayShootEffect(owner, moonbeamInstance);

        LineRenderer lineRenderer = moonbeamInstance.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Moonbeam prefab does not have a LineRenderer component.");
            return;
        }

        GameObject impactColliderInstance = Instantiate(moonbeamPrefab, Vector3.zero, Quaternion.identity);
        CircleCollider2D circleCollider = impactColliderInstance.GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.radius = effectRange; // Set the collision radius here
        }
        else
        {
            Debug.LogError("Impact collider prefab does not have a CircleCollider2D component.");
            return;
        }

        // Find and store the splash effect instance
        splashEffectInstance = moonbeamInstance.transform.Find("splashEffect").GetComponent<ParticleSystem>();

        owner.GetComponent<MonoBehaviour>().StartCoroutine(ActivateRaycastAbility(owner, lineRenderer, moonbeamInstance, impactColliderInstance));
    }

    private IEnumerator ActivateRaycastAbility(GameObject owner, LineRenderer lineRenderer, GameObject moonbeamInstance, GameObject impactColliderInstance)
    {
        float endTime = Time.time + abilityDuration;
        float nextDamageTime = Time.time;

        // Set the LayerMask to detect only walls
        int wallLayerMask = LayerMask.GetMask("Wall");

        while (Time.time < endTime)
        {
            // Get the current control scheme
            ControlScheme controlScheme = GameManager.Instance.ControlScheme;

            Vector2 dir;
            Vector3 cursorWorldPosition;

            if (controlScheme == ControlScheme.Gamepad)
            {
                Vector3 gamepadPosition = GameManager.Instance.GamepadCursor.VirtualMouse.position.ReadValue();
                cursorWorldPosition = Camera.main.ScreenToWorldPoint(gamepadPosition);
                dir = cursorWorldPosition - owner.transform.position;
            }
            else
            {
                // Default to mouse position if the control scheme is not Gamepad
                cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                cursorWorldPosition.z = owner.transform.position.z;
                dir = cursorWorldPosition - owner.transform.position;
            }

            dir.Normalize();

            // Perform a raycast to detect walls
            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, dir, Vector3.Distance(owner.transform.position, cursorWorldPosition), wallLayerMask);

            Vector3 endPosition = cursorWorldPosition;

            // Debug the raycast direction
            Debug.DrawRay(owner.transform.position, dir * Vector3.Distance(owner.transform.position, dir), Color.red);

            if (hit.collider != null)
            {
                // If the raycast hits a wall, set the end position to the hit point
                endPosition = hit.point;
                Debug.Log("Raycast hit: " + hit.collider.name);

                // Play splash effect at the hit point
                PlaySplashEffectAtPosition(hit.point);
            }
            else
            {
                Debug.Log("Raycast did not hit any walls.");
            }

            

            // Update LineRenderer positions to start at the owner's position and end at the hit point or target position
            lineRenderer.SetPosition(0, owner.transform.position); // Start at the player's position
            lineRenderer.SetPosition(1, endPosition); // End at the hit point or target position

            // Move the impact collider to follow the end position
            impactColliderInstance.transform.position = endPosition;

            // Update the shoot effect's position and rotation
            UpdateShootEffectPositionAndRotation(owner, endPosition);

            if (Time.time >= nextDamageTime)
            {
                ApplyDamage(owner, endPosition);
                nextDamageTime += damageInterval; // Schedule the next damage application
            }

            yield return null; // Wait until the next frame
        }

        // Clean up instances when done
        Destroy(moonbeamInstance);
        Destroy(impactColliderInstance);
        Destroy(shootEffectInstance); // Destroy the shoot effect instance
    }

    private void ApplyDamage(GameObject owner, Vector3 position)
    {
        // Find all colliders within the effect range at the position
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, effectRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Boss"))
            {
                // Apply damage to each 'Boss' object found within the range
                entityHealth.DamageEntity(changeHealthAmount);

                // Play the splash effect
                PlaySplashEffectAtPosition(position);
            }
            else if (hitCollider.CompareTag("Pill"))
            {
                // Destroy the pill object
                Destroy(hitCollider.gameObject);
            }
        }
    }

    private void PlayShootEffect(GameObject owner, GameObject moonbeamInstance)
    {
        // Find the shoot effect ParticleSystem in the moonbeam instance
        ParticleSystem shootEffect = moonbeamInstance.transform.Find("shootEffect").GetComponent<ParticleSystem>();

        if (shootEffect != null)
        {
            // Set the shoot effect's position to the player's position
            shootEffect.transform.position = owner.transform.position;

            // Make the shoot effect a child of the player to follow them
            shootEffect.transform.SetParent(owner.transform);

            // Store the shoot effect instance
            shootEffectInstance = shootEffect.gameObject;

            // Ensure the shoot effect is pointing towards the target position
            UpdateShootEffectPositionAndRotation(owner, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // Play the shoot effect
            shootEffect.Play();
        }
        else
        {
            Debug.LogError("ShootEffect ParticleSystem not found in moonbeamPrefab.");
        }
    }

    private void UpdateShootEffectPositionAndRotation(GameObject owner, Vector3 targetPosition)
    {
        if (shootEffectInstance != null)
        {
            // Calculate the direction from the shoot effect to the target position
            Vector3 directionToTarget = (targetPosition - owner.transform.position).normalized;

            // Calculate the angle to rotate the shoot effect to face the target
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            // Apply the rotation to the shoot effect
            shootEffectInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Adjusted angle to correct for left curving

            // Update the shoot effect's position to the player's position
            shootEffectInstance.transform.position = owner.transform.position;
        }
    }

    private void PlaySplashEffectAtPosition(Vector3 position)
    {
        if (splashEffectInstance != null)
        {
            // Set the splash effect's position to the specified position
            splashEffectInstance.transform.position = position;

            // Play the splash effect
            splashEffectInstance.Play();
        }
    }
}
