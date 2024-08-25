using UnityEngine;

public class GrapeMinionBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackDistance = 2f; // Distance threshold to trigger attack animation
    public ChangeHealth changeHealthAmount; 
    private EntityHealth entityHealth;
    private GameObject target;
    private Animator animator;
    private bool isAttackingBoss = false;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Boss");
        entityHealth = target.GetComponent<EntityHealth>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (target != null && target.activeInHierarchy)
        {
            // Move towards the boss
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // Check distance to boss
            float distanceToBoss = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToBoss <= attackDistance && !isAttackingBoss)
            {
                // Set attacking animation boolean
                isAttackingBoss = true;
                animator.SetBool("isAttackingBoss", true);
                Invoke(nameof(StopAttackAnimation), 5f); // Stop animation after 2 seconds
            }
        }
    }

    private void StopAttackAnimation()
    {
        animator.SetBool("isAttackingBoss", false);
        Destroy(gameObject); // Destroy grape minion after animation ends
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pill"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            entityHealth.DamageEntity(changeHealthAmount);
            Destroy(gameObject);
        }
    }
}
