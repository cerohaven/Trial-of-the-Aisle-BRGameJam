using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public PlayerHealthBar playerHealthBar;

    [Header("Ability Prefabs")]
    public GameObject bulletPrefab;
    public GameObject abilityOnePrefab;
    public GameObject abilityTwoPrefab;
    public GameObject abilityThreePrefab; // Healing effect prefab

    [Header("Forces")]
    public float bulletForce = 20f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference abilityOneAction;
    [SerializeField] private InputActionReference abilityTwoAction;
    [SerializeField] private InputActionReference abilityThreeAction; // Healing ability action

    public event Action OnShoot;

    [Header("Variables")]
    public bool canUseAbilityOne = true;
    public bool canUseAbilityTwo = true;
    public bool canUseAbilityThree = true; // Healing ability usage check

    public float abilityOneCooldown = 7f;
    public float abilityTwoCooldown = 10f;
    public float abilityThreeCooldown = 15f; // Healing ability cooldown
    public float healingAmount = 10f;

    private void start() 
    {
        playerHealthBar = FindObjectOfType<PlayerHealthBar>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.25f);
        shootAction.action.Enable();
        abilityOneAction.action.Enable();
        abilityTwoAction.action.Enable();
        abilityThreeAction.action.Enable(); // Enable healing ability action

        shootAction.action.performed += OnFirePerformed;
        abilityOneAction.action.performed += OnAbilityOnePerformed;
        abilityTwoAction.action.performed += OnAbilityTwoPerformed;
        abilityThreeAction.action.performed += OnAbilityThreePerformed; // Healing ability action performed
    }

    private void OnDisable()
    {
        shootAction.action.Disable();
        abilityOneAction.action.Disable();
        abilityTwoAction.action.Disable();
        abilityThreeAction.action.Disable(); // Disable healing ability action

        shootAction.action.performed -= OnFirePerformed;
        abilityOneAction.action.performed -= OnAbilityOnePerformed;
        abilityTwoAction.action.performed -= OnAbilityTwoPerformed;
        abilityThreeAction.action.performed -= OnAbilityThreePerformed; // Remove healing ability action performed
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (bulletPrefab != null)
        {
            Shoot(bulletPrefab);
        }
    }

    private void OnAbilityOnePerformed(InputAction.CallbackContext context)
    {
        if (canUseAbilityOne && abilityOnePrefab != null)
        {
            Debug.Log("Ability 1 activated");
            Shoot(abilityOnePrefab);
            canUseAbilityOne = false;
            StartCoroutine(AbilityOneCooldown());
        }
    }

    private void OnAbilityTwoPerformed(InputAction.CallbackContext context)
    {
        if (canUseAbilityTwo && abilityTwoPrefab != null)
        {
            Shoot(abilityTwoPrefab);
            canUseAbilityTwo = false;
            StartCoroutine(AbilityTwoCooldown());
        }
    }
    private void OnAbilityThreePerformed(InputAction.CallbackContext context)
    {
        if (canUseAbilityThree && abilityThreePrefab != null)
        {
            Debug.Log("Healing ability activated");

            // Instantiate the healing effect prefab at the player's position
            GameObject instantiatedPrefab = Instantiate(abilityThreePrefab, transform.position, Quaternion.identity);

            // Play the particle system if not set to Play on Awake
            ParticleSystem ps = instantiatedPrefab.GetComponentInChildren<ParticleSystem>();
            if (ps != null && !ps.main.playOnAwake)
            {
                ps.Play();
            }

            // Heal the player
            playerHealthBar.Heal(healingAmount);

            // Set the ability to cooldown
            canUseAbilityThree = false;
            StartCoroutine(AbilityThreeCooldown());
        }
    }


    public void Update()
    {
        firePoint.up = transform.up;
    }

    void Shoot(GameObject bulletType)
    {
        Debug.Log("Bullet fired");
        GameObject bullet = Instantiate(bulletType, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * bulletForce, ForceMode2D.Impulse);

        OnShoot?.Invoke();
    }



    IEnumerator AbilityOneCooldown()
    {
        yield return new WaitForSeconds(abilityOneCooldown);
        canUseAbilityOne = true;
    }

    IEnumerator AbilityTwoCooldown()
    {
        yield return new WaitForSeconds(abilityTwoCooldown);
        canUseAbilityTwo = true;
    }

    IEnumerator AbilityThreeCooldown()
    {
        yield return new WaitForSeconds(abilityThreeCooldown);
        canUseAbilityThree = true;
    }
}

