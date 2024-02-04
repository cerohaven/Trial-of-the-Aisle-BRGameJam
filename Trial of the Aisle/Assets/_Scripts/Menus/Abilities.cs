using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // For Dictionary

public class Abilities : MonoBehaviour
{
    public Shooting shooting; // Reference to the Shooting script

    public Image abilityOneFill;
    public Image abilityTwoFill;
    public Image abilityThreeFill;

    private Dictionary<IAbility, float> lastAbilityUseTimes = new Dictionary<IAbility, float>();

    private void Start()
    {
        // Initialize the dictionary with current abilities
        lastAbilityUseTimes[shooting.abilityOne] = 0f;
        lastAbilityUseTimes[shooting.abilityTwo] = 0f;
        if (shooting.abilityThree != null)
        {
            lastAbilityUseTimes[shooting.abilityThree] = 0f;
        }

        // Subscribe to the AbilityUsed event
        shooting.AbilityUsed += OnAbilityUsed;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        shooting.AbilityUsed -= OnAbilityUsed;
    }

    private void Update()
    {
        UpdateAbilityFill(abilityOneFill, shooting.abilityOne);
        UpdateAbilityFill(abilityTwoFill, shooting.abilityTwo);
        if (shooting.abilityThree != null)
        {
            abilityThreeFill.gameObject.SetActive(true);
            UpdateAbilityFill(abilityThreeFill, shooting.abilityThree);
        }
        else
        {
            abilityThreeFill.gameObject.SetActive(false);
        }
    }

    void OnAbilityUsed(IAbility ability)
    {
        // Record the time when the ability was used
        lastAbilityUseTimes[ability] = Time.time;
    }

    void UpdateAbilityFill(Image fillImage, IAbility ability)
    {
        if (fillImage == null || ability == null) return;

        float timeSinceUsed = Time.time - lastAbilityUseTimes[ability];
        float cooldownProgress = Mathf.Clamp(timeSinceUsed / ability.Cooldown, 0f, 1f);

        fillImage.fillAmount = ability.CanUse ? 1f : 1f - cooldownProgress;
    }
}