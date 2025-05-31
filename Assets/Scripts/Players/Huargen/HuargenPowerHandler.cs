using UnityEngine;

public class HuargenPowerHandler : PowerHandler
{
    protected override void InitializePowerSystem()
    {
        powerSystem = gameObject.AddComponent<HuargenPowerSystem>();
    }

    protected override void InitializePowerEffects()
    {
        powerEffects[PowerSystem.PowerType.ForceField] = gameObject.AddComponent<ForcePushEffect>();
        powerEffects[PowerSystem.PowerType.Shield] = gameObject.AddComponent<ShieldEffect>();
        powerEffects[PowerSystem.PowerType.AirJump] = gameObject.AddComponent<AirJumpEffect>();
    }

    public override void HandlePowerInput()
    {
        if (!player.IsPenalized)
        {
            if (Input.GetKey(GameInput.Player2.FORCE_PUSH))
                ActivatePower(PowerSystem.PowerType.ForceField);
            if (Input.GetKey(GameInput.Player2.SHIELD))
                ActivatePower(PowerSystem.PowerType.Shield);
            if (Input.GetKey(GameInput.Player2.AIR_JUMP))
                ActivatePower(PowerSystem.PowerType.AirJump);
        }
    }
}