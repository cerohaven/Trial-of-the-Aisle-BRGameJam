using UnityEngine;

public class GrapeMinionBehavior : MonoBehaviour
{
    [SerializeField]
    public float speed = 5f;
    public ChangeHealth changeHealthAmount; // The enum representing the amount of health change
    public SO_AdjustHealth adjustHealthSO; // The SO responsible for changing health


    private GameObject target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Boss");
    }

    private void Update()
    {
        // Check if the target is assigned and active
        if (target != null && target.activeInHierarchy)
        {
            // Move towards the boss
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pill"))
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            adjustHealthSO.ChangeBossHealthEventSend(changeHealthAmount, HealthType.Damage, Vector2.zero);
            Destroy(gameObject); 
        }
    }
}
