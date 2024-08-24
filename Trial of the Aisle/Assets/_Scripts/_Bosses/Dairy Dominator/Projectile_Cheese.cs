using UnityEngine;

public class Projectile_Cheese : MonoBehaviour
{
    [SerializeField] private ChangeHealth damageDealt;
    [SerializeField] private GameObject particleHitEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityHealth entityHealth = collision.gameObject.GetComponent<EntityHealth>();


        if (collision.gameObject.CompareTag("Player")) 
        {
            entityHealth.DamageEntity(damageDealt);
            
            InstantiateHitParticles();
            Destroy(gameObject);
            return;
        }
           
    }
    protected void InstantiateHitParticles()
    {
        Instantiate(particleHitEffect, transform.position, Quaternion.identity);
    }
}
