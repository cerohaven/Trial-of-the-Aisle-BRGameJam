using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using FMODUnity;
using System.Collections;

public enum SAMPLEFROMBOSSTYPE
{
    FALSE,
    TRUE
}

public class EntityHealth : MonoBehaviour
{
    //Stores the max health, current health, and phases if the enemy has.

    //Fields
    [SerializeField] private SAMPLEFROMBOSSTYPE _sampleFromBoss;

    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _currentPhase = 0;

    [SerializeField] private List<UnitHealthPhases> _unitHealthPhases = new List<UnitHealthPhases>();

    [SerializeField] private Image _healthBar;

    [SerializeField] private SO_EntityHealthEventBase _deathEvent;

    [SerializeField] private bool _showHealthGraphic;

    [SerializeField] private string _hurtSFXEventName;
    [SerializeField] private GameObject _hurtEffectPrefab;

    [SerializeField] private string _healSFXEventName;
    [SerializeField] private GameObject _healEffectPrefab;

    //Invincibility Stuff
    [SerializeField] private bool _invincibleAfterDamage;
    [SerializeField] [Range(1,10)] private int _amountOfInvincibilityFlickers;
    [SerializeField][Range(0,1)] private float _invincibilityDelayBetweenFlickers = 0.1f;


    //Hurt Colour Stuff
    [SerializeField] private bool _hurtFlickerAfterDamage;
    [SerializeField][Range(1, 10)] private int _amountOfHurtFlickers;
    [SerializeField][Range(0, 1)] private float _hurtDelayBetweenFlickers = 0.1f;

    private bool _isInvincible = false; // determines if the entity should take damage or not

    private SO_HealthAdjustments healthAdjustments;
    private SpriteRenderer _spriteRenderer; //For changing the sprite's colours when we're invincible or get hit
    private Color colour = Color.white;

    FMOD.Studio.EventInstance UI_EntityHurt;
    FMOD.Studio.EventInstance UI_EntityHeal;

    //Properties
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHealth { get => _maxHealth; }
    public List<UnitHealthPhases> UnitHealthPhases { get => _unitHealthPhases; set => _unitHealthPhases = value; }
    public int CurrentPhase { get => _currentPhase; }
    public Image HealthBar { get => _healthBar; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        healthAdjustments = GameManager.Instance.HealthAdjustments;

        if (!_hurtSFXEventName.Equals(""))
        {
            UI_EntityHurt = RuntimeManager.CreateInstance(_hurtSFXEventName);
        }
        if (!_healSFXEventName.Equals(""))
        {
            UI_EntityHeal = RuntimeManager.CreateInstance(_healSFXEventName);
        }
        
    }

    private void Start()
    {
        SampleValuesFromBoss();

        //Initialize Events for the boss phases
        for (int i = 0; i < _unitHealthPhases.Count; i++)
        {
            if (_unitHealthPhases[i].unitPhaseEvent == null) continue;

            _unitHealthPhases[i].unitPhaseEvent.Initialize(gameObject);
        }
    }

    /// <summary>
    /// If we're sampling health values from the Boss Profile, fill in the fields
    /// </summary>
    private void SampleValuesFromBoss()
    {
        if (_sampleFromBoss == SAMPLEFROMBOSSTYPE.FALSE) return;

        SO_BossProfile bossProfile = GetComponent<InitializeBoss>().ThisBossProfile;

        if (bossProfile == null) { Debug.LogError($"No Boss Profile Attached to this {gameObject.name} Game Object"); return; }

        _maxHealth = bossProfile.B_MaxHealth;
        _currentHealth = _maxHealth;
        _deathEvent = bossProfile.B_BossDeathEvent;

        //Now Add to the List of Health Phases
        _unitHealthPhases.Clear();

        for(int i = 0; i < bossProfile.B_BossPhases.Length; i++)
        {
            _unitHealthPhases.Add(new UnitHealthPhases());
        }
        

        for (int i = 0; i < _unitHealthPhases.Count; i++)
        {

            _unitHealthPhases[i].phaseHealthPercent = bossProfile.B_BossPhases[i].healthPercent;
            _unitHealthPhases[i].unitPhaseEvent = bossProfile.B_BossPhases[i].phaseEvent;
        }

        //Set the fill bar to 0 to prepare for the intro animation
        _healthBar.fillAmount = 0;

    }

    /// <summary>
    /// Heal the unit this script is attached to
    /// </summary>
    public void HealUnit(ChangeHealth _changeHealth)
    {
        if (_currentHealth >= _maxHealth) return;

        int healAmount = GetHealthValue(_changeHealth, HealthType.Healing);

        //heal the unit and make sure their health can't go over their maxHealth
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        Debug.Log($"{gameObject.name} healed {healAmount} HP");

        UpdateHealthBar();

        if(!UI_EntityHurt.Equals(null))
        {
            UI_EntityHeal.start();
        }
        


        if (_healEffectPrefab)
        {
            Instantiate(_healEffectPrefab, transform.position, Quaternion.identity);
        }



        //Now update the current unit's phase based on the new health
        if (_unitHealthPhases.Count != 0) _currentPhase = SetCurrentHealthPhase();
    }


    /// <summary>
    /// Deal Damage to the unit this script is attached to
    /// </summary>
    public void DamageEntity(ChangeHealth _changeHealth)
    {
        if (_currentHealth <= 0) return;
        if (_isInvincible) return;

        int damageAmount = GetHealthValue(_changeHealth, HealthType.Damage);

        //reduce health and make sure we can't go into the negatives
        _currentHealth += damageAmount;
        Mathf.Clamp(_currentHealth, 0, _maxHealth);

        Debug.Log($"{gameObject.name} took {damageAmount} damage");

        UpdateHealthBar();

        UI_EntityHurt.start();


        if (_currentHealth <= 0)
        {
            PerformDeathLogic();
        }

        if(_invincibleAfterDamage)
        {
            StartCoroutine(InvincibleCoroutine());
        }

        if(_hurtFlickerAfterDamage && gameObject.activeSelf == true)
        {
            StartCoroutine(BossColourFlicker());
        }

        if(_hurtEffectPrefab)
        {
            Instantiate(_hurtEffectPrefab, transform.position, Quaternion.identity);
        }
        
        

        //Now update the current unit's phase based on the new health
        if (_unitHealthPhases.Count != 0) _currentPhase = SetCurrentHealthPhase();
    }



    /// <summary>
    /// This turns the input parameters of the health into a number that is sent to the Player or Boss Health Bar
    /// </summary>
    private int GetHealthValue(ChangeHealth _changeHealthState, HealthType _healthType)
    {
        int healthToReturn = 0;
        int healthType = 1;

        if (_healthType == HealthType.Damage)
        {
            healthType = -1;
        }
        else
        {
            healthType = 1;
        }

        switch (_changeHealthState)
        {
            case ChangeHealth.X_Small_Health:
                healthToReturn = healthAdjustments.xSmallHealthAdjustment * healthType;
                break;
            case ChangeHealth.Small_Health:
                healthToReturn = healthAdjustments.smallHealthAdjustment * healthType;
                break;
            case ChangeHealth.Medium_Health:
                healthToReturn = healthAdjustments.mediumHealthAdjustment * healthType;
                break;
            case ChangeHealth.Large_Health:
                healthToReturn = healthAdjustments.largeHealthAdjustment * healthType;
                break;
            case ChangeHealth.X_Large_Health:
                healthToReturn = healthAdjustments.xLargeHealthAdjustment * healthType;
                break;



        }
        return healthToReturn;

    }



    /// <summary>
    /// Mainly for bosses, get the current phase the unit is at if it has any
    /// </summary>
    private int SetCurrentHealthPhase()
    {
        //Get percent health from max health
        float healthPercent = ((float)_currentHealth / (float)_maxHealth * 100.00f);

        //For loop to see if it is > the current increment or not
        for (int i = _unitHealthPhases.Count - 1; i >= 0; i--)
        {

            //eg. if the current health is 65%, then we want to check if <25, then <50, then <75, then <100

            if (healthPercent > _unitHealthPhases[i].phaseHealthPercent)
                continue;

            //Debug.Log($"Paseed Phase {(i+1)}");
            //Play the special Health Phase event if this phase has one and it hasn't already played
            if (_unitHealthPhases[i].unitPhaseEvent != null)
            {
                if(!_unitHealthPhases[i].unitPhaseEvent.AlreadyPerformedFunction)
                {
                    _unitHealthPhases[i].unitPhaseEvent.Initialize(gameObject);
                    _unitHealthPhases[i].unitPhaseEvent.StartEventMethod();
                }
                
            }

            return i + 1;
        }

        return 0;
    }



