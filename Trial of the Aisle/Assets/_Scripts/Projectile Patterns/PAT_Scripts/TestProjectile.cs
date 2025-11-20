using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectilePatterns;

public class TestProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public void Initialize(Vector2 travelDir, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = travelDir * speed;
        Invoke(nameof(DestroyObj), 3);
    }

    void DestroyObj()
    {
        Destroy(gameObject);
    }
}
