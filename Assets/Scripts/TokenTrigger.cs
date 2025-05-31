using UnityEngine;

public class TokenTrigger : MonoBehaviour
{
    private TokenMoneda tokenMoneda;

    private void Start()
    {
        // Try to get component from parent's parent (Token -> CoinGold)
        if (transform.parent != null && transform.parent.parent != null)
        {
            Transform coinGold = transform.parent.parent.Find("CoinGold");
            if (coinGold != null)
            {
                tokenMoneda = coinGold.GetComponent<TokenMoneda>();
            }
        }
        
        if (tokenMoneda == null)
        {
            Debug.LogError($"TokenMoneda component not found. Hierarchy should be: Token -> CoinGold (TokenMoneda) -> DetectorCollision (TokenTrigger)");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tokenMoneda != null && (other.CompareTag("Player") || other.CompareTag("Player2")))
        {
            tokenMoneda.Recolectar(other.gameObject);
        }
    }
}
