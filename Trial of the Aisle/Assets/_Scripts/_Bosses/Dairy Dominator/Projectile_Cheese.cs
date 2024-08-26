using UnityEngine;

public class Projectile_Cheese : MonoBehaviour
{
    [SerializeField] private ChangeHealth damageDealt;
    [SerializeField] private GameObject particleHitEffect;
    [SerializeField] private GameObject cheeseArtGO;
    [SerializeField] private GameObject warningCircleGO;
    private Collider2D col;
    private bool startToDisappear = false;
    private SpriteRenderer sr;
    private float fade;

    private FMOD.Studio.EventInstance FetaFrenzyThrowInstance;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = cheeseArtGO.GetComponent<SpriteRenderer>();
        col.enabled = false;
    }
    private void Start()
    {
        
        cheeseArtGO.SetActive(false);
        warningCircleGO.SetActive(true);
        Invoke(nameof(ShowCheese), 0.7f);
    }

    private void ShowCheese()
    {
        col.enabled = true;
        cheeseArtGO.SetActive(true);
        warningCircleGO.SetActive(false);

        FetaFrenzyThrowInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_DD/B_Feta_Frenzy");
        FetaFrenzyThrowInstance.start();
    }

    private void Update()
    {
        void Update()
        {
            if (!startToDisappear) return;
            fade -= Time.deltaTime;

            float a = fade;

            sr.color = new Color(1, 1, 1, a);

            if (a <= 0.05f)
            {
                Destroy(gameObject);
            }
        }
    }
    public void StartToDisappear()
    {
        startToDisappear = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityHealth entityHealth = collision.gameObject.GetComponent<EntityHealth>();


        if (collision.gameObject.CompareTag("Player")) 
        {
            CancelInvoke(nameof(ShowCheese));
            entityHealth.DamageEntity(damageDealt);
            
            InstantiateHitParticles();
            Destroy(gameObject);
            return;
        }
           
    }
    protected void InstantiateHitParticles()
    {
        Instantiate(particleHitEffect, transform.position, Quaternion.identity);
    }
}
