using System.Collections;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance; // Singleton instance
    private CinemachineVirtualCamera[] virtualCameras;
    private Dictionary<string, CinemachineBasicMultiChannelPerlin> noiseSettings;

    private void Awake()
    {
        instance = this;
        virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        noiseSettings = new Dictionary<string, CinemachineBasicMultiChannelPerlin>();

        foreach (var vcam in virtualCameras)
        {
            var noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                noiseSettings.Add(vcam.Name, noise);
            }
        }
    }

    // Method to perform screen shake
    public void Shake(float duration, float strength)
    {
        StartCoroutine(ShakeCoroutine(duration, strength));
    }

    private IEnumerator ShakeCoroutine(float duration, float strength)
    {
        foreach (var noise in noiseSettings.Values)
        {
            noise.m_AmplitudeGain = strength;
        }

        yield return new WaitForSeconds(duration);

        foreach (var noise in noiseSettings.Values)
        {
            noise.m_AmplitudeGain = 0f;
        }
    }
}
