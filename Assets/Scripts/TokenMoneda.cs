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

        // Intentar obtener cualquiera de los dos tipos de movimiento que heredan de PlayerBase
        var magoMov = jugador.GetComponent<MagoOscuroMovement>();
        var huargenMov = jugador.GetComponent<HuargenMovement>();

        // Actualizar puntaje y árbol según el tipo de jugador
        PlayerBase player = magoMov as PlayerBase ?? huargenMov as PlayerBase;
        
        if (player != null && player.ArbolJugador != null)
        {
            player.ArbolJugador.Insert(valor);  // Insertar en el árbol
            player.AgregarPuntaje(valor);       // Actualizar puntaje
            
            Debug.Log($"Token de valor {valor} insertado en el árbol del jugador {jugador.name}");
            recogido = true;
            Destroy(gameObject);
        }
    }

    private void Autodestruir()
    {
        if (!recogido)
            Destroy(gameObject);
    }
}