using UnityEngine;
using TMPro;  // Añadir esta referencia para TextMeshPro

public class PlayerTreeViewer : MonoBehaviour 
{
    public PlayerBase player;
    public Vector2 viewportPosition = new Vector2(0, 1f);
    public Vector2 viewportSize = new Vector2(0.3f, 0.3f);
    
    // Ajustar los valores para mejor visualización
    private float baseNodeSize = 1.5f;            // Aumentado de 0.8f a 1.5f
    private float baseHorizontalSpacing = 2.0f;   // Aumentado de 1.5f a 2.0f
    private float baseVerticalSpacing = 1.8f;     // Aumentado de 1.2f a 1.8f
    private float viewportPadding = 0.1f;
    
    private Camera treeCamera;
    private GameObject treeViewContainer;
    private static PlayerTreeViewer instance;
    private bool isInitialized = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Multiple PlayerTreeViewer instances found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in PlayerTreeViewer!");
            return;
        }

        if (!isInitialized)
        {
            InitializeViewer();
            isInitialized = true;
        }
    }

    private void InitializeViewer()
    {
        // Limpiar cualquier cámara o contenedor existente
        CleanupExistingObjects();
        
        CreateTreeCamera();
        CreateTreeContainer();
    }

    private void CleanupExistingObjects()
    {
        // Limpiar cámaras antiguas
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            if (cam.gameObject.name.Contains("TreeCamera_"))
            {
                Destroy(cam.gameObject);
            }
        }

        // Limpiar objetos del árbol usando Layer
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("TreeView"))
            {
                Destroy(obj);
            }
        }
    }

    private void CreateTreeCamera()
    {
        if (treeCamera != null)
        {
            Destroy(treeCamera.gameObject);
        }

        GameObject camObj = new GameObject("TreeCamera_" + player.gameObject.name);
        treeCamera = camObj.AddComponent<Camera>();
        treeCamera.orthographic = true;
        treeCamera.orthographicSize = 3f;        // Reducido para mejor visualización
        treeCamera.cullingMask = LayerMask.GetMask("TreeView");
        treeCamera.clearFlags = CameraClearFlags.SolidColor;  // Cambiado para mejor debug
        treeCamera.backgroundColor = new Color(0, 0, 0, 0.1f); // Fondo semi-transparente
        treeCamera.depth = 1;
        
        // Ajustar el viewport para la esquina superior izquierda
        treeCamera.rect = new Rect(0, 0.7f, 0.3f, 0.3f);
        
        camObj.transform.position = new Vector3(0, 0, -10f);
        camObj.transform.SetParent(transform);
    }

    private void CreateTreeContainer()
    {
        // Primero destruir el contenedor anterior si existe
        if (treeViewContainer != null)
        {
            Destroy(treeViewContainer);
        }

        treeViewContainer = new GameObject("TreeViewContainer_" + player.gameObject.name);
        treeViewContainer.layer = LayerMask.NameToLayer("TreeView");
        treeViewContainer.transform.SetParent(transform);
        
        Vector3 viewportCenter = treeCamera.ViewportToWorldPoint(
            new Vector3(viewportSize.x * 0.5f, 1f - (viewportSize.y * 0.5f), 10f)
        );
        treeViewContainer.transform.position = viewportCenter;
    }

    private void ClearTreeVisualization()
    {
        if (treeViewContainer != null)
        {
            foreach (Transform child in treeViewContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void UpdateTreeVisualization()
    {
        ClearTreeVisualization();

        if (player?.ArbolJugador == null)
        {
            Debug.LogWarning("No tree available to visualize");
            return;
        }

        Debug.Log($"Updating tree visualization. Tree type: {player.ArbolJugador.GetType()}");
        float initialScale = CalculateInitialScale();

        if (player.ArbolJugador is BinarySearchTree2D bst)
        {
            var root = bst.GetRoot();
            if (root != null)
            {
                Debug.Log($"Visualizing BST with root value: {root.value}");
                VisualizeBSTNode(root, Vector3.zero, initialScale);
            }
        }
        else if (player.ArbolJugador is AVLTree avl)
        {
            var root = avl.GetRootAVL();
            if (root != null)
            {
                Debug.Log($"Visualizing AVL with root value: {root.value}");
                VisualizeAVLNode(root, Vector3.zero, initialScale);
            }
        }
    }

    private void VisualizeBSTNode(BSTNode node, Vector3 position, float scale = 1.0f)
    {
        if (node == null) return;

        CreateVisualNode(position, node.value, scale);

        // Calcular espaciado ajustado
        float adjustedHorizontalSpacing = baseHorizontalSpacing * scale;
        float adjustedVerticalSpacing = baseVerticalSpacing * scale;

        // Ajustar posiciones de los hijos
        if (node.leftChild != null)
        {
            Vector3 leftPos = position + new Vector3(-adjustedHorizontalSpacing, -adjustedVerticalSpacing, 0);
            CreateVisualLine(position, leftPos);
            VisualizeBSTNode(node.leftChild, leftPos, scale * 0.9f); // Reducir escala gradualmente
        }

        if (node.rightChild != null)
        {
            Vector3 rightPos = position + new Vector3(adjustedHorizontalSpacing, -adjustedVerticalSpacing, 0);
            CreateVisualLine(position, rightPos);
            VisualizeBSTNode(node.rightChild, rightPos, scale * 0.9f); // Reducir escala gradualmente
        }
    }

    private void VisualizeAVLNode(AVLNode node, Vector3 position, float scale = 1.0f)
    {
        if (node == null) return;

        // Similar a VisualizeBSTNode pero para nodos AVL
        CreateVisualNode(position, node.value, scale);

        // Calcular espaciado ajustado
        float adjustedHorizontalSpacing = baseHorizontalSpacing * scale;
        float adjustedVerticalSpacing = baseVerticalSpacing * scale;

        if (node.leftChild != null)
        {
            Vector3 leftPos = position + new Vector3(-adjustedHorizontalSpacing, -adjustedVerticalSpacing, 0);
            CreateVisualLine(position, leftPos);
            VisualizeAVLNode(node.leftChild, leftPos, scale * 0.9f);
        }

        if (node.rightChild != null)
        {
            Vector3 rightPos = position + new Vector3(adjustedHorizontalSpacing, -adjustedVerticalSpacing, 0);
            CreateVisualLine(position, rightPos);
            VisualizeAVLNode(node.rightChild, rightPos, scale * 0.9f);
        }
    }

    private float CalculateInitialScale()
    {
        int depth = player.ArbolJugador.CalcularProfundidad();
        // Ajustar escala basada en la profundidad y el viewport
        float scaleByDepth = Mathf.Max(0.4f, 1.0f - (depth * 0.15f));
        // Aplicar padding al viewport
        float viewportScale = Mathf.Min(viewportSize.x, viewportSize.y) * (1 - viewportPadding);
        return scaleByDepth * viewportScale;
    }

    void Update()
    {
        // Solo actualizar cuando sea necesario
        if (player != null && player.ArbolJugador != null)
        {
            if (!isInitialized)
            {
                InitializeViewer();
                isInitialized = true;
            }
            
            // Agregar verificación de cambios en el árbol
            if (ShouldUpdateVisualization())
            {
                UpdateTreeVisualization();
            }
        }
    }

    private bool ShouldUpdateVisualization()
    {
        // Implementar lógica para verificar si el árbol ha cambiado
        // Por ahora, actualizamos cada frame
        return true;
    }

    private void CreateVisualNode(Vector3 position, int value, float scale)
    {
        GameObject nodeObj = new GameObject("Node");
        nodeObj.transform.SetParent(treeViewContainer.transform);
        nodeObj.transform.localPosition = position;
        nodeObj.transform.localScale = Vector3.one * (baseNodeSize * scale);
        nodeObj.layer = LayerMask.NameToLayer("TreeView");

        // Agregar SpriteRenderer para el círculo
        SpriteRenderer sr = nodeObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = Color.white;
        sr.sortingOrder = 1; // Asegurar que se dibuje sobre las líneas

        // Crear texto
        GameObject textObj = new GameObject("NodeText");
        textObj.transform.SetParent(nodeObj.transform);
        textObj.transform.localPosition = Vector3.zero;
        textObj.layer = LayerMask.NameToLayer("TreeView");

        // Usar TextMeshPro en lugar de TextMeshProUGUI
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = value.ToString();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 3;
        tmp.color = Color.black;
        tmp.transform.localScale = Vector3.one * 0.5f;
    }

    private Sprite CreateCircleSprite()
    {
        // Crear un sprite circular básico
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float dx = x - 15.5f;
                float dy = y - 15.5f;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (distance < 15)
                    colors[y * 32 + x] = Color.white;
                else
                    colors[y * 32 + x] = Color.clear;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    private void CreateVisualLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(treeViewContainer.transform);
        lineObj.layer = LayerMask.NameToLayer("TreeView");

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.sortingOrder = 0; // Dibujar detrás de los nodos

        // Usar material con color sólido
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.black;
        line.endColor = Color.black;
    }

    void OnEnable()
    {
        if (isInitialized)
        {
            UpdateTreeVisualization();
        }
    }

    void OnDisable()
    {
        ClearTreeVisualization();
    }

    private void OnDestroy()
    {
        if (treeCamera != null)
            Destroy(treeCamera.gameObject);
            
        if (treeViewContainer != null)
            Destroy(treeViewContainer);
    }
}