using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePushEffect : MonoBehaviour
{
    public float radius = 3f;  // Radio de efecto del Force-Push
    public float forceMagnitude = 20f;  // Fuerza del empuje
    public float effectDuration = 0.2f;  // Duración del efecto visual

    public GameObject visualEffect;  // Prefab del efecto visual (opcional)

    private PowerSystem powerSystem;
    private MagoOscuroMovement playerController;

    void Start()
    {
        powerSystem = GetComponent<PowerSystem>();
        playerController = GetComponent<MagoOscuroMovement>();

        // Suscribirse al evento de activación del Force-Push
        StartCoroutine(CheckForForcePushActivation());
    }

    IEnumerator CheckForForcePushActivation()
    {
        while (true)
        {
            // Verificar si se activó el Force-Push
            if (powerSystem.IsPowerActive(PowerSystem.PowerType.ForceField))
            {
                // Ejecutar el efecto
                ApplyForcePushEffect();

                // Esperar a que termine el efecto
                yield return new WaitForSeconds(effectDuration);
            }

            yield return null;
        }
    }

    void ApplyForcePushEffect()
    {
        // Mostrar efecto visual (opcional)
        if (visualEffect != null)
        {
            GameObject effect = Instantiate(visualEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }

        // Buscar todos los jugadores dentro del radio
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D collider in colliders)
        {
            // Ignorar al jugador que usa el poder
            if (collider.gameObject == gameObject)
                continue;

            // Verificar si es otro jugador
            MagoOscuroMovement otherPlayer = collider.GetComponent<MagoOscuroMovement>();
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Calcular dirección del empuje (desde el jugador hacia el objetivo)
                Vector2 direction = (collider.transform.position - transform.position).normalized;

                // Aplicar la fuerza
                rb.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);

                Debug.Log("Force-Push aplicado a: " + collider.gameObject.name);

                // Si el objetivo es otro jugador, podemos notificarle
                if (otherPlayer != null)
                {
                    // Aquí podrías activar alguna animación o efecto en el otro jugador
                }
            }
        }
    }

    // Método para dibujar el radio de efecto en el editor (solo visible en el editor)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}