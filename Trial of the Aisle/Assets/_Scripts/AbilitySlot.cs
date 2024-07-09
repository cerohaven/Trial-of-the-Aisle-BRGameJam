using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class AbilitySlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private NewAbilitySelectionUI newAbilitySelectionUI;
    [SerializeField]
    private GameObject ability;
    [SerializeField]
    private GameObject storedAbility;
    [SerializeField]
    private int slotIndex;

    private void Awake()
    {
        newAbilitySelectionUI = GetComponentInParent<NewAbilitySelectionUI>();
    }

    public void OnDrop(PointerEventData data)
    {
       if (data.pointerDrag != null && storedAbility == null)
        {
            //set ability icon's position to the ability slot's position
            data.pointerDrag.transform.position = transform.position;
            storedAbility = data.pointerDrag.gameObject;
            
            //swap ability methods (called from other classes)
            PlayerAbilities.Instance.SwapAbility(slotIndex, data.pointerDrag.GetComponent<DragDrop>().ability);

            newAbilitySelectionUI.swappedAbilities.Add(data.pointerDrag.GetComponent<DragDrop>().ability);
            
            data.pointerDrag.GetComponent<DragDrop>().filled = true;

            //scale the ability icon down and destroy it
            LeanTween.scale(storedAbility, data.pointerDrag.transform.localScale * 0.57f, 0.6f).setEaseInOutQuad().setDestroyOnComplete(true);
        }
    }

}
