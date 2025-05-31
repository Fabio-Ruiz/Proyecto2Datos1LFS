using UnityEngine;

public class Player3PowerHandler : PowerHandler
{
    protected override void InitializePowerSystem()
    {
        powerSystem = gameObject.AddComponent<Player3PowerSystem>();
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
            // Check for power activations
            if (Input.GetKeyDown(GameInput.Player3.FORCE_PUSH))
                ActivatePower(PowerSystem.PowerType.ForceField);
            
            if (Input.GetKeyDown(GameInput.Player3.SHIELD))
                ActivatePower(PowerSystem.PowerType.Shield);
            
            if (Input.GetKeyDown(GameInput.Player3.AIR_JUMP))
                ActivatePower(PowerSystem.PowerType.AirJump);
        }
    }
}