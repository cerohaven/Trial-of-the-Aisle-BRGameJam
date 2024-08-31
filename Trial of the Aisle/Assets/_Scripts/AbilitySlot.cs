using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class AbilitySlot : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private GameObject ability;
    private GameObject storedAbility;

    [SerializeField]
    private int slotIndex;

    private NewAbilitySelectionUI newAbilitySelectionUI;

    private bool playAnim = true;
    
     public void OnPointerEnter(PointerEventData data)
     {
        
        // if (storedAbility == null)
        // {
        //     Debug.Log("AAAA");
        //     //set ability icon's position to the ability slot's position
        //     //if (GameManager.Instance.CurrentDragDrop.transform != null && GameManager.Instance.ControlScheme == ControlScheme.Mouse)
        //     //get pointerDrag
        //     //set pointerDrag Position to transform.position
        //     GameManager.Instance.CurrentDragDrop.transform.position = transform.position;

        //     storedAbility = GameManager.Instance.CurrentDragDrop;

        //     //Debug.Log(PlayerAbilities.Instance);

        //     //swap ability methods (called from other classes)
        //     PlayerAbilities.Instance.SwapAbility(slotIndex, GameManager.Instance.CurrentDragDrop.GetComponent<DragDrop>().ability);

        //     //Get component when the Post Battle Canvas Spawns in
        //     if(newAbilitySelectionUI == null) newAbilitySelectionUI = FindObjectOfType<NewAbilitySelectionUI>();

        //     newAbilitySelectionUI.swappedAbilities.Add(data.pointerDrag.GetComponent<DragDrop>().ability);
            
        //     //data.pointerDrag.GetComponent<DragDrop>().filled = true;
        //     //GameManager.Instance.ControlScheme = ControlScheme.Mouse;

        //     //scale the ability icon down and destroy it
        //     LeanTween.scale(storedAbility, data.pointerDrag.transform.localScale * 0.57f, 0.6f).setEaseInOutQuad().setDestroyOnComplete(true);

        //     GameManager.Instance.CurrentDragDrop = null;
        // }
    }

    private void Update()
    {
        if(!GameManager.Instance.CurrentDragDrop)
        {
            //print("CurrentDragDrop is null");
            return;
        }

        Vector3 dragDropWorldPos = Camera.main.ScreenToWorldPoint(GameManager.Instance.CurrentDragDrop.transform.position);
        Vector3 transformPos = Camera.main.ScreenToWorldPoint(transform.position);


        float distanceToDragged = (dragDropWorldPos - transformPos).magnitude;


        //print(distanceToDragged);
        //print(GameManager.Instance.dragging);



        if (storedAbility == null && !GameManager.Instance.dragging && distanceToDragged < 1f)
        {
            Debug.Log("AAAA");
            //set ability icon's position to the ability slot's position
            //if (GameManager.Instance.CurrentDragDrop.transform != null && GameManager.Instance.ControlScheme == ControlScheme.Mouse)
            
            GameManager.Instance.CurrentDragDrop.transform.position = transform.position;

            storedAbility = GameManager.Instance.CurrentDragDrop;

            //Debug.Log(PlayerAbilities.Instance);

            //swap ability methods (called from other classes)
            PlayerAbilities.Instance.SwapAbility(slotIndex, GameManager.Instance.CurrentDragDrop.GetComponent<DragDrop>().ability);

            //Get component when the Post Battle Canvas Spawns in
            if(newAbilitySelectionUI == null) newAbilitySelectionUI = FindObjectOfType<NewAbilitySelectionUI>();

            newAbilitySelectionUI.swappedAbilities.Add(GameManager.Instance.CurrentDragDrop.GetComponent<DragDrop>().ability);
            
            storedAbility.GetComponent<DragDrop>().filled = true;
            //GameManager.Instance.ControlScheme = ControlScheme.Mouse;

            //scale the ability icon down and destroy it
            playAnim = true;
            ScaleObject();
            //Destroy(storedAbility);

            GameManager.Instance.CurrentDragDrop = null;
            GameManager.Instance.dropped = false;
        }
    }

    private void ScaleObject()
    {
        if (playAnim)
        {
            LeanTween.scale(storedAbility, GameManager.Instance.CurrentDragDrop.transform.localScale * 0.57f, 0.6f).setEaseInOutQuad().setDestroyOnComplete(true);
            playAnim = false;
        }
    }

}
