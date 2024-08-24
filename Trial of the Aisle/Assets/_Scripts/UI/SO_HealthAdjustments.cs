using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health Adjustments", menuName = "Scriptable Objects/Health Adjustments")]
public class SO_HealthAdjustments : ScriptableObject
{

    [Separator()]
    public int xSmallHealthAdjustment;
    public int smallHealthAdjustment;
    public int mediumHealthAdjustment;
    public int largeHealthAdjustment;
    public int xLargeHealthAdjustment;

    

}
