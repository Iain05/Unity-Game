using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    const int MAX_JUMPS = 2;
    public float maxMovementVelocity = 10;
    public float movementAcceleration = 1;
    public int dashForce = 20;
    public float friction = 0.1f;
    public int numJumps = 2;
    public float jumpForce = 13f;
    public float jumpVelocityFalloff = 2f;
    public float fallMultiplier = 6f;
    public Rigidbody2D rb;

    private const float jumpCooldown = 0.05f;
    private bool canJump = true;

    // grounded variables
    public LayerMask groundLayer;
    public Vector2 boxSize = new(0.75f, 0.4f);
    public float castDistance = 0.4f;


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

        if (Input.GetKeyDown(KeyCode.Space)) {jump();}
        
        Debug.Log("Jumps: " + numJumps + " | Grounded: " + isGrounded());

        if (rb.velocity.y < jumpVelocityFalloff || rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && rb.velocity.x != 0)
        {
            dash();
        }
    }

    private void jump() {
        if (isGrounded() && canJump)
        {
            numJumps = MAX_JUMPS;
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
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }



    private bool isGrounded()
    {
        // TODO Make this not suck for some reason
        //if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
    }

    private void dash()
    {
        Vector2 directionVector = new Vector2(Mathf.Sign(rb.velocity.x), 0);
        rb.AddForce(directionVector * dashForce, ForceMode2D.Impulse);
    }

    private void walk()
    {
        // if (Input.GetKey(KeyCode.A))
        // {
        //     if (rb.velocity.x > -maxMovementVelocity)
        //     {
        //         rb.velocity += new Vector2(-movementAcceleration, 0);
        //     }
        // }
        // else if (rb.velocity.x < 0)
        // {
        //     rb.velocity += new Vector2(friction, 0);
        // }

        // if (Input.GetKey(KeyCode.D))
        // {
        //     if (rb.velocity.x < maxMovementVelocity)
        //     {
        //         rb.velocity += new Vector2(movementAcceleration, 0);
        //     }
        // }
        // else if (rb.velocity.x > 0)
        // {
        //     rb.velocity -= new Vector2(friction, 0);
        // }

        // if (rb.velocity.x > -0.1 && rb.velocity.x < 0.1)
        // {
        //     rb.velocity = new Vector2(0, rb.velocity.y);
        // }
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-maxMovementVelocity, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(maxMovementVelocity, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
}
