using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragDrop : MonoBehaviour,  IPointerDownHandler
{
    [SerializeField]
    private Canvas canvas;
    public UnityEngine.CanvasGroup canvasGroup;
    public int abilityIndex;
    public Ability ability;
    private NewAbilitySelectionUI abilitySelectionUI;
    private bool pressed;
    private bool isScaling;
    public bool filled;
    private Vector3 originalScale;
    private void Awake()
    {
        canvasGroup = GetComponent<UnityEngine.CanvasGroup>();
        canvas = canvas.GetComponent<Canvas>();
        abilitySelectionUI = FindObjectOfType<NewAbilitySelectionUI>();
    }

    private void Start()
    {
        switch (abilityIndex)
        {
            case 1:
                ability = abilitySelectionUI.abilityOne;
                break;

            case 2:
                ability = abilitySelectionUI.abilityTwo;
                break;
        }
        filled = false;
        originalScale = transform.localScale;
        isScaling = false;
    }

  public void OnPointerDown(PointerEventData data)
    {
        if (!filled)
        {
            pressed = !pressed;

            if (pressed && !isScaling)
            {
                isScaling = true;
                GameManager.Instance.CurrentDragDrop = this.gameObject;
                GameManager.Instance.dragging = true;
                LeanTween.scale(this.gameObject, originalScale * 0.8f, 0.5f).setEasePunch().setOnComplete(() => isScaling = false);
                Debug.Log("PRESSED");
            }
            else if (!pressed && !isScaling)
            {
                isScaling = true;
                GameManager.Instance.dragging = false;
                LeanTween.scale(this.gameObject, originalScale, 0.5f).setEasePunch().setOnComplete(() => isScaling = false);
                Debug.Log("LET GO");
            }
        }
    }
    

    private void Update()
    {
        if(pressed)
        {

            //GameManager.Instance.GamepadCursor.CursorMouse.WarpCursorPosition( GameManager.Instance.GamepadCursor.VirtualMouse.position.ReadValue());

             float padding = transform.GetComponent<RectTransform>().rect.width / 2;

            Vector2 position;
            //converts Mouse Screen Position to Local Rect Position to make it fit in the canvas. 
            if(GameManager.Instance.ControlScheme == ControlScheme.Gamepad)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, GameManager.Instance.GamepadCursor.VirtualMouse.position.ReadValue(), canvas.worldCamera, out position);

            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, GameManager.Instance.GamepadCursor.CursorMouse.position.ReadValue(), canvas.worldCamera, out position);
            }


            //constrain the icon within the border of the screen
            if (Input.mousePosition.x < 15)
            {
                position.x = -canvas.GetComponent<RectTransform>().rect.width / 2 + padding;
            }
            if (Input.mousePosition.x > Screen.width - 15)
            {
                position.x = canvas.GetComponent<RectTransform>().rect.width / 2 - padding;
            }
            if (Input.mousePosition.y < 15)
            {
                position.y = -canvas.GetComponent<RectTransform>().rect.height / 2 + padding;
            }
            if (Input.mousePosition.y > Screen.height - 15)
            {
                position.y = canvas.GetComponent<RectTransform>().rect.height / 2 - padding;
            }
            transform.position = canvas.transform.TransformPoint(position);
        }
        
    }
    // public void OnDrag(PointerEventData data)
    // {
    //     if (!filled)
    //     {
    //         float padding = transform.GetComponent<RectTransform>().rect.width / 2;

    //         Vector2 position;
    //         //converts Mouse Screen Position to Local Rect Position to make it fit in the canvas. 
    //         RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, data.position, canvas.worldCamera, out position);

    //         //constrain the icon within the border of the screen
    //         if (Input.mousePosition.x < 15)
    //         {
    //             position.x = -canvas.GetComponent<RectTransform>().rect.width / 2 + padding;
    //         }
    //         if (Input.mousePosition.x > Screen.width - 15)
    //         {
    //             position.x = canvas.GetComponent<RectTransform>().rect.width / 2 - padding;
    //         }
    //         if (Input.mousePosition.y < 15)
    //         {
    //             position.y = -canvas.GetComponent<RectTransform>().rect.height / 2 + padding;
    //         }
    //         if (Input.mousePosition.y > Screen.height - 15)
    //         {
    //             position.y = canvas.GetComponent<RectTransform>().rect.height / 2 - padding;
    //         }


    //         transform.position = canvas.transform.TransformPoint(position);
    //     }
    // }

    // public void OnEndDrag(PointerEventData data) 
    // {
    //     canvasGroup.blocksRaycasts = true;
    //     GameManager.Instance.dragging = false;
    //     pressed = false;
    // }

    

}
