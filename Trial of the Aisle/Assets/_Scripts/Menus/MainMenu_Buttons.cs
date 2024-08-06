using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu_Buttons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.1f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, Vector3.one, 0.1f);
    }
}
