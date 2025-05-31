using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BSTUIController2D : MonoBehaviour
{
    [SerializeField] private BinarySearchTree2D bst;
    [SerializeField] private TMP_InputField valueInput;
    [SerializeField] private Button insertButton;


    private void Start()
    {
        insertButton.onClick.AddListener(OnInsertClicked);
    }

    private void OnInsertClicked()
    {
        if (int.TryParse(valueInput.text, out int value))
        {
            bst.Insert(value);
            valueInput.text = "";
        }
        else
        {
            return;
        }
    }
}