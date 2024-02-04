using UnityEngine;
using System.Linq;


public class PlayerInteractWithObjects : MonoBehaviour
{
    ///This will cast an overlap sphere around the player and check for any object that can be interacted with
    ///If there is an object in radius, it's interaction prompt will appear, if it's further away, it hides 


    //References
    [SerializeField] private SO_InteractableObject interactableObject;
    private PlayerController pc;

    //Variables
    [Header("Interaction Variables")]
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactableMask;

    private Collider2D closestCollider = null;
    private Transform objTransform;

    private float closestDistance = Mathf.Infinity;
    private float currentClosestColliderDistance;

    
    private void Awake()
    {

        pc = GetComponent<PlayerController>();
        objTransform = transform;
    }

    private void Update()
    {
        if (pc.playerInput.actions["Interact"].WasPressedThisFrame())
            interactableObject.ClickedInteractButtonEventSend(gameObject);

        InteractObjects();
    }

    #region Interacting With Objects
    private void InteractObjects()
    {
        //overlap sphere around player, 1 for showing the UI, and the other to Hide it after it left the interaction radius
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableMask);
        Collider2D[] hideCollider = Physics2D.OverlapCircleAll(transform.position, interactRadius + 0.3f, interactableMask);

        //To save computational power, just don't do anything if there's nothing to detect
        if (collider.Length == 0 && hideCollider.Length == 0) return;

        //for each collider detected, check to see which one is closest and then show that UI
        foreach (Collider2D col in collider)
        {
            InteractableObject io = col.GetComponent<InteractableObject>();
            Vector3 colPos = col.transform.position;

            //if we have an object that is closest to the player
            Vector3 directionToTarget = colPos - objTransform.position;
            float distanceFromPlayer = directionToTarget.sqrMagnitude;

           

            // COLLIDER WITH CLOSEST DISTANCE //
            if (distanceFromPlayer < closestDistance || distanceFromPlayer < currentClosestColliderDistance)
            {

                if (col != closestCollider)
                {
                    closestCollider = col;
                    closestDistance = distanceFromPlayer;
                    io.PlayerInRange();

                }
            }

    
            //For any other collider, OR any collider that exits the interaction Radius, HIDE their UI
            if (col != closestCollider)
            {
                if (io.InPlayerRange)
                {
                    //Hide UI
                    io.PlayerOutOfRange();

                }

            }

            //Get the current closest collider's distance from the player, and if the distance is < than that, we switch it.
            //Without this variable, the closestDistance keeps getting smaller and smaller, and only resets when the current-
            //closest collider exits the interact radius
            else
            {
                currentClosestColliderDistance = distanceFromPlayer;

            }

        }

        // HIDE UI WHEN LEAVE RADIUS//

        foreach (Collider2D col in hideCollider)
        {
            //If they are outside the collider array, Hide the UI
            if (!collider.Contains(col))
            {
                //Hide UI
                InteractableObject io = col.GetComponent<InteractableObject>();

                if (io.InPlayerRange)
                    io.PlayerOutOfRange();

                //if it's the current closest collider, we have to reset the variables or else when we enter the radius again-
                //the UI won't show again since it's the same object
                if (col == closestCollider)
                {
                    closestDistance = Mathf.Infinity;
                    closestCollider = null;
                }

            }
        }

    }


    #endregion


    private void OnDrawGizmos()
    {
        //Displays the interaction radius around the player
        Gizmos.color = new Color(0, 255, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, interactRadius);

        //make closest collider white
        if (closestCollider != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(closestCollider.transform.position, 0.6f);
        }

        //Displays object interaction showing and hiding UI
        Gizmos.color = new Color(0, 0, 25, 0.1f);
        Gizmos.DrawWireSphere(transform.position, interactRadius + 0.3f);

        if(!Application.isEditor)
            Debug.DrawLine(objTransform.position, closestCollider.transform.position);
            
    }
}
