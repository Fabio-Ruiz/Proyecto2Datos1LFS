using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    
    // Mantener track de los puntajes
    private int player1CurrentScore = 0;
    private int player2CurrentScore = 0;

    void Start()
    {
        // Initialize scores to 0
        UpdateScoreDisplay();
    }

    public void UpdatePlayerScore(string playerTag, int score)
    {
        if (playerTag == "Player")
        {
            player1CurrentScore = score;
            UpdateScoreDisplay();
        }
        else if (playerTag == "Player2")
        {
            player2CurrentScore = score;
            UpdateScoreDisplay();
        }
    }

    private void UpdateScoreDisplay()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = $"P1 Score: {player1CurrentScore}";
            
        if (player2ScoreText != null)
            player2ScoreText.text = $"P2 Score: {player2CurrentScore}";
    }
}