    /// <summary>
    /// This code can now be universally shared with any unit we put the script on. We can code custom defeated logic in a separate
    /// Scriptable Object
    /// </summary>
    private void PerformDeathLogic()
    {
        if (_deathEvent != null)
        {
            _deathEvent.Initialize(gameObject);
        }
        else
        {
            //Default to just destroying the unit if we haven't coded any extra defeated logic.
            DestroyObject();
        }

    }


    #region Invincibility Frames 

    private void Invincible()
    {
        colour.a++;
        colour.a %= 2;
        _spriteRenderer.color = new Color(1, 1, 1, colour.a);
    }

    private IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;

        for(int i = 0; i < _amountOfInvincibilityFlickers; i++)
        {
            Invincible();
            yield return new WaitForSeconds(_invincibilityDelayBetweenFlickers);
        }

        colour.a = 1;
        _spriteRenderer.color = new Color(1, 1, 1, colour.a);
        _isInvincible = false;
    }

    #endregion

    #region Hurt Change Colour Frames 

    IEnumerator BossColourFlicker()
    {
        for (int i = 0; i < _amountOfHurtFlickers; i++)
        {
            colour.r = 1;
            colour.g = 0;
            colour.b = 0;
            _spriteRenderer.color = new Color(colour.r, colour.g, colour.b, 1);

            yield return new WaitForSeconds(_hurtDelayBetweenFlickers * 0.5f);

            colour.r = 1;
            colour.g = 1;
            colour.b = 1;
            _spriteRenderer.color = new Color(colour.r, colour.g, colour.b, 1);
            yield return new WaitForSeconds(_hurtDelayBetweenFlickers * 0.5f);
        }

        _spriteRenderer.color = Color.white;
        yield return null;
    }

    #endregion


    #region Updating Health Bar UI Fill
    private void UpdateHealthBar()
    {
        
        if (_healthBar != null)
        {
            _healthBar.fillAmount = 1.00f / ((float)_maxHealth / (float)_currentHealth);
        }
    }

    /// <summary>
    /// Plays when we first load into the boss scene. The UI health bar increases from 0-1.
    /// </summary>
    public void IncreaseHealthBar(float goalFillValue, float fillDuration)
    {
        
        StartCoroutine(FillHealthBarToValue(goalFillValue, fillDuration));
    }

    private IEnumerator FillHealthBarToValue(float goalFillValue, float fillDuration)
    {
        //how much we want to increment to go to value in certain amount of time.
        //I need To get the length, and then do length / seconds?
        float increment = (goalFillValue - _healthBar.fillAmount) / fillDuration;

        while(_healthBar.fillAmount < goalFillValue)
        {
            _healthBar.fillAmount += increment * Time.deltaTime;

            //We want to go to the goal fill value, but if the boss takes damage we don't want to go past that.
            _healthBar.fillAmount = Mathf.Clamp(_healthBar.fillAmount, 0, 1.00f / ((float)_maxHealth / (float)_currentHealth));
            yield return null;
        }

        yield return null;
    }

    #endregion


    #region Extra Functions

    //I make this public in case we want to access it through animation events
    public void DestroyObject()
    {
        //Destroy Unit
        Destroy(gameObject);
    }

    /// <summary>
    /// Reduce the entity's health to 0
    /// </summary>
    public void DestroyEntity()
    {
        _currentHealth = 0;
        UpdateHealthBar();
        PerformDeathLogic();
    }
    
    #endregion

 
}

[System.Serializable]
public class UnitHealthPhases
{
    [HideInInspector] public string phaseName = "Phase ";

    [Range(0, 100)]
    public float phaseHealthPercent = 55;

    public SO_EntityHealthEventBase unitPhaseEvent = null;

}
