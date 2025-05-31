using UnityEngine;
using TMPro;

public class StatusTextController : MonoBehaviour
{
    private TMP_Text statusText;
    private TreeManager treeManager;
    private float messageDisplayTime = 3f;
    private float fadeTime = 1f;

    void Start()
    {
        statusText = GetComponent<TMP_Text>();
        treeManager = FindFirstObjectByType<TreeManager>();
        ConfigureStatusText();
    }

    private void ConfigureStatusText()
    {
        if (statusText != null)
        {
            statusText.text = "Tree Manager Ready";
            statusText.fontSize = 24;
            statusText.alignment = TextAlignmentOptions.Center;
        }
    }

    public void ShowMessage(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.alpha = 1f;
            CancelInvoke();
            Invoke(nameof(FadeOutText), messageDisplayTime);
        }
    }

    private void FadeOutText()
    {
        StartCoroutine(FadeTextRoutine());
    }

    private System.Collections.IEnumerator FadeTextRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            statusText.alpha = alpha;
            yield return null;
        }
    }
}