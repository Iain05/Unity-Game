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
    private const float GRAVITY = -25f;
    public const float FALL_MULTIPLIER = 2.5f;
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
    }

    // Update is called once per frame
    void Update()
    {
        state = getState();
        Debug.Log("State: " + state);
        // Movement
        if ((state == State.LeftWall || state == State.RightWall) && !cooldown.wallJumpCoolingDown)
        {
            rb.velocity = new Vector2(0, 0);
        }
        walk();
        if (Input.GetKeyDown(KeyCode.Space)) { 
            jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && rb.velocity.x != 0)
        {
            dash();
        }
        if (state == State.Jumping || state == State.Falling)
        {
            if (rb.velocity.y < JUMP_VELOCITY_FALLOFF || rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.velocity += Vector2.up * GRAVITY * FALL_MULTIPLIER * Time.deltaTime;
            }
            else {
                rb.velocity += Vector2.up * GRAVITY * Time.deltaTime;
            }
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
        switch (state)
        {
            case State.LeftWall:
                rb.AddForce(new Vector2(1, 2).normalized * jumpForce, ForceMode2D.Impulse);
                numJumps = maxJumps - 1;
                cooldown.startCooldown(Cooldown.CooldownTypes.WallJump);
                break;
            case State.RightWall:
                rb.AddForce(new Vector2(-1, 2).normalized * jumpForce, ForceMode2D.Impulse);
                numJumps = maxJumps - 1;
                cooldown.startCooldown(Cooldown.CooldownTypes.WallJump);
                break;
            case State.Jumping or State.Falling:
                if (numJumps <= 0) return;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                numJumps--;
                break;
        }
        cooldown.startCooldown(Cooldown.CooldownTypes.Jump);
    }

    private State getState()
    {
        if (isGrounded())
        {
            return State.Grounded;
        }
        if (Physics2D.Raycast(transform.position, -transform.right, castDistance, wallLayer)) {
            return State.LeftWall;
        }
        if (Physics2D.Raycast(transform.position, transform.right, castDistance, wallLayer)) {
            return State.RightWall;
        }
        if (rb.velocity.y > 0)
        {
            return State.Jumping;
        }
        return State.Falling;
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
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
        if (cooldown.wallJumpCoolingDown) return;
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
