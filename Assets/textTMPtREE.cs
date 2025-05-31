using TMPro;
using UnityEngine;

[ExecuteAlways]
public class NodeTextFix : MonoBehaviour
{
    private TextMeshProUGUI _text;

    void OnEnable()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if (_text == null)
        {
            Debug.LogError($"Falta TextMeshProUGUI en {gameObject.name}", gameObject);
            return;
        }

        // Configuración forzada
        _text.color = Color.black;
        _text.raycastTarget = false;

        // Ajuste de escala relativa al padre
        transform.localScale = Vector3.one * (1f / transform.parent.lossyScale.x);
    }

    void Update()
    {
        // Opcional: Mantiene el texto mirando a cámara
        transform.forward = Camera.main?.transform.forward ?? Vector3.forward;
    }
}