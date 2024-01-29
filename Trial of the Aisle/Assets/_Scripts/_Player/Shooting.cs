using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Shooting : MonoBehaviour
{
    public Transform firePoint;

    [Header("Ability Prefabs")]
    public GameObject bulletPrefab;
    public GameObject abilityOnePrefab;
    public GameObject abilityTwoPrefab;

    [Header("Forces")]
    public float bulletForce = 20f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference abilityOneAction;
    [SerializeField] private InputActionReference abilityTwoAction;

    public event Action OnShoot;

    [Header("Variables")]
    public bool canUseAbilityOne = true; // Made public
    public bool canUseAbilityTwo = true; // Made public

    public float abilityOneCooldown = 7f; // Changed to public instance variable
    public float abilityTwoCooldown = 10f; // Changed to public instance variable

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.25f);
        shootAction.action.Enable();
        abilityOneAction.action.Enable();
        abilityTwoAction.action.Enable();

        shootAction.action.performed += OnFirePerformed;
        abilityOneAction.action.performed += OnAbilityOnePerformed;
        abilityTwoAction.action.performed += OnAbilityTwoPerformed;
    }

    private void OnDisable()
    {
        shootAction.action.Disable();
        abilityOneAction.action.Disable();
        abilityTwoAction.action.Disable();

        shootAction.action.performed -= OnFirePerformed;
        abilityOneAction.action.performed -= OnAbilityOnePerformed;
        abilityTwoAction.action.performed -= OnAbilityTwoPerformed;
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

    private void Update()
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
}
