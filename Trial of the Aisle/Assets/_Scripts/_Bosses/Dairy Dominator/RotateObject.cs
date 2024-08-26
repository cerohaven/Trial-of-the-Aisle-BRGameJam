using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 40;
    private Animator animator;

    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }

    private void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
        Invoke("DestroyObj", 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0, rotateSpeed * Time.deltaTime));
    }

    private void DestroyObj()
    {
        if (animator != null)
            animator.SetInteger("animState", 7);
     
        Destroy(gameObject);
    }


}
