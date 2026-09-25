using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI scoreTextHUD;      // Untuk UI HUD
    public TextMeshProUGUI scoreTextGameOver; // Untuk UI Game Over
    public TextMeshProUGUI scoreTextWin;      // Untuk UI Win

    private float _score;
    private float _currentScore;
    public int maxScore = 100; // Skor maksimum untuk menang

    void Start()
    {
        _score = 0;
        _currentScore = 0; // Inisialisasi _currentScore
        UpdateScoreHUD();
    }

    void Update()
    {
        if (PlayerManager.GameOver)
        {
            return;
        }

        // Update total score with current score
        _score = _currentScore;
        UpdateScoreHUD();

        // Cek apakah skor mencapai 100
        if (_score >= maxScore)
        {
            PlayerManager.GameOver = true; // Set game over jika menang
        }
    }

    public void TerasiHit()
    {
        _currentScore += 1f; // Menambah skor
        UpdateScoreHUD();
    }

    public void ObstacleHit()
    {
        _currentScore -= 5f; // Mengurangi skor
        if (_currentScore < 0)
        {
            _currentScore = 0;
        }
        UpdateScoreHUD();
    }

    private void UpdateScoreHUD()
    {
        if (scoreTextHUD != null)
        {
            scoreTextHUD.text = Mathf.Round(_score).ToString() + "/100";
        }
    }

    public void DisplayGameOverScore()
    {
        if (scoreTextGameOver != null)
        {
            scoreTextGameOver.text = "Branch: " + Mathf.Round(_score).ToString();
        }
        if (scoreTextWin != null) // Pastikan ini hanya diupdate di kondisi menang
        {
            scoreTextWin.text = "Branch: " + Mathf.Round(_score).ToString();
        }
    }

    public bool HasPlayerWon()
    {
        return _score >= maxScore;
    }
}