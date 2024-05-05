using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private Canvas canvas;
    private RectTransform rectTransform;
    private UnityEngine.CanvasGroup canvasGroup;

    private HashSet<Ability> swappedAbilities = new HashSet<Ability>();


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<UnityEngine.CanvasGroup>();
        canvas = canvas.GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData data) 
    { 
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData data)
    {
        float padding = transform.GetComponent<RectTransform>().rect.width/2;

        Vector2 position;
        //converts Mouse Screen Position to Local Rect Position to make it fit in the canvas. 
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, data.position, canvas.worldCamera, out position);
        
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

    public void OnEndDrag(PointerEventData data) 
    {
        canvasGroup.blocksRaycasts = true;

    }

    private void Update()
    {
        Debug.Log(Input.mousePosition.x);
    }

}
