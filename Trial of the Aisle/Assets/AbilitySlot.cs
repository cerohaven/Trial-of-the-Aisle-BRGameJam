using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IDropHandler
{
    NewAbilitySelectionUI abilitySelection;


    private void Awake()
    {
        abilitySelection = FindObjectOfType<NewAbilitySelectionUI>();
    }

    public void OnDrop(PointerEventData data)
    {
       // data.pointerDrag.transform.position = transform.position;
       data.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
       
    }
}
