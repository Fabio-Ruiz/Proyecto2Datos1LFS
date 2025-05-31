using UnityEngine;

public class SpawnMonedas : MonoBehaviour
{
    public GameObject[] monedas; // lista de prefabs de monedas
    public float tiempoInicial = 1f;
    public float intervalo = 0.5f;

    public Transform xRangeLeft;
    public Transform xRangeRight;
    public Transform ySpawnHeight;

    void Start()
    {
        InvokeRepeating(nameof(GenerarMoneda), tiempoInicial, intervalo);
    }

    public void GenerarMoneda()
    {
        Vector3 posicion = new Vector3(
            Random.Range(xRangeLeft.position.x, xRangeRight.position.x),
            ySpawnHeight.position.y,
            0
        );

        int index = Random.Range(0, monedas.Length);
        Instantiate(monedas[index], posicion, Quaternion.identity);
    }
}