using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public RectTransform rectTransform;
    public Vector2 offset = new Vector2(10f, -10f); // Adjust the offset to be top-right

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = Input.mousePosition;

        // Adjust the position by the offset
        position += offset;

        // Calculate the pivot adjustments
        float pivotX = Mathf.Clamp01(position.x / Screen.width);
        float pivotY = Mathf.Clamp01(position.y / Screen.height);

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        // Clamp the position to make sure the tooltip stays within screen bounds
        float clampedX = Mathf.Clamp(position.x, rectTransform.rect.width * pivotX, Screen.width - rectTransform.rect.width * (1 - pivotX));
        float clampedY = Mathf.Clamp(position.y, rectTransform.rect.height * pivotY, Screen.height - rectTransform.rect.height * (1 - pivotY));

        rectTransform.position = new Vector2(clampedX, clampedY);
    }
}
