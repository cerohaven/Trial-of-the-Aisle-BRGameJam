using UnityEngine;

public class BeamFollowMouse : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform

    private void Update()
    {
        if (playerTransform == null) return;

        // Update the beam's position to the current mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = playerTransform.position.z; // Ensure it's on the same plane as the player
        transform.position = mousePosition;

        // Update the beam's rotation to point towards the player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90); // Adjust angle based on your beam's orientation

        // Adjust the beam's scale to stretch from the mouse position back to the player
        // This will depend on how your beam is visually represented
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        Vector3 newScale = transform.localScale;
        newScale.y = distance; // Assuming the beam's length is scaled along the y-axis; adjust accordingly
        transform.localScale = newScale;
    }
}
