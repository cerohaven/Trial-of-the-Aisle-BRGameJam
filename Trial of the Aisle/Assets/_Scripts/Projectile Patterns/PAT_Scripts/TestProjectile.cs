using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectilePatterns;

public class TestProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public void Initialize(Vector2 travelDir, float speed, float lifetimeDuration)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = travelDir * speed;

        
        if(lifetimeDuration > 0)
            Invoke(nameof(DestroyObj), lifetimeDuration);
    }

    void DestroyObj()
    {
        Destroy(gameObject);
    }
}
