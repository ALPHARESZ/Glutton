using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score Settings")]
    public int score = 20;
    public int winningScore = 30;
    public int losingScore = 0;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text resultText;

    [Header("Game State")]
    public bool isGameOver = false;

    void Awake()
    {
        // Pastikan hanya satu instance yang aktif
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // opsional jika ingin persist
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Reset kondisi game
        isGameOver = false;
        Time.timeScale = 1f;

        // Pastikan UI direset setiap game dijalankan
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (resultText != null)
        {
            resultText.text = "";
            resultText.enabled = true; // sembunyikan dulu
        }
    }

    // Dipanggil setiap kali skor berubah
    public void AddScore(int amount)
    {
        if (isGameOver) return; // Jangan ubah skor kalau game sudah selesai

        score += amount;
        UpdateScoreUI();
        CheckGameState();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    // Mengecek kondisi menang atau kalah
    void CheckGameState()
    {
        if (score >= winningScore)
        {
            GameOver(true);
        }
        else if (score <= losingScore)
        {
            GameOver(false);
        }
    }

    void GameOver(bool win)
    {
        isGameOver = true;

        if (resultText != null)
        {
            resultText.enabled = true; // pastikan aktif
            resultText.text = win ? "YOU WIN!" : "YOU LOSE!";
            resultText.color = win ? Color.green : Color.red;
        }

        // Tidak perlu tunggu frame, langsung freeze setelah teks tampil
        Invoke(nameof(FreezeGame), 0.05f);
    }

    void FreezeGame()
    {
        Time.timeScale = 0f; // hentikan permainan setelah pesan tampil
    }
}
