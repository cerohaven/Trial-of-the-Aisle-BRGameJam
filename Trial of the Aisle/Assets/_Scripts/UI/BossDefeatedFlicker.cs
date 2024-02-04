using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDefeatedFlicker : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;

    //Components
    private SpriteRenderer sprite;
    
    //Variables
    private Color[] color = { Color.white, Color.red, Color.black };


    //beginning flickering variables
    [SerializeField] private float flickerTime; //how long does each flicker last
    [SerializeField] private int maxFlickers; //the max amount of flickers we want to happen

    private int amountOfFlickers; //records how many flickers have happened
    private int j; //increasing the index of the color array


    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        //set the alpha value of each of these colors to 0.30
        for (int i = 0; i < color.Length; i++)
        {
            color[i].a = 0.30f;
        }

        bossDefeatedEventSender.flickerScreenEvent.AddListener(StartFlickers);
    }

    private void StartFlickers()
    {
        
        StartCoroutine(flickerRoutine());
    }

    IEnumerator flickerRoutine()
    {

        while(amountOfFlickers <= maxFlickers)
        {
            j++;
            j %= 3;
            sprite.color = color[j];

            amountOfFlickers++;
            CinemachineShake.Instance.ShakeCamera(10, 1);
            yield return new WaitForSeconds(flickerTime);
        }

        //resets colour back to normal
        sprite.color = new Color(1, 1, 1, 0);

        //if the flickers reached it's max, lower the volume of the battle music
        amountOfFlickers = 0;
    }
}
