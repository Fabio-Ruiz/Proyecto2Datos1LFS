using UnityEngine;
using TMPro;

public class TreeTypeDropdownController : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private TreeManager treeManager;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        treeManager = FindFirstObjectByType<TreeManager>();

        // Clear and add options
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string> { "BST", "AVL" });
        
        // Add listener for value changes
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int value)
    {
        if (treeManager != null)
        {
            treeManager.OnTreeTypeChanged(value);
        }
    }
}