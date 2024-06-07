using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class BossSceneTransitionProperties
{
    public Texture2D _maskTexture2D;
    public Color _backgroundColour;
    public Color _itemsColour;

}

public class Transit_BossZoom : MonoBehaviour, ISceneTransition
{
    [SerializeField] private AnimationClip _enterSceneAnimationClip;
    [SerializeField] private AnimationClip _exitSceneAnimationClip;
    public AnimationClip EnterSceneAnimationClip { get { return _enterSceneAnimationClip; } }

    public AnimationClip ExitSceneAnimationClip { get { return _exitSceneAnimationClip; } }

    //References
    SceneTransitionController controller;

    //Components
    private Image image;

    //Variables
    [Space]
    [Header("Colour and Image for Boss Transitions")]
    [SerializeField] private BossSceneTransitionProperties painKillerTransitionProperties;
    [SerializeField] private BossSceneTransitionProperties alexanderTransitionProperties;
    [SerializeField] private BossSceneTransitionProperties dairyTransitionProperties;

    //Animation Variables
    [HideInInspector] public float maskSize;
    [HideInInspector] public float maskRotationSpeed;
    [HideInInspector] public Color backgroundColour;
    [HideInInspector] public Color itemsColour;
    [HideInInspector] public Texture2D maskTexture;

    private readonly int _maskSizeID = Shader.PropertyToID("_circleSize");
    private readonly int _maskRotationSpeedID = Shader.PropertyToID("_rotationSpeed");
    private readonly int _backgroundColourID = Shader.PropertyToID("_backgroundColour");
    private readonly int _itemsColourID = Shader.PropertyToID("_circlesColour");
    private readonly int _maskTextureID = Shader.PropertyToID("_alphaTexture");

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
    }

    private void OnEnable()
    {
        controller = SceneTransitionController.Instance;
    }
    private void Update()
    {
        image.materialForRendering.SetFloat(_maskSizeID, maskSize);
    }
    public void Initialize()
    {
        StartCoroutine(LerpColour());
    }

    //Called from the SceneTransitionController
    public IEnumerator LerpColour()
    {

        Color startColour = image.materialForRendering.GetColor(_backgroundColourID);
        Color endColour = GetColourToUse()._backgroundColour;
        Color currentColor;

        Color startColourItems = image.materialForRendering.GetColor(_itemsColourID);
        Color endColourItems = GetColourToUse()._itemsColour;
        Color currentColourItems;

        float currentTime = 0;

        //Change Sprite as well
        image.materialForRendering.SetTexture(_maskTextureID, GetColourToUse()._maskTexture2D);

        while (currentTime < 1)
        {
            currentTime += Time.deltaTime;
            currentColor = Color.Lerp(startColour, endColour, currentTime);
            currentColourItems = Color.Lerp(startColourItems, endColourItems, currentTime);
            image.materialForRendering.SetColor(_backgroundColourID, currentColor);
            image.materialForRendering.SetColor(_itemsColourID, currentColourItems);

            yield return null;
        }

        yield return null;
    }



    //Called from the Level Loader
    public BossSceneTransitionProperties GetColourToUse()
    {
        //find the boss and get the name of the boss
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "Boss_PainKiller":
                return painKillerTransitionProperties;

            case "Boss_AlexanderTheGrape":
                return alexanderTransitionProperties;

            case "Boss_DairyDominator":
                return dairyTransitionProperties;

            default:
                return painKillerTransitionProperties;
        }
    }

}
