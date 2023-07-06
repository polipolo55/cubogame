using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //variables to tweak
    [Range(0f, 1f)] public float timeScale;


    [Header("gravetat")]
    private float gravityStrength; 
    private float gravityScale; 
    [Space(5)]
    public float fallGravityMult; //mult al caure d'un salt
    public float maxFallSpeed;

    public float frictionAmount;

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
    public float wallFallSpeed;
    public float slideAccel;


    public float floorSlideSpeed;
    [Space(10)]

    //public float slideSpeed;
    //public float slideAccel;

    [Header("dash")]
    public int dashAmount;
    public float dashSpeed;
    public float dashSleepTime; 
    public float dashStartTime;
    [Space(5)]
    public float dashEndTime; 
    public Vector2 dashEndSpeed; 
    [Range(0f, 1f)] public float dashEndRunLerp; 
    [Space(5)]
    public float dashRefillTime;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime;


    private int dashesLeft;
    private bool dashRefilling;
    private Vector2 lastDashDir;
    private bool isDashingStarting;
    public bool canInput = true;

    [Range(0f, 1f)] public float grabWallGravMult;

    [Header("assits")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //temps despres de plataforma
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;
    [Range(0.01f, 0.5f)] public float groundSlideInputBufferTime;
    public float groundSlideSpeed;
    public float groundSlideAcc;




    //Toxicity - SOAD 

    //Components
    public Rigidbody2D rb { get; private set; }


    public bool isFacingRight { get; private set; }
    public bool isJumping { get; private set; }
    public bool isWallJumping { get; private set; }
    public bool isSliding { get; private set; }
    public bool isDashing { get; private set; }
    public bool isGroundSlide { get; private set; }
    public bool isAlive;



    public float lastOnGroundTime { get; private set; }
    public float lastOnWallTime { get; private set; }
    public float lastOnWallRightTime { get; private set; }
    public float lastOnWallLeftTime { get; private set; }

    public float timeGroundSliding { get; private set; }

    public bool onRightWall { get; private set; }
    public bool onLeftWall { get; private set; }

    public float groundSlideDirection { get; private set; }


    //Jump
    private bool isJumpCut;
    private bool isJumpFalling;

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    private bool isGroundSlideCut;


    

    public Vector2 moveInput;
    public float lastPressedJumpTime { get; private set; }
    public float lastPressedDashTime { get; private set; }
    public float lastPressedGroundSlideTime { get; private set; }

    //Casiopea - Galactic Funk

    [SerializeField] private Transform groundPoint;

    private Vector2 groundCheckSize = new Vector2(0.9f, 0.03f);

    [Space(5)]
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform leftPoint;    
    [SerializeField] private LayerMask floor;
    public Animator anim;

    [Space(20)]
    [Header("DEBUG")]
    public bool onWall;
    public bool gravityUpdateCheck = true;

    private Vector2 wallCheckSize;


    public ParticleSystem dust;
    public ParticleSystemRenderer psr;
    public GameObject teleport;

    public AudioClip jump;
    public AudioClip fall;
    public AudioClip dash;

    private void Awake()
    { 
        GameManager.Instance.player = this; 
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;
        

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

        runAccel = Mathf.Clamp(runAccel, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);


        SetGravityTo(gravityScale);
        isFacingRight = true;

    }
    private void Update()
    {
        Time.timeScale = timeScale;

        Timers();

        Collisions();

        Comprovations();

        GravityHandling();

        Friction();

        AnimationHandler();
        


    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {

            if (isWallJumping)
                Run(wallJumpRunLerp);
            else
                Run(1);
        }
        else if (isDashingStarting)
        {
            Run(dashEndRunLerp);
        }
        if (isSliding)
        {
            Slide();
        }

       // aqui shauria de posar el slide a part
    }
    public void tp(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            transform.position = teleport.transform.position;
        }
    }
    public void HorizontalMove(InputAction.CallbackContext ctx)
    {
        if(canInput) moveInput.x = ctx.ReadValue<float>();
    }
    public void VerticalMove(InputAction.CallbackContext ctx)
    {
        if(canInput)
        {   
            moveInput.y = ctx.ReadValue<float>();

            if (ctx.started && moveInput.y < 0f)
            {
                onGroundSlideInput();
            }
            else if (ctx.canceled && isGroundSlide)
            {
                timeGroundSliding = 0f;
                onGroundSlideRelease();
            }
        }
    }
    public void JumpMove(InputAction.CallbackContext ctx)
    {
        if(canInput)
        {
            if (ctx.started)
            {
                OnJumpInput();
            }
            else if (ctx.canceled)
            {
                OnJumpRelease();
            }
        }
    }
    public void DashMove(InputAction.CallbackContext ctx)
    {
        if(canInput)
        {
            if (ctx.started)
            {
                OnDashInput();
            }
        }
    }


    public void SetGravityTo(float amount)
    {
        rb.gravityScale = amount;
    }

    private void Collisions()
    {
        if (!isDashing && !isJumping)
        {
            if (Physics2D.OverlapBox(groundPoint.position, groundCheckSize, 0, floor) && !isJumping) lastOnGroundTime = coyoteTime;

            if (((Physics2D.OverlapBox(rightPoint.position, wallCheckSize, 0, floor) && isFacingRight) || (Physics2D.OverlapBox(leftPoint.position, wallCheckSize, 0, floor) && !isFacingRight)) && !isWallJumping)
            {
                lastOnWallRightTime = coyoteTime;
                onRightWall = true;
            }
            else 
            {
                onRightWall = false;

            }
            if (((Physics2D.OverlapBox(rightPoint.position, wallCheckSize, 0, floor) && !isFacingRight) || (Physics2D.OverlapBox(leftPoint.position, wallCheckSize, 0, floor) && isFacingRight)) && !isWallJumping)
            {
                lastOnWallLeftTime = coyoteTime;
                onLeftWall = true;
                if (moveInput.x <= 0) CheckDirectionToFace(false);
                else CheckDirectionToFace(true);
            }
            else
            {
                onLeftWall = false;

            }
            if (onRightWall || onLeftWall) onWall = true;
            else onWall = false;
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
        lastPressedDashTime -= Time.deltaTime;
    }
    private void Comprovations()
    {
        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;

            if (!isWallJumping)
                isJumpFalling = true;
        }

        if (isWallJumping && Time.time - _wallJumpStartTime > wallJumpTime)
        {
            isWallJumping = false;
        }

        if (lastOnGroundTime > 0 && !isJumping && !isWallJumping)
        {
            isJumpCut = false;

            if (!isJumping)
                isJumpFalling = false;
        }
        if (!isDashing)
        {
            //jump
            if (CanJump() && lastPressedJumpTime > 0)
            {
                isJumping = true;
                isWallJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                isGroundSlide = false;
                SoundManager.instance.PlaySound(jump);
                Jump();
            }
            //walljump
            else if (CanWallJump() && lastPressedJumpTime > 0)
            {
                isWallJumping = true;
                isJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                isGroundSlide = false;
                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;
                SoundManager.instance.PlaySound(jump);
                WallJump(_lastWallJumpDir);
            }
        }

        if (CanDash() && lastPressedDashTime > 0)
        {
            SoundManager.instance.PlaySound(dash);
            Sleep(dashSleepTime);
            if (moveInput != Vector2.zero) lastDashDir = moveInput;
            else lastDashDir = isFacingRight ? Vector2.right : Vector2.left;

            isDashing = true;
            isJumping = false;
            isGroundSlide = false;
            isWallJumping = false;
            isJumpCut = false;

            StartCoroutine(nameof(StartDash), lastDashDir);
        }
        if (CanSlide() && ((lastOnWallLeftTime > 0 && moveInput.x < 0) || (lastOnWallRightTime > 0 && moveInput.x > 0)))
        {
            isSliding = true;
        }
        else isSliding = false;

        if (canGroundSlide() && lastPressedGroundSlideTime > 0)
        {
            isGroundSlide = true;
            isJumping = false;
            isWallJumping = false;
            isDashing = false;
            isJumpCut = false;
            groundSlide();
        }
        if ((isGroundSlide && lastOnGroundTime != jumpInputBufferTime) || Mathf.Abs(rb.velocity.x) < 0.5f) 
        {
            isGroundSlide = false;
            timeGroundSliding = 0f;
        }

    }
    private void GravityHandling()
    {
        //Just the way you are - Masayoshi Takanaka

        if (!isDashingStarting) { 

            if (isSliding)
            {
                SetGravityTo(0);
            }
            else if (isJumpCut)
            {
                SetGravityTo(gravityScale * jumpCutGravityMult);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));

            }
            else if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
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
        else
        {
            SetGravityTo(0);
        }
    }

    private void Friction()
    {
        if (lastOnGroundTime > 0 && moveInput.x == 0f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void makeDust()
    {
        if (lastOnGroundTime > 0f) {
            if (isFacingRight)
            {
                dust.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                psr.flip = new Vector3(1f, 0f, 0f);
            }
            else
            {
                dust.transform.eulerAngles = new Vector3(0f, 180f, 0f);
                psr.flip = new Vector3(0f, 0f, 0f);
            }
            dust.Play();
        }

    }

    public void OnJumpInput()
    {
        lastPressedJumpTime = jumpInputBufferTime;
    }

    public void OnDashInput()
    {
        lastPressedDashTime = dashInputBufferTime;
    }

    public void OnJumpRelease()
    {
        if (CanJumpCut() || CanWallJumpCut())
            isJumpCut = true;
    }

    public void onGroundSlideInput()
    {
        lastPressedGroundSlideTime = groundSlideInputBufferTime;
        if (!isGroundSlide) groundSlideDirection = moveInput.x;
    }

    public void onGroundSlideRelease()
    {
        if(CanGroundSlideCut())
        {
            isGroundSlide = false;
        }
    }


    private void Turn()
    {
        makeDust();
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    public void Sleep(float duration)
    {
        StartCoroutine(nameof(doSleep), duration);
    }

    private IEnumerator doSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }


    //moviment, aquesta part es conserva del script original pero incorpora lerp per smoothing;

    private void Run(float lerp)
    {
        if (moveInput.x != 0) CheckDirectionToFace(moveInput.x > 0);

        float targetSpeed = moveInput.x * runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerp);

        float accelRate;

        if (lastOnGroundTime > 0) accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
        else accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * AirAccel : runDeccelAmount * Airdeccel;

        //el script bo fa aqui una implementacio de mes acceleracio en el apex del jump, la podriem posar pero mes endevant

        if ((doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0) || isGroundSlide)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed - rb.velocity.x;

        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    private void groundSlide()
    {
        lastPressedGroundSlideTime = 0;
        float targetSpeed = groundSlideDirection * groundSlideSpeed;

        float accelRate;

        if ((doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0))
        {
            accelRate = 0;
        } 
        else
        {
            accelRate = groundSlideAcc;
        }
        float speedDif = targetSpeed - rb.velocity.x;

        float movement = speedDif * accelRate;

        timeGroundSliding += Time.time;

        GameManager.Instance.cameraShake.ShakeWithDuration(0.5f);


        makeDust();

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


    private IEnumerator StartDash(Vector2 dir)
    {
        anim.SetTrigger("Dash");
        lastOnGroundTime = 0;
        lastPressedDashTime = 0;

        float startTime = Time.time;

        dashesLeft--;
        isDashingStarting = true;

        SetGravityTo(0);

        while (Time.time - startTime <= dashStartTime)
        {
            rb.velocity = dir.normalized * dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        isDashingStarting = false;

        SetGravityTo(gravityScale);
        rb.velocity = dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= dashEndTime)
        {
            yield return null;
        }

        isDashing = false;
    }

    private IEnumerator RefillDash(int amount)
    {
        dashRefilling = true;
        yield return new WaitForSeconds(dashRefillTime);
        dashRefilling = false;
        dashesLeft = Mathf.Min(dashAmount, dashesLeft + 1);
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

    private void AnimationHandler()
    {
        float speed = Mathf.Abs(rb.velocity.x / runMaxSpeed);
        anim.SetFloat("speed", Mathf.Abs(speed));
        anim.SetBool("isJumping", isJumping || isWallJumping);
        anim.SetBool("isInAir", lastOnGroundTime != jumpInputBufferTime);
        anim.SetFloat("moveDir", moveInput.x);
        anim.SetBool("isInWall", onWall);
        anim.SetBool("isFacingRight", isFacingRight);
        anim.SetBool("isOnRightWall", onRightWall);
        anim.SetBool("isOnLeftWall", onLeftWall);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isGroundSlide", isGroundSlide);
    }

    //condocions fetes be =)

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight && !isGroundSlide)
            Turn();
    }

    public void Slide()
    {
        float speedDif = wallFallSpeed - rb.velocity.y;
        float movement = speedDif * slideAccel;
        makeDust();
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        rb.AddForce(movement * Vector2.up);
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

    private bool CanGroundSlideCut()
    {
        return isGroundSlide;
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && rb.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!isDashing && dashesLeft < dashAmount && lastOnGroundTime > 0 && !dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return dashesLeft > 0;
    }

    private bool CanSlide()
    {
        return (lastOnWallTime > 0 && !isJumping && !isWallJumping && lastOnGroundTime <= 0);
    }

    private bool canGroundSlide()
    {
        bool slide = (lastOnGroundTime > 0 && !isDashing && !isJumping && !isGroundSlide && moveInput.x != 0);
        return slide;
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