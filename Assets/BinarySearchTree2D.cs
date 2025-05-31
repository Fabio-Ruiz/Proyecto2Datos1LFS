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
    [Header("Visual Settings")]
    [SerializeField] protected GameObject nodePrefab;
    [SerializeField] protected float horizontalSpacing = 1.5f;
    [SerializeField] protected float verticalSpacing = 1.0f;
    [SerializeField] protected Color nodeColor = Color.white;
    [SerializeField] protected Color lineColor = Color.white;
    
    protected BSTNode root;
    protected List<GameObject> nodeObjects = new List<GameObject>();
    protected List<LineRenderer> lines = new List<LineRenderer>();

    protected virtual void Start()
    {
        InitializeTree();
    }

    protected virtual void OnNodeInserted(int value)
    {
        Debug.Log($"Node inserted: {value}");
        UpdateVisualization();
    }

    public void Insert(int value)
    {
        if (root == null)
            root = new BSTNode(value);
        else
            InsertRec(root, value);

        OnNodeInserted(value);
    }

    protected virtual void InitializeTree()
    {
        root = null;
        ClearVisualization();
    }

    protected virtual void VisualizeTree()
    {
        ClearVisualization();
        if (root != null)
        {
            CalculateNodePositions(root, Vector2.zero, horizontalSpacing);
            CreateVisualNodes();
            DrawConnections();
        }
    }

    // Change UpdateVisualization to use VisualizeTree
    protected void UpdateVisualization()
    {
        VisualizeTree();
    }

    protected void ClearVisualization()
    {
        foreach (var obj in nodeObjects)
            Destroy(obj);
        foreach (var line in lines)
            Destroy(line.gameObject);
            
        nodeObjects.Clear();
        lines.Clear();
    }

    protected void CalculateNodePositions(BSTNode node, Vector2 position, float horizontalOffset)
    {
        node.position = position;

        if (node.leftChild != null)
            CalculateNodePositions(node.leftChild, 
                new Vector2(position.x - horizontalOffset, position.y - verticalSpacing), 
                horizontalOffset * 0.5f);

        if (node.rightChild != null)
            CalculateNodePositions(node.rightChild, 
                new Vector2(position.x + horizontalOffset, position.y - verticalSpacing), 
                horizontalOffset * 0.5f);
    }

    protected virtual void CreateVisualNodes()  // Added virtual keyword here
    {
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
            }

            nodeObjects.Add(nodeObj);

            CreateNodeVisual(node.leftChild);
            CreateNodeVisual(node.rightChild);
        }

        CreateNodeVisual(root);
    }

    protected void DrawConnections()
    {
        void DrawNodeConnections(BSTNode node)
        {
            if (node == null) return;

            void DrawLine(Vector2 from, Vector2 to)
            {
                GameObject lineObj = new GameObject("Line");
                lineObj.transform.parent = transform;
                
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.startWidth = 0.05f;  // Reduced from 0.1f
                lr.endWidth = 0.05f;    // Reduced from 0.1f
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = lr.endColor = lineColor;
                
                // Adjust line positions to start from text center
                Vector3 startPos = (Vector3)from + transform.position;
                Vector3 endPos = (Vector3)to + transform.position;
                
                lr.SetPosition(0, startPos);
                lr.SetPosition(1, endPos);
                
                lines.Add(lr);
            }

            if (node.leftChild != null)
                DrawLine(node.position, node.leftChild.position);
            if (node.rightChild != null)
                DrawLine(node.position, node.rightChild.position);

            DrawNodeConnections(node.leftChild);
            DrawNodeConnections(node.rightChild);
        }

        DrawNodeConnections(root);
    }

    public bool Search(int value)
    {
        return SearchRec(root, value);
    }

    private bool SearchRec(BSTNode node, int value)
    {
        if (node == null) return false;
        if (node.value == value) return true;
        
        if (value < node.value)
            return SearchRec(node.leftChild, value);
        return SearchRec(node.rightChild, value);
    }

    public void Delete(int value)
    {
        root = DeleteRec(root, value);
        UpdateVisualization();
    }

    private BSTNode DeleteRec(BSTNode node, int value)
    {
        if (node == null) return null;

        if (value < node.value)
            node.leftChild = DeleteRec(node.leftChild, value);
        else if (value > node.value)
            node.rightChild = DeleteRec(node.rightChild, value);
        else
        {
            if (node.leftChild == null)
                return node.rightChild;
            if (node.rightChild == null)
                return node.leftChild;

            node.value = MinValue(node.rightChild);
            node.rightChild = DeleteRec(node.rightChild, node.value);
        }
        return node;
    }

    private int MinValue(BSTNode node)
    {
        int minv = node.value;
        while (node.leftChild != null)
        {
            minv = node.leftChild.value;
            node = node.leftChild;
        }
        return minv;
    }

    public void CreateExampleTree()
    {
        ClearTree();
        int[] values = new int[] { 10, 5, 15, 3, 7, 12, 17 };
        foreach (int value in values)
        {
            Insert(value);
        }
    }

    public int CalcularProfundidad()
    {
        return CalculateDepthRec(root);
    }

    private int CalculateDepthRec(BSTNode node)
    {
        if (node == null) return 0;
        
        int leftDepth = CalculateDepthRec(node.leftChild);
        int rightDepth = CalculateDepthRec(node.rightChild);
        
        return Math.Max(leftDepth, rightDepth) + 1;
    }

    public BSTNode GetRoot()
    {
        return root;
    }

    public AVLNode GetRootAVL()
    {
        throw new System.NotImplementedException("Este método es solo para árboles AVL");
    }

    public void ClearTree()
    {
        root = null;
        ClearVisualization();
    }

    private void InsertRec(BSTNode node, int value)
    {
        if (value < node.value)
        {
            if (node.leftChild == null)
                node.leftChild = new BSTNode(value);
            else
                InsertRec(node.leftChild, value);
        }
        else
        {
            if (node.rightChild == null)
                node.rightChild = new BSTNode(value);
            else
                InsertRec(node.rightChild, value);
        }
    }
}