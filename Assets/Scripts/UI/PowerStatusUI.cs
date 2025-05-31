using UnityEngine;
using TMPro;

public class PowerStatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text magoForcePushText;
    [SerializeField] private TMP_Text magoShieldText;
    [SerializeField] private TMP_Text magoAirJumpText;
    
    [SerializeField] private TMP_Text huargenForcePushText;
    [SerializeField] private TMP_Text huargenShieldText;
    [SerializeField] private TMP_Text huargenAirJumpText;

    public void UpdatePowerStatus(string playerTag, PowerSystem.PowerType powerType, int count)
    {
        TMP_Text targetText = null;
        
        if (playerTag == "MagoOscuro")
        {
            targetText = powerType switch
            {
                PowerSystem.PowerType.ForceField => magoForcePushText,
                PowerSystem.PowerType.Shield => magoShieldText,
                PowerSystem.PowerType.AirJump => magoAirJumpText,
                _ => null
            };
        }
        else if (playerTag == "Huargen")
        {
            targetText = powerType switch
            {
                PowerSystem.PowerType.ForceField => huargenForcePushText,
                PowerSystem.PowerType.Shield => huargenShieldText,
                PowerSystem.PowerType.AirJump => huargenAirJumpText,
                _ => null
            };
        }

        if (targetText != null)
            targetText.text = count.ToString();
    }
}