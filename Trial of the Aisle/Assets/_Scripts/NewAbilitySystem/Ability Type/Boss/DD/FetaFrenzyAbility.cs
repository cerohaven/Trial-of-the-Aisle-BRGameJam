using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Add this for the new Input System

[CreateAssetMenu(fileName = "GattlingGunAbility", menuName = "Abilities/DD/Feta Frenzy")]
public class FetaFrenzy : Ability
{
    public GameObject cheesePrefab; // The cheese projectile prefab
    public float throwInterval = 0.2f; // Interval between throws
    public int numberOfCheeses = 5; // Total number of cheeses to throw
    public float cheeseSpeed = 5f; // Speed of the cheese projectiles
    ControlScheme controlScheme;

    public override void Activate(GameObject owner)
    {
        // Get the current control scheme
        controlScheme = GameManager.Instance.ControlScheme;
        owner.GetComponent<MonoBehaviour>().StartCoroutine(ThrowCheeseSequence(owner));
    }

    private IEnumerator ThrowCheeseSequence(GameObject owner)
    {
        for (int i = 0; i < numberOfCheeses; i++)
        {
            Vector2 dir;
            Quaternion rotation;
            
            if (controlScheme == ControlScheme.Gamepad)
            {
                // Gamepad control
                Vector3 gamepadPosition = GameManager.Instance.GamepadCursor.VirtualMouse.position.ReadValue();
                Vector3 gamepadWorldPosition = Camera.main.ScreenToWorldPoint(gamepadPosition);
                dir = gamepadWorldPosition - owner.transform.position;

                dir = (gamepadWorldPosition - owner.transform.position).normalized;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Adjust to match direction
            }
            else
            {
                // Default to mouse position if the control scheme is not Gamepad
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = owner.transform.position.z;
                dir = mouseWorldPosition - owner.transform.position;

                dir = (mouseWorldPosition - owner.transform.position).normalized;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Adjust to match direction
            }

            // Instantiate the cheese prefab with the calculated rotation
            GameObject cheese = Instantiate(cheesePrefab, owner.transform.position, rotation);
            Rigidbody2D rb = cheese.GetComponent<Rigidbody2D>();

            // Set the cheese's velocity to make it move in the calculated direction
            rb.velocity = dir * cheeseSpeed;

            // Wait for the specified throw interval before instantiating the next cheese projectile
            yield return new WaitForSeconds(throwInterval);
        }
    }
}
