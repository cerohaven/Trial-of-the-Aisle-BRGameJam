using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [Header("General")]
    public Transform firePoint;
    public PlayerController playerController;
    public AbilityManager abilityManager; // Reference to the Ability Manager

    [Header("Input Actions")]
    private InputAction abilityOneAction;
    private InputAction abilityTwoAction;
    private InputAction abilityThreeAction;

    private void Awake()
    {
        InitializeFirePoint();
        InitializeInputActions();
    }

    private void InitializeFirePoint()
    {
        firePoint = transform.Find("Firepoint");
        if (firePoint == null) Debug.LogError("FirePoint not found on the player");
    }

    private void InitializeInputActions()
    {
        var actionAsset = playerController.playerInput.actions;

        abilityOneAction = actionAsset.FindAction("AbilityOne");
        abilityTwoAction = actionAsset.FindAction("AbilityTwo");
        abilityThreeAction = actionAsset.FindAction("AbilityThree");

        if (abilityOneAction == null || abilityTwoAction == null || abilityThreeAction == null)
        {
            Debug.LogError("One or more Ability Actions are not found in the Input Action Asset.");
        }
    }

    private void OnEnable()
    {
        abilityOneAction.performed += _ => UseAbility(0);
        abilityTwoAction.performed += _ => UseAbility(1);
        abilityThreeAction.performed += _ => UseAbility(2);
    }

    private void OnDisable()
       {
        abilityOneAction.performed -= _ => UseAbility(0);
        abilityTwoAction.performed -= _ => UseAbility(1);
        abilityThreeAction.performed -= _ => UseAbility(2);
    }

    private void UseAbility(int slotIndex)
    {
        IAbility ability = abilityManager.GetEquippedAbility(slotIndex);

        if (ability != null)
        {
            if (ability.CanUse)
            {
                Debug.Log($"Activating {ability.AbilityName}");

                // Get the prefab from the AbilityPrefabManager
                GameObject prefab = AbilityPrefabManager.Instance.GetAbilityPrefab(ability);

                if (prefab != null)
                {
                    ability.Activate(firePoint, ability.AbilityForce);
                    // The cooldown is now managed within each ability
                }
                else
                {
                    Debug.LogError($"Prefab for {ability.AbilityName} is missing.");
                }
            }
            else
            {
                Debug.Log($"Ability {ability.AbilityName} is on cooldown.");
            }
        }
        else
        {
            Debug.Log($"Ability slot {slotIndex + 1} is empty.");
        }
    }


    private IEnumerator AbilityCooldown(IAbility ability)
    {
        ability.CanUse = false; // Set the ability's CanUse flag to false to start the cooldown
        yield return StartCoroutine(ability.CooldownRoutine()); // Wait for the cooldown routine to complete
        Debug.Log($"{ability.AbilityName} is ready to use again."); // Log when the ability is ready again
        ability.CanUse = true; // Set the ability's CanUse flag back to true after the cooldown is complete
    }

}
