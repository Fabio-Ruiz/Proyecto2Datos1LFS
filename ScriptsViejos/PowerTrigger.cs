using UnityEngine;

public class PowerTrigger : MonoBehaviour
{
    private PowerPickup powerPickup;

    private void Start()
    {
        powerPickup = transform.parent?.GetComponent<PowerPickup>();
        
        if (powerPickup == null)
        {
            Debug.LogError($"PowerPickup component not found on parent of {gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (powerPickup == null) return;
        
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            powerPickup.Collect(other.gameObject);
        }
    }
}