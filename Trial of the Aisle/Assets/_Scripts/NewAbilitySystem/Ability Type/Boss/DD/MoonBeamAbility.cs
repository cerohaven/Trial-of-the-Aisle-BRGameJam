using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "RaycastAbility", menuName = "Abilities/General/Raycast Ability")]
public class RaycastAbility : Ability
{
    public GameObject moonbeamPrefab; // Reference to the Moonbeam prefab with a LineRenderer
    public float abilityDuration = 2f; // Duration of the ability's effect
    public float splashDuration = 1f; // Duration of the splash effect
    public float damageInterval = 0.5f; // Time between each damage tick
    public float effectRange = 2f; // Radius of the CircleCollider2D's effective area

    public SO_AdjustHealth adjustHealthSO; // The SO responsible for changing health
    public ChangeHealth changeHealthAmount; // Amount of damage to apply

    public override void Activate(GameObject owner)
    {
        GameObject moonbeamInstance = Instantiate(moonbeamPrefab, owner.transform.position, Quaternion.identity);
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

            // Update LineRenderer positions
            lineRenderer.SetPosition(0, owner.transform.position);
            lineRenderer.SetPosition(1, mousePosition);

            // Move the impact collider to follow the mouse position
            impactColliderInstance.transform.position = mousePosition;

            if (Time.time >= nextDamageTime)
            {
                ApplyDamage(owner, mousePosition);
                nextDamageTime += damageInterval; // Schedule the next damage application
            }

            yield return null; // Wait until the next frame
        }

        Destroy(moonbeamInstance); // Clean up the Moonbeam instance when done
        Destroy(impactColliderInstance); // Clean up the impact collider instance when done
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

                // Play the splash effect at the position
                PlaySplashEffectAtPosition(position, owner);
            }
            else if (hitCollider.CompareTag("Pill"))
            {
                // Destroy the pill object
                Destroy(hitCollider.gameObject);
            }
        }
    }

    private void PlaySplashEffectAtPosition(Vector3 position, GameObject owner)
    {
        // Instantiate the moonbeam prefab to access the splash effect
        GameObject moonbeamInstance = Instantiate(moonbeamPrefab, position, Quaternion.identity);
        ParticleSystem splashEffect = moonbeamInstance.GetComponentInChildren<ParticleSystem>();

        if (splashEffect != null)
        {
            // Calculate direction towards the player
            Vector3 directionToPlayer = (owner.transform.position - position).normalized;

            // Calculate the angle to rotate the splash effect to face the player
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            splashEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            splashEffect.Play();
            owner.GetComponent<MonoBehaviour>().StartCoroutine(DestroySplashEffectAfterDuration(moonbeamInstance, splashDuration));
        }
        else
        {
            Debug.LogError("No ParticleSystem found in moonbeamPrefab.");
            Destroy(moonbeamInstance);
        }
    }

    private IEnumerator DestroySplashEffectAfterDuration(GameObject splashInstance, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(splashInstance);
    }
}
