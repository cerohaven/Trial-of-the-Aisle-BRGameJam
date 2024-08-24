using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HealingAbility", menuName = "Abilities/Support/Healing Ability")]
public class HealingAbility : Ability
{
    public GameObject healingEffectPrefab; // The prefab containing the healing particle effect
    private EntityHealth entityHealth;

    public override void Activate(GameObject owner)
    {
        // Find the player object by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Get Player health
        entityHealth = player.GetComponent<EntityHealth>();

        // Heal the player
        entityHealth.HealUnit(ChangeHealth.Medium_Health);

        // Apply the heal effect shader to the player
        if (player != null)
        {
            PlayerHealEffect healEffect = player.GetComponent<PlayerHealEffect>();
            if (healEffect != null)
            {
                healEffect.ApplyHealEffect();
            }
            else
            {
                Debug.LogWarning("PlayerHealEffect component not found on " + player.name);
            }
        }
        else
        {
            Debug.LogWarning("Player object not found. Ensure the player has the 'Player' tag.");
        }

        // Instantiate the healing effect prefab at the owner's position
        if (healingEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(healingEffectPrefab, owner.transform.position, Quaternion.identity);
            // Optionally, make the effect a child of the owner to keep the scene hierarchy organized
            effectInstance.transform.SetParent(owner.transform);

            // Get references to the particle systems
            ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>();
            ParticleSystem suckingEffect = particleSystems.Length > 0 ? particleSystems[0] : null;
            ParticleSystem burstEffect = particleSystems.Length > 1 ? particleSystems[1] : null;

            // Start the sucking effect if it exists
            if (suckingEffect != null)
            {
                suckingEffect.Play();

                // Start a coroutine to trigger the burst effect after the sucking effect duration if burst effect exists
                if (burstEffect != null)
                {
                    MonoBehaviour coroutineRunner = owner.GetComponent<MonoBehaviour>();
                    if (coroutineRunner == null)
                    {
                        coroutineRunner = owner.AddComponent<CoroutineRunner>();
                    }
                    coroutineRunner.StartCoroutine(TriggerBurst(suckingEffect, burstEffect, suckingEffect.main.duration));
                }

                // Destroy the effect after its duration (assuming burst duration is 2 seconds)
                Destroy(effectInstance, suckingEffect.main.duration + 2f);
            }
        }
    }

    private IEnumerator TriggerBurst(ParticleSystem suckingEffect, ParticleSystem burstEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (suckingEffect != null) suckingEffect.Stop();
        if (burstEffect != null) burstEffect.Play();
    }

    // Helper MonoBehaviour class to run coroutines
    private class CoroutineRunner : MonoBehaviour { }
}
