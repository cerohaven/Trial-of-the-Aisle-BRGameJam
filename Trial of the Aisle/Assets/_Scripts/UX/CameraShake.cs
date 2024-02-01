using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    public PlayerController playerController;
    [SerializeField]private float ShakeIntensity = .5f;
    [SerializeField]private float shakeTime = 0.2f;

    private float timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        StopShake();
    }

    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        playerController = FindObjectOfType<PlayerController>();
        _cbmcp.m_AmplitudeGain = ShakeIntensity;

        timer = shakeTime;

    }

    void StopShake() 
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;
        timer = 0f;
    }

    void Update()
    {
        if (playerController.isDodging)
        {
            ShakeCamera();
        }

        if (timer > 0) { 
            timer -= Time.deltaTime;
            if (timer < 0) {
                StopShake();
            }
        
        }
    }

}
