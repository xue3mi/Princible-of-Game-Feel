using System;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    private Vector2 velocity;
    [SerializeField] private float maxSpeed;
    private Rigidbody2D rb;
    private Vector2 acceleration;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float counterAccelerationModifier;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        acceleration = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) acceleration.y += 1;
        if (Input.GetKey(KeyCode.S)) acceleration.y -= 1;
        if (Input.GetKey(KeyCode.D)) acceleration.x += 1;
        if (Input.GetKey(KeyCode.A)) acceleration.x -= 1;

        acceleration = acceleration.normalized;
        acceleration *= accelerationSpeed;
        acceleration *= Time.fixedDeltaTime;

        //apply counter acceleration logic
        float angleVelocityVsAcceleration = Vector2.Angle(velocity, acceleration);
        float counterPushRatio = angleVelocityVsAcceleration / 180f;
        acceleration += acceleration * (counterPushRatio * counterAccelerationModifier);

        //apply acceleration to velocity
        velocity += acceleration;

        //clamp velocity to maxSpeed per second
        if (velocity.magnitude > (maxSpeed * Time.fixedDeltaTime))
        {
            velocity = velocity.normalized * (maxSpeed * Time.fixedDeltaTime);
        }

        //friction logic
        //if velocity is less than friction * deltaTime, set velocity to zero
        // if (acceleration == Vector2.zero)
        //{
        if (velocity.magnitude < friction * Time.fixedDeltaTime)
        {
            velocity = Vector3.zero;
        }
        else //otherwise, apply friction
        {
            velocity -= velocity.normalized * (friction * Time.fixedDeltaTime);
        }

        rb.MovePosition(rb.position + velocity);
    }
}
