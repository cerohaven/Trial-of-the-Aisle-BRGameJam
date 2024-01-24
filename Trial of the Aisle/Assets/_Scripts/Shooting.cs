using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    [SerializeField] private InputActionReference shootAction;

    public event Action OnShoot; // Event to handle shooting

    private void OnEnable()
    {
        shootAction.action.Enable();
        shootAction.action.performed += OnFirePerformed;
    }

    private void OnDisable()
    {
        shootAction.action.Disable();
        shootAction.action.performed -= OnFirePerformed;
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (bulletPrefab != null)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Bullet fired");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        // Invoke the OnShoot event when shooting.
        OnShoot?.Invoke();
    }
}