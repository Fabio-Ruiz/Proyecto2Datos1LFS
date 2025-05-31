using UnityEngine;

public class Player3Movement : PlayerBase
{
    [SerializeField] private Player3PowerHandler powerHandler;
    protected override PowerSystem PowerSystemComponent => powerHandler?.PowerSystem;

    protected override void Start()
    {
        playerTag = "Player3";
        base.Start();
        
        if (powerHandler == null)
        {
            powerHandler = GetComponent<Player3PowerHandler>();
            if (powerHandler == null)
                powerHandler = gameObject.AddComponent<Player3PowerHandler>();
        }
    }

    protected override void InitializeTree()
    {
        base.InitializeTree();
        if (ArbolJugador == null)
        {
            // Create tree instance through prefab instantiation
            Vector3 treePosition = GetTreePosition();
            GameObject treeObj = Instantiate(TreePrefab, treePosition, Quaternion.identity);
            treeObj.name = $"{playerTag}_Tree";
            ArbolJugador = treeObj.GetComponent<ITree>();

            if (ArbolJugador == null)
            {
                Debug.LogError($"No se pudo encontrar componente ITree en {TreePrefab.name} para Player3");
            }
        }
    }

    protected override void HandleMovement()
    {
        // Get controller input first
        horizontal = Input.GetAxis(GameInput.Player3.HORIZONTAL_AXIS);

        // Fallback to keyboard if no controller input
        if (Mathf.Abs(horizontal) < 0.1f)
        {
            if (Input.GetKey(GameInput.Player3.LEFT)) horizontal = -1f;
            if (Input.GetKey(GameInput.Player3.RIGHT)) horizontal = 1f;
        }

        if (CanMove())
        {
            rb2D.linearVelocity = new Vector2(horizontal * speed, rb2D.linearVelocity.y);
            transform.localScale = new Vector3(horizontal > 0 ? -1 : (horizontal < 0 ? 1 : transform.localScale.x), 1, 1);
        }
    }

    protected override void HandleJump()
    {
        if ((Input.GetKeyDown(GameInput.Player3.JUMP) || 
             Input.GetKeyDown(GameInput.Player3.JUMP_BUTTON)) && 
            isGrounded)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator?.SetTrigger("jump");
            
            if (saltoClip != null)
                audioSource.PlayOneShot(saltoClip);
        }
    }

    private void PerformJump()
    {
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);
        Vector2 jumpForceVector = Vector2.up * jumpForce;
        rb2D.AddForce(jumpForceVector, ForceMode2D.Impulse);

        animator?.SetTrigger("jump");
        if (saltoClip != null)
            audioSource.PlayOneShot(saltoClip);
    }

    private void FixedUpdate()
    {
        if (!IsPenalized)
        {
            rb2D.linearVelocity = new Vector2(horizontal * speed, rb2D.linearVelocity.y);
        }
    }

    protected override void Update()
    {
        HandleGroundCheck();
        HandleFallCheck();
        
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
        if (Input.GetKeyDown(GameInput.Player3.AIR_JUMP) && !isGrounded)
        {
            if (PowerSystemComponent != null)
            {
                PowerSystemComponent.UsePower(PowerSystem.PowerType.AirJump);
                PerformJump();
            }
        }
    }
}