using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    public GameObject shieldVisual;  // El objeto visual del escudo (un sprite circular)
    public float shieldDuration = 3f;  // Duración del escudo en segundos

    private PowerSystem powerSystem;
    private bool isShieldActive = false;

    void Start()
    {
        powerSystem = GetComponent<PowerSystem>();

        // Crear el objeto visual del escudo si no existe
        if (shieldVisual == null)
        {
            // Crear un objeto hijo para el escudo
            GameObject shield = new GameObject("Shield");
            shield.transform.parent = transform;
            shield.transform.localPosition = Vector3.zero;

            // Añadir un SpriteRenderer
            SpriteRenderer shieldRenderer = shield.AddComponent<SpriteRenderer>();

            // Intentar cargar un sprite circular (puedes usar uno predeterminado de Unity)
            shieldRenderer.sprite = Resources.Load<Sprite>("Circle");

            // Si no se puede cargar, usar un sprite cualquiera y ajustarlo
            if (shieldRenderer.sprite == null)
            {
                Debug.LogWarning("No se encontró un sprite circular para el escudo. Por favor asigna uno manualmente.");
            }

            // Configurar el renderer
            shieldRenderer.color = new Color(0.2f, 0.5f, 1f, 0.5f);  // Azul semi-transparente
            shield.transform.localScale = new Vector3(2.5f, 2.5f, 1f);  // Ajustar tamaño

            // Guardar referencia
            shieldVisual = shield;
        }

        // Inicialmente desactivado
        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        // Iniciar la comprobación del escudo
        StartCoroutine(CheckShieldActivation());
    }

    IEnumerator CheckShieldActivation()
    {
        while (true)
        {
            // Si el escudo se activa
            if (powerSystem.IsPowerActive(PowerSystem.PowerType.Shield) && !isShieldActive)
            {
                ActivateShield();
            }
            // Si el escudo está activo pero el power system dice que ya no
            else if (isShieldActive && !powerSystem.IsPowerActive(PowerSystem.PowerType.Shield))
            {
                DeactivateShield();
            }

            yield return null;
        }
    }

    void ActivateShield()
    {
        isShieldActive = true;

        // Activar el visual
        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        Debug.Log("Escudo activado");
    }

    void DeactivateShield()
    {
        isShieldActive = false;

        // Desactivar el visual
        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        Debug.Log("Escudo desactivado");
    }

    // Este método maneja la lógica cuando un proyectil u otro objeto colisiona con el escudo
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo procesar si el escudo está activo
        if (!isShieldActive) return;

        // Verificar si es un proyectil (necesitarás tener un tag "Bullet" en tus proyectiles)
        if (collision.CompareTag("Bullet"))
        {
            // Destruir o rebotar el proyectil
            Destroy(collision.gameObject);

            // Efecto visual opcional (partículas, sonido, etc.)
            Debug.Log("¡Proyectil bloqueado por el escudo!");
        }
    }
}