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
    [Header("AVL Tree Settings")]
    [SerializeField] protected GameObject nodePrefab;
    [SerializeField] protected float horizontalSpacing = 1.0f;  // Reducido de 1.5f
    [SerializeField] protected float verticalSpacing = 0.8f;    // Reducido de 1.0f
    
    protected AVLNode root;
    protected Dictionary<AVLNode, Vector2> nodePositions = new();
    protected List<GameObject> lineRenderers = new();

    protected virtual void Start()
    {
        if (nodePrefab == null)
        {
            Debug.LogError("NodePrefab no está asignado en AVLTree");
            return;
        }

        // Solo limpiar el árbol al inicio
        ClearTree();
        UpdateVisualization();
    }

    private void OnDestroy()
    {
        ClearTree();
    }

    private void OnDisable()
    {
        ClearTree();
    }

    public virtual void Insert(int value)
    {
        root = InsertNode(root, value);
        UpdateVisualization();
    }

    public virtual bool Search(int value) => SearchNode(root, value);

    public virtual void Delete(int value)
    {
        root = DeleteNode(root, value);
        UpdateVisualization();
    }

    public virtual void ClearTree()
    {
        // Primero destruir todos los GameObjects
        if (root != null)
        {
            VisitAllNodes(root, (node) => {
                if (node.nodeObject != null)
                {
                    Destroy(node.nodeObject);
                }
            });
        }

        foreach (var line in lineRenderers)
        {
            if (line != null)
                Destroy(line);
        }

        // Luego limpiar todas las estructuras de datos
        root = null;
        nodePositions.Clear();
        lineRenderers.Clear();
        
        // Forzar actualización visual
        UpdateVisualization();
        
        Debug.Log($"Tree cleared for {gameObject.name}");
    }
    // Método vacío - no queremos crear un árbol de ejemplo
    public void CreateExampleTree()
    {
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
    protected virtual void UpdateVisualization()
    {
        ClearVisualization();
        nodePositions.Clear();
        CalculateNodePositions(root, 0, 0, new HashSet<int>());
        VisualizeNodes(root);
        VisualizeLines(root);
    }

    protected void CalculateNodePositions(AVLNode node, float x, int depth, HashSet<int> usedX)
    {
        if (node == null) return;
        
        float startY = 3f; // Reducido de 4f para bajar la posición inicial
        float y = startY - (depth * verticalSpacing);

        CalculateNodePositions(node.leftChild, x - horizontalSpacing / (depth + 1), depth + 1, usedX);

        while (usedX.Contains(Mathf.RoundToInt(x * 100)))
            x += 0.1f;
        usedX.Add(Mathf.RoundToInt(x * 100));

        nodePositions[node] = new Vector2(x, y);

        CalculateNodePositions(node.rightChild, x + horizontalSpacing / (depth + 1), depth + 1, usedX);
    }

    protected void VisualizeNodes(AVLNode node)
    {
        if (node == null) return;

        Vector2 position = nodePositions[node];
        node.nodeObject = Instantiate(nodePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, transform);

        // Configure TextMeshPro
        var tmpText = node.nodeObject.GetComponent<TMPro.TextMeshPro>();
        if (tmpText != null)
        {
            tmpText.text = node.value.ToString();
            tmpText.fontSize = 3;  // Mantener mismo tamaño que BST
            tmpText.alignment = TMPro.TextAlignmentOptions.Center;
            tmpText.color = Color.black;
            tmpText.sortingOrder = 1;
        }

        // Ajustar tamaño del fondo
        var background = new GameObject("Background");
        background.transform.parent = node.nodeObject.transform;
        background.transform.localPosition = Vector3.zero;
        
        var spriteRenderer = background.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Square");
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = 0;
        background.transform.localScale = new Vector3(0.4f, 0.4f, 1f); // Reducido de 0.5f

        VisualizeNodes(node.leftChild);
        VisualizeNodes(node.rightChild);
    }


    protected void VisualizeLines(AVLNode node)
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

        line.startWidth = 0.05f;  // Mismo ancho que BST
        line.endWidth = 0.05f;    // Mismo ancho que BST
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(start.x, start.y, 0));
        line.SetPosition(1, new Vector3(end.x, end.y, 0));
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white; // Cambiado a blanco para igualar BST
        line.endColor = Color.white;   // Cambiado a blanco para igualar BST
        lineRenderers.Add(lineObj);
    }

    private void ClearVisualization()
    {
        if (root != null)
        {
            ClearNodes(root);
        }
    }

    private void ClearNodes(AVLNode node)
    {
        if (node == null) return;
        
        ClearNodes(node.leftChild);
        ClearNodes(node.rightChild);
        
        if (node.nodeObject != null)
        {
            Destroy(node.nodeObject);
            node.nodeObject = null;
        }
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
