using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualController : MonoBehaviour
{
    public float speed;
    public float angular_drag;
    public float acceleration;
    public float angular_velocity;
    public float acceleration_rate;
    public float angular_velocity_rate;

    public Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            acceleration += acceleration_rate;
            if(acceleration > 2)
            {
                acceleration = 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            acceleration -= acceleration_rate;
            if (acceleration < 0)
            {
                acceleration = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            angular_velocity += angular_velocity_rate;
            if(angular_velocity > 1)
            {
                angular_velocity = 1;
            }
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            angular_velocity -= angular_velocity_rate;
            if(angular_velocity < -1)
            {
                angular_velocity = -1;
            }
        }
        Move();
    }

    private void Move()
    {
        rb.velocity = transform.up * speed * acceleration;
        rb.angularVelocity = angular_drag * angular_velocity;
    }
}
