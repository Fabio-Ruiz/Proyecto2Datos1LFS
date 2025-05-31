using UnityEngine;

public class HuargenMovement : PlayerBase
{
    public GameObject BulletPrefab;
    public float Speed;
    public float JumpForce;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    
    public int Puntaje = 0;
    private ScoreUI scoreUI;
    [SerializeField] private HuargenPowerSystem _powerSystem;

    protected override PowerSystem PowerSystemComponent => _powerSystem;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        scoreUI = FindObjectOfType<ScoreUI>();
        
        if (_powerSystem == null)
            _powerSystem = gameObject.AddComponent<HuargenPowerSystem>();

        InitializeTree(); // Inicializar árbol y retos
        ConfigureRigidbody();
    }

    private void ConfigureRigidbody()
    {
        Rigidbody2D.mass = 1f;
        Rigidbody2D.linearDamping = 0f;                  // Sin resistencia al aire
        Rigidbody2D.angularDamping = 0.05f;
        Rigidbody2D.gravityScale = 1f;
        Rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        Rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        Rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;  // Mantener física activa
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
        HandleGroundCheck();
        HandleJump();
        HandleRetos(); // Añadir esta línea
    }

    private void HandleMovement()
    {
        Horizontal = 0;
        if (Input.GetKey(KeyCode.J)) Horizontal = -1;
        if (Input.GetKey(KeyCode.L)) Horizontal = 1;

        if(Horizontal > 0.0f) 
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal < 0.0f) 
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private void HandleAnimation()
    {
        Animator.SetBool("running", Horizontal != 0.0f);
    }

    private void HandleGroundCheck()
    {
        Grounded = Physics2D.Raycast(transform.position, Vector3.down, 0.3f);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.I) && Grounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

    private void HandleRetos()
    {
        if (VerificarReto())
        {
            // Use PowerSystemComponent directly instead of GetPowerSystem()
            if (PowerSystemComponent != null)
            {
                PowerSystemComponent.ObtainPower(PowerSystem.PowerType.ForceField);
                PowerSystemComponent.ObtainPower(PowerSystem.PowerType.Shield);
                PowerSystemComponent.ObtainPower(PowerSystem.PowerType.AirJump);
            }

            // Feedback visual/auditivo
            Debug.Log($"¡{gameObject.name} completó un reto!");
            AgregarPuntaje(100); // Bonus de puntos por completar reto
        }
    }

    // Cambiar de public a protected y añadir override
    public override void AgregarPuntaje(int valor)
    {
        Puntaje += valor;
        if (scoreUI != null)
            scoreUI.UpdatePlayerScore(gameObject.tag, Puntaje);
    }
}
