using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private RectTransform abilityRectTransform;
    public RectTransform rectTransform;
    public Vector2 offset = new Vector2(10f, -10f); // Adjust the offset to be top-right

    private readonly float blackBarWidth = 150;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.localPosition = KeepFullyOnScreen(rectTransform.gameObject, rectTransform.localPosition);

    }

    Vector3 KeepFullyOnScreen(GameObject panel, Vector3 newPos)
    {
        //as we get closer to the end of the screen, push back the rect transform more 

        if (abilityRectTransform.position.x > Screen.width - rectTransform.rect.width/2)
        {
            newPos.x = Screen.width - abilityRectTransform.position.x * 2;
        }

        //if (abilityRectTransform.position.x > Screen.height - 15)
        //{
        //    newPos.y = -rectTransform.rect.height;
        //}

        return newPos;
    }
}
