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

        // Apply force to the jam to throw it in the direction
        Rigidbody2D rb = jam.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * jamThrowForce, ForceMode2D.Impulse);

        //add the logic for the jam to slow down the boss when it comes in contact (Unimplemented)
    }
}