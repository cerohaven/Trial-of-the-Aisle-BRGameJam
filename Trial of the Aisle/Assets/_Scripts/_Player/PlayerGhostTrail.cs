using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerGhostTrail : MonoBehaviour
{
    [SerializeField, Tooltip("The ghosts color. The alpha is set here.")]
    protected Color ghostColor = new Color(1, 1, 1, 175f);

    [SerializeField, Tooltip("If Enabled, the ghosts will be drawn using only one color(the one above).")]
    protected bool useSingleColor = true;

    [SerializeField, Range(1, 10), Tooltip("The alpha value for the ghosts (1 to 10)")]
    protected float ghostAlpha = 5f;

    [SerializeField, Range(1, 10), Tooltip("The frequency of updates per second")]
    protected int updatesPerSecond = 4;

    [SerializeField, Range(1, 10), Tooltip("The number of Ghosts")]
    protected int ghostNumber = 4;

    [SerializeField, Tooltip("Shader used in the materials to draw the ghosts")]
    protected Shader ghostShader;

    [SerializeField, Tooltip("The maximum distance for the ghost to fade out completely")]
    protected float maxDistance = 10f;
   // public float B_BaseMoveSpeed { get => b_BaseMoveSpeed; set => b_BaseMoveSpeed = value; }
    public int GhostNumber { get => ghostNumber; set => ghostNumber = value;}
    protected SpriteRenderer spriteRenderer;

    protected string containerSuffix = "Ghosts";
    private readonly string SPRITE_GHOST_SHADER_NAME = "Custom/GhostShader";
    private readonly int SHADER_SINGLE_COLOR_PROPERTY = Shader.PropertyToID("_Color");
    private readonly int SHADER_USE_SINGLE_COLOR_PROPERTY = Shader.PropertyToID("_UseSingleColor");
    private readonly int SHADER_PLAYER_POS_PROPERTY = Shader.PropertyToID("_PlayerPos");
    private readonly int SHADER_MAX_DISTANCE_PROPERTY = Shader.PropertyToID("_MaxDistance");

    protected float updateTime = 0f;
    protected int ghostIndex = 0;
    protected float frequencyTime;
    protected Material ghostMaterial;
    protected SpriteRenderer[] ghostRenderers;
    private bool isInitialized = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        ghostMaterial = new Material(ghostShader ? ghostShader : Shader.Find(SPRITE_GHOST_SHADER_NAME));
        UpdateGhostMaterial();
    }

    private void Start()
    {
        frequencyTime = 1f / updatesPerSecond;
    }

    private void Update()
    {
        if (isInitialized)
        {
            updateTime += Time.deltaTime;

            if (updateTime >= frequencyTime)
            {
                updateTime = 0f;
                CreateGhost();
            }
        }
    }

    private void InitializeGhostRenderers()
    {
        ghostRenderers = new SpriteRenderer[ghostNumber];

        for (int i = 0; i < ghostNumber; i++)
        {
            GameObject ghostObject = new GameObject("Ghost");
            ghostObject.transform.SetParent(transform);
            ghostObject.transform.localPosition = Vector3.zero;
            ghostObject.transform.localRotation = Quaternion.identity;
            ghostObject.transform.localScale = Vector3.one;

            SpriteRenderer ghost = ghostObject.AddComponent<SpriteRenderer>();
            ghost.material = ghostMaterial;
            ghost.sortingOrder = spriteRenderer.sortingOrder - 1;
            ghostRenderers[i] = ghost;

            ghostObject.SetActive(false);
        }

        isInitialized = true;
    }

    public void CreateGhost()
    {
        GameObject ghostObject = new GameObject("Ghost");
        SpriteRenderer ghost = ghostObject.AddComponent<SpriteRenderer>();
        ghost.sprite = spriteRenderer.sprite;
        ghost.transform.position = spriteRenderer.transform.position;
        ghost.transform.rotation = spriteRenderer.transform.rotation;
        ghost.transform.localScale = spriteRenderer.transform.localScale;
        ghost.material = ghostMaterial;
        ghost.sortingOrder = spriteRenderer.sortingOrder - 1;

        ghost.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, ghostAlpha / 10f);
        ghost.material.SetFloat(SHADER_USE_SINGLE_COLOR_PROPERTY, useSingleColor ? 1.0f : 0.0f);
        ghost.material.SetFloat(SHADER_MAX_DISTANCE_PROPERTY, maxDistance);
        ghost.material.SetVector(SHADER_PLAYER_POS_PROPERTY, transform.position);

        StartCoroutine(FadeAndDestroyGhost(ghost, 0.25f));
    }

    private IEnumerator FadeAndDestroyGhost(SpriteRenderer ghost, float lifetime)
    {
        float elapsedTime = 0f;
        Color startColor = ghost.color;
        Color endColor = new Color(ghost.color.r, ghost.color.g, ghost.color.b, 0);

        while (elapsedTime < lifetime)
        {
            ghost.color = Color.Lerp(startColor, endColor, elapsedTime / lifetime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(ghost.gameObject);
    }

    public void OnDodge()
    {
        if (!isInitialized)
        {
            InitializeGhostRenderers();
        }
    }

    private void OnValidate()
    {
        UpdateGhostMaterial();
    }

    private void UpdateGhostMaterial()
    {
        if (ghostMaterial != null)
        {
            Color updatedColor = new Color(ghostColor.r, ghostColor.g, ghostColor.b, ghostAlpha / 10f);
            ghostMaterial.SetColor(SHADER_SINGLE_COLOR_PROPERTY, updatedColor);
            ghostMaterial.SetFloat(SHADER_USE_SINGLE_COLOR_PROPERTY, useSingleColor ? 1.0f : 0.0f);
            ghostMaterial.SetFloat(SHADER_MAX_DISTANCE_PROPERTY, maxDistance);
        }
    }
}