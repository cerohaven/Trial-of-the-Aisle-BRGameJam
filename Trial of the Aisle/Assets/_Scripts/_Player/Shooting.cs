using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [Header("General")]
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
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
        GameObject prefabToUse = null;

        // Ensure the AbilityPrefabManager is available
        if (AbilityPrefabManager.Instance == null)
        {
            Debug.LogError("AbilityPrefabManager instance not found.");
            return;
        }

        // Determine which prefab to use based on the ability
        if (ability is HighVelocityShot)
        {
            prefabToUse = AbilityPrefabManager.Instance.highVelocityShotPrefab;
        }
        // Add else-if blocks for other abilities as needed

        if (ability != null && prefabToUse != null)
        {
            if (ability.CanUse)
            {
                Debug.Log($"Activating {ability.AbilityName}");
                ability.Activate(firePoint, ability.AbilityForce);
                StartCoroutine(AbilityCooldown(ability)); // Start the cooldown routine through a separate method
            }
            else
            {
                Debug.Log($"Ability {ability.AbilityName} is on cooldown.");
            }
        }
        else
        {
            Debug.Log($"Ability slot {slotIndex + 1} is empty or prefab is missing.");
        }
    }




    private IEnumerator AbilityCooldown(IAbility ability)
    {
        ability.CanUse = false; // Set the ability's CanUse flag to false to start the cooldown
        yield return StartCoroutine(ability.CooldownRoutine()); // Wait for the cooldown routine to complete
        ability.CanUse = true; // Set the ability's CanUse flag back to true after the cooldown is complete
        Debug.Log($"{ability.AbilityName} is ready to use again."); // Log when the ability is ready again
    }
}
