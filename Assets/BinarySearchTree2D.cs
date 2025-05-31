using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class BSTNode
{
    public int value;
    public BSTNode leftChild;
    public BSTNode rightChild;
    public Vector2 position;

    public BSTNode(int value)
    {
        this.value = value;
        leftChild = null;
        rightChild = null;
    }
}

public class BinarySearchTree2D : MonoBehaviour, ITree
{
    public GameObject nodePrefab;  // Changed from [SerializeField] to public
    [SerializeField] private float horizontalSpacing = 1.5f;
    [SerializeField] private float verticalSpacing = 1f;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float lineWidth = 0.1f;

    private BSTNode root;
    private List<GameObject> nodeObjects = new();

    // Implementaci�n de ITree
    public void Insert(int value)
    {
        root = InsertRecursive(root, value);
        VisualizeTree();
    }

    public bool Search(int value)
    {
        return SearchRecursive(root, value);
    }

    public void Delete(int value)
    {
        root = DeleteRecursive(root, value);
        VisualizeTree();
    }

    public void ClearTree()
    {
        root = null;
        ClearVisualization();
    }

    public void CreateExampleTree()
    {
        ClearTree();
        foreach (int val in new[] { 50, 30, 70, 20, 40, 60, 80 })
        {
            Insert(val);
        }
    }

    // ======================
    // L�gica interna del BST
    // ======================

    private BSTNode InsertRecursive(BSTNode current, int value)
    {
        if (current == null)
            return new BSTNode(value);

        if (value < current.value)
            current.leftChild = InsertRecursive(current.leftChild, value);
        else if (value > current.value)
            current.rightChild = InsertRecursive(current.rightChild, value);

        return current;
    }

    private bool SearchRecursive(BSTNode current, int value)
    {
        if (current == null)
            return false;
        if (current.value == value)
            return true;
        return value < current.value
            ? SearchRecursive(current.leftChild, value)
            : SearchRecursive(current.rightChild, value);
    }

    private BSTNode DeleteRecursive(BSTNode node, int value)
    {
        if (node == null) return null;

        if (value < node.value)
        {
            node.leftChild = DeleteRecursive(node.leftChild, value);
        }
        else if (value > node.value)
        {
            node.rightChild = DeleteRecursive(node.rightChild, value);
        }
        else
        {
            // Nodo encontrado
            if (node.leftChild == null) return node.rightChild;
            if (node.rightChild == null) return node.leftChild;

            BSTNode minLargerNode = FindMin(node.rightChild);
            node.value = minLargerNode.value;
            node.rightChild = DeleteRecursive(node.rightChild, minLargerNode.value);
        }

        return node;
    }

    private BSTNode FindMin(BSTNode node)
    {
        while (node.leftChild != null)
            node = node.leftChild;
        return node;
    }

    // ======================
    // Visualizaci�n
    // ======================

    private void VisualizeTree()
    {
        ClearVisualization();
        if (root != null)
        {
            CalculatePositions(root, Vector2.zero, horizontalSpacing);
            CreateNodeObjects(root);
        }
    }

    private void CalculatePositions(BSTNode node, Vector2 position, float spacing)
    {
        if (node == null) return;

        node.position = position;
        float childSpacing = spacing * 0.5f;

        if (node.leftChild != null)
        {
            CalculatePositions(node.leftChild, position + new Vector2(-spacing, -verticalSpacing), childSpacing);
        }

        if (node.rightChild != null)
        {
            CalculatePositions(node.rightChild, position + new Vector2(spacing, -verticalSpacing), childSpacing);
        }
    }

    private void CreateNodeObjects(BSTNode node)
    {
        if (node == null) return;

        Vector3 worldPos = new Vector3(node.position.x, node.position.y, 0);
        GameObject nodeObj = Instantiate(nodePrefab, worldPos, Quaternion.identity, transform);

        nodeObj.transform.SetParent(transform);
        nodeObj.name = "Node_" + node.value;

        nodeObj.GetComponent<SpriteRenderer>().color = Color.red;


        NodeUI nodeUI = nodeObj.GetComponent<NodeUI>();
        if (nodeUI != null)
        {
            nodeUI.SetValue(node.value);
        }


        nodeObjects.Add(nodeObj);

        if (node.leftChild != null)
        {
            DrawLine(node.position, node.leftChild.position);
            CreateNodeObjects(node.leftChild);
        }

        if (node.rightChild != null)
        {
            DrawLine(node.position, node.rightChild.position);
            CreateNodeObjects(node.rightChild);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(transform);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.sortingOrder = -1;

        line.startColor = lineColor;
        line.endColor = lineColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(start.x, start.y, 0));
        line.SetPosition(1, new Vector3(end.x, end.y, 0));
        line.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void ClearVisualization()
    {
        foreach (var obj in nodeObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        nodeObjects.Clear();

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Line"))
                Destroy(child.gameObject);
        }
    }

    public int CalcularProfundidad()
    {
        return CalcularProfundidadRecursivo(root);
    }

    private int CalcularProfundidadRecursivo(BSTNode node)
    {
        if (node == null)
            return 0;
        
        int izquierda = CalcularProfundidadRecursivo(node.leftChild);
        int derecha = CalcularProfundidadRecursivo(node.rightChild);
        
        return Math.Max(izquierda, derecha) + 1;
    }

    public BSTNode GetRoot()
    {
        return root;
    }

    public AVLNode GetRootAVL()
    {
        throw new System.NotImplementedException("Este método es solo para árboles AVL");
    }
}