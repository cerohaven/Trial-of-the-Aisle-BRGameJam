using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JamBehaviour : MonoBehaviour
{
    [SerializeField] private float bounceAngle = 45f; // Angle to bounce off walls
    [SerializeField] private int maxBounces = 3; // Maximum number of bounces
    private int currentBounces = 0;
    private Rigidbody2D rb;
    private EntityHealth entityHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            // Handle damage to the boss
            entityHealth = collision.gameObject.GetComponent<EntityHealth>();
            if (entityHealth != null)
            {
                entityHealth.DamageEntity(ChangeHealth.Small_Health);
                Destroy(gameObject); // Destroy the jam if it hits the boss
                return;
            }
        }

        // Handle wall bouncing
        if (collision.collider.CompareTag("Wall"))
        {
            // Calculate bounce direction
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflection = Vector2.Reflect(rb.velocity, normal);
            rb.velocity = reflection.normalized * rb.velocity.magnitude; // Maintain current speed

            currentBounces++;
            if (currentBounces >= maxBounces)
            {
                Destroy(gameObject); // Destroy the jam after max bounces
            }
        }
    }

    public void Initialize(int maxBounces)
    {
        this.maxBounces = maxBounces;
    }
}
