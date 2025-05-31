using UnityEngine;

public class EscenarioRandom : MonoBehaviour
{
    public GameObject[] escenarios;
    public AudioClip sonidoInicio;

    private AudioSource audioSource;

    void Start()
    {
        // Instanciar escenario aleatorio
        int index = Random.Range(0, escenarios.Length);
        Instantiate(escenarios[index], Vector3.zero, Quaternion.identity);

        // Reproducir música de fondo
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = sonidoInicio;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.6f;
        audioSource.Play();


    }
}