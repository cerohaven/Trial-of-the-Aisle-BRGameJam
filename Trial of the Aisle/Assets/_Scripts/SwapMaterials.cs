using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMaterials : MonoBehaviour
{
    private SpriteRenderer spriteRender;
    //array of materials used by this object
    public Material[] materials;

    public void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    //swap current material to the material from the array
    public void Swap(int materialIndex)
    {
        //swap to the specified material
        spriteRender.material = materials[materialIndex];
    }


}
