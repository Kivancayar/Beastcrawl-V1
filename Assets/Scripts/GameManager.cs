using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int score = 0;
    public Text scoreText;

    public static int killScore = 0;
    public Text killScoreText;

    void Update()
    {
        // Coin sayısı
        scoreText.text = "Coins: " + score.ToString();

        // Kill skoru
        killScoreText.text = "Score: " + killScore.ToString();
    }
}
