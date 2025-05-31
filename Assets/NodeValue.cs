using TMPro;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public int value;

    public void SetValue(int newValue)
    {
        value = newValue;
        valueText.text = newValue.ToString();
    }
}