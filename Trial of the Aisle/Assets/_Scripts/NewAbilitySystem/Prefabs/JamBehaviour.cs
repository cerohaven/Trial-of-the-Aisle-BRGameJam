using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    private void OnTriggerEnter2D(Collider2D collision)
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
        if (collision.gameObject.CompareTag("Walls"))
        {
            LayerMask mask = LayerMask.GetMask("Wall");
            RaycastHit2D raycastHitUp = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.up * Mathf.Sign(rb.velocity.y), mask);
            RaycastHit2D raycastHitRight = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.right * Mathf.Sign(rb.velocity.x), mask);

            if (raycastHitUp)
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
            if (raycastHitRight)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }

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
