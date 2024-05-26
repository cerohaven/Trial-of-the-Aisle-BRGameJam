using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private Camera mainCamera;
    private RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector3 tooltipMin = corners[0]; // Bottom-left corner
        Vector3 tooltipMax = corners[2]; // Top-right corner

        Vector3 viewPos = rectTransform.position;

        float leftBound = mainCamera.ViewportToWorldPoint(Vector3.zero).x;
        float rightBound = mainCamera.ViewportToWorldPoint(Vector3.one).x;
        float bottomBound = mainCamera.ViewportToWorldPoint(Vector3.zero).y;
        float topBound = mainCamera.ViewportToWorldPoint(Vector3.one).y;

        viewPos.x = Mathf.Clamp(viewPos.x, leftBound + (rectTransform.position.x - tooltipMin.x), rightBound - (tooltipMax.x - rectTransform.position.x));
        viewPos.y = Mathf.Clamp(viewPos.y, bottomBound + (rectTransform.position.y - tooltipMin.y), topBound - (tooltipMax.y - rectTransform.position.y));

        rectTransform.position = viewPos;
    }
}
