using UnityEngine;

public class MagoOscuroTree : BinarySearchTree2D
{
    protected override void Start()
    {
        base.Start();
        // Configuración específica para el árbol del MagoOscuro
        horizontalSpacing = 0.5f;     // Reducido aún más para compactar horizontalmente
        verticalSpacing = 0.5f;       // Mantiene la separación vertical
        
        // Cambiar a morado claro
        Color purpleColor = new Color(0.8f, 0.3f, 0.8f, 1f); // Light purple
        nodeColor = purpleColor;
        lineColor = purpleColor;

        // Ajustar posición más hacia la derecha (-6 en vez de -8)
        transform.position = new Vector3(-6f, 3.5f, 0f);
        // Escalar el árbol para hacerlo más pequeño
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);  // Reducido de 0.75f a 0.5f
    }

    protected override void OnNodeInserted(int value)
    {
        base.OnNodeInserted(value);
        Debug.Log($"MagoOscuro tree: Inserted {value}");
        VisualizeTree();
    }
}

