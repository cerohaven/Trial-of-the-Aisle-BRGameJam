using UnityEngine;
using System.Linq;


public class PlayerInteractWithObjects : MonoBehaviour
{
    ///This will cast an overlap sphere around the player and check for any object that can be interacted with
    ///If there is an object in radius, it's interaction prompt will appear, if it's further away, it hides 


    //References
    [SerializeField] private SO_InteractableObject interactableObject;
    private PlayerCarryProjectile carryProjectile;
    private PlayerController pc;

    //Variables
    [Header("Interaction Variables")]
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactableMask;


    private Transform objTransform;

    private void Awake()
    {
        carryProjectile = GetComponentInChildren<PlayerCarryProjectile>();
        pc = GetComponent<PlayerController>();
        objTransform = transform;
    }

    private void Update()
    {

        if (pc.PlayerInput.actions["Interact"].WasPressedThisFrame())
        {
            //If the player is carrying an object and they press the 
            if(carryProjectile.IsCarryingObject)
            {
                //Throw the object instead
                interactableObject.LaunchProjectileButtonEventSend(gameObject);
            }
            else
            {
                interactableObject.ClickedInteractButtonEventSend(gameObject);
            }

        }
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


        // HIDE UI WHEN LEAVE RADIUS//

        foreach (Collider2D col in hideCollider)
        {
            //If they are outside the collider array, Hide the UI

            //Hide UI
            InteractableObject io = col.GetComponent<InteractableObject>();

            if (io.InPlayerRange)
                io.PlayerOutOfRange();

        }

        // REVEAL UI //
        if (collider.Length == 0) return;

        InteractableObject ios = collider[0].GetComponent<InteractableObject>();
        ios.PlayerInRange();




    }


    #endregion


    private void OnDrawGizmos()
    {
        //Displays the interaction radius around the player
        Gizmos.color = new Color(0, 255, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, interactRadius);


        //Displays object interaction showing and hiding UI
        Gizmos.color = new Color(0, 0, 25, 0.1f);
        Gizmos.DrawWireSphere(transform.position, interactRadius + 0.3f);


            
    }
}
