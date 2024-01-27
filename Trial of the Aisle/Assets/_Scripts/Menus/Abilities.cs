using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    public Shooting shootingScript; // Reference to the Shooting script
    public Image abilityOneFillImage; // UI Image for Ability One's fill
    public Image abilityTwoFillImage; // UI Image for Ability Two's fill

    private float abilityOneCooldownTime; // Time when Ability One was activated
    private float abilityTwoCooldownTime; // Time when Ability Two was activated

    private void Update()
    {
        // Update the fill amount based on the cooldown timer
        abilityOneFillImage.fillAmount = shootingScript.canUseAbilityOne ? 1 : CalculateFill(Time.time - abilityOneCooldownTime, shootingScript.abilityOneCooldown);
        abilityTwoFillImage.fillAmount = shootingScript.canUseAbilityTwo ? 1 : CalculateFill(Time.time - abilityTwoCooldownTime, shootingScript.abilityTwoCooldown);

        // Update the cooldown time when the ability is used
        if (!shootingScript.canUseAbilityOne)
        {
            abilityOneCooldownTime = Time.time;
        }
        if (!shootingScript.canUseAbilityTwo)
        {
            abilityTwoCooldownTime = Time.time;
        }
    }

    private float CalculateFill(float elapsedTime, float cooldown)
    {
        // Calculate the fill amount based on the elapsed time and the cooldown
        return Mathf.Clamp01(elapsedTime / cooldown);
    }
}
