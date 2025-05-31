public interface ITree
{
    void Insert(int value);
    bool Search(int value);
    void Delete(int value);
    void ClearTree();  // This is our existing clear method
    void CreateExampleTree();
    int CalcularProfundidad();
    BSTNode GetRoot();     // Para BST
    AVLNode GetRootAVL();  // Para AVL
}