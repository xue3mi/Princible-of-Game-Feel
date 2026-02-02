using System;
using UnityEngine;

public class hw1Movement : MonoBehaviour
{
    private Vector2 velocity;
    [SerializeField] private float maxSpeed;
    private Rigidbody2D rb;
    private Vector2 acceleration;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float counterAccelerationModifier;

    //skating mechanic
    [Header("Skating Mechanic")]
    [SerializeField] private float maxCharge = 1.5f;
    [SerializeField] private float chargeSpeed = 1.5f; //holding longer = higher speed
    [SerializeField] private float releaseDecay = 2f;  //release = speed decays
    private float currentCharge = 0f;

    [Header("Dash")]
    [SerializeField] private Sprite dashSprite; //ready to dash
    [SerializeField] private Sprite normalSprite; //skiing
    private SpriteRenderer sr;

    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCooldown = 1f;
    private float dashTimer = 0f;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashRemainingDistance = 0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {

        // dash cooldown timer
        if (dashTimer > 0f)
            dashTimer -= Time.fixedDeltaTime;

        acceleration = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) acceleration.y += 1;
        if (Input.GetKey(KeyCode.S)) acceleration.y -= 1;
        if (Input.GetKey(KeyCode.D)) acceleration.x += 1;
        if (Input.GetKey(KeyCode.A)) acceleration.x -= 1;

        //inputDir for adjusting player input velocity only
        Vector2 inputDir = acceleration.normalized;

        //input
        if (Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0f && inputDir != Vector2.zero)
        {
            StartDash(inputDir);
        }

        bool hasInput = inputDir != Vector2.zero;

        if (hasInput)
        {
            //sprite rotation radian to degree
            float targetAngle = Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float rotateSpeed = 0.5f;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotateSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);


            //holding longer = higher speed
            currentCharge += chargeSpeed * Time.fixedDeltaTime;
            //clamp velocity to cureent inputDir
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        }
        else //release = speed decays
        {
            currentCharge -= releaseDecay * Time.fixedDeltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
        }

        //add charge to acceleration
        acceleration = inputDir;
        acceleration *= accelerationSpeed;
        //slide
        acceleration *= (1f + currentCharge);
        acceleration *= Time.fixedDeltaTime;



        //apply counter acceleration logic
        float angleVelocityVsAcceleration = Vector2.Angle(velocity, acceleration);
        float counterPushRatio = angleVelocityVsAcceleration / 180f;
        acceleration += acceleration * (counterPushRatio * counterAccelerationModifier);

        //apply acceleration to velocity
        velocity += acceleration;


        //dash velcity
        //dash velcity
        if (isDashing)
        {
            //pause adding velocity
            Vector2 tempVelocity = velocity;
            velocity = Vector2.zero;

            //calculate move how far
            float dashStep = dashSpeed * Time.fixedDeltaTime;

            if (dashStep >= dashRemainingDistance)
            {
                dashStep = dashRemainingDistance;
                isDashing = false;
            }

            dashRemainingDistance -= dashStep;

            // move to destination using input direction
            rb.MovePosition(rb.position + dashDirection * dashStep);
            velocity = tempVelocity;
            return;
        }




        /*clamp velocity to maxSpeed per second
        if (velocity.magnitude > (maxSpeed * Time.fixedDeltaTime))
        {
            velocity = velocity.normalized * (maxSpeed * Time.fixedDeltaTime);
        }*/

        //friction logic
        //if velocity is less than friction * deltaTime, set velocity to zero
        // if (acceleration == Vector2.zero)

        // only when not sliding have friction
        if (!hasInput)
        { 
            if (velocity.magnitude < friction * Time.fixedDeltaTime)
            {
                velocity = Vector3.zero;
            }
            else //otherwise, apply friction
            {
                velocity -= velocity.normalized * (friction * Time.fixedDeltaTime);
            }
        }

        rb.MovePosition(rb.position + velocity);
    }

    private void StartDash(Vector2 dir)
    {
        isDashing = true;
        dashTimer = dashCooldown;
        dashDirection = dir.normalized;
        dashRemainingDistance = dashDistance;

        // change sprite to dashSprite
        if (sr != null && dashSprite != null)
        {
            sr.sprite = dashSprite;
            // 0.5s back to normalSprite
            StartCoroutine(ResetSprite());
        }
    }
    private System.Collections.IEnumerator ResetSprite()
    {
        yield return new WaitForSeconds(0.5f);
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }
}
