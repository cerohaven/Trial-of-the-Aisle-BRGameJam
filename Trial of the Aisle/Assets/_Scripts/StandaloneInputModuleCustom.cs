using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class StandaloneInputModuleCustom : StandaloneInputModule
{
    public PointerEventData GetLastPointerEventDataPublic(int id)
    {
        return GetLastPointerEventData(id);
    }
}

