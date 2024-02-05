using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMaterialDemo : MonoBehaviour
{
    // Example of how to implement reference and call the method

    private SwapMaterials _sM;

    void Start()
    {
        _sM = GetComponentInChildren<SwapMaterials>();
    }
    public void Swap(int index)
    {
        _sM.Swap(index);
    }
   
}
