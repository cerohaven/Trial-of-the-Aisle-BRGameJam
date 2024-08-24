using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    [SerializeField]private Animator anim; // Reference to the Animator on the separate "StateDrivenCamera" object
    [SerializeField]private bool BBCam = true; // Boss battle camera

    private void Awake()
    {
        // Initialize the Animator component from the "StateDrivenCamera"
        InitializeAnimator();
        GameManager.Instance.EventSender.switchCameraStateEvent.AddListener(SwitchState);
    }

    private void InitializeAnimator()
    {
        // Find the "State-Driven Camera" object in the scene
        GameObject stateDrivenCamera = GameObject.Find("State-Driven Camera");

        if (stateDrivenCamera != null)
        {
            anim = stateDrivenCamera.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("State-Driven Camera not found in the scene.");
        }
    }

    public void SwitchState()
    {
        if (BBCam)
        {
            anim.Play("PBCam");
        }
        else
        {
            anim.Play("BossBattleCam");
        }
        BBCam = !BBCam;
    }
}
