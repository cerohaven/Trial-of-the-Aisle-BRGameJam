using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "RaycastAbility", menuName = "Abilities/General/Raycast Ability")]
public class RaycastAbility : Ability
{
    public GameObject moonbeamPrefab; // Reference to the Moonbeam prefab with a LineRenderer
    public float abilityDuration = 2f; // Duration of the ability's effect
    public float damageInterval = 0.5f; // Time between each damage tick
    public float effectRange = 5f; // Radius of the CircleCollider2D's effective area
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
            circleCollider.radius = effectRange;
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
                ApplyDamage(mousePosition);
                nextDamageTime += damageInterval; // Schedule the next damage application
            }

            yield return null; // Wait until the next frame
        }

        Destroy(moonbeamInstance); // Clean up the Moonbeam instance when done
        Destroy(impactColliderInstance); // Clean up the impact collider instance when done
    }

    private void ApplyDamage(Vector3 position)
    {
        // Find all colliders within the effect range at the mouse position
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, effectRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Boss"))
            {
                // Apply damage to each 'Boss' object found within the range
                adjustHealthSO.ChangeBossHealthEventSend(changeHealthAmount, HealthType.Damage, Vector2.zero);
            }
        }
    }
}
