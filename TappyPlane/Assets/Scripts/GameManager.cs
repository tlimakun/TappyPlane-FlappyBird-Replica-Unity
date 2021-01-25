using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    [Header("Pages")]
    [SerializeField] private GameObject startPage;
    [SerializeField] private GameObject gameOverPage;
    [SerializeField] private GameObject countdownPage;

    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text roundScoreText;

    private int score;
    private bool _isGameOver = true;
    
    public bool IsGameOver { get { return _isGameOver; } }

    private enum Page { None, Start, GameOver, Countdown};

    private void Awake()
    {
        if (instance) { return; }
        instance = this;
    }

    private void Start()
    {
        highScoreText.text = "Highscore: " + PlayerPrefs.GetInt("HighScore").ToString();
        scoreText.text = "0";
    }

    private void OnEnable()
    {
        Countdown.OnCountdownFinished += OnCountdownFinished;
        PlaneController.OnPlayerScored += OnPlayerScored;
        PlaneController.OnPlayerDead += OnPlayerDead;
    }

    private void OnDisable()
    {
        Countdown.OnCountdownFinished -= OnCountdownFinished;
        PlaneController.OnPlayerScored -= OnPlayerScored;
        PlaneController.OnPlayerDead -= OnPlayerDead;
    }

    private void OnCountdownFinished()
    {
        SetPage(Page.None);
        OnGameStarted();
        score = 0;
        _isGameOver = false;
    }

    private void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    private void OnPlayerDead()
    {
        _isGameOver = true;
        roundScoreText.text = "Score: " + score.ToString();
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreText.text = "Highscore: " + score.ToString();
        }

        SetPage(Page.GameOver);
    }

    private void SetPage(Page page)
    {
        switch (page)
        {
            case Page.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case Page.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case Page.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case Page.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            default:
                Debug.LogError("No selected page");
                break;
        }
    }

    public void Replay()
    {
        // activate when replay button is hit
        OnGameOverConfirmed(); // event that can be called in this class only
        scoreText.text = "0";
        SetPage(Page.Countdown);
    }

    public void Play()
    {
        // activate when play button is hit
        SetPage(Page.Countdown);
    }
}
