using UnityEngine;

using UnityEngine.UI;

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

    [SerializeField] private UnitHealthPhases[] _unitHealthPhases = new UnitHealthPhases[0];

    [SerializeField] private Image _healthBar;

    [SerializeField] private SO_EntityHealthEventBase _deathEvent;

    [SerializeField] private bool _showHealthGraphic;

    private SO_HealthAdjustments healthAdjustments;

    //Properties
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHealth { get => _maxHealth; }
    public UnitHealthPhases[] UnitHealthPhases { get => _unitHealthPhases; set => _unitHealthPhases = value; }
    public int CurrentPhase { get => _currentPhase; }
    public Image HealthBar { get => _healthBar; }

    private void Awake()
    {
        healthAdjustments = GameManager.Instance.HealthAdjustments;
    }

    private void Start()
    {
        //Initialize Events for the boss phases
        for (int i = 0; i < _unitHealthPhases.Length; i++)
        {
            _unitHealthPhases[i].unitPhaseEvent.Initialize(gameObject);
        }

        SampleValuesFromBoss();
        
    }

    /// <summary>
    /// If we're sampling health values from the Boss Profile, fill in the fields
    /// </summary>
    private void SampleValuesFromBoss()
    {
        if (_sampleFromBoss == SAMPLEFROMBOSSTYPE.FALSE) return;

        SO_BossProfile bossProfile = GetComponent<InitializeBoss>().ThisBossProfile;

        if (bossProfile == null) return;

        _maxHealth = bossProfile.B_MaxHealth;
        _unitHealthPhases = new UnitHealthPhases[bossProfile.B_BossPhases.Length];

        for (int i = 0; i < _unitHealthPhases.Length; i++)
        {
            _unitHealthPhases[i].phaseHealthPercent = bossProfile.B_BossPhases[i].healthPercent;
            _unitHealthPhases[i].unitPhaseEvent = bossProfile.B_BossPhases[i].phaseEvent;
        }
    }

    /// <summary>
    /// Heal the unit this script is attached to
    /// </summary>
    public void HealUnit(ChangeHealth _changeHealth)
    {
        if (_currentHealth >= _maxHealth) return;

        int healAmount = GetHealthValue(_changeHealth, HealthType.Healing);

        //heal the unit and make sure their health can't go over their maxHealth
        _currentHealth = Mathf.Clamp(_currentHealth + healAmount, 0, _maxHealth);
        Debug.Log($"{gameObject.name} healed {healAmount} HP");

        UpdateHealthBar();

        //Now update the current unit's phase based on the new health
        if (_unitHealthPhases.Length != 0) _currentPhase = SetCurrentHealthPhase();
    }


    /// <summary>
    /// Deal Damage to the unit this script is attached to
    /// </summary>

    public void DamageEntity(ChangeHealth _changeHealth)
    {
        if (_currentHealth <= 0) return;

        int damageAmount = GetHealthValue(_changeHealth, HealthType.Damage);

        //reduce health and make sure we can't go into the negatives
        _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0, _maxHealth);
        Debug.Log($"{gameObject.name} took {damageAmount} damage");


        if (_currentHealth <= 0)
        {
            PerformDeathLogic();

        }

        UpdateHealthBar();

        
        //Now update the current unit's phase based on the new health
        if (_unitHealthPhases.Length != 0) _currentPhase = SetCurrentHealthPhase();
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
        for (int i = _unitHealthPhases.Length - 1; i >= 0; i--)
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


    private void UpdateHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.fillAmount = 1.00f / ((float)_maxHealth / (float)_currentHealth);
        }
    }




    #region Extra Functions

    //I make this public in case we want to access it through animation events
    public void DestroyObject()
    {
        //Destroy Unit
        Destroy(gameObject);
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
