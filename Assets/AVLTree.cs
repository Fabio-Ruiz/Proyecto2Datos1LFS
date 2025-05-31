using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class AVLNode
{
    public int value;
    public AVLNode leftChild;
    public AVLNode rightChild;
    public int height;
    [HideInInspector] public GameObject nodeObject;
    public Vector2 position; // Add this line
    
    public AVLNode(int val)
    {
        value = val;
        height = 1;
        position = Vector2.zero;
    }
}


public class AVLTree : MonoBehaviour, ITree
{
    public GameObject nodePrefab;
    public float horizontalSpacing = 2.0f;
    public float verticalSpacing = 1.5f;
    private AVLNode root;
    private Dictionary<AVLNode, Vector2> nodePositions = new();
    private List<GameObject> lineRenderers = new();

    public void Insert(int value)
    {
        root = InsertNode(root, value);
        UpdateVisualization();
    }
    public bool Search(int value) => SearchNode(root, value);
    public void Delete(int value)
    {
        root = DeleteNode(root, value);
        UpdateVisualization();
    }
    public void ClearTree()
    {
        ClearVisualization();
        root = null;
    }
    public void CreateExampleTree()
    {
        ClearTree();
        foreach (int val in new[] { 10, 20, 30, 40, 50, 25, 5 })
            Insert(val);
    }

    // L�gica del AVL
    private int Height(AVLNode node) => node == null ? 0 : node.height;

    private void UpdateHeight(AVLNode node)
    {
        if (node != null)
            node.height = Math.Max(Height(node.leftChild), Height(node.rightChild)) + 1;
    }

    private int BalanceFactor(AVLNode node) => node == null ? 0 : Height(node.leftChild) - Height(node.rightChild);

    private AVLNode RotateRight(AVLNode y)
    {
        AVLNode x = y.leftChild;
        AVLNode T2 = x.rightChild;

        x.rightChild = y;
        y.leftChild = T2;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    private AVLNode RotateLeft(AVLNode x)
    {
        AVLNode y = x.rightChild;
        AVLNode T2 = y.leftChild;

        y.leftChild = x;
        x.rightChild = T2;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
    }

    private AVLNode InsertNode(AVLNode node, int value)
    {
        if (node == null)
            return new AVLNode(value);

        if (value < node.value)
            node.leftChild = InsertNode(node.leftChild, value);
        else if (value > node.value)
            node.rightChild = InsertNode(node.rightChild, value);
        else
            return node;

        UpdateHeight(node);

        int balance = BalanceFactor(node);

        if (balance > 1 && value < node.leftChild.value) return RotateRight(node);
        if (balance < -1 && value > node.rightChild.value) return RotateLeft(node);
        if (balance > 1 && value > node.leftChild.value)
        {
            node.leftChild = RotateLeft(node.leftChild);
            return RotateRight(node);
        }
        if (balance < -1 && value < node.rightChild.value)
        {
            node.rightChild = RotateRight(node.rightChild);
            return RotateLeft(node);
        }

        return node;
    }

    private bool SearchNode(AVLNode node, int value)
    {
        if (node == null) return false;
        if (value == node.value) return true;
        return value < node.value ? SearchNode(node.leftChild, value) : SearchNode(node.rightChild, value);
    }

    private AVLNode FindMinValueNode(AVLNode node)
    {
        AVLNode current = node;
        while (current.leftChild != null)
            current = current.leftChild;
        return current;
    }

    private AVLNode DeleteNode(AVLNode node, int value)
    {
        if (node == null)
            return null;

        if (value < node.value)
            node.leftChild = DeleteNode(node.leftChild, value);
        else if (value > node.value)
            node.rightChild = DeleteNode(node.rightChild, value);
        else
        {
            if (node.leftChild == null) return node.rightChild;
            if (node.rightChild == null) return node.leftChild;

            AVLNode temp = FindMinValueNode(node.rightChild);
            node.value = temp.value;
            node.rightChild = DeleteNode(node.rightChild, temp.value);
        }

        UpdateHeight(node);
        int balance = BalanceFactor(node);

        if (balance > 1 && BalanceFactor(node.leftChild) >= 0) return RotateRight(node);
        if (balance > 1 && BalanceFactor(node.leftChild) < 0)
        {
            node.leftChild = RotateLeft(node.leftChild);
            return RotateRight(node);
        }
        if (balance < -1 && BalanceFactor(node.rightChild) <= 0) return RotateLeft(node);
        if (balance < -1 && BalanceFactor(node.rightChild) > 0)
        {
            node.rightChild = RotateRight(node.rightChild);
            return RotateLeft(node);
        }

        return node;
    }

    // VISUALIZACI�N
    private void UpdateVisualization()
    {
        ClearVisualization();
        nodePositions.Clear();
        CalculateNodePositions(root, 0, 0, new HashSet<int>());
        VisualizeNodes(root);
        VisualizeLines(root);
    }

    private void CalculateNodePositions(AVLNode node, float x, int depth, HashSet<int> usedX)
    {
        if (node == null) return;
        float y = -depth * verticalSpacing;

        CalculateNodePositions(node.leftChild, x - horizontalSpacing / (depth + 1), depth + 1, usedX);

        while (usedX.Contains(Mathf.RoundToInt(x * 100)))
            x += 0.1f;
        usedX.Add(Mathf.RoundToInt(x * 100));

        nodePositions[node] = new Vector2(x, y);

        CalculateNodePositions(node.rightChild, x + horizontalSpacing / (depth + 1), depth + 1, usedX);
    }

    private void VisualizeNodes(AVLNode node)
    {
        if (node == null) return;

        Vector2 position = nodePositions[node];
        node.nodeObject = Instantiate(nodePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, transform);
        node.nodeObject.GetComponent<SpriteRenderer>().color = Color.blue;


        NodeUI nodeUI = node.nodeObject.GetComponent<NodeUI>();
        if (nodeUI != null)
            nodeUI.SetValue(node.value);

        VisualizeNodes(node.leftChild);
        VisualizeNodes(node.rightChild);
    }


    private void VisualizeLines(AVLNode node)
    {
        if (node == null) return;

        Vector2 parentPos = nodePositions[node];
        if (node.leftChild != null)
            CreateLine(parentPos, nodePositions[node.leftChild]);
        if (node.rightChild != null)
            CreateLine(parentPos, nodePositions[node.rightChild]);

        VisualizeLines(node.leftChild);
        VisualizeLines(node.rightChild);
    }

    private void CreateLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(transform);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.sortingOrder = -1;

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(start.x, start.y, 0));  // z = 0
        line.SetPosition(1, new Vector3(end.x, end.y, 0));
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.black;
        line.endColor = Color.black;
        lineRenderers.Add(lineObj);
    }

    private void ClearVisualization()
    {
        VisitAllNodes(root, (node) =>
        {
            if (node.nodeObject != null)
                Destroy(node.nodeObject);
        });

        foreach (GameObject line in lineRenderers)
        {
            if (line != null)
                Destroy(line);
        }
        lineRenderers.Clear();
    }

    private void VisitAllNodes(AVLNode node, Action<AVLNode> action)
    {
        if (node == null) return;
        action(node);
        VisitAllNodes(node.leftChild, action);
        VisitAllNodes(node.rightChild, action);
    }

    // Implementación de los nuevos métodos de ITree
    public int CalcularProfundidad()
    {
        return Height(root);  // Aprovechamos el método Height existente de AVL
    }

    public AVLNode GetRootAVL()
    {
        return root;
    }

    public BSTNode GetRoot()
    {
        throw new System.NotImplementedException("Este método es solo para árboles BST");
    }
}
