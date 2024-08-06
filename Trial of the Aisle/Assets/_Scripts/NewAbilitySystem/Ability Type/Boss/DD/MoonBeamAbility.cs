using UnityEngine;
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

    public SO_AdjustHealth adjustHealthSO; // The SO responsible for changing health
    public ChangeHealth changeHealthAmount; // Amount of damage to apply

    private GameObject shootEffectInstance; // Store a reference to the shoot effect instance

    public override void Activate(GameObject owner)
    {
        // Instantiate the moonbeam prefab
        GameObject moonbeamInstance = Instantiate(moonbeamPrefab, owner.transform.position, Quaternion.identity);

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

        owner.GetComponent<MonoBehaviour>().StartCoroutine(ActivateRaycastAbility(owner, lineRenderer, moonbeamInstance, impactColliderInstance));
    }

    private IEnumerator ActivateRaycastAbility(GameObject owner, LineRenderer lineRenderer, GameObject moonbeamInstance, GameObject impactColliderInstance)
    {
        float endTime = Time.time + abilityDuration;
        float nextDamageTime = Time.time;

        while (Time.time < endTime)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure it's in the 2D plane

            // Calculate the direction from the owner to the mouse position
            Vector3 direction = (mousePosition - owner.transform.position).normalized;

            // Shorten the beam tip by the specified offset
            Vector3 shortenedEndPosition = owner.transform.position + direction * beamOffset;

            // Update LineRenderer positions to swap start and end points
            lineRenderer.SetPosition(0, mousePosition); // Start at the mouse position
            lineRenderer.SetPosition(1, shortenedEndPosition); // End at the shortened position

            // Move the impact collider to follow the mouse position
            impactColliderInstance.transform.position = mousePosition;

            // Update the shoot effect's position and rotation
            UpdateShootEffectPositionAndRotation(owner, mousePosition);

            if (Time.time >= nextDamageTime)
            {
                ApplyDamage(owner, mousePosition);
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
        // Find all colliders within the effect range at the mouse position
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, effectRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Boss"))
            {
                // Apply damage to each 'Boss' object found within the range
                adjustHealthSO.ChangeBossHealthEventSend(changeHealthAmount, HealthType.Damage, Vector2.zero);

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

            // Ensure the shoot effect is pointing towards the mouse direction
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
        // Instantiate the moonbeam prefab to access the splash effect
        GameObject moonbeamInstance = Instantiate(moonbeamPrefab, position, Quaternion.identity);
        ParticleSystem splashEffect = moonbeamInstance.transform.Find("splashEffect").GetComponent<ParticleSystem>();

        if (splashEffect != null)
        {
            // Play the splash effect
            splashEffect.Play();

            // Destroy the splash effect instance after duration
            Destroy(moonbeamInstance, splashDuration);
        }
        else
        {
            Debug.LogError("SplashEffect ParticleSystem not found in moonbeamPrefab.");
            Destroy(moonbeamInstance);
        }
    }
}
