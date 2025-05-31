using UnityEngine;
using TMPro;

public class Player3Tree : BinarySearchTree2D
{
    protected override void Start()
    {
        base.Start();
        // Configuración específica para el árbol del Player3
        horizontalSpacing = 0.5f;
        verticalSpacing = 0.5f;
        
        // Color verde aún más claro para mejor contraste
        Color lightGreenColor = new Color(0.8f, 1f, 0.8f, 1f); // Verde muy claro
        nodeColor = lightGreenColor;
        lineColor = lightGreenColor;

        // Posicionar en la esquina superior derecha
        transform.position = new Vector3(6f, 3f, 0f);
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // Verificar que el nodePrefab está asignado
        if (nodePrefab == null)
        {
            Debug.LogError("NodePrefab no está asignado en Player3Tree");
        }
    }

    protected override void OnNodeInserted(int value)
    {
        base.OnNodeInserted(value);
        Debug.Log($"Player3 tree: Inserted {value}");
        // Forzar actualización visual después de cada inserción
        VisualizeTree();
    }

    protected override void VisualizeTree()
    {
        ClearVisualization();
        if (root != null)
        {
            CalculateNodePositions(root, Vector2.zero, horizontalSpacing);
            CreateVisualNodes();
            DrawConnections();
        }
    }

    protected override void CreateVisualNodes()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("NodePrefab is null in Player3Tree");
            return;
        }

        void CreateNodeVisual(BSTNode node)
        {
            if (node == null) return;

            GameObject nodeObj = Instantiate(nodePrefab, 
                (Vector3)node.position + transform.position, 
                Quaternion.identity, 
                transform);
            
            var tmpText = nodeObj.GetComponent<TMPro.TextMeshPro>();
            if (tmpText != null)
            {
                tmpText.text = node.value.ToString();
                tmpText.alignment = TMPro.TextAlignmentOptions.Center;
                tmpText.fontSize = 3;
                tmpText.color = Color.black;
                tmpText.sortingOrder = 1;
                Debug.Log($"Created node with value: {node.value}");
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on instantiated node");
            }

            nodeObjects.Add(nodeObj);

            CreateNodeVisual(node.leftChild);
            CreateNodeVisual(node.rightChild);
        }

        ClearVisualization();
        CreateNodeVisual(root);
    }
}