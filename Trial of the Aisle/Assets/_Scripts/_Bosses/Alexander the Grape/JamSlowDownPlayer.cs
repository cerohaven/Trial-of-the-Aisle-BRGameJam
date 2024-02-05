using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamSlowDownPlayer : MonoBehaviour
{
    [SerializeField] private float disappearTime = 15f;

    private bool startToDisappear = false;
    private SpriteRenderer sr;
    private float fade;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        fade = 1;
        Invoke("Disappear", disappearTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startToDisappear) return;
        fade -= Time.deltaTime;

        float a = fade;

        sr.color = new Color(1, 1, 1, a);

        if(a <= 0.05f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.MoveSpeed /= 2;
            pc.DodgeSpeed /= 5;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.MoveSpeed *= 2;
            pc.DodgeSpeed *= 5;
        }
    }
    private void Disappear()
    {
        startToDisappear = true;
    }
}
