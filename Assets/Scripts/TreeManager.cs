using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TreeManager : MonoBehaviour
{
    [Header("Tree Management")]
    public GameObject bstTreeObject;
    public GameObject avlTreeObject;
    public TMP_InputField valueInputField;

    [Header("UI References")]
    public TMP_Dropdown treeTypeDropdown;
    public TMP_Text statusText;

    [Header("Visualización de Árbol de Jugador")]
    public Transform playerTreeContainer;
    public GameObject nodePrefab;
    public float miniTreeScale = 0.5f;
    public Vector2 miniTreeOffset = new Vector2(-8f, 4f);
    public float miniTreeHorizontalSpacing = 0.75f;
    public float miniTreeVerticalSpacing = 0.5f;
    public float lineWidth = 0.1f;

    [Header("Canvas Setup")]
    public Canvas treeCanvas;
    public RectTransform treeContainer;
    public Vector2 cornerOffset = new Vector2(10f, 10f);
    public Vector2 treeDisplaySize = new Vector2(200f, 150f);

    private ITree bstTree;
    private ITree avlTree;
    private ITree activeTree;
    private GameObject activeTreeGO;
    private enum TreeType { BST, AVL }
    private TreeType activeTreeType = TreeType.BST;
    private Dictionary<ITree, List<GameObject>> treeVisualizations = new Dictionary<ITree, List<GameObject>>();
    private StatusTextController statusTextController;

    void Start()
    {
        bstTree = bstTreeObject.GetComponent<ITree>();
        avlTree = avlTreeObject.GetComponent<ITree>();

        treeTypeDropdown.ClearOptions();
        treeTypeDropdown.AddOptions(new List<string> { "BST", "AVL" });
        treeTypeDropdown.onValueChanged.AddListener(OnTreeTypeChanged);

        SetActiveTree(TreeType.BST);
        statusTextController = statusText.GetComponent<StatusTextController>();
    }

    // Métodos de gestión del árbol principal
    public void OnTreeTypeChanged(int index)
    {
        // 0 = BST, 1 = AVL
        activeTreeType = (TreeType)index;
        activeTree = index == 0 ? bstTree : avlTree;
        VisualizarArbolJugador(activeTree, activeTreeGO.name);
    }

    private void SetActiveTree(TreeType treeType)
    {
        activeTreeType = treeType;

        if (activeTreeGO != null)
        {
            Destroy(activeTreeGO);
            activeTree = null;
        }

        activeTreeGO = treeType switch
        {
            TreeType.BST => bstTreeObject,
            TreeType.AVL => avlTreeObject,
            _ => null
        };

        if (activeTreeGO != null)
        {
            activeTreeGO.SetActive(true);
            activeTree = activeTreeGO.GetComponent<ITree>();
            VisualizarArbolJugador(activeTree, activeTreeGO.name);
        }

        UpdateStatusText();
    }

    public void InsertValue()
    {
        if (int.TryParse(valueInputField.text, out int value) && activeTree != null)
        {
            activeTree.Insert(value);
            VisualizarArbolJugador(activeTree, activeTreeGO.name);
            ShowStatus("Valor insertado: " + value);
        }
        else
        {
            ShowStatus("Entrada no válida.");
        }
    }

    public void SearchValue()
    {
        if (int.TryParse(valueInputField.text, out int value) && activeTree != null)
        {
            bool found = activeTree.Search(value);
            ShowStatus(found ? "Valor encontrado: " + value : "Valor no encontrado: " + value);
        }
        else
        {
            ShowStatus("Entrada no válida.");
        }
    }

    public void DeleteValue()
    {
        if (int.TryParse(valueInputField.text, out int value) && activeTree != null)
        {
            activeTree.Delete(value);
            VisualizarArbolJugador(activeTree, activeTreeGO.name);
            ShowStatus("Valor eliminado: " + value);
        }
        else
        {
            ShowStatus("Entrada no válida.");
        }
    }

    public void ClearTree()
    {
        if (activeTree != null)
        {
            activeTree.ClearTree();  // Changed from Clear() to ClearTree()
            VisualizarArbolJugador(activeTree, activeTreeGO.name);
            ShowStatus("Árbol limpiado.");
        }
    }

    public void CreateExampleTree()
    {
        if (activeTree != null)
        {
            activeTree.ClearTree();  // Changed from Clear() to ClearTree()
            int[] exampleValues = { 50, 30, 70, 20, 40, 60, 80 };
            foreach (int value in exampleValues)
            {
                activeTree.Insert(value);
            }
            VisualizarArbolJugador(activeTree, activeTreeGO.name);
            ShowStatus("Árbol de ejemplo creado.");
        }
    }

    // Métodos de visualización de árboles de jugadores
    public void VisualizarArbolJugador(ITree arbol, string jugadorNombre)
    {
        if (arbol == null) return;

        // Limpiar visualización anterior si existe
        if (treeVisualizations.ContainsKey(arbol))
        {
            foreach (var obj in treeVisualizations[arbol])
            {
                if (obj != null) Destroy(obj);
            }
            treeVisualizations[arbol].Clear();
        }
        else
        {
            treeVisualizations[arbol] = new List<GameObject>();
        }

        GameObject container = new GameObject($"TreeContainer_{jugadorNombre}");
        container.transform.SetParent(playerTreeContainer);
        container.transform.position = new Vector3(miniTreeOffset.x, miniTreeOffset.y, 0);
        container.transform.localScale = Vector3.one * miniTreeScale;

        if (arbol is BinarySearchTree2D bst)
        {
            BSTNode root = bst.GetRoot();
            if (root != null)
            {
                CalcularPosicionesMiniTree(root, Vector2.zero, miniTreeHorizontalSpacing);
                CrearNodosMiniTree(root, container.transform, arbol);
            }
        }
        else if (arbol is AVLTree avl)
        {
            AVLNode root = avl.GetRootAVL();
            if (root != null)
            {
                CalcularPosicionesMiniAVL(root, Vector2.zero, miniTreeHorizontalSpacing);
                CrearNodosMiniAVL(root, container.transform, arbol);
            }
        }
    }

    private void CalcularPosicionesMiniTree(BSTNode node, Vector2 position, float spacing)
    {
        if (node == null) return;

        node.position = position;
        float childSpacing = spacing * 0.5f;

        if (node.leftChild != null)
            CalcularPosicionesMiniTree(node.leftChild, 
                position + new Vector2(-spacing, -miniTreeVerticalSpacing), 
                childSpacing);

        if (node.rightChild != null)
            CalcularPosicionesMiniTree(node.rightChild, 
                position + new Vector2(spacing, -miniTreeVerticalSpacing), 
                childSpacing);
    }

    private void CrearNodosMiniTree(BSTNode node, Transform parent, ITree arbol)
    {
        if (node == null) return;

        // Crear nodo
        GameObject nodeObj = Instantiate(nodePrefab, 
            new Vector3(node.position.x, node.position.y, 0), 
            Quaternion.identity, 
            parent);

        nodeObj.GetComponent<NodeUI>()?.SetValue(node.value);
        treeVisualizations[arbol].Add(nodeObj);

        // Crear líneas
        if (node.leftChild != null)
        {
            CrearLineaMiniTree(node.position, node.leftChild.position, parent, arbol);
            CrearNodosMiniTree(node.leftChild, parent, arbol);
        }

        if (node.rightChild != null)
        {
            CrearLineaMiniTree(node.position, node.rightChild.position, parent, arbol);
            CrearNodosMiniTree(node.rightChild, parent, arbol);
        }
    }

    private void CrearLineaMiniTree(Vector2 start, Vector2 end, Transform parent, ITree arbol)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(parent);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        
        line.startWidth = lineWidth * miniTreeScale;
        line.endWidth = lineWidth * miniTreeScale;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(start.x, start.y, 0));
        line.SetPosition(1, new Vector3(end.x, end.y, 0));
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = -1;

        treeVisualizations[arbol].Add(lineObj);
    }

    private void CalcularPosicionesMiniAVL(AVLNode node, Vector2 position, float spacing)
    {
        if (node == null) return;

        node.position = position;
        float childSpacing = spacing * 0.5f;

        if (node.leftChild != null)
            CalcularPosicionesMiniAVL(node.leftChild, 
                position + new Vector2(-spacing, -miniTreeVerticalSpacing), 
                childSpacing);

        if (node.rightChild != null)
            CalcularPosicionesMiniAVL(node.rightChild, 
                position + new Vector2(spacing, -miniTreeVerticalSpacing), 
                childSpacing);
    }

    private void CrearNodosMiniAVL(AVLNode node, Transform parent, ITree arbol)
    {
        if (node == null) return;

        // Crear nodo
        GameObject nodeObj = Instantiate(nodePrefab, 
            new Vector3(node.position.x, node.position.y, 0), 
            Quaternion.identity, 
            parent);

        nodeObj.GetComponent<NodeUI>()?.SetValue(node.value);
        treeVisualizations[arbol].Add(nodeObj);

        // Crear líneas
        if (node.leftChild != null)
        {
            CrearLineaMiniTree(node.position, node.leftChild.position, parent, arbol);
            CrearNodosMiniAVL(node.leftChild, parent, arbol);
        }

        if (node.rightChild != null)
        {
            CrearLineaMiniTree(node.position, node.rightChild.position, parent, arbol);
            CrearNodosMiniAVL(node.rightChild, parent, arbol);
        }
    }

    private void ShowStatus(string message)
    {
        if (statusTextController != null)
        {
            statusTextController.ShowMessage(message);
        }
        Debug.Log(message);
    }

    private void UpdateStatusText()
    {
        string name = activeTreeType switch
        {
            TreeType.BST => "Árbol Binario de Búsqueda (BST)",
            TreeType.AVL => "Árbol AVL",
            _ => "Árbol"
        };
        ShowStatus("Árbol activo: " + name);
    }
}