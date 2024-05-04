using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private bool BBCam = true; //boss battle cam


    public void SwitchState()
    {
        if (BBCam)
        {
            anim.Play("PBCam");
        }
        else {
            {
                anim.Play("BossBattleCam");
            }
        }
        BBCam =!BBCam;
    }
}
