using UnityEngine;
using TMPro;

public class TokenMoneda : MonoBehaviour
{
    public int valorMin = 1;
    public int valorMax = 10;
    public TextMeshPro textValor;
    public float vidaDelToken = 5f;

    private int valor;
    private bool recogido = false;

    void Start()
    {
        valor = Random.Range(valorMin, valorMax + 1);
        textValor.text = valor.ToString();

        Invoke("Autodestruir", vidaDelToken);
    }

    public void Recolectar(GameObject jugador)
    {
        if (recogido) return;

        if (jugador == null)
        {
            Debug.LogError("Jugador es null en Recolectar");
            return;
        }

        Debug.Log($"Intentando recolectar token con {jugador.name} (Tag: {jugador.tag})");
        PlayerBase player = jugador.GetComponent<PlayerBase>();
        
        if (player == null)
        {
            Debug.LogError($"No se encontró PlayerBase en {jugador.name}");
            return;
        }

        if (player.Tree == null)
        {
            Debug.LogError($"El árbol no está inicializado en {jugador.name}");
            return;
        }

        Debug.Log($"Recolección exitosa. Valor: {valor}");
        try
        {
            player.HandleTokenCollection(valor);
            recogido = true;
            Destroy(gameObject);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al procesar token: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void Autodestruir()
    {
        if (!recogido)
            Destroy(gameObject);
    }
}