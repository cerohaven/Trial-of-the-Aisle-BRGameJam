using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerGhostTrail : MonoBehaviour
{
    [SerializeField, Tooltip("The ghosts color. The alpha is set here.")]
    protected Color ghostColor = new Color(1, 1, 1, 0.5f);

    [SerializeField, Tooltip("If Enabled, the ghosts will be drawn using only one color (the one above).")]
    protected bool useSingleColor = true;

    [SerializeField, Range(0, 1), Tooltip("The alpha value for the ghosts (0 to 1)")]
    protected float ghostAlpha;

    [SerializeField, Range(1, 10), Tooltip("The frequency of updates per second")]
    protected int updatesPerSecond = 4;

    [SerializeField, Range(1, 10), Tooltip("The number of Ghosts")]
    protected int ghostNumber = 4;

    [SerializeField, Tooltip("Shader used in the materials to draw the ghosts")]
    protected Shader ghostShader;

    [SerializeField, Tooltip("The maximum distance for the ghost to fade out completely")]
    protected float maxDistance = 10f;

    protected SpriteRenderer spriteRenderer;

    // Suffix for naming ghost containers
    protected string containerSuffix = "Ghosts";

    // Constants for shader property names
    private readonly string SPRITE_GHOST_SHADER_NAME = "Custom/GhostShader";
    private readonly int SHADER_SINGLE_COLOR_PROPERTY = Shader.PropertyToID("_Color");
    private readonly int SHADER_USE_SINGLE_COLOR_PROPERTY = Shader.PropertyToID("_UseSingleColor");
    private readonly int SHADER_PLAYER_POS_PROPERTY = Shader.PropertyToID("_PlayerPos");
    private readonly int SHADER_MAX_DISTANCE_PROPERTY = Shader.PropertyToID("_MaxDistance");

    // Timing and indexing variables
    protected float updateTime = 0f;
    protected int ghostIndex = 0;
    protected float frequencyTime;

    // References to material and renderers
    protected Material ghostMaterial;
    protected SpriteRenderer[] ghostRenderers;
    private bool isInitialized = false;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Get the sprite renderer component attached to this game object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create a new material instance using the ghost shader
        ghostMaterial = new Material(ghostShader ? ghostShader : Shader.Find(SPRITE_GHOST_SHADER_NAME));
        UpdateGhostMaterial();
    }

    // Called before the first frame update
    private void Start()
    {
        // Calculate the time interval between updates based on updates per second
        frequencyTime = 1f / updatesPerSecond;
    }

    // Called once per frame
    private void Update()
    {
        // Check if the ghost effect has been initialized
        if (isInitialized)
        {
            updateTime += Time.deltaTime;

            // Create a ghost if the update time exceeds the frequency time
            if (updateTime >= frequencyTime)
            {
                updateTime = 0f;
                CreateGhost();
            }
        }
    }

    // Initialize the array of ghost sprite renderers
    private void InitializeGhostRenderers()
    {
        ghostRenderers = new SpriteRenderer[ghostNumber];

        for (int i = 0; i < ghostNumber; i++)
        {
            // Create a new game object for each ghost
            GameObject ghostObject = new GameObject("Ghost");
            ghostObject.transform.SetParent(transform);
            ghostObject.transform.localPosition = Vector3.zero;
            ghostObject.transform.localRotation = Quaternion.identity;
            ghostObject.transform.localScale = Vector3.one;

            // Add a sprite renderer to the ghost and configure its properties
            SpriteRenderer ghost = ghostObject.AddComponent<SpriteRenderer>();
            ghost.material = ghostMaterial;
            ghost.sortingOrder = spriteRenderer.sortingOrder - 1;
            ghostRenderers[i] = ghost;

            // Deactivate the ghost object initially
            ghostObject.SetActive(false);
        }

        isInitialized = true;
    }

    // Create a new ghost sprite at the current position
    public void CreateGhost()
    {
        // Create a new game object for the ghost
        GameObject ghostObject = new GameObject("Ghost");
        SpriteRenderer ghost = ghostObject.AddComponent<SpriteRenderer>();
        ghost.sprite = spriteRenderer.sprite;
        ghost.transform.position = spriteRenderer.transform.position;
        ghost.transform.rotation = spriteRenderer.transform.rotation;
        ghost.transform.localScale = spriteRenderer.transform.localScale;
        ghost.material = ghostMaterial;
        ghost.sortingOrder = spriteRenderer.sortingOrder - 1;

        // Clamp the alpha value between 0 and 1
        float clampedAlpha = Mathf.Clamp(ghostAlpha, 0, 1);

        // Set ghost color and shader properties
        ghost.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, clampedAlpha);
        ghost.material.SetFloat(SHADER_USE_SINGLE_COLOR_PROPERTY, useSingleColor ? 1.0f : 0.0f);
        ghost.material.SetFloat(SHADER_MAX_DISTANCE_PROPERTY, maxDistance);
        ghost.material.SetVector(SHADER_PLAYER_POS_PROPERTY, transform.position);

        // Start fading and destroying the ghost
        StartCoroutine(FadeAndDestroyGhost(ghost, 0.25f));
    }

    // Coroutine to fade and destroy the ghost sprite
    private IEnumerator FadeAndDestroyGhost(SpriteRenderer ghost, float lifetime)
    {
        float elapsedTime = 0f;
        Color startColor = ghost.color;
        Color endColor = new Color(ghost.color.r, ghost.color.g, ghost.color.b, 0);

        while (elapsedTime < lifetime)
        {
            // Lerp the color to create a fading effect
            ghost.color = Color.Lerp(startColor, endColor, elapsedTime / lifetime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the ghost object after fading
        Destroy(ghost.gameObject);
    }

    // Called when the player dodges to initialize ghost renderers
    public void OnDodge()
    {
        if (!isInitialized)
        {
            InitializeGhostRenderers();
        }
    }

    // Called when the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        UpdateGhostMaterial();
    }

    // Update the properties of the ghost material
    private void UpdateGhostMaterial()
    {
        if (ghostMaterial != null)
        {
            // Clamp the alpha value between 0 and 1
            float clampedAlpha = Mathf.Clamp(ghostAlpha, 0, 1);

            // Update color and shader properties
            Color updatedColor = new Color(ghostColor.r, ghostColor.g, ghostColor.b, clampedAlpha);
            ghostMaterial.SetColor(SHADER_SINGLE_COLOR_PROPERTY, updatedColor);
            ghostMaterial.SetFloat(SHADER_USE_SINGLE_COLOR_PROPERTY, useSingleColor ? 1.0f : 0.0f);
            ghostMaterial.SetFloat(SHADER_MAX_DISTANCE_PROPERTY, maxDistance);
        }
    }
}
