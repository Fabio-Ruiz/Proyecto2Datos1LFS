using UnityEngine;
using System.Collections;

public class MagoOscuroMovement : PlayerBase
{
    [SerializeField] private MagoOscuroPowerHandler powerHandler;
    // Now this will work because PowerHandler exposes PowerSystem
    protected override PowerSystem PowerSystemComponent => powerHandler?.PowerSystem;

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
        }
    }

    protected override void Start()
    {
        playerTag = "Player";
        base.Start();
        
        // Find or create ScoreUI reference
        scoreUI = FindObjectOfType<ScoreUI>();
        if (scoreUI == null)
        {
            Debug.LogError("ScoreUI not found in scene!");
        }
        
        // Initialize PowerHandler
        if (powerHandler == null)
        {
            powerHandler = GetComponent<MagoOscuroPowerHandler>();
            if (powerHandler == null)
            {
                powerHandler = gameObject.AddComponent<MagoOscuroPowerHandler>();
            }
        }
    }

    protected override void Update()
    {
        HandleGroundCheck();
        HandleFallCheck();  // Add this line
        
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
        if (Input.GetKeyDown(GameInput.Player1.AIR_JUMP) && !isGrounded)
        {
            if (PowerSystemComponent != null)
            {
                PowerSystemComponent.UsePower(PowerSystem.PowerType.AirJump);
                PerformJump();
            }
        }
    }

    protected override void HandleMovement()
    {
        horizontal = 0;
        if (Input.GetKey(GameInput.Player1.LEFT)) horizontal = -1;
        if (Input.GetKey(GameInput.Player1.RIGHT)) horizontal = 1;
        
        // Add null check for PowerSystemComponent
        if (PowerSystemComponent != null && PowerSystemComponent.IsPowerActive(PowerSystem.PowerType.Shield))
        {
            horizontal = 0;
        }

        if (horizontal < 0.0f) 
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else if (horizontal > 0.0f) 
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        animator?.SetBool("running", horizontal != 0.0f);
    }

    protected override void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Debug.Log("Attempting to jump..."); // Debug temporal
            PerformJump();
        }
    }

    // Método auxiliar para realizar el salto
    private void PerformJump()
    {
        // Reset velocidad vertical antes del salto
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);
        
        // Aplicar fuerza de salto
        Vector2 jumpForceVector = Vector2.up * jumpForce;
        rb2D.AddForce(jumpForceVector, ForceMode2D.Impulse);
        
        Debug.Log($"Jump force applied: {jumpForceVector}"); // Debug temporal
        
        // Efectos
        animator?.SetTrigger("jump");
        if (saltoClip != null)
            audioSource.PlayOneShot(saltoClip);
    }

    private void FixedUpdate()
    {
        if (!IsPenalized)
        {
            float currentVelY = rb2D.linearVelocity.y;
            rb2D.linearVelocity = new Vector2(horizontal * speed, currentVelY);
        }
    }

    public override void AgregarPuntaje(int valor)
    {
        base.AgregarPuntaje(valor);
        // Lógica específica del MagoOscuro si es necesaria
    }

    // ... rest of specific MagoOscuro behavior ...
}