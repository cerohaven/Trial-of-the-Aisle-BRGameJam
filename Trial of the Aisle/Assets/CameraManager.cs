using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    [SerializeField]private Animator anim; // Reference to the Animator on the separate "StateDrivenCamera" object
    [SerializeField]private bool BBCam = true; // Boss battle camera

    // Transforms to keep track of player and boss
    private Transform playerTransform;
    private Transform bossTransform;

    private void Awake()
    {
        // Ensure this GameObject persists across scene loads
        DontDestroyOnLoad(gameObject);

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize player and boss references
        FindPlayerAndBoss();

        // Initialize the Animator component from the "StateDrivenCamera"
        InitializeAnimator();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update player and boss references when a new scene is loaded
        FindPlayerAndBoss();

        // Re-initialize the Animator component from the "StateDrivenCamera"
        InitializeAnimator();
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

    private void FindPlayerAndBoss()
    {
        // Find the player and boss GameObjects by tag and update their transforms
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found with tag 'Player'");
        }

        if (boss != null)
        {
            bossTransform = boss.transform;
        }
        else
        {
            Debug.LogError("Boss GameObject not found with tag 'Boss'");
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
