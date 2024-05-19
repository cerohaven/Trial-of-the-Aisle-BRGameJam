using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialsButton : MonoBehaviour
{
    public void OpenSocialMedia(string URL)
    {
        Application.OpenURL(URL);
    }
}
