using UnityEngine;
using System.Collections;

public class ForceBallScript : MonoBehaviour
{
    public float speed = 15f;
    public float pushForce = 50f; // Aumentada la fuerza para ver mejor el efecto
    private Vector2 direction;
    private string shooterTag;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Vector2 dir, string tag)
    {
        direction = dir.normalized;
        shooterTag = tag;
        Debug.Log($"ForceBall initialized with direction {direction} and tag {tag}");
    }

    private void Update()
    {
        // Mover la bola
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Destruir si sale de la pantalla
        if (mainCamera != null)
        {
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
            if (viewportPoint.x < -0.1f || viewportPoint.x > 1.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"ForceBall collision with: {other.gameObject.name} (Tag: {other.tag})");

        // Ignorar colisión con el lanzador
        if (other.CompareTag(shooterTag))
        {
            Debug.Log($"Ignoring collision with shooter ({shooterTag})");
            return;
        }

        // Verificar si es un jugador válido
        if (other.CompareTag("Player") || other.CompareTag("Player2") || other.CompareTag("Player3"))
        {
            PlayerBase hitPlayer = other.GetComponent<PlayerBase>();
            Rigidbody2D hitRb = other.GetComponent<Rigidbody2D>();

            if (hitRb != null)
            {
                // Detener movimiento actual
                hitRb.linearVelocity = Vector2.zero;
                
                // Aplicar fuerza
                Vector2 pushDirection = direction * pushForce;
                hitRb.AddForce(pushDirection, ForceMode2D.Impulse);
                
                Debug.Log($"Applied force {pushDirection} to {other.gameObject.name}");

                // Deshabilitar movimiento temporalmente
                if (hitPlayer != null)
                {
                    StartCoroutine(DisableMovementTemporarily(hitPlayer));
                }
            }

            // Destruir la bola después del impacto
            Destroy(gameObject);
        }
    }

    private IEnumerator DisableMovementTemporarily(PlayerBase player)
    {
        player.SetMovementEnabled(false);
        yield return new WaitForSeconds(0.5f);
        player.SetMovementEnabled(true);
    }
}