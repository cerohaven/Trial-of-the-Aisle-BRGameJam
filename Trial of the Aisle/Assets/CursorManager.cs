using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D reticleCursor;

    void Start()
    {
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetReticleCursor()
    {
        Cursor.SetCursor(reticleCursor, Vector2.zero, CursorMode.Auto);
    }
}
