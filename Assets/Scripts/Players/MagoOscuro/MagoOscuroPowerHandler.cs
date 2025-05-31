using UnityEngine;

public class MagoOscuroPowerHandler : PowerHandler
{
    protected override void InitializePowerSystem()
    {
        powerSystem = gameObject.AddComponent<MagoOscuroPowerSystem>();
    }

    protected override void InitializePowerEffects()
    {
        // No crear nuevos componentes aquí, solo obtener referencias
        powerEffects[PowerSystem.PowerType.ForceField] = GetComponent<ForcePushEffect>();
        powerEffects[PowerSystem.PowerType.Shield] = GetComponent<ShieldEffect>();
        powerEffects[PowerSystem.PowerType.AirJump] = GetComponent<AirJumpEffect>();
    }

    public override void HandlePowerInput()
    {
        if (!player.IsPenalized)
        {
            if (Input.GetKey(GameInput.Player1.FORCE_PUSH))
                ActivatePower(PowerSystem.PowerType.ForceField);
            if (Input.GetKey(GameInput.Player1.SHIELD))
                ActivatePower(PowerSystem.PowerType.Shield);
            if (Input.GetKey(GameInput.Player1.AIR_JUMP))
                ActivatePower(PowerSystem.PowerType.AirJump);
        }
    }
}