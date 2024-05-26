using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField]
    private Canvas canvas;
    private UnityEngine.CanvasGroup canvasGroup;
    public int abilityIndex;
    public Ability ability;
    private NewAbilitySelectionUI abilitySelectionUI;

    public bool filled;
    private bool isScaling;

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
        isScaling = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!filled && !isScaling)
        {
            isScaling = true;
            LeanTween.scale(this.gameObject, transform.localScale * 0.8f, 0.5f).setEasePunch().setOnComplete(() =>
            {
                isScaling = false;
            });
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (!filled)
        {
            canvasGroup.blocksRaycasts = false;
            GameManager.Instance.dragging = true;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (!filled)
        {
            float padding = transform.GetComponent<RectTransform>().rect.width / 2;

            Vector2 position;
            // Converts Mouse Screen Position to Local Rect Position to make it fit in the canvas. 
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, data.position, canvas.worldCamera, out position);

            // Constrain the icon within the border of the screen
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

    public void OnEndDrag(PointerEventData data)
    {
        canvasGroup.blocksRaycasts = true;
        GameManager.Instance.dragging = false;
    }
}
