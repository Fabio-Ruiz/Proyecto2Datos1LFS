using UnityEngine;

public class CameraAutoCenter : MonoBehaviour
{
    public string nodeTag = "Node"; // Asegúrate de asignar este tag a tus nodos
    public float padding = 5f; // Distancia adicional para encuadrar mejor
    public float zoomSize = 10f; // Tamaño ortográfico base (ajustable)

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        CenterCamera();
    }

    public void CenterCamera()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag(nodeTag);
        if (nodes.Length == 0)
        {
            Debug.LogWarning("No se encontraron nodos con el tag '" + nodeTag + "'.");
            return;
        }

        Vector3 totalPos = Vector3.zero;
        float maxDistance = 0f;

        // Calcular el centro promedio y el radio más grande
        foreach (GameObject node in nodes)
        {
            totalPos += node.transform.position;
        }

        Vector3 center = totalPos / nodes.Length;

        foreach (GameObject node in nodes)
        {
            float distance = Vector3.Distance(center, node.transform.position);
            if (distance > maxDistance)
                maxDistance = distance;
        }

        cam.transform.position = new Vector3(center.x, center.y, cam.transform.position.z);
        cam.orthographicSize = maxDistance + padding;
    }
}
