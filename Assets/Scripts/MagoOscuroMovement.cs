using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagoOscuroMovement : PlayerBase
{
    public float Speed;
    public float JumpForce;
    public GameObject BulletPrefab;

    public int Puntaje = 0; // de momento esto es para guardar el valor del token que se agarr�
    public AudioClip saltoClip;
    public AudioClip disparoClip;
    public AudioClip powerClip; // Nuevo clip para la activación de poderes

    public Vector3 puntoReaparicion = new Vector3(0, 3, 0);
    public float tiempoPenalizacion = 2f;

    private bool estaPenalizado = false;
    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    public bool Grounded;
    private float LastShoot;
    private AudioSource audioSource;
    [SerializeField] private PowerSystem _powerSystem;
    private ScoreUI scoreUI; // Add this line
    protected override PowerSystem PowerSystemComponent => _powerSystem;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (_powerSystem == null)
        {
            _powerSystem = gameObject.AddComponent<PowerSystem>();
        }
        scoreUI = FindObjectOfType<ScoreUI>();
        
        InitializeTree(); // Inicializar árbol y retos
        ConfigureRigidbody();
    }

    void Update()
    {
        // Si está penalizado, no hacer nada
        if (estaPenalizado) return;

        // Movimiento
        Horizontal = Input.GetAxisRaw("Horizontal");

        // No permitir movimiento si el shield está activo
        if (PowerSystemComponent.IsPowerActive(PowerSystem.PowerType.Shield))
        {
            Horizontal = 0;
        }

        if (Horizontal < 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        Animator.SetBool("running", Horizontal != 0.0f);

        // Detectar Suelo
        Debug.DrawRay(transform.position, Vector3.down * 0.3f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.3f))
        {
            Grounded = true;
        }
        else Grounded = false;

        // Salto
        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Jump();
            Animator.SetTrigger("jump");

            // Reproducir sonido de salto
            if (saltoClip != null)
                audioSource.PlayOneShot(saltoClip);
        }

        // Disparar
        if (Input.GetKey(KeyCode.Alpha1) && Time.time > LastShoot + 0.25f)
        {
            // Verificar si tiene Force Push disponible
            if (PowerSystemComponent.HasPowerStock(PowerSystem.PowerType.ForceField))
            {
                Shoot();
                LastShoot = Time.time;
                PowerSystemComponent.UsePower(PowerSystem.PowerType.ForceField);

                if (disparoClip != null)
                    audioSource.PlayOneShot(disparoClip);
            }
        }
        
        // Detectar caída
        if (!estaPenalizado && transform.position.y < -5f)
        {
            // Comprobar si tenemos Air-Jump disponible
            if (PowerSystemComponent.IsPowerAvailable(PowerSystem.PowerType.AirJump) &&
                !PowerSystemComponent.IsPowerActive(PowerSystem.PowerType.AirJump))
            {
                // Indicación visual para que el jugador sepa que puede usar Air-Jump
                Debug.Log("¡Puedes usar Air-Jump para salvarte! Presiona R");

                // Dar un pequeño tiempo para que el jugador reaccione
                StartCoroutine(CheckForAirJumpRescue());
            }
            else
            {
                StartCoroutine(ReaparecerSuspendido());
            }
            return;
        }

        // Verificar retos
        HandleRetos();
    }

    private IEnumerator CheckForAirJumpRescue()
    {
        // Dar un breve momento para que el jugador use el Air-Jump
        float rescueTime = 1.0f;
        float timer = 0f;

        while (timer < rescueTime)
        {
            // Si el Air-Jump se activó durante este tiempo
            if (PowerSystemComponent.IsPowerActive(PowerSystem.PowerType.AirJump))
            {
                // El poder ya aplicó la fuerza de salto, así que salimos del corrutina
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Si llegamos aquí, el jugador no usó el Air-Jump a tiempo
        if (transform.position.y < -5f)
        {
            StartCoroutine(ReaparecerSuspendido());
        }
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector3.left;
        else direction = Vector3.right;

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.3f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    private IEnumerator ReaparecerSuspendido()
    {
        estaPenalizado = true;

        // Teletransportar al punto de reaparici�n
        transform.position = puntoReaparicion;

        // Congelar f�sica: sin gravedad ni movimiento
        Rigidbody2D.linearVelocity = Vector2.zero;
        Rigidbody2D.gravityScale = 0;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        // Esperar penalizaci�n
        yield return new WaitForSeconds(tiempoPenalizacion);

        // Restaurar gravedad y movimiento
        Rigidbody2D.gravityScale = 1;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        estaPenalizado = false;
    }

    // Método para aplicar el Force-Push a otro jugador
    public void ApplyForcePush(GameObject target)
    {
        if (target == null) return;

        Rigidbody2D targetRB = target.GetComponent<Rigidbody2D>();
        if (targetRB != null)
        {
            // Determinar la dirección del empuje (desde este jugador hacia el objetivo)
            Vector2 pushDirection = (target.transform.position - transform.position).normalized;

            // Aplicar la fuerza
            float pushPower = 20f; // Ajustar según sea necesario
            targetRB.AddForce(pushDirection * pushPower, ForceMode2D.Impulse);

            // Reproducir sonido si está disponible
            if (powerClip != null)
                audioSource.PlayOneShot(powerClip);
        }
    }

    public override void AgregarPuntaje(int valor)
    {
        Puntaje += valor;
        if (scoreUI != null)
        {
            scoreUI.UpdatePlayerScore(gameObject.tag, Puntaje);
        }
    }

    private void ConfigureRigidbody()
    {
        Rigidbody2D.mass = 1f;
        Rigidbody2D.linearDamping = 0f;
        Rigidbody2D.angularDamping = 0.05f;
        Rigidbody2D.gravityScale = 1f;
        Rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        Rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        Rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    private void HandleRetos()
    {
        if (VerificarReto())
        {
            // Update to use PowerSystemComponent directly
            PowerSystemComponent.ObtainPower(PowerSystem.PowerType.ForceField);
            PowerSystemComponent.ObtainPower(PowerSystem.PowerType.Shield);
            PowerSystemComponent.ObtainPower(PowerSystem.PowerType.AirJump);

            // Feedback visual/auditivo
            Debug.Log($"¡{gameObject.name} completó un reto!");
            AgregarPuntaje(100); // Bonus de puntos

            if (powerClip != null)
                audioSource.PlayOneShot(powerClip);
        }
    }
}