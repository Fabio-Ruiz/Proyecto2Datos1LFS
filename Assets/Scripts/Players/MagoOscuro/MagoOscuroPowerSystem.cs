using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagoOscuroPowerSystem : PowerSystem
{
    private PlayerStats stats;

    [Header("Power Effects")]
    [SerializeField] private ShieldEffect shieldEffect;
    [SerializeField] private AirJumpEffect airJumpEffect;
    [SerializeField] private ForcePushEffect forcePushEffect;

    [Header("Power Settings")]
    [SerializeField] private float airJumpForceMultiplier = 1.2f;

    [SerializeField] private PowerStatusUI powerUI;  // Keep it here

    protected override void Start()
    {
        playerTag = "Player"; // Set this before base.Start()
        base.Start();
        
        // Initialize stats first
        var playerBase = GetComponent<PlayerBase>();
        if (playerBase != null)
        {
            stats = playerBase.Stats;
            if (stats == null)
            {
                Debug.LogError("PlayerStats not initialized in PlayerBase");
                stats = new PlayerStats();
            }
        }
        else
        {
            Debug.LogError("PlayerBase component not found");
            return;
        }

        powerUI = FindObjectOfType<PowerStatusUI>();
        InitializeEffects();
        InitializePowerData();
    }

    private void InitializeEffects()
    {
        if (shieldEffect == null) shieldEffect = gameObject.AddComponent<ShieldEffect>();
        if (airJumpEffect == null) airJumpEffect = gameObject.AddComponent<AirJumpEffect>();
        if (forcePushEffect == null) forcePushEffect = gameObject.AddComponent<ForcePushEffect>();
    }

    private void InitializePowerData()
    {
        // Initialize power stocks directly through AddPowerStock
        foreach (PowerType type in System.Enum.GetValues(typeof(PowerType)))
        {
            if (type != PowerType.None)
            {
                // Set initial stock to max for each power
                int currentStock = stats.GetPowerStock(type);
                int difference = PowerData.MAX_STOCK - currentStock;
                if (difference > 0)
                {
                    stats.AddPowerStock(type, difference);
                }
            }
        }
    }

    public void ObtainPower(PowerType type)
    {
        AddPowerStock(type);
        if (powerUI != null)
        {
            powerUI.UpdatePowerStatus("Player", type, stats.PowerStocks[type]); // Changed from "Player1" to "Player"
        }
        Debug.Log($"¡Has obtenido +1 {type}! Total: {stats.PowerStocks[type]}");
    }

    public override bool HasPowerStock(PowerType powerType)
    {
        return stats.PowerStocks[powerType] > 0;
    }

    public override void UsePower(PowerType powerType)
    {
        if (stats.UsePowerStock(powerType))
        {
            powerUI?.UpdatePowerStatus("Player", powerType, stats.PowerStocks[powerType]);
            ActivatePowerEffect(powerType);
            StartCoroutine(StartPowerCooldown(powerType));
        }
    }

    private void ActivatePowerEffect(PowerType powerType)
    {
        if (!powers[powerType].isActive)
        {
            powers[powerType].isActive = true;
            
            switch (powerType)
            {
                case PowerType.ForceField:
                    forcePushEffect.SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
                    StartCoroutine(DeactivatePowerAfterDelay(powerType, 0.5f));
                    break;
                    
                case PowerType.Shield:
                    shieldEffect.SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
                    StartCoroutine(DeactivatePowerAfterDelay(powerType, 3f));
                    break;
                    
                case PowerType.AirJump:
                    airJumpEffect.SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
                    StartCoroutine(DeactivatePowerAfterDelay(powerType, 0.5f));
                    break;
            }
        }
    }

    private IEnumerator DeactivatePowerAfterDelay(PowerType powerType, float delay)
    {
        yield return new WaitForSeconds(delay);
        powers[powerType].isActive = false;
        
        switch (powerType)
        {
            case PowerType.Shield:
                shieldEffect.SendMessage("EndEffect", SendMessageOptions.DontRequireReceiver);
                break;
        }
    }

    protected override IEnumerator StartPowerCooldown(PowerType powerType)
    {
        SetPowerCooldown(powerType, true);
        yield return new WaitForSeconds(GameConstants.SHOOT_COOLDOWN);
        SetPowerCooldown(powerType, false);
    }

    public override void AddPowerStock(PowerType powerType, int amount = 1)
    {
        stats.AddPowerStock(powerType, amount);
        powerUI?.UpdatePowerStatus("Player", powerType, stats.PowerStocks[powerType]); // Changed from "Player1" to "Player"
    }
}