using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    public float accelerationSpeed = 10;
    public float turnSpeed = 2;

    private float accelerationAmount = 0;
    private float turnAmount = 0;

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity += transform.up * accelerationAmount * Time.deltaTime * accelerationSpeed;
        float speed = GetComponent<Rigidbody>().velocity.magnitude;
        transform.Rotate(-turnSpeed * Time.deltaTime * turnAmount, 0, 0);
        GetComponent<Rigidbody>().velocity = transform.up * speed;
    }

    public void OnMove(InputValue axis)
    {
        Debug.Log("Move: " + (float)axis.Get());
        turnAmount = (float)axis.Get();
    }

    public void OnAccelerate(InputValue axis)
    {
        Debug.Log("Acc: " + (float)axis.Get());
        accelerationAmount = (float)axis.Get();
        
    }

    public void OnFire()
    {
        //Debug.Log("Fire!");
    }

    public void OnJungle()
    {
        //Debug.Log("Jungle!");
    }
}
