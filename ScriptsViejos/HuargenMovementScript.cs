using UnityEngine;

public class HuargenMovement : PlayerBase
{
    public GameObject BulletPrefab;
    public AudioClip disparoClip;
    public AudioClip saltoClip;
    public AudioClip powerClip;
    private AudioSource audioSource;
    private float LastShoot;

    public float Speed;
    public int Puntaje = 0;

    [SerializeField] private HuargenPowerSystem _powerSystem;
    protected override PowerSystem PowerSystemComponent => _powerSystem;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;

    private ScoreUI scoreUI;

    protected override void Start()
    {
        base.Start(); // Llamar primero al Start de PlayerBase

        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        scoreUI = FindFirstObjectByType<ScoreUI>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

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

    private void Update()
    {
        if (!estaPenalizado)
        {
            HandleMovement();
            HandleAnimation();
            HandleGroundCheck();
            HandleJump();
            HandleShooting();
            HandleRetos();
        }
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

    protected override void HandleGroundCheck()
    {
        base.HandleGroundCheck();
        // Si necesitas lógica adicional específica del Huargen, agrégala aquí
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
        Rigidbody2D.AddForce(Vector2.up * JumpForce); // Usar la propiedad heredada de PlayerBase
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Alpha7) && Time.time > LastShoot + 0.25f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!PowerSystemComponent.HasPowerStock(PowerSystem.PowerType.ForceField))
            return;

        Vector3 direction = transform.localScale.x == 1.0f ? Vector3.left : Vector3.right;
        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.3f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);

        LastShoot = Time.time;
        PowerSystemComponent.UsePower(PowerSystem.PowerType.ForceField);

        if (disparoClip != null)
            audioSource.PlayOneShot(disparoClip);
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
