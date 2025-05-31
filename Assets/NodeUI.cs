using TMPro;
using UnityEngine;

public class NodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    public void SetValue(int value)
    {
        if (valueText != null)
            valueText.text = value.ToString();
    }

    private void Awake()
    {
        if (valueText == null)
            valueText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.worldCamera = Camera.main;
        }
    }

}