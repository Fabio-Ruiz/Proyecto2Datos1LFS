using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSystem : MonoBehaviour
{
    // Enumeraci�n para identificar los tipos de poderes
    public enum PowerType
    {
        None,
        ForceField,
        Shield,
        AirJump
    }


    // Variables de configuración para los poderes
    [Header("Configuración de Poderes")]
    public float airJumpForceMultiplier = 1.2f;  // Multiplicador de fuerza para el Air-jump
    // Clase para gestionar cada poder
    [System.Serializable]
    public class Power
    {
        public PowerType type;
        public bool isActive = false;
        public bool isAvailable = true;      // Añadido
        public float cooldown = 1f;          // Añadido
        public int stockCount = 0;        // Cantidad acumulada del poder
        public const int MAX_STOCK = 20;  // Máximo de poderes acumulables
        public KeyCode activationKey;
        public bool inCooldown = false;
    }

    // Lista de todos los poderes disponibles
    public List<Power> powers = new List<Power>();

    // Referencias a componentes
    private Rigidbody2D rb;
    private PlayerBase playerController; // Cambiar a PlayerBase en lugar de MagoOscuroMovement

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerBase>(); // Obtener PlayerBase genérico
        
        if (playerController == null)
        {
            Debug.LogError("No se encontró componente PlayerBase en " + gameObject.name);
        }

        // Inicializar poderes
        InitializePowers();
    }

    protected virtual void InitializePowers()
    {
        // Poder Force-push
        Power forcePush = new Power
        {
            type = PowerType.ForceField,
            activationKey = KeyCode.Alpha1  // Tecla 1
        };
        powers.Add(forcePush);

        // Poder Shield
        Power shield = new Power
        {
            type = PowerType.Shield,
            activationKey = KeyCode.Alpha2  // Tecla 2
        };
        powers.Add(shield);

        // Poder Air-jump
        Power airJump = new Power
        {
            type = PowerType.AirJump,
            activationKey = KeyCode.Alpha3  // Tecla 3
        };
        powers.Add(airJump);
    }

    void Update()
    {
        // Revisar activación de poderes
        foreach (Power power in powers)
        {
            if (power.stockCount > 0 && !power.inCooldown && Input.GetKeyDown(power.activationKey))
            {
                ActivatePower(power.type);
            }
        }
    }

    // M�todo para activar un poder
    void ActivatePower(PowerType type)
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController es null en " + gameObject.name);
            return;
        }

        Power power = powers.Find(p => p.type == type);
        if (power != null && power.stockCount > 0 && !power.inCooldown)
        {
            Debug.Log($"Activando poder: {type.ToString()}. Restantes: {power.stockCount - 1}");
            power.isActive = true;
            power.stockCount--;

            switch (type)
            {
                case PowerType.ForceField:
                    StartCoroutine(DeactivatePowerAfterDelay(power, 0.5f));
                    break;

                case PowerType.Shield:
                    StartCoroutine(DeactivatePowerAfterDelay(power, 3f));
                    break;

                case PowerType.AirJump:
                    if (!playerController.Grounded)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                        rb.AddForce(Vector2.up * playerController.JumpForce * airJumpForceMultiplier);
                        StartCoroutine(DeactivatePowerAfterDelay(power, 0.5f));
                    }
                    else
                    {
                        power.stockCount++; // Devolver el poder si no se puede usar
                        Debug.Log("Air-jump solo se puede usar en el aire");
                        return;
                    }
                    break;
            }
        }
    }

    // M�todo para desactivar un poder despu�s de cierto tiempo
    IEnumerator DeactivatePowerAfterDelay(Power power, float delay)
    {
        yield return new WaitForSeconds(delay);
        power.isActive = false;

        // Aplicar cooldown
        power.inCooldown = true;
        yield return new WaitForSeconds(power.cooldown);
        power.inCooldown = false;

        Debug.Log("Poder " + power.type.ToString() + " ha terminado su cooldown");
    }

    // Obtener estado de un poder
    public bool IsPowerAvailable(PowerType type)
    {
        Power power = powers.Find(p => p.type == type);
        return power != null && power.stockCount > 0;
    }

    public bool IsPowerActive(PowerType type)
    {
        Power power = powers.Find(p => p.type == type);
        return power != null && power.isActive;
    }

    // M�todo para obtener un poder al colisionar con un objeto
    public void ObtainPower(PowerType type)
    {
        foreach (Power power in powers)
        {
            if (power.stockCount < Power.MAX_STOCK)
            {
                power.stockCount++;
                Debug.Log($"¡Has obtenido +1 {power.type}! Total: {power.stockCount}");
            }
        }
    }

    public bool HasPowerStock(PowerType type)
    {
        Power power = powers.Find(p => p.type == type);
        return power != null && power.stockCount > 0;
    }

    public void UsePower(PowerType type)
    {
        Power power = powers.Find(p => p.type == type);
        if (power != null && power.stockCount > 0)
        {
            power.stockCount--;
        }
    }
}