using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum State
    {
        Walking,
        Jumping,
        Falling,
        Dashing,
        LeftWall,
        RightWall,
        Grounded
    }
    [SerializeField] private Cooldown cooldown;
    private State state = State.Falling;
    public const float FRICTION = 0.1f;
    public const float FALL_MULTIPLIER = 6f;
    public const float JUMP_VELOCITY_FALLOFF = 2f;
    public int maxJumps = 2;
    public float maxMovementVelocity = 10;
    public float movementAcceleration = 1;
    public int dashForce = 20;
    public int numJumps = 2;
    public float jumpForce = 13f;
    public Rigidbody2D rb;

    // grounded variables
    public LayerMask groundLayer;
    public Vector2 boxSize = new(0.75f, 0.4f);
    public float castDistance = 0.4f;

    // wall jump variables
    public LayerMask wallLayer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Hello World!");
    }

    // Update is called once per frame
    void Update()
    {
        state = getState();
        Debug.Log("State: " + state);
        // Movement
        walk();
        if (Input.GetKeyDown(KeyCode.Space)) { 
            jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && rb.velocity.x != 0)
        {
            dash();
        }

        if (rb.velocity.y < JUMP_VELOCITY_FALLOFF || rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * FALL_MULTIPLIER * Time.deltaTime;
        }

    }

    private void jump()
    {
        if (cooldown.jumpCoolingDown) return;
        if (state == State.Grounded || state == State.Walking) {
            numJumps = maxJumps;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            numJumps = maxJumps - 1;
            return;
        }
        if (numJumps <= 0) return;
        switch (state)
        {
            case State.LeftWall:
                rb.velocity = new Vector2((float)(-jumpForce / 1.2), jumpForce);
                numJumps = maxJumps - 1;
                break;
            case State.RightWall:
                rb.velocity = new Vector2((float)(jumpForce / 1.2), jumpForce);
                numJumps = maxJumps - 1;
                break;
            case State.Jumping or State.Falling:
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                numJumps--;
                break;
        }
        cooldown.startCooldown(Cooldown.CooldownTypes.Jump);
        state = State.Jumping;
    }

    private State getState()
    {
        if (isGrounded())
        {
            return State.Grounded;
        }
        else if (rb.velocity.y > 0)
        {
            return State.Jumping;
        }
        else if (rb.velocity.y < 0)
        {
            return State.Falling;
        }
        return State.Walking;
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
    }

    private bool isOnWall()
    {
        if (Physics2D.Raycast(transform.position, -transform.right, castDistance, wallLayer))
        {
            state = State.RightWall;
            return true;
        }
        else if (Physics2D.Raycast(transform.position, transform.right, castDistance, wallLayer))
        {
            state = State.LeftWall;
            return true;
        }
        return false;
    }

    private void dash()
    {
        if (cooldown.dashCoolingDown) return;
        Vector2 directionVector = new Vector2(Mathf.Sign(rb.velocity.x), 0);
        rb.AddForce(directionVector * dashForce, ForceMode2D.Impulse);
        cooldown.startCooldown(Cooldown.CooldownTypes.Dash);
    }

    private void walk()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (rb.velocity.x > -maxMovementVelocity)
            {
                rb.velocity += new Vector2(-movementAcceleration, 0);
            }
        }
        else if (rb.velocity.x < 0)
        {
            rb.velocity += new Vector2(FRICTION, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (rb.velocity.x < maxMovementVelocity)
            {
                rb.velocity += new Vector2(movementAcceleration, 0);
            }
        }
        else if (rb.velocity.x > 0)
        {
            rb.velocity -= new Vector2(FRICTION, 0);
        }

        if (rb.velocity.x > -0.1 && rb.velocity.x < 0.1)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
}
