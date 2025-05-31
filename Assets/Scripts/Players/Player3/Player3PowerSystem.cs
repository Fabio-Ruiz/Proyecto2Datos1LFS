using UnityEngine;
using System.Collections;

public class Player3PowerSystem : PowerSystem
{
    [SerializeField] private PowerStatusUI powerUI;  // Keep it here

    [Header("Power Effects")]
    [SerializeField] private ShieldEffect shieldEffect;
    [SerializeField] private AirJumpEffect airJumpEffect;
    [SerializeField] private ForcePushEffect forcePushEffect;

    [Header("Power Settings")]
    [SerializeField] private float airJumpForceMultiplier = 1.2f;

    protected override void Start()
    {
        base.Start();
        InitializeEffects();
    }

    private void InitializeEffects()
    {
        if (shieldEffect == null) shieldEffect = gameObject.AddComponent<ShieldEffect>();
        if (airJumpEffect == null) airJumpEffect = gameObject.AddComponent<AirJumpEffect>();
        if (forcePushEffect == null) forcePushEffect = gameObject.AddComponent<ForcePushEffect>();
    }

    public override bool HasPowerStock(PowerType powerType)
    {
        return stats.PowerStocks[powerType] > 0;
    }

    public override void UsePower(PowerType powerType)
    {
        if (stats.UsePowerStock(powerType))
        {
            powerUI?.UpdatePowerStatus("Player3", powerType, stats.PowerStocks[powerType]);
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
        powerUI?.UpdatePowerStatus("Player3", powerType, stats.PowerStocks[powerType]);
    }
}