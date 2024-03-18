using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "GattlingGunAbility", menuName = "Abilities/DD/Feta Frenzy")]
public class FetaFrenzy : Ability
{
    public GameObject cheesePrefab; // The cheese projectile prefab
    public float throwInterval = 0.2f; // Interval between throws
    public int numberOfCheeses = 5; // Total number of cheeses to throw
    public float cheeseSpeed = 5f; // Speed of the cheese projectiles

    public override void Activate(GameObject owner)
    {
        owner.GetComponent<MonoBehaviour>().StartCoroutine(ThrowCheeseSequence(owner));
    }

    private IEnumerator ThrowCheeseSequence(GameObject owner)
    {
        for (int i = 0; i < numberOfCheeses; i++)
        {
            // Convert the mouse position from screen coordinates to world coordinates
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Ensure the z-coordinate is the same as the owner's to keep the cheese in the correct plane
            mouseWorldPosition.z = owner.transform.position.z;

            // Calculate the direction from the owner to the mouse position
            Vector3 throwDirection = (mouseWorldPosition - owner.transform.position).normalized;

            // Instantiate the cheese prefab and get its Rigidbody2D component
            GameObject cheese = Instantiate(cheesePrefab, owner.transform.position, Quaternion.identity);
            Rigidbody2D rb = cheese.GetComponent<Rigidbody2D>();

            // Set the cheese's velocity to make it move in the calculated direction
            rb.velocity = throwDirection * cheeseSpeed;

            // Wait for the specified throw interval before instantiating the next cheese projectile
            yield return new WaitForSeconds(throwInterval);
        }
    }
}
