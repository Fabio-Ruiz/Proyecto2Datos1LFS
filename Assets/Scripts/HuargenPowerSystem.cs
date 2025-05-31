using UnityEngine;

public class HuargenPowerSystem : PowerSystem
{
    protected override void InitializePowers()
    {
        // Poder Force-push (Disparo)
        Power forcePush = new Power
        {
            type = PowerType.ForceField,
            activationKey = KeyCode.Alpha7,
            stockCount = 0,        // Munición inicial
            inCooldown = false,    // Sin cooldown
            isAvailable = true     // Siempre disponible si hay munición
        };
        powers.Add(forcePush);

        // Poder Shield
        Power shield = new Power
        {
            type = PowerType.Shield,
            activationKey = KeyCode.Alpha8,
            stockCount = 0,
            inCooldown = false
        };
        powers.Add(shield);

        // Poder Air-jump
        Power airJump = new Power
        {
            type = PowerType.AirJump,
            activationKey = KeyCode.Alpha9,
            stockCount = 0,
            inCooldown = false
        };
        powers.Add(airJump);
    }
}
