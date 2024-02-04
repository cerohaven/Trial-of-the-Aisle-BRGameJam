using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [Header("Ability Prefabs")]
    public GameObject bulletPrefab;
    public GameObject abilityOnePrefab;
    public GameObject abilityTwoPrefab;
    public GameObject abilityThreePrefab;

    [Header("Forces")]
    public float bulletForce = 20f;

    //public InputAction shootAction;
    public InputAction abilityOneAction;
    public InputAction abilityTwoAction;
    public InputAction abilityThreeAction;

    public Transform firePoint;
    public PlayerController playerController;
    public SO_AdjustHealth adjustHealth;
    [SerializeField] public ChangeHealth healAmount;

    public IAbility abilityOne;
    public IAbility abilityTwo;
    public IAbility abilityThree;

    // Define events
    public event Action<IAbility> AbilityUsed;
    public event Action<IAbility> AbilityCooldownCompleted;

    private Camera mainCamera;

    private void Awake()
    {
        firePoint = transform.Find("Firepoint");
        if (firePoint == null) Debug.LogError("FirePoint not found on the player");

        mainCamera = Camera.main;
        playerController = GetComponent<PlayerController>();
        var actionAsset = playerController.playerInput.actions;
        //shootAction = actionAsset.FindAction("Fire");
        abilityOneAction = actionAsset.FindAction("AbilityOne");
        abilityTwoAction = actionAsset.FindAction("AbilityTwo");
        abilityThreeAction = actionAsset.FindAction("AbilityThree");

        abilityOne = new HighVelocityShot();
        abilityTwo = new ExplosiveShot();
        abilityThree = null;  // Starts with no third ability
    }

    private void OnEnable()
    {
        //shootAction.performed += _ => Shoot(bulletPrefab);
        abilityOneAction.performed += _ => UseAbility(abilityOnePrefab, abilityOne);
        abilityTwoAction.performed += _ => UseAbility(abilityTwoPrefab, abilityTwo);
        abilityThreeAction.performed += _ => UseAbility(abilityThreePrefab, abilityThree);
    }

    private void OnDisable()
    {
        //shootAction.performed -= _ => Shoot(bulletPrefab);
        abilityOneAction.performed -= _ => UseAbility(abilityOnePrefab, abilityOne);
        abilityTwoAction.performed -= _ => UseAbility(abilityTwoPrefab, abilityTwo);
        abilityThreeAction.performed -= _ => UseAbility(abilityThreePrefab, abilityThree);
    }

    void Shoot(GameObject bulletType)
    {
        if (!playerController.CanMove || bulletType == null) return;

        // Instantiate the bullet at the firePoint
        GameObject bullet = Instantiate(bulletType, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Calculate direction towards the mouse cursor
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (mouseWorldPosition - (Vector2)firePoint.position).normalized;
        bullet.GetComponent<Bullet>().Direction = direction;
        // Apply force in the direction of the mouse cursor
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
    }


    void UseAbility(GameObject abilityPrefab, IAbility ability)
    {
        Debug.Log($"Attempting to use ability: {ability?.GetType().Name}"); // Log which ability is being attempted

        if (!playerController.CanMove || ability == null || !ability.CanUse)
        {
            Debug.Log($"Cannot use ability: {ability?.GetType().Name}");
            return;
        }

        Debug.Log($"Activating ability: {ability.GetType().Name}"); // Confirm activation
        ability.Activate(firePoint, abilityPrefab, bulletForce);
        AbilityUsed?.Invoke(ability); // Notify that the ability has been used
        StartCoroutine(AbilityCooldownRoutine(ability));
    }


    public void AssignAbilityThree(IAbility newAbility)
    {
        abilityThree = newAbility;
        // Optionally, update the UI or other game elements to reflect the new ability
    }

    IEnumerator AbilityCooldownRoutine(IAbility ability)
    {
        ability.CanUse = false;
        yield return new WaitForSeconds(ability.Cooldown);
        ability.CanUse = true;
        AbilityCooldownCompleted?.Invoke(ability); // Invoke the AbilityCooldownCompleted event
    }

}
