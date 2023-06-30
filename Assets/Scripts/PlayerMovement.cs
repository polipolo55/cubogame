using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("components")]
    public Rigidbody2D rb;
    public Transform groundPoint;
    public Transform leftPoint;
    public Transform rightPoint;


    [Space(5)]

    [Header("publiques")]
    public float moveSpeed;
    public float acceleration;
    public float deceleration;
    public float velPower;
    public float jumpForce;
    public float fallGravMultiplyer;
    public float jumpBufferTime;
    public float cutForce;
    public bool frictionEnabled;
    public float frictionMult;
    public float dashAmount;
    public float dashCD;
    public float freezeAmountDash;
    public int numberOfJumps;
    [Range(0f, 1)] public float accelAir;
    [Range(0f, 1)] public float deccelAir;


    private Vector2 groundCheckSize = new Vector2(0.49f, 0.05f);
    private Vector2 wallCheckSize = new Vector2(0.35f, 1f);
    public LayerMask floor;

    [Space(5)]


    [Header("debugging")]
    
    private float jumpInput;
    private float moveInput;
    private float dashInput;


    private float lastGroundTime;
    private float lastJumpTime;
    private float lastDashTime;
    public float freezeTime;


    private float dashTime;

    private float gravityScale;


    private bool isGrounded;
    public bool isJumping;
    private bool isDashing;
    public bool isFrozen;

    private bool canJump;
    public bool canDash;

    private float accelRate;
    private Vector2 lastVel;


    // Start is called before the first frame update
    void Start()
    {
        gravityScale = rb.gravityScale;
        canDash = true;

    }

    // Update is called once per frame

    private void Update()
    {

        /*float timer = 2f;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer < 0f) { 
                timer = 0f;
                canDash = true;
            }
        }

        
         {
            canDash = false;
            timer = time;
        }
         */

        lastGroundTime -= Time.deltaTime;
        lastDashTime -= Time.deltaTime;
        dashTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
        freezeTime -= Time.deltaTime;

        if (isDashing)
        {
            if (dashTime > 0f) whileDashing();
            else finishedDashing();
        }

        if (lastDashTime > 0f) canDash = false;
        else canDash = true;

        if (freezeTime > 0) isFrozen = true;
        else if (isFrozen == true)
        {
            rb.velocity = lastVel;
            isFrozen = false;
        }
        
        if (!isJumping)
        {
            isGrounded = Physics2D.OverlapBox(groundPoint.position, groundCheckSize, 0, floor);

        }

        
        /*
        if (lastGroundTime > 0f)
        {
            lastGroundTime -= Time.deltaTime;

            if (lastGroundTime < 0f)
            {
                lastGroundTime = 0f;

            }

        }
        if (lastJumpTime > 0f)
        {
            lastJumpTime -= Time.deltaTime;

            if (lastJumpTime < 0f)
            {
                lastJumpTime = 0f;

            }

        }
        if (dashTime > 0f)
        {
            dashTime -= Time.deltaTime;
            if (isDashing) whileDashing();

            if (dashTime < 0f)
            {
                if (isDashing) finishedDashing();
                dashTime = 0f;
            }
        }

        if (lastDashTime > 0f) 
        {
            canDash = false;
            lastDashTime -= Time.deltaTime;
            if (lastDashTime < 0f)
            {
                lastDashTime = 0f;
                if (!isDashing && isGrounded) canDash = true;
                else lastDashTime = 0.1f;
            }
        } 
       
        if (freezeTime > 0f)
        {
            isFrozen = true;
            freezeTime -= Time.deltaTime;
            if (freezeTime < 0f)
            {
                rb.velocity = lastVel;
                isFrozen = false;
                freezeTime = 0f;
            }
        }
        
        */



        if (lastGroundTime > 0f && lastJumpTime > 0f && !isJumping) canJump = true;
        else canJump = false;



    }
    void FixedUpdate()
    {
 
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        if (isGrounded) accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        else accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration*accelAir : deceleration * deccelAir;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);



        if (!isDashing || !isFrozen) rb.AddForce(movement * Vector2.right);




        


        if (isGrounded)
        {
            lastGroundTime = jumpBufferTime;
            isJumping = false;
            //canDash = true;
        }

        if (canJump) 
        {
            Jump();
        }

        if (jumpInput == 0 && isJumping)
        {
            whileJumping();
        }


        if (canDash && dashInput == 1 && moveInput != 0)
        {
            Dash();
        }

        
        if (lastGroundTime > 0 && Mathf.Abs(moveInput) < 0.01f && frictionEnabled)
        {
            float frictionAmout = Mathf.Min(Mathf.Abs(rb.velocity.x), frictionMult);

            frictionAmout *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -frictionAmout, ForceMode2D.Impulse);
        }

        if (isFrozen)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else
        {

            if (rb.velocity.y < 0.01f && !isDashing)
            {
                rb.gravityScale = gravityScale * fallGravMultiplyer;
                isJumping = false;
            }
            else if (!isDashing) rb.gravityScale = gravityScale;
        }
    }

    public void HorizontalMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<float>();
    }

    public void VerticalMove(InputAction.CallbackContext ctx)
    {
        jumpInput = ctx.ReadValue<float>();
        lastJumpTime = jumpBufferTime;
    }
    public void DashMove(InputAction.CallbackContext ctx)
    {
        dashInput = ctx.ReadValue<float>();
    }



    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        lastJumpTime = -1f;
        lastGroundTime = -1f;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;


    }

    private void Dash()
    {
        lastVel = new Vector2(rb.velocity.x, 0);
        rb.velocity = Vector2.zero; 
        dashTime = dashAmount * 0.015f;
        isDashing = true;
        rb.AddForce(Vector2.right * dashAmount * 8 * moveInput, ForceMode2D.Impulse);
        canDash = false;    }

    private void whileDashing()
    {
        rb.gravityScale = 0f;
    }

    private void finishedDashing()
    {   
        freezeTime = freezeAmountDash;
        lastDashTime = dashCD;
        rb.gravityScale = gravityScale;
        isDashing = false;
    }

    private void whileJumping()
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * cutForce, ForceMode2D.Impulse);
        } 
        else if( rb.velocity.y < 0 && isJumping)
        {
            isJumping = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundPoint.transform.position, groundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rightPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(leftPoint.position, wallCheckSize);
        
    }
}





/*
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(0.5f,0.05f,0), Color.red, 1f);
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(-.5f, 0.05f, 0), Color.red, 1f);
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(.5f, -0.05f, 0), Color.red, 1f);
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(-.5f, -0.05f, 0), Color.red, 1f);
*/