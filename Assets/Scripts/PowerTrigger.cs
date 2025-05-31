using UnityEngine;

public class PowerTrigger : MonoBehaviour
{
    public PowerPickup powerPickup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            PowerSystem powerSystem = other.GetComponent<PowerSystem>();
            if (powerSystem != null)
            {
                // Otorgar +1 de cada poder
                powerSystem.ObtainPower(PowerSystem.PowerType.ForceField);
                powerSystem.ObtainPower(PowerSystem.PowerType.Shield);
                powerSystem.ObtainPower(PowerSystem.PowerType.AirJump);
            }
        }
    }
}