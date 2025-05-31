using UnityEngine;
using UnityEngine.UI;  // Add this line
using TMPro;

public class ChallengeUI : MonoBehaviour
{
    private TextMeshProUGUI player1ChallengeText;
    private TextMeshProUGUI player2ChallengeText;
    private TextMeshProUGUI player3ChallengeText;
    private TextMeshProUGUI player1DepthText;
    private TextMeshProUGUI player2DepthText;
    private TextMeshProUGUI player3DepthText;

    [Header("Colors")]
    [SerializeField] private Color targetColor = Color.yellow;
    [SerializeField] private Color currentColor = Color.white;
    [SerializeField] private Color depthColor = Color.cyan;

    private void Awake()
    {
        CreateUIElements();
    }

    private void CreateUIElements()
    {
        // Create Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("UI Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Player 1 (Top left)
        player1ChallengeText = CreateTextElement("P1Challenge", canvas.transform, new Vector2(50, -25), TextAlignmentOptions.Left, 22);
        player1DepthText = CreateTextElement("P1Depth", canvas.transform, new Vector2(50, -45), TextAlignmentOptions.Left, 18);

        // Player 2 (Top center)
        player2ChallengeText = CreateTextElement("P2Challenge", canvas.transform, new Vector2((Screen.width/2) - 100, -25), TextAlignmentOptions.Center, 22);
        player2DepthText = CreateTextElement("P2Depth", canvas.transform, new Vector2((Screen.width/2) - 100, -45), TextAlignmentOptions.Center, 18);

        // Player 3 (Top right)
        player3ChallengeText = CreateTextElement("P3Challenge", canvas.transform, new Vector2(Screen.width - 250, -25), TextAlignmentOptions.Right, 22);
        player3DepthText = CreateTextElement("P3Depth", canvas.transform, new Vector2(Screen.width - 250, -45), TextAlignmentOptions.Right, 18);
    }

    private TextMeshProUGUI CreateTextElement(string name, Transform parent, Vector2 position, TextAlignmentOptions alignment, float fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1); // Anchor to top
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(200, 30);

        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.fontSize = fontSize;
        tmpText.alignment = alignment;
        tmpText.font = TMP_Settings.defaultFontAsset;
        tmpText.color = Color.white;

        return tmpText;
    }

    private void Update()
    {
        if (ChallengeManager.Instance == null)
        {
            Debug.LogWarning("ChallengeManager.Instance is still null in ChallengeUI");
            return;
        }

        UpdatePlayerInfo("Player", player1ChallengeText, player1DepthText);
        UpdatePlayerInfo("Player2", player2ChallengeText, player2DepthText);
        UpdatePlayerInfo("Player3", player3ChallengeText, player3DepthText);
    }

    private void UpdatePlayerInfo(string playerTag, TextMeshProUGUI challengeText, TextMeshProUGUI depthText)
    {
        if (challengeText != null && depthText != null)
        {
            int currentIndex = ChallengeManager.Instance.GetCurrentChallengeIndex(playerTag);
            int targetDepth = ChallengeManager.Instance.GetCurrentChallengeDepth(playerTag);
            int currentDepth = GetPlayerTreeDepth(playerTag);
            
            // Update challenge text
            if (currentIndex >= 0 && targetDepth > 0)
            {
                challengeText.text = $"Reto {currentIndex + 1}: Prof. {targetDepth}";
                challengeText.color = targetColor;
            }
            else
            {
                challengeText.text = "¡Retos Completados!";
                challengeText.color = currentColor;
            }

            // Update current depth text
            depthText.text = $"Prof. actual: {currentDepth}";
            depthText.color = depthColor;
        }
    }

    private int GetPlayerTreeDepth(string playerTag)
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            PlayerBase playerBase = player.GetComponent<PlayerBase>();
            if (playerBase != null && playerBase.Tree != null)
            {
                return playerBase.Tree.CalcularProfundidad();
            }
        }
        return 0;
    }
}