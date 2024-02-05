using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMaterialDemo : MonoBehaviour
{
    // Example of how to implement reference and call the method

    private SwapMaterials _sM;

    void Start()
    {
        _sM = GetComponent<SwapMaterials>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            _sM.Swap(1);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            _sM.Swap(0);
        }
    }
}
