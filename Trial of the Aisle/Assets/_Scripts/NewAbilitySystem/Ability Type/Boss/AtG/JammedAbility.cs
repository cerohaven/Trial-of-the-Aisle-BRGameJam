using UnityEngine;

[CreateAssetMenu(fileName = "SlowAbility", menuName = "Abilities/Alexander/Jammed Ability")]
public class JammedAbility : Ability
{
    public GameObject jamPrefab;
    public float jamThrowForce;

    public override void Activate(GameObject owner)
    {
        // Instantiate the jam at the owner's position
        GameObject jam = Instantiate(jamPrefab, owner.transform.position, Quaternion.identity);

        // Calculate the direction to throw the jam towards the mouse position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = owner.transform.position.z;
        Vector2 direction = (mouseWorldPosition - owner.transform.position).normalized;

        // Debug: Log direction and force
        Debug.Log("Direction: " + direction);
        Debug.Log("Force: " + (direction * jamThrowForce));

        // Apply force to the jam to throw it in the direction
        Rigidbody2D rb = jam.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * jamThrowForce, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogError("Rigidbody2D component is missing from jamPrefab");
        }

        // Initialize the JamBehavior component
        JamBehaviour jamBehavior = jam.GetComponent<JamBehaviour>();
        if (jamBehavior != null)
        {
            jamBehavior.Initialize(3); // Set the max number of bounces
        }
    }
}
