using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagoOscuroMovement : PlayerBase
{
    public float Speed;
    public int Puntaje = 0;

    // Remover las variables de audio duplicadas
    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private float LastShoot;
    private ScoreUI scoreUI;
    [SerializeField] private PowerSystem _powerSystem;
    protected override PowerSystem PowerSystemComponent => _powerSystem;

    protected override void Start()
    {
        base.Start();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        // Remover inicialización de audioSource ya que viene de la clase base
        if (_powerSystem == null)
            _powerSystem = gameObject.AddComponent<PowerSystem>();
        
        scoreUI = FindFirstObjectByType<ScoreUI>();
        InitializeTree();
        ConfigureRigidbody();
    }

    void Update()
    {
        // Si está penalizado, no hacer nada
        if (estaPenalizado) return;

        HandleMovement();
        HandleAnimation();
        HandleGroundCheck();
        HandleJump();
        HandleShooting();
        HandleRetos();
    }

    private void HandleMovement()
    {
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
    }

    private void HandleAnimation()
    {
        // Aquí puedes manejar las animaciones relacionadas con el movimiento
    }

    // Salto
    private void HandleJump()
    {
        // Salto
        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Jump();
            Animator.SetTrigger("jump");

            // Reproducir sonido de salto
            if (saltoClip != null)
                audioSource.PlayOneShot(saltoClip);
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
        Rigidbody2D.AddForce(Vector2.up * JumpForce);  // Usar la propiedad de PlayerBase
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Alpha1) && Time.time > LastShoot + 0.25f)
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
        if (ArbolJugador != null)
        {
            ArbolJugador.Insert(valor);
            Debug.Log($"Valor {valor} insertado en el árbol del MagoOscuro");
            
            // Actualizar el Score UI
            if (scoreUI != null)
            {
                scoreUI.UpdatePlayerScore("Player", Puntaje);
            }
            else
            {
                Debug.LogWarning("ScoreUI not found in MagoOscuroMovement");
            }
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

    protected override void HandleGroundCheck()
    {
        base.HandleGroundCheck();  // Opcional: llamar a la implementación base
        // Agregar lógica adicional específica del MagoOscuro si es necesaria
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