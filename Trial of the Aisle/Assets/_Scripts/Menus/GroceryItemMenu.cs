using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryItemMenu : MonoBehaviour
{
    [SerializeField] private Sprite[] grocerySprites;
    private SpriteRenderer sr;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();    
    }
    void Start()
    {
        sr.sprite = grocerySprites[Random.Range(0, grocerySprites.Length)]; 
    }

}
