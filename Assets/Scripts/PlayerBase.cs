using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerBase : MonoBehaviour
{
    // Change from protected to public get, keep protected set
    public ITree ArbolJugador { get; protected set; }
    protected Queue<int> Retos { get; private set; }
    protected TreeManager treeManager;

    protected virtual void Start()
    {
        treeManager = FindObjectOfType<TreeManager>();
        InitializeTree();
    }

    protected virtual void InitializeTree()
    {
        // Reference to TreeManager's nodePrefab
        GameObject nodePrefab = treeManager.nodePrefab;
        
        if (nodePrefab == null)
        {
            Debug.LogError("NodePrefab no encontrado en TreeManager!");
            return;
        }

        // 50% probabilidad de cada tipo de árbol
        if (Random.value > 0.5f)
        {
            var bst = gameObject.AddComponent<BinarySearchTree2D>();
            bst.nodePrefab = nodePrefab;
            ArbolJugador = bst;
        }
        else
        {
            var avl = gameObject.AddComponent<AVLTree>();
            avl.nodePrefab = nodePrefab;
            ArbolJugador = avl;
        }
        
        GenerarRetos();
    }

    protected bool VerificarReto()
    {
        if (Retos == null || Retos.Count == 0 || ArbolJugador == null) 
        {
            return false;
        }
        
        int profundidadObjetivo = Retos.Peek();
        int profundidadActual = ArbolJugador.CalcularProfundidad();
        
        if (profundidadActual >= profundidadObjetivo)
        {
            Retos.Dequeue(); // Remover el reto completado
            return true;
        }
        
        return false;
    }

    protected virtual void GenerarRetos()
    {
        Retos = new Queue<int>();
        for (int i = 0; i < 3; i++)
        {
            Retos.Enqueue(Random.Range(2, 6));
        }
        Debug.Log($"Retos generados para {gameObject.name}: {string.Join(", ", Retos.ToArray())}");
    }

    // Método abstracto que las clases hijas deben implementar
    public abstract void AgregarPuntaje(int valor);

    // Remove powerSystem field from base class
    // Instead, use an abstract property
    protected abstract PowerSystem PowerSystemComponent { get; }
}