using System;
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
    public AbilityManager abilityManager; // Reference to the AbilityManager

    [Header("Input Actions")]
    public InputAction shootAction;
    private InputAction abilityOneAction;
    private InputAction abilityTwoAction;
    private InputAction abilityThreeAction;

    private Camera mainCamera;

    public event Action<IAbility> AbilityUsed; // You might need to adjust how this is used
    public event Action<IAbility> AbilityCooldownCompleted; // And this as well, depending on your implementation

    private void Awake()
    {
        firePoint = transform.Find("Firepoint");
        if (firePoint == null) Debug.LogError("FirePoint not found on the player");

        mainCamera = Camera.main;
        playerController = GetComponent<PlayerController>();

        var actionAsset = playerController.playerInput.actions;
        shootAction = actionAsset.FindAction("Fire");

        // Get ability actions from the AbilityManager's PlayerInput component
        abilityOneAction = abilityManager.playerInput.actions["AbilityOne"];
        abilityTwoAction = abilityManager.playerInput.actions["AbilityTwo"];
        abilityThreeAction = abilityManager.playerInput.actions["AbilityThree"];
    }

    private void OnEnable()
    {
        shootAction.performed += _ => Shoot(bulletPrefab);

        // Use AbilityManager to handle abilities
        abilityOneAction.performed += _ => UseAbility(0);
        abilityTwoAction.performed += _ => UseAbility(1);
        abilityThreeAction.performed += _ => UseAbility(2);
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => Shoot(bulletPrefab);

        // Unsubscribe from the ability actions
        abilityOneAction.performed -= _ => UseAbility(0);
        abilityTwoAction.performed -= _ => UseAbility(1);
        abilityThreeAction.performed -= _ => UseAbility(2);
    }

    private void Shoot(GameObject bulletType)
    {
        if (!playerController.CanMove || bulletType == null) return;

        GameObject bullet = Instantiate(bulletType, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (mouseWorldPosition - (Vector2)firePoint.position).normalized;

        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
    }

    private void UseAbility(int slotIndex)
    {
        var ability = abilityManager.GetEquippedAbility(slotIndex);
        if (ability != null && ability.CanUse)
        {
            ability.Activate(firePoint, ability.AbilityPrefab, bulletForce);
            StartCoroutine(ability.CooldownRoutine());
            AbilityUsed?.Invoke(ability); // Notify that the ability has been used
        }
    }

    // CooldownRoutine might be handled within each ability or the AbilityManager, depending on your design
}
