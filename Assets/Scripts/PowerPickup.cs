using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPickup : MonoBehaviour
{
    // Tipo de poder que otorga este objeto
    public PowerSystem.PowerType powerType;

    // Efectos visuales al recoger el poder (opcional)
    public GameObject pickupEffect;

    // Colores para cada tipo de poder (para identificaci�n visual)
    private Dictionary<PowerSystem.PowerType, Color> powerColors = new Dictionary<PowerSystem.PowerType, Color>
    {
        { PowerSystem.PowerType.ForceField, Color.red },
        { PowerSystem.PowerType.Shield, Color.blue },
        { PowerSystem.PowerType.AirJump, Color.green }
    };

    void Start()
    {
        // Asignar color seg�n el tipo de poder
        if (powerColors.ContainsKey(powerType))
        {
            GetComponent<SpriteRenderer>().color = powerColors[powerType];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player2"))
        {
            PowerSystem powerSystem = collision.GetComponent<PowerSystem>();

            if (powerSystem != null)
            {
                powerSystem.ObtainPower(powerType);
                
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }
                
                Destroy(gameObject);
            }
        }
    }
}