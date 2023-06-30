using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("components")]
    public Rigidbody2D rb;
    public GameObject groundPoint;


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


    private Vector2 checkSize;
    public LayerMask floor;

    [Space(5)]


    [Header("debugging")]
    
    private float jumpInput;
    private float moveInput;
    private float dashInput;


    private float lastGroundTime;
    private float lastJumpTime;
    private float dashTime;


    private float gravityScale;


    private bool isGrounded;
    private bool isJumping;
    private bool isDashing;

    private bool canJump;
    private bool canDash;


    private float coyotejump;
    private bool jumpInputReleased;
    

    private bool onGroundTime;
    private bool onJumpTime;

    // Start is called before the first frame update
    void Start()
    {
        gravityScale = rb.gravityScale;
        checkSize = new Vector2(0.5f, 0.1f);

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

        if (lastGroundTime > 0f)
        {
            lastGroundTime -= Time.deltaTime;

            if (lastGroundTime < 0f)
            {
                lastGroundTime = 0f;
                onGroundTime = false;

            }

        }
        if (lastJumpTime > 0f)
        {
            lastJumpTime -= Time.deltaTime;

            if (lastJumpTime < 0f)
            {
                lastJumpTime = 0f;
                onJumpTime = false;

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

        if (lastGroundTime > 0 && lastJumpTime > 0 && !isJumping) canJump = true;
        else canJump = false;



    }
    void FixedUpdate()
    {
            
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);

        

        isGrounded = Physics2D.OverlapBox(groundPoint.transform.position, checkSize, 0, floor);

        if (isGrounded)
        {
            lastGroundTime = jumpBufferTime;
            isJumping = false;
            onGroundTime = true;
            if(!isDashing) canDash = true;
            //canDash = true;
        }


        coyotejump = 1f + lastGroundTime;

        if (canJump) 
        {
            Jump();
        }

        if (jumpInput == 0 && isJumping)
        {
            whileJumping();
        }

    

        if (canDash && dashInput == 1)
        {
            Dash();
        }




        if (rb.velocity.y < 0.01f && !isDashing)
        {
            rb.gravityScale = gravityScale * fallGravMultiplyer;
            isJumping = false;
        }
        else if (!isDashing) rb.gravityScale = gravityScale;
     
    

      

        
        if (lastGroundTime > 0 && Mathf.Abs(moveInput) < 0.01f && frictionEnabled)
        {
            float frictionAmout = Mathf.Min(Mathf.Abs(rb.velocity.x), frictionMult);

            frictionAmout *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -frictionAmout, ForceMode2D.Impulse);
        }

    }

    public void HorizontalMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<float>();
    }

    public void VerticalMove(InputAction.CallbackContext ctx)
    {
        jumpInput = ctx.ReadValue<float>();
        if(jumpInput == 1) lastJumpTime = jumpBufferTime;
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
        jumpInputReleased = false;


    }

    private void Dash()
    {
        dashTime = dashAmount * 0.1f;
        isDashing = true;
        rb.AddForce(Vector2.right * dashAmount * 5 * moveInput, ForceMode2D.Impulse);
        canDash = false;    }

    private void whileDashing()
    {
        rb.gravityScale = 0f;
    }

    private void finishedDashing()
    {
        rb.gravityScale = gravityScale;
        rb.velocity = Vector2.zero;
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
        Gizmos.DrawWireCube(groundPoint.transform.position, checkSize);
    }
}





/*
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(0.5f,0.05f,0), Color.red, 1f);
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(-.5f, 0.05f, 0), Color.red, 1f);
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(.5f, -0.05f, 0), Color.red, 1f);
Debug.DrawLine(groundPoint.transform.position, groundPoint.transform.position + new Vector3(-.5f, -0.05f, 0), Color.red, 1f);
*/