using UnityEngine;
using TMPro;

public class ValueInputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    private TreeManager treeManager;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        treeManager = FindFirstObjectByType<TreeManager>();

        // Configurar el input field
        ConfigureInputField();
    }

    private void ConfigureInputField()
    {
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.characterLimit = 3;
        
        // Añadir validación en tiempo real
        inputField.onValidateInput += ValidateNumber;
        
        // Añadir listener para cuando se presiona Enter
        inputField.onSubmit.AddListener(OnSubmitValue);
    }

    private char ValidateNumber(string text, int charIndex, char addedChar)
    {
        // Solo permitir números
        if (char.IsDigit(addedChar))
            return addedChar;
        return '\0';
    }

    private void OnSubmitValue(string value)
    {
        if (int.TryParse(value, out int number))
        {
            treeManager.InsertValue();
            inputField.text = ""; // Limpiar después de insertar
        }
    }
}