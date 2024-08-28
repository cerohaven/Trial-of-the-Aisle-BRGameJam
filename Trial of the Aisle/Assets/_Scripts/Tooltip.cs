using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private RectTransform abilityRectTransform;
    public RectTransform rectTransform;
    private readonly float blackBorderWidth = 150;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        KeepFullyOnScreen(rectTransform.gameObject, rectTransform.localPosition);
        
    }

    void KeepFullyOnScreen(GameObject panel, Vector3 newPos)
    {
        //as we get closer to the end of the screen, push back the rect transform more 
        float max = Screen.width /2.5f;

        if (abilityRectTransform.position.x > max)
        {
            //rectTransform.position = new Vector3(max,rectTransform.position.y, rectTransform.position.z);
        }
        else
        {
            //rectTransform.localPosition = new Vector3(0, rectTransform.localPosition.y, rectTransform.localPosition.z);
        }

        float maxY = Screen.height / 1.8f;
        if(abilityRectTransform.position.y > maxY)
        {
            //rectTransform.position = new Vector3(rectTransform.position.x, maxY, rectTransform.position.z);
        }
        else
        {
            //rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, 0, rectTransform.localPosition.z);
        }
        //Get the current position - the max position to be the new local offset

        
        
       
    }
}
