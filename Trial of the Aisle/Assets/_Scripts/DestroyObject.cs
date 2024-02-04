using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public void DestroyObjectEvent()
    {
        Destroy(gameObject);
    }

    public void DestroyParentObjectEvent()
    {
        Destroy(transform.parent.gameObject);
    }
}
