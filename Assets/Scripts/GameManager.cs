using UnityEngine;
using UnityEngine.UI; // UI kütüphanesini eklemeyi unutma

public class GameManager : MonoBehaviour
{
    public static int score = 0;
    public Text scoreText; // Ekranda göstereceğimiz yazı alanı

    void Update()
    {
        // Her karede skor yazısını güncelle
        scoreText.text = "Score: " + score.ToString();
    }
}