using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //variables to tweak

    [Header("gravetat")]
    private float gravityStrength; 
    private float gravityScale; 
    [Space(5)]
    public float fallGravityMult; //mult al caure d'un salt
    public float maxFallSpeed;

    [Space(20)]

    [Header("run")]
    public float runMaxSpeed; 
    public float runAccel; 
    private float runAccelAmount; // força aplicada al jugador
    public float runDecceleration; 
    private float runDeccelAmount; 
    [Space(5)]
    [Range(0f, 1)] public float AirAccel; 
    [Range(0f, 1)] public float Airdeccel;
    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("jump")]
    public float jumpHeight; 
    public float jumpTimeToApex; 
    private float jumpForce; 

    public float jumpCutGravityMult; 
    [Range(0f, 1)] public float jumpHangGravityMult; 
    public float jumpHangTimeThreshold; 
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Header("walljump")]
    public Vector2 wallJumpForce; 
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; 
    [Range(0f, 1.5f)] public float wallJumpTime; 
    public bool doTurnOnWallJump; 

    [Space(10)]

    [Header("Slide")]
    //public float slideSpeed;
    //public float slideAccel;

    [Header("assits")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //temps despres de plataforma
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; 


    //Toxicity - SOAD 

    //Components
    public Rigidbody2D rb { get; private set; }


    public bool isFacingRight { get; private set; }
    public bool isJumping { get; private set; }
    public bool isWallJumping { get; private set; }
    public bool isSliding { get; private set; }

    [Header("Degub")]
    public float lastOnGroundTime;
    public float lastOnWallTime;
    public float lastOnWallRightTime;
    public float lastOnWallLeftTime;

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    private Vector2 moveInput;
    public float lastPressedJumpTime { get; private set; }

    //Casiopea - Galactic Funk

    [SerializeField] private Transform groundPoint;

    private Vector2 groundCheckSize = new Vector2(0.9f, 0.03f);

    [Space(5)]
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform leftPoint;
    private Vector2 wallCheckSize;
    [SerializeField] private LayerMask floor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();


    }
    private void Start()
    {

        //the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices the voices 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;
        runAccelAmount = (50 * runAccel) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        wallCheckSize = new Vector2(0.5f, 0.9f * transform.localScale.y);

        #region Variable Ranges
        runAccel = Mathf.Clamp(runAccel, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion

        SetGravityTo(gravityScale);
        isFacingRight = true;

    }
    private void Update()
    {
        Timers();

        Collisions();

        JumpComprovations();

        //aqui es comprovaria si pot dashear i fer slide

        GravityHandling();


    }

    private void FixedUpdate()
    {

        if (isWallJumping)
            Run(wallJumpRunLerp);
        else
            Run(1);

       // aqui shauria de posar el slide a part
    }

    public void HorizontalMove(InputAction.CallbackContext ctx)
    {
        moveInput.x = ctx.ReadValue<float>();
        if(moveInput.x != 0) CheckDirectionToFace(moveInput.x > 0);
        Debug.Log(moveInput.x);
    }

    public void VerticalMove(InputAction.CallbackContext ctx)
    {
        moveInput.y = ctx.ReadValue<float>();
        Debug.Log(moveInput.y);
    }
    public void JumpMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnJumpInput();
            Debug.Log("jump");
        } else if (ctx.canceled)
        {
            OnJumpRelease();
            Debug.Log("stopped jump");
        }
    }
    public void DashMove(InputAction.CallbackContext ctx)
    {
        //dashInput = ctx.ReadValue<float>();
    }


    public void SetGravityTo(float amount)
    {
        rb.gravityScale = amount;
    }

    private void Collisions()
    {
        if (!isJumping)
        {
            if (Physics2D.OverlapBox(groundPoint.position, groundCheckSize, 0, floor) && !isJumping) lastOnGroundTime = coyoteTime; 

            if (((Physics2D.OverlapBox(rightPoint.position, wallCheckSize, 0, floor) && isFacingRight) || (Physics2D.OverlapBox(leftPoint.position, wallCheckSize, 0, floor) && !isFacingRight)) && !isWallJumping)
                lastOnWallRightTime = coyoteTime;

            if (((Physics2D.OverlapBox(rightPoint.position, wallCheckSize, 0, floor) && !isFacingRight) || (Physics2D.OverlapBox(leftPoint.position, wallCheckSize, 0, floor) && isFacingRight)) && !isWallJumping)
                lastOnWallLeftTime = coyoteTime;

            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }
    }

    private void Timers()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
    }
    private void JumpComprovations()
    {
        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;

            if (!isWallJumping)
                _isJumpFalling = true;
        }

        if (isWallJumping && Time.time - _wallJumpStartTime > wallJumpTime)
        {
            isWallJumping = false;
        }

        if (lastOnGroundTime > 0 && !isJumping && !isWallJumping)
        {
            _isJumpCut = false;

            if (!isJumping)
                _isJumpFalling = false;
        }

        //jump
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            isWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        //walljump
        else if (CanWallJump() && lastPressedJumpTime > 0)
        {
            isWallJumping = true;
            isJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
        }
    }
    private void GravityHandling() 
    {
        //Just the way you are - Masayoshi Takanaka
        if (isSliding)
        {
            SetGravityTo(0);
        }
        else if (_isJumpCut)
        {
            SetGravityTo(gravityScale * jumpCutGravityMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));

        }
        else if ((isJumping || isWallJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
        {
            SetGravityTo(gravityScale * jumpHangGravityMult);
        }
        else if (rb.velocity.y < 0)
        {
            SetGravityTo(gravityScale * fallGravityMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            SetGravityTo(gravityScale);
        }
    }

    public void OnJumpInput()
    {
        lastPressedJumpTime = jumpInputBufferTime;
    }

    public void OnJumpRelease()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }


    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    //moviment, aquesta part es conserva del script original pero incorpora lerp per smoothing;

    private void Run(float lerp)
    {
        float targetSpeed = moveInput.x * runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerp);

        float accelRate;

        if (lastOnGroundTime > 0) accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
        else accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * AirAccel : runDeccelAmount * Airdeccel;

        //el script bo fa aqui una implementacio de mes acceleracio en el apex del jump, la podriem posar pero mes endevant

        if (doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed - rb.velocity.x;

        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    private void Jump()
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        float force = jumpForce;
        if (rb.velocity.y < 0) force -=rb.velocity.y; //en comtpes de fer vel.y = 0 fem li restem la força en negatiu

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

    }

    private void WallJump(int dir)
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;
        lastOnWallRightTime = 0;
        lastOnWallLeftTime = 0;

        Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if(rb.velocity.y < 0)
            force.y -= rb.velocity.y;
        rb.AddForce(force, ForceMode2D.Impulse);
    }   

    //condocions fetes be =)

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
            Turn();
    }
    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanWallJump()
    {
        return lastPressedJumpTime > 0 && lastOnWallTime > 0 && lastOnGroundTime <= 0 && (!isWallJumping ||
             (lastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && rb.velocity.y > 0;
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