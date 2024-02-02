using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractableObject : MonoBehaviour
{
    /// <summary>
    /// Do not put this script in any object, this is purely to hold methods that other scripts will inherit and use
    /// Holds basic methods and variables that every interactable object will have
    /// </summary>

    //References
    [SerializeField] protected SO_InteractableObject SO_interactableObject;
    [SerializeField] private SO_ControlSchemeHUD SO_controlSchemeHUD;
    protected PlayerInputHandler playerInputHandler;

    //Variables
    [Header("Variables")]
    [SerializeField] private GameObject interactPromptPanel; //The "Click E" panel
    [SerializeField] private Image interactButtonSprite;
    protected float tweenTime = 0.2f;

    protected bool inPlayerRange;
    private Transform objTransform; //used for the gizmo draw


    //Properties
    public bool InPlayerRange { get => inPlayerRange; }

    //Variables for each Interactable Object

    //Variables for customization

    [Tooltip("If the player is in the target's range, should we reveal any UI or not? Even if the UI is hidden the object will still be interactable")]
    [SerializeField] protected bool isInteractPointVisible = true;

    [Tooltip("If the object is interactable or not")]
    [SerializeField] protected bool isInteractable = true;


    /// <summary>
    /// In the "PlayerInteractWithObjects.cs" script, it checks if the player clicked the "E" button on Update
    /// If they did click the E Button, we send off an event through the "SO_InteractableObject.cs" scriptable Object
    /// And ALL interactable objects receive the event, however the listener calls a method that checks to see if THIS OBJECT 
    /// is the one the player wants to interact with (by seeing if it's UI is enabled) and if so, then we run the OnInteract method.
    /// </summary>

    virtual protected void Awake()
    {
        SO_interactableObject.clickedInteractButtonEvent.AddListener(CheckIfUTargetctive);
        SO_interactableObject.changedControlSchemeEvent.AddListener(UpdateSpriteAndText);
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private void Start()
    {
        
    }
    private void CheckIfUTargetctive()
    {
        if (!inPlayerRange) return;
        if (!IsInteractable()) return;

        OnInteract();

    }

    /// <summary>
    /// Event is called from the "PlayerInputHandler" whenever a new control scheme is detected and is called here
    /// This goes in the scriptable object and finds the corresponding sprite and text to apply to the interact prompt
    /// </summary>
    private void UpdateSpriteAndText(string _controlScheme)
    {
        if(interactButtonSprite == null) return;

        interactButtonSprite.sprite = SO_controlSchemeHUD.UpdateSpriteHUD(_controlScheme, SO_ControlSchemeHUD.SpriteType.Img_Interact);
    }


    /// <summary>
    /// Through the PlayerInteractScript.cs, the PlayerInRange() and PlayerOutOfRange() methods are called if the object is in range of the player
    /// </summary>

    #region Player In Range
    public virtual void PlayerInRange()
    {
        
        inPlayerRange = true;
        ShowUI();

    }


    public void SetInteractable(bool _interactable)
    {
        isInteractable = _interactable;
    }

    private void ShowUI()
    {
        if (!IsTargetPointVisible()) return;

        //The UI is only shown if the child class declares the "IsTargetPointVisible" bool method true.
        interactPromptPanel.SetActive(true);

        //Tween animation
        interactPromptPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(interactPromptPanel, Vector3.one, tweenTime);
    }

    #endregion

    #region Player Out of Range
    public virtual void PlayerOutOfRange()
    {
        inPlayerRange = false;

        //If the player walks out of the range of interaction and there's a HUD being displayed, hide that HUD
        //UIController.Instance.HideCurrentUI();

        if (IsTargetPointVisible())
        {
            HideUI();
        }
    }

    protected void HideUI()
    {
        //Play Animation
        interactPromptPanel.SetActive(false);
    }

    #endregion

    //abstract function so ALL scripts that inherit from InteractableObject require this function
    protected abstract void OnInteract();
    protected abstract bool IsTargetPointVisible(); //If we want the UI to appear or not. Like animal Crossing if false, where they can still interact, but no UI

    protected abstract bool IsInteractable(); //If true, then it's interactable, if false, then it's not
}