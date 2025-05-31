using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI player3ScoreText;  // Nuevo texto para Player 3
    
    private readonly Color32 playerTextColor = new Color32(0, 0, 0, 255);      // Negro
    private readonly Color32 scoreTextColor = new Color32(255, 255, 0, 255);   // Amarillo
    
    // Mantener track de los puntajes
    private int player1CurrentScore = 0;
    private int player2CurrentScore = 0;
    private int player3CurrentScore = 0;  // Nuevo score para Player 3

    void Start()
    {
        ConfigureScorePositions();
        UpdateScoreDisplay();
    }

    private void ConfigureScorePositions()
    {
        if (player1ScoreText != null)
        {
            // Configurar Player 1 Score (esquina inferior izquierda)
            var rectTransform1 = player1ScoreText.rectTransform;
            rectTransform1.anchorMin = new Vector2(0, 0);
            rectTransform1.anchorMax = new Vector2(0, 0);
            rectTransform1.pivot = new Vector2(0, 0);
            rectTransform1.anchoredPosition = new Vector2(10, 10); // Padding desde la esquina
            ConfigureTextStyle(player1ScoreText);
        }

        if (player2ScoreText != null)
        {
            // Configurar Player 2 Score (centro inferior)
            var rectTransform2 = player2ScoreText.rectTransform;
            rectTransform2.anchorMin = new Vector2(0.5f, 0);
            rectTransform2.anchorMax = new Vector2(0.5f, 0);
            rectTransform2.pivot = new Vector2(0.5f, 0);
            rectTransform2.anchoredPosition = new Vector2(0, 10); // Padding desde abajo
            ConfigureTextStyle(player2ScoreText);
        }

        if (player3ScoreText != null)
        {
            // Configurar Player 3 Score (esquina inferior derecha)
            var rectTransform3 = player3ScoreText.rectTransform;
            rectTransform3.anchorMin = new Vector2(1, 0);
            rectTransform3.anchorMax = new Vector2(1, 0);
            rectTransform3.pivot = new Vector2(1, 0);
            rectTransform3.anchoredPosition = new Vector2(-100, 10); // Changed from -50 to -100
            ConfigureTextStyle(player3ScoreText);
        }
    }

    private void ConfigureTextStyle(TextMeshProUGUI text)
    {
        // Configurar outline para mejor legibilidad
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;
        // Activar el outline
        text.enabled = true;
    }

    public void UpdatePlayerScore(string playerTag, int score)
    {
        switch (playerTag)
        {
            case "Player":  // Changed from "Player1" to "Player"
                player1CurrentScore = score;
                break;
            case "Player2":
                player2CurrentScore = score;
                break;
            case "Player3":
                player3CurrentScore = score;
                break;
        }
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = $"<color=#000000>P1</color> <color=#FFFF00>SCORE: {player1CurrentScore}</color>";
            
        if (player2ScoreText != null)
            player2ScoreText.text = $"<color=#000000>P2</color> <color=#FFFF00>SCORE: {player2CurrentScore}</color>";

        if (player3ScoreText != null)
            player3ScoreText.text = $"<color=#000000>P3</color> <color=#FFFF00>SCORE: {player3CurrentScore}</color>";
    }
}
