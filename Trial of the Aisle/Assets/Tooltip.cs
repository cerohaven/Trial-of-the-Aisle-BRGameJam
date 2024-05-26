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

        // Clamp the position to make sure the tooltip stays within screen bounds
        float clampedX = Mathf.Clamp(position.x, 0, Screen.width - rectTransform.rect.width);
        float clampedY = Mathf.Clamp(position.y, rectTransform.rect.height, Screen.height);

        rectTransform.position = new Vector2(clampedX, clampedY);
    }
}
