using UnityEngine;

public class HuargenMovement : PlayerBase
{
    [SerializeField] private HuargenPowerHandler powerHandler;
    protected override PowerSystem PowerSystemComponent => powerHandler?.PowerSystem;

    protected override void Start()
    {
        playerTag = "Player2";
        base.Start();
        
        if (powerHandler == null)
        {
            powerHandler = GetComponent<HuargenPowerHandler>();
            if (powerHandler == null)
                powerHandler = gameObject.AddComponent<HuargenPowerHandler>();
        }
    }

    protected override void HandleMovement()
    {
        horizontal = 0;
        if (Input.GetKey(KeyCode.J)) horizontal = -1;
        if (Input.GetKey(KeyCode.L)) horizontal = 1;

        if (PowerSystemComponent.IsPowerActive(PowerSystem.PowerType.Shield))
        {
            horizontal = 0;
        }

        if (horizontal < 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else if (horizontal > 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        animator?.SetBool("running", horizontal != 0.0f); // Using base class animator
    }

    protected override void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.I) && isGrounded)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f); // Using base class rb2D
        Vector2 jumpForceVector = Vector2.up * jumpForce;
        rb2D.AddForce(jumpForceVector, ForceMode2D.Impulse);

        animator?.SetTrigger("jump"); // Using base class animator
        if (saltoClip != null)
            audioSource.PlayOneShot(saltoClip);
    }

    private void FixedUpdate()
    {
        if (!IsPenalized)
        {
            rb2D.linearVelocity = new Vector2(horizontal * speed, rb2D.linearVelocity.y); // Using base class variables
        }
    }

    protected override void InitializeTree()
    {
        base.InitializeTree();  // Solo llamar al método base
    }

    protected override void Update()
    {
        HandleGroundCheck();
        HandleFallCheck();  // Added this line
        
        if (!IsPenalized)
        {
            HandleMovement();
            HandleAnimation();
            HandleJump();
            HandleAirJump();
            powerHandler?.HandlePowerInput();
        }
    }

    private void HandleAirJump()
    {
        if (Input.GetKeyDown(GameInput.Player2.AIR_JUMP) && !isGrounded)
        {
            if (PowerSystemComponent != null)
            {
                PowerSystemComponent.UsePower(PowerSystem.PowerType.AirJump);
                PerformJump();
            }
        }
    }
}