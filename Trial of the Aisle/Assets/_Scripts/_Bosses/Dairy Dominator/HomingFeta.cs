using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingFeta : MonoBehaviour
{
    [SerializeField] private SO_AdjustHealth adjustHealth;

    [SerializeField] private GameObject hitParticles; //On collision, spawn particles

    [SerializeField] private float ForwardSpeed = 1;
    [SerializeField] float RotateSpeedInDeg = 45;

    private Transform playerTransform;


    //Properties
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    // In Update, you should rotate and move the missile to rotate it towards the player.  It should move forward with ForwardSpeed and rotate at RotateSpeedInDeg.
    // Do not use the RotateTowards or LookAt methods.

    //got it working because used Debug.Log to find that the calc provides a NaN
    //Put brackets around a . b and it worked. Also added time.deltaTime to Rot
    //now problem is that the rotation is not optimized.

    void Update()
    {
        Homing();
    }

    private void Homing()
    {
        //get missile and enemy positions in variables for easy use
        Vector3 a = transform.position;
        Vector3 b = playerTransform.position;

        //get direction between enemy and missile
        Vector3 target = b - a;

        //normalize vector between enemy and missile so it can be crossed with the already normalized transform.up
        target.Normalize();

        //Cross Product to find out how to rotate missile through it's z axis
        float angle = Vector3.Cross(target, transform.up).z;

        //rotate the missile based on cross direction and how fast it should go (RotateSpeedInDeg)
        transform.Rotate(new Vector3(0, 0, -angle * RotateSpeedInDeg * Time.deltaTime));

        //move the missile up and a certain speed
        transform.position += transform.up * ForwardSpeed * Time.deltaTime;

    }

    protected  void OnCollisionEnter2D(Collision2D collision)
    {
        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        if (GameManager.gameEnded) return;

        //On Collision with the player, deal damage UNLESS it can be picked up 
        if (collision.gameObject.CompareTag("Player"))
        {
            adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Small_Health, HealthType.Damage);

            DestroyObj(collision);
        }

        if(!collision.gameObject.CompareTag("Boss") && !collision.gameObject.CompareTag("Feta"))
        {
            DestroyObj(collision);
        }
    }

    private void DestroyObj(Collision2D collision)
    {
        GameObject go = Instantiate(hitParticles, transform.position, Quaternion.identity);
        go.transform.up = collision.contacts[0].point;
        Destroy(gameObject);
    }


}
