using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerBase : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float jumpForce = 7f;
    [SerializeField] protected float speed = 5f;
    protected bool estaPenalizado = false;

    [Header("Combat")]
    [SerializeField] protected GameObject bulletPrefab;
    protected float lastShootTime;

    [Header("Audio")]
    [SerializeField] protected AudioClip saltoClip;
    [SerializeField] protected AudioClip disparoClip;
    [SerializeField] protected AudioClip powerClip;
    protected AudioSource audioSource;

    [Header("Score")]
    protected int puntaje = 0;

    // Referencias protegidas
    protected Rigidbody2D rb2D;
    protected Animator animator;
    protected ScoreUI scoreUI;
    protected bool isGrounded;
    protected float horizontal;

    // Referencias compartidas
    public ITree ArbolJugador { get; protected set; }
    protected Queue<int> Retos { get; private set; }
    protected TreeManager treeManager;
    protected abstract PowerSystem PowerSystemComponent { get; }

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        scoreUI = FindFirstObjectByType<ScoreUI>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        ConfigureRigidbody();
        InitializeTree();
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
            rb2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }

    protected virtual void InitializeTree()
    {
        // Reference to TreeManager's nodePrefab
        GameObject nodePrefab = treeManager.nodePrefab;

        if (nodePrefab == null)
        {
            Debug.LogError("NodePrefab no encontrado en TreeManager!");
            return;
        }

        // 50% probabilidad de cada tipo de árbol
        if (Random.value > 0.5f)
        {
            var bst = gameObject.AddComponent<BinarySearchTree2D>();
            bst.nodePrefab = nodePrefab;
            ArbolJugador = bst;
        }
        else
        {
            var avl = gameObject.AddComponent<AVLTree>();
            avl.nodePrefab = nodePrefab;
            ArbolJugador = avl;
        }

        GenerarRetos();
    }

    protected bool VerificarReto()
    {
        if (Retos == null || Retos.Count == 0 || ArbolJugador == null)
        {
            return false;
        }

        int profundidadObjetivo = Retos.Peek();
        int profundidadActual = ArbolJugador.CalcularProfundidad();

        if (profundidadActual >= profundidadObjetivo)
        {
            Retos.Dequeue(); // Remover el reto completado
            return true;
        }

        return false;
    }

    protected virtual void GenerarRetos()
    {
        Retos = new Queue<int>();
        for (int i = 0; i < 3; i++)
        {
            Retos.Enqueue(Random.Range(2, 6));
        }
        Debug.Log($"Retos generados para {gameObject.name}: {string.Join(", ", Retos.ToArray())}");
    }

    // Método abstracto que las clases hijas deben implementar
    public virtual void AgregarPuntaje(int valor)
    {
        puntaje += valor;
        if (scoreUI != null)
            scoreUI.UpdatePlayerScore(gameObject.tag, puntaje);
    }

    protected virtual void HandleGroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            0.1f,
            LayerMask.GetMask("Ground")
        );
        Grounded = hit.collider != null;
    }
}