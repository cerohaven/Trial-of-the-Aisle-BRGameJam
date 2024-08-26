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

    [SerializeField]
    private GameObject storedAbility;

    [SerializeField]
    private int slotIndex;

    private NewAbilitySelectionUI newAbilitySelectionUI;
    
     public void OnPointerEnter(PointerEventData data)
     {
        Debug.Log("Waaaah");
        if (storedAbility == null)
        {
            Debug.Log("AAAA");
            //set ability icon's position to the ability slot's position
            data.pointerDrag.transform.position = transform.position;
            storedAbility = data.pointerDrag.gameObject;
            Debug.Log(PlayerAbilities.Instance);
            //swap ability methods (called from other classes)
            PlayerAbilities.Instance.SwapAbility(slotIndex, GameManager.Instance.CurrentDragDrop.ability);

            //Get component when the Post Battle Canvas Spawns in
            if(newAbilitySelectionUI == null) newAbilitySelectionUI = FindObjectOfType<NewAbilitySelectionUI>();

            newAbilitySelectionUI.swappedAbilities.Add(data.pointerDrag.GetComponent<DragDrop>().ability);
            
            data.pointerDrag.GetComponent<DragDrop>().filled = true;

            //scale the ability icon down and destroy it
            LeanTween.scale(storedAbility, data.pointerDrag.transform.localScale * 0.57f, 0.6f).setEaseInOutQuad().setDestroyOnComplete(true);
        }
    }

    public void OnDrop(PointerEventData data)
    {
        
       
    }

}
