using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HealingAbility", menuName = "Abilities/Support/Healing Ability")]
public class HealingAbility : Ability
{
    public ChangeHealth changeHealthAmount; // The enum value specifying the amount of health to adjust
    public GameObject healingEffectPrefab; // The prefab containing the healing particle effect

    public override void Activate(GameObject owner)
    {
        // Use the AdjustHealth SO to invoke the health adjustment event
        GameManager.Instance.EventSender.ChangePlayerHealthEventSend(changeHealthAmount, HealthType.Healing);

        // Find the player object by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

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
            ParticleSystem suckingEffect = particleSystems[0];
            ParticleSystem burstEffect = particleSystems[1];

            // Start the sucking effect
            suckingEffect.Play();

            // Start a coroutine to trigger the burst effect after the sucking effect duration
            MonoBehaviour coroutineRunner = owner.GetComponent<MonoBehaviour>();
            if (coroutineRunner == null)
            {
                coroutineRunner = owner.AddComponent<CoroutineRunner>();
            }
            coroutineRunner.StartCoroutine(TriggerBurst(suckingEffect, burstEffect, suckingEffect.main.duration));

            // Destroy the effect after its duration (assuming burst duration is 3 seconds)
            Destroy(effectInstance, suckingEffect.main.duration + 2f);
        }
    }

    private IEnumerator TriggerBurst(ParticleSystem suckingEffect, ParticleSystem burstEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        suckingEffect.Stop();
        burstEffect.Play();
    }

    // Helper MonoBehaviour class to run coroutines
    private class CoroutineRunner : MonoBehaviour { }
}
