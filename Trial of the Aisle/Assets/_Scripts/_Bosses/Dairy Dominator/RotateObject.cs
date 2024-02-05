using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 40;

    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }

    private void Start()
    {
        Invoke("DestroyObj", 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0, rotateSpeed * Time.deltaTime));
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
