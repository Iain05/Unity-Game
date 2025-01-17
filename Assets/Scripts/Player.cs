using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        LeftWall,
        RightWall
    }
    private State state = State.Idle;
    public const float FRICTION = 0.1f;
    public const float FALL_MULTIPLIER = 6f;
    public const float JUMP_VELOCITY_FALLOFF = 2f;
    private const float JUMP_COOLDOWN = 0.05f;
    public int maxJumps = 2;
    public float maxMovementVelocity = 10;
    public float movementAcceleration = 1;
    public int dashForce = 20;
    public int numJumps = 2;
    public float jumpForce = 13f;
    public Rigidbody2D rb;
    private bool canJump = true;

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
        // Movement
        walk();

        // Debug.Log("State: " + state);

        if (Input.GetKeyDown(KeyCode.Space)) { 
            state = State.Jumping;
            if (isOnWall()) {
                if (state == State.LeftWall) {
                    rb.velocity = new Vector2((float)(-jumpForce / 1.4), jumpForce);
                }
                else if (state == State.RightWall) {
                    rb.velocity = new Vector2((float)(jumpForce / 1.4), jumpForce);
                }
            } else {
                jump(); 
            }
        }

        // Debug.Log("Jumps: " + numJumps + " | Grounded: " + isGrounded());


        if (rb.velocity.y < JUMP_VELOCITY_FALLOFF || rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * FALL_MULTIPLIER * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && rb.velocity.x != 0)
        {
            dash();
        }
    }

    private void jump()
    {
        if (isGrounded() && canJump)
        {
            numJumps = maxJumps;
        }
        if (numJumps > 0)
        {
            StartCoroutine(cooldownJump());
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            numJumps--;
        }
    }
    IEnumerator cooldownJump()
    {
        canJump = false;
        yield return new WaitForSeconds(JUMP_COOLDOWN);
        canJump = true;
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
    }

    private bool isOnWall()
    {
        if (Physics2D.Raycast(transform.position, -transform.right, castDistance, wallLayer))
        {
            Debug.Log("Right Wall");
            state = State.RightWall;
            return true;
        }
        else if (Physics2D.Raycast(transform.position, transform.right, castDistance, wallLayer))
        {
            Debug.Log("Left Wall");
            state = State.LeftWall;
            return true;
        }
        return false;
    }

    private void dash()
    {
        Vector2 directionVector = new Vector2(Mathf.Sign(rb.velocity.x), 0);
        rb.AddForce(directionVector * dashForce, ForceMode2D.Impulse);
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
