using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HuargenTree : AVLTree
{
    [Header("Huargen Visual Settings")]
    [SerializeField] private Color nodeColor = Color.red;
    [SerializeField] private Color lineColor = Color.red;

    protected override void Start()
    {
        base.Start();
        
        // Ajustar espaciado para que sea más visible
        horizontalSpacing = 1.0f;
        verticalSpacing = 1.0f;
        
        // Posición en el centro superior de la pantalla
        transform.position = new Vector3(0f, 3.5f, 0f);
        transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        if (nodePrefab == null)
        {
            Debug.LogError("NodePrefab no está asignado en HuargenTree");
            return;
        }

        // No llamar a UpdateVisualization aquí, ya que root es null
    }

    public override void Insert(int value)
    {
        Debug.Log($"HuargenTree: Intentando insertar valor {value}");
        base.Insert(value);
        Debug.Log($"HuargenTree: Valor insertado {value}, Root: {(root != null ? root.value.ToString() : "null")}");
        UpdateVisualization();
    }

    protected override void UpdateVisualization()
    {
        if (root == null)
        {
            Debug.Log("HuargenTree: Root es null, saltando visualización");
            return;
        }

        base.UpdateVisualization();

        // Actualizar colores de las líneas
        var lines = GetComponentsInChildren<LineRenderer>();
        foreach (var line in lines)
        {
            if (line != null)
            {
                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = 0.05f;
                line.endWidth = 0.05f;
                line.sortingOrder = -1;
            }
        }

        Debug.Log($"HuargenTree: Visualización actualizada. Nodos: {nodePositions.Count}");
    }

    private void ClearVisualization()
    {
        // Limpiar líneas
        foreach (var lineObj in lineRenderers)
        {
            if (lineObj != null)
                Destroy(lineObj);
        }
        lineRenderers.Clear();

        // Limpiar nodos
        var existingNodes = GetComponentsInChildren<TextMeshPro>();
        foreach (var node in existingNodes)
        {
            if (node != null)
                Destroy(node.gameObject);
        }

        nodePositions.Clear();
    }
}