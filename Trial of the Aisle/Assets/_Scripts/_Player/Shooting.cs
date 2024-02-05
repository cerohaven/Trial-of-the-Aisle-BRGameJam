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
        if (ability != null && ability.CanUse) // Check if the ability is not null and can be used
        {
            Debug.Log($"Activating {ability.AbilityName}");
            ability.Activate(firePoint, ability.AbilityPrefab, ability.AbilityForce);
            StartCoroutine(AbilityCooldown(ability)); // Start the cooldown routine for this specific ability
        }
        else
        {
            if (ability == null)
                Debug.Log($"Ability {slotIndex + 1} is null.");
            else
                Debug.Log($"Ability {ability.AbilityName} is on cooldown.");
        }
    }

    private IEnumerator AbilityCooldown(IAbility ability)
    {
        ability.CanUse = false; // Set the ability's CanUse flag to false to start the cooldown
        yield return StartCoroutine(ability.CooldownRoutine()); // Wait for the cooldown routine to complete
        ability.CanUse = true; // Set the ability's CanUse flag back to true after the cooldown is complete
    }
}