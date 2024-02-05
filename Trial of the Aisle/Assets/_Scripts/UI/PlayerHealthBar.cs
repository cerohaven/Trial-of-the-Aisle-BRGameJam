using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private float playerHealth;
    [SerializeField] private float maxInvincibilityTimer;
    [SerializeField] private float invincibilityFlickerRate;
    [SerializeField] private float maxHealth;

    [Header("Heal Particle Effect")]
    [SerializeField] private GameObject healEffect;

    private SpriteRenderer playerSr;
    private GameObject playerGameObject;
    private RectTransform playerRectTransform;

    private float currentInvincibilityTime;
    private Color color = Color.white;
    

    private void Awake()
    {
        playerGameObject = GameObject.FindObjectOfType<PlayerInput>().gameObject;
        playerRectTransform = GetComponent<RectTransform>();
        playerSr = playerGameObject.GetComponent<SpriteRenderer>();

        currentInvincibilityTime = maxInvincibilityTimer + 1;
        maxHealth = playerRectTransform.sizeDelta.x;
    }

    void Update()
    {
        currentInvincibilityTime += Time.deltaTime;
        bool isInvincible = currentInvincibilityTime <= maxInvincibilityTimer;

        if (isInvincible)
            InvokeRepeating("Invincible", 0, invincibilityFlickerRate);
        else
        {
            CancelInvoke();
            color.a = 1;
            playerSr.color = new Color(1, 1, 1, color.a);
        }
    }



    private void Invincible()
    {
        color.a++;
        color.a %= 2;
        playerSr.color = new Color(1, 1, 1, color.a);
    }
    public void PlayerChangeHealth(float _playerChangedHealth)
    {
        //Check to see if healing or damage is being passed
        bool isDamage = _playerChangedHealth < 0;
        if(isDamage)
        {
            bool isInvincible = currentInvincibilityTime <= maxInvincibilityTimer;
            if (isInvincible)
                return;

            UpdateHealthBar(_playerChangedHealth);

            bool lostAllHealth = playerHealth <= 0;
            //if (lostAllHealth)
                //anim.SendMessage("Outro", "LoseGameAnimation");

                //Set them invincible for a certain period of time

            currentInvincibilityTime = 0;
            AudioManager.instance.Play("p_hurt");

        }
        else
        {
            UpdateHealthBar(_playerChangedHealth);
            Instantiate(healEffect, playerGameObject.transform.position, Quaternion.identity);
            AudioManager.instance.Play("heal");
        }

    }

    private void UpdateHealthBar(float _health)
    {
        playerHealth += _health;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        playerRectTransform.sizeDelta = new Vector2(playerHealth, playerRectTransform.sizeDelta.y);
        
    }

}
