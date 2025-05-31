using UnityEngine;
using System.Collections;

public class HuargenPowerSystem : PowerSystem
{
    [SerializeField] private PowerStatusUI powerUI;  // Keep it here

    [Header("Power Effects")]
    [SerializeField] private ShieldEffect shieldEffect;
    [SerializeField] private AirJumpEffect airJumpEffect;
    [SerializeField] private ForcePushEffect forcePushEffect;

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
            powerUI?.UpdatePowerStatus("Player2", powerType, stats.PowerStocks[powerType]);
            ActivatePowerEffect(powerType);
            StartCoroutine(StartPowerCooldown(powerType));
        }
    }

    private void ActivatePowerEffect(PowerType powerType)
    {
        switch (powerType)
        {
            case PowerType.ForceField:
                forcePushEffect.SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
                break;
            case PowerType.Shield:
                shieldEffect.SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
                break;
            case PowerType.AirJump:
                airJumpEffect.SendMessage("ApplyEffect", SendMessageOptions.DontRequireReceiver);
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
        powerUI?.UpdatePowerStatus("Player2", powerType, stats.PowerStocks[powerType]);
    }
}