using UnityEngine;

public class TokenTrigger : MonoBehaviour
{
    private TokenMoneda tokenMoneda;

    private void Start()
    {
        // Primero intentar obtener del padre inmediato
        tokenMoneda = transform.parent?.GetComponent<TokenMoneda>();
        
        // Si no se encuentra, buscar en la estructura Token -> CoinGold
        if (tokenMoneda == null && transform.parent != null)
        {
            Transform coinGold = transform.parent.Find("CoinGold");
            if (coinGold != null)
            {
                tokenMoneda = coinGold.GetComponent<TokenMoneda>();
            }
        }
        
        if (tokenMoneda == null)
        {
            Debug.LogError($"TokenMoneda no encontrado para {gameObject.name}. " +
                         "Verifica la jerarquía del objeto.", gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tokenMoneda == null)
        {
            Debug.LogError("TokenMoneda es null en " + gameObject.name);
            return;
        }
        
        // Añadir más logging para debug
        Debug.Log($"Colisión detectada con {other.name} (Tag: {other.tag})");
        
        if (other.CompareTag("Player") || other.CompareTag("Player2") || other.CompareTag("Player3"))
        {
            Debug.Log($"Token collected by {other.tag}!");
            tokenMoneda.Recolectar(other.gameObject);
        }
    }
}
