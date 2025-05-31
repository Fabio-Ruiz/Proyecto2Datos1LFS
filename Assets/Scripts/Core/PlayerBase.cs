using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerBase : MonoBehaviour
{
    // Componentes UI
    protected ScoreUI scoreUI;

    // Árbol del jugador
    protected ITree ArbolJugador;
    public ITree Tree => ArbolJugador;
    [SerializeField] protected GameObject TreePrefab;

    // Movement Components & State
    protected Rigidbody2D rb2D;
    protected Animator animator;
    protected AudioSource audioSource;
    protected float horizontal;
    protected bool canMove = true;
    protected float lastShootTime;

    [Header("Movement")]
    [SerializeField] protected float jumpForce = 7f;
    [SerializeField] protected float speed = 5f;

    [Header("Combat")]
    [SerializeField] protected GameObject bulletPrefab; // Asignable desde Inspector

    [Header("Audio")]
    [SerializeField] protected AudioClip saltoClip;    // Asignable desde Inspector
    [SerializeField] protected AudioClip disparoClip;   // Asignable desde Inspector
    [SerializeField] protected AudioClip powerClip;     // Asignable desde Inspector

    [Header("Score")]
    protected int puntaje = 0;  // <-- Mantener esta declaración con el header

    [Header("Respawn Settings")]
    protected Vector3 puntoReaparicion = Vector3.zero; // Punto central del mapa
    protected float tiempoPenalizacion = 2f;
    protected bool estaPenalizado = false;  // Keep this one under Respawn Settings

    // Referencias protegidas
    protected bool isGrounded;
    protected abstract PowerSystem PowerSystemComponent { get; }

    [Header("Player Identity")]
    protected string playerTag;
    public bool IsPenalized => estaPenalizado;

    // Stats
    public PlayerStats Stats { get; private set; }

    public float JumpForce => jumpForce;
    public bool Grounded => isGrounded;
    
    private bool isInvulnerable = false;

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
        // Optional: Change sprite alpha or add visual effect
        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            Color color = renderer.color;
            color.a = invulnerable ? 0.7f : 1f;
            renderer.color = color;
        }
    }

    public bool IsInvulnerable() => isInvulnerable;

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // Initialize ScoreUI reference
        scoreUI = FindObjectOfType<ScoreUI>();
        if (scoreUI == null)
        {
            Debug.LogError("ScoreUI not found in scene!");
        }
        
        // Initialize Stats
        Stats = new PlayerStats();
        
        // Initialize tree
        InitializeTree();
        
        // Configure physics
        ConfigureRigidbody();

        // Log de inicialización
        Debug.Log($"Inicializado {gameObject.name} con tag {playerTag}");
        Debug.Log($"Árbol inicializado: {(ArbolJugador != null ? "Sí" : "No")}");
    }

    protected virtual void InitializeTree()
    {
        if (TreePrefab == null)
        {
            Debug.LogError($"TreePrefab no asignado para {gameObject.name}");
            return;
        }

        // Crear el árbol en una posición específica basada en el jugador
        Vector3 treePosition = GetTreePosition();
        GameObject treeObj = Instantiate(TreePrefab, treePosition, Quaternion.identity);
        treeObj.name = $"{playerTag}_Tree";
        ArbolJugador = treeObj.GetComponent<ITree>();
        
        if (ArbolJugador == null)
        {
            Debug.LogError($"No se pudo encontrar componente ITree en {TreePrefab.name} para {gameObject.name}");
        }
        else
        {
            Debug.Log($"Árbol creado exitosamente para {gameObject.name} en posición {treePosition}");
        }
    }

    protected virtual Vector3 GetTreePosition()
    {
        switch (playerTag)
        {
            case "Player": return new Vector3(-6f, 3f, 0f);
            case "Player2": return new Vector3(0f, 3f, 0f);
            case "Player3": return new Vector3(6f, 3f, 0f);
            default: return Vector3.zero;
        }
    }

    public virtual void AgregarPuntaje(int valor)
    {
        puntaje += valor;
        
        if (ArbolJugador != null)
        {
            ArbolJugador.Insert(valor);
        }
        
        if (scoreUI != null)
        {
            scoreUI.UpdatePlayerScore(playerTag, puntaje);
            Debug.Log($"Updated score for {playerTag}: {puntaje}");
        }
        else
        {
            Debug.LogError($"ScoreUI is null for {gameObject.name}");
        }
    }

    protected virtual void HandleTreeDepthChallenge()
    {
        if (Tree == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Tree is null in HandleTreeDepthChallenge");
            return;
        }

        int currentDepth = Tree.CalcularProfundidad();
        
        // Find ChallengeManager if not already referenced
        var challengeManager = FindObjectOfType<ChallengeManager>();
        if (challengeManager == null)
        {
            Debug.LogError($"[{gameObject.name}] ChallengeManager not found in scene");
            return;
        }

        bool challengeCompleted = challengeManager.CheckChallengeCompletion(playerTag, currentDepth);
        if (challengeCompleted)
        {
            // Optional: Add visual/audio feedback when challenge is completed
            Debug.Log($"[{gameObject.name}] Completed tree depth challenge! Current depth: {currentDepth}");
        }
    }

    public virtual void HandleTokenCollection(int valor)
    {
        if (Tree == null)
        {
            Debug.LogError($"[{gameObject.name}] Cannot collect token - Tree is null");
            return;
        }

        try
        {
            Tree.Insert(valor);
            HandleTreeDepthChallenge();
        }
        catch (Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error processing token: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    protected virtual void ConfigureRigidbody()
    {
        if (rb2D != null)
        {
            rb2D.mass = 1f;
            rb2D.linearDamping = 0f;
            rb2D.angularDamping = 0.05f;
            rb2D.gravityScale = 1f;
            rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    protected virtual void HandleGroundCheck()
    {
        // Usamos el tamaño del collider para el raycast
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return;

        // Calculamos el origen desde la parte inferior del collider
        Vector2 rayOrigin = new Vector2(
            transform.position.x,
            transform.position.y - (collider.bounds.size.y / 2f)
        );
        
        float rayDistance = 0.1f;
        int layerMask = LayerMask.GetMask("Default");

        // Lanzamos tres raycasts para mejor detección
        bool leftGrounded = Physics2D.Raycast(rayOrigin - new Vector2(0.1f, 0), Vector2.down, rayDistance, layerMask);
        bool centerGrounded = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, layerMask);
        bool rightGrounded = Physics2D.Raycast(rayOrigin + new Vector2(0.1f, 0), Vector2.down, rayDistance, layerMask);

        isGrounded = leftGrounded || centerGrounded || rightGrounded;

        // Debug visual
        Color debugColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin - new Vector2(0.1f, 0), Vector2.down * rayDistance, debugColor);
        Debug.DrawRay(rayOrigin, Vector2.down * rayDistance, debugColor);
        Debug.DrawRay(rayOrigin + new Vector2(0.1f, 0), Vector2.down * rayDistance, debugColor);
        
        Debug.Log($"Ground Check - Left: {leftGrounded}, Center: {centerGrounded}, Right: {rightGrounded}");
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            rb2D.AddForce(direction * GameConstants.COLLISION_FORCE, ForceMode2D.Impulse);
        }
    }

    protected abstract void Update();

    protected virtual void HandleJump()
    {
        // Usar el sistema de input en lugar de KeyCode directamente
        KeyCode jumpKey = GetJumpKey();
        
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator?.SetTrigger("jump");
            
            if (saltoClip != null)
                audioSource.PlayOneShot(saltoClip);
        }
    }

    protected virtual KeyCode GetJumpKey()
    {
        return gameObject.CompareTag("Player1") ? 
            GameInput.Player1.JUMP : GameInput.Player2.JUMP;
    }

    protected virtual void HandleShooting()
    {
        KeyCode shootKey = gameObject.CompareTag("Player1") ? 
            GameInput.Player1.FORCE_PUSH : GameInput.Player2.FORCE_PUSH;

        if (Input.GetKey(shootKey) && Time.time > lastShootTime + 0.25f)
        {
            Shoot();
        }
    }

    protected virtual void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError($"Bullet Prefab no asignado en {gameObject.name}");
            return;
        }

        if (!PowerSystemComponent.HasPowerStock(PowerSystem.PowerType.ForceField))
            return;

        Vector3 direction = transform.localScale.x == 1.0f ? Vector3.left : Vector3.right;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + direction * 0.3f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);

        lastShootTime = Time.time;
        PowerSystemComponent.UsePower(PowerSystem.PowerType.ForceField);

        if (disparoClip != null)
            audioSource.PlayOneShot(disparoClip);
    }

    protected virtual void HandleAnimation()
    {
        // Base implementation
    }

    protected virtual void HandleMovement()
    {
        // Base implementation
    }

    protected virtual void HandleRetos()
    {
        // Base implementation
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    protected virtual bool CanMove()
    {
        return canMove && !estaPenalizado;
    }

    public void AddPowerStock(PowerSystem.PowerType powerType, int amount = 1)
    {
        if (Stats != null)
        {
            Stats.AddPowerStock(powerType, amount);
            Debug.Log($"{playerTag}: Added {amount} stock to {powerType}. New total: {Stats.GetPowerStock(powerType)}");
        }
        else
        {
            Debug.LogError($"{playerTag}: Can't add power stock - Stats is null");
        }
    }

    protected virtual void PerformAirJump()
    {
        // Reset velocidad vertical antes del salto
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);
        
        // Aplicar fuerza de salto con multiplicador
        Vector2 airJumpForce = Vector2.up * (jumpForce * PowerConstants.AIR_JUMP_MULTIPLIER);
        rb2D.AddForce(airJumpForce, ForceMode2D.Impulse);
        
        Debug.Log($"{playerTag} realizó Air Jump con fuerza: {airJumpForce}");
        
        // Efectos
        animator?.SetTrigger("jump");
        if (saltoClip != null)
            audioSource.PlayOneShot(saltoClip);
    }

    protected virtual void HandleFallCheck()
    {
        if (!estaPenalizado && transform.position.y < GameConstants.FALL_THRESHOLD)
        {
            StartCoroutine(ReaparecerSuspendido());
        }
    }

    protected virtual IEnumerator ReaparecerSuspendido()
    {
        estaPenalizado = true;
        Debug.Log($"{gameObject.name} cayó y será reposicionado");

        // Congelar al jugador
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.gravityScale = 0;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Teletransportar al punto de reaparición
        transform.position = new Vector3(0, 2, 0); // Punto central del mapa, ligeramente elevado

        // Esperar la penalización
        yield return new WaitForSeconds(tiempoPenalizacion);

        // Restaurar física
        if (rb2D != null)
        {
            rb2D.gravityScale = 1;
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        estaPenalizado = false;
        Debug.Log($"{gameObject.name} ha reaparecido");
    }
}