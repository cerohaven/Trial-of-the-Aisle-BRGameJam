using UnityEngine;

public class Abilities : MonoBehaviour
{
    // Reference to the Shooting script
    public Shooting shooting;

    // Variables for ability cooldown UI
    public UnityEngine.UI.Image abilityOneFill;
    public UnityEngine.UI.Image abilityTwoFill;

    private void Update()
    {
        shooting.Update(); // Call the Update method of the Shooting script

        // Update the fill amount based on the cooldown
        if (!shooting.canUseAbilityOne)
        {
            abilityOneFill.fillAmount -= 1.0f / shooting.abilityOneCooldown * Time.deltaTime;
            if (abilityOneFill.fillAmount <= 0)
            {
                shooting.canUseAbilityOne = true;
                abilityOneFill.fillAmount = 1;
            }
        }

        if (!shooting.canUseAbilityTwo)
        {
            abilityTwoFill.fillAmount -= 1.0f / shooting.abilityTwoCooldown * Time.deltaTime;
            if (abilityTwoFill.fillAmount <= 0)
            {
                shooting.canUseAbilityTwo = true;
                abilityTwoFill.fillAmount = 1;
            }
        }
    }
}


