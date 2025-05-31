using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPickup : MonoBehaviour
{
    [Header("Power Settings")]
    public PowerSystem.PowerType powerType;
    public float lifeTime = 5f;
    private bool collected = false;

    [Header("Visual")]
    [SerializeField] private GameObject pickupEffect;
    private SpriteRenderer spriteRenderer;

    private readonly Dictionary<PowerSystem.PowerType, Color> powerColors = new Dictionary<PowerSystem.PowerType, Color>
    {
        { PowerSystem.PowerType.ForceField, Color.red },
        { PowerSystem.PowerType.Shield, Color.blue },
        { PowerSystem.PowerType.AirJump, Color.green }
    };

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (powerColors.ContainsKey(powerType))
        {
            spriteRenderer.color = powerColors[powerType];
        }

        Invoke(nameof(AutoDestroy), lifeTime);
    }

    public void Collect(GameObject player)
    {
        if (collected) return;

        var magoMov = player.GetComponent<MagoOscuroMovement>();
        var huargenMov = player.GetComponent<HuargenMovement>();
        PlayerBase playerBase = magoMov as PlayerBase ?? huargenMov as PlayerBase;

        if (playerBase != null)
        {
            playerBase.PowerSystemComponent.ObtainPower(powerType);
            
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            collected = true;
            Destroy(gameObject);
        }
    }

    private void AutoDestroy()
    {
        if (!collected)
            Destroy(gameObject);
    }
}