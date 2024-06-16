using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D reticleCursor;
    [SerializeField]private float WaitTime = 1f;

    private static CursorManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        StartCoroutine(DelayedSetReticleCursor());
    }

    private IEnumerator DelayedSetReticleCursor()
    {
        yield return new WaitForSeconds(WaitTime);
        Cursor.SetCursor(reticleCursor, Vector2.zero, CursorMode.Auto);
    }
}
