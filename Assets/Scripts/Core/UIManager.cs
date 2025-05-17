using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : SingletonBase<UIManager>
{
    [SerializeField] private TMP_Text coinScoreText;
    [SerializeField] private TMP_Text distanceScoreText;

    [SerializeField] private GameObject titleImage;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderBoardPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject designhealthSlider;

    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button leaderBoardBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button gameQuitBtn;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button supportBtn;

    [SerializeField] private Button restartBtn_PauseMenu;
    [SerializeField] private Button restartBtn_GameOverMenu;
    [SerializeField] private Button quitBtn_PauseMenu;
    [SerializeField] private Button quitBtn_GameOverMenu;

    [SerializeField] private TMP_Text gameOverCurrentScoreText;
    [SerializeField] private TMP_Text gameOverHighScoreText;

    private Coroutine healthDesignCoroutine;

    private void Start()
    {
        EventManager.Instance.OnCoinScoreChanged += UpdateCoinScore;
        EventManager.Instance.OnDistanceUpdated += UpdateDistanceScore;
        EventManager.Instance.OnLifeUpdated += UpdateLivesUI;
        EventManager.Instance.OnPlayerDied += OpenGameOverPanel;
        EventManager.Instance.OnShieldCollected += UpdateHealthSliderDesign;
        EventManager.Instance.OnGameOverUIUpdate += SetGameOverScoreTexts;

        startBtn.onClick.AddListener(StartButtonClicked);
        settingsBtn.onClick.AddListener(SettingsButtonClicked);
        leaderBoardBtn.onClick.AddListener(LeaderBoardButtonClicked);
        exitBtn.onClick.AddListener(ExitButtonClicked);
        gameQuitBtn.onClick.AddListener(QuitApplicationButtonClicked);
        pauseBtn.onClick.AddListener(PauseButtonClicked);
        resumeBtn.onClick.AddListener(ResumeButtonClicked);

        bgmSlider.onValueChanged.AddListener(UpdateBGMSlider);
        sfxSlider.onValueChanged.AddListener(UpdateSFXSlider);
        supportBtn.onClick.AddListener(OpenPortfolio);

        restartBtn_PauseMenu.onClick.AddListener(OnResetButtonClicked);
        restartBtn_GameOverMenu.onClick.AddListener(OnResetButtonClicked);
        quitBtn_PauseMenu.onClick.AddListener(OnQuitButtonClicked);
        quitBtn_GameOverMenu.onClick.AddListener(OnQuitButtonClicked);
    }

    private void UpdateCoinScore(string score)
    {
        coinScoreText.text = score;
    }

    private void UpdateDistanceScore(float score)
    {
        string newScore = ((int)score).ToString() + "m";
        distanceScoreText.text = newScore;
    }

    private void UpdateLivesUI(int lives)
    {
        healthSlider.value = lives;
    }

    private void SetGameOverScoreTexts(string currentScore, string highScore)
    {
        gameOverCurrentScoreText.text = "Score : " + currentScore;
        gameOverHighScoreText.text = "HighScore : " + highScore;
    }

    private void UpdateHealthSliderDesign(float duration)
    {
        if (healthDesignCoroutine == null)
        {
            healthDesignCoroutine = StartCoroutine(UpdateHealthSliderDesign_Coroutine(duration));
        }
        else
        {
            StopAllCoroutines();
            healthDesignCoroutine = StartCoroutine(UpdateHealthSliderDesign_Coroutine(duration));
        }
    }

    IEnumerator UpdateHealthSliderDesign_Coroutine(float duration)
    {
        designhealthSlider.SetActive(true);
        yield return new WaitForSeconds(duration);
        designhealthSlider.SetActive(false);
        healthDesignCoroutine = null;
    }

    private void CloseMainMenuPanel()
    {
        mainMenuPanel.SetActive(false);
    }

    private void OpenGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    private void StartButtonClicked()
    {
        startBtn.gameObject.SetActive(false);
        titleImage.gameObject.SetActive(false);
        CloseMainMenuPanel();
        gameUIPanel.SetActive(true);
        pausePanel.SetActive(false);
        EventManager.Instance.TriggerGameStart();
    }

    private void SettingsButtonClicked()
    {
        settingsPanel.SetActive(true);
    }

    private void LeaderBoardButtonClicked()
    {
        leaderBoardPanel.SetActive(true);
    }

    private void ExitButtonClicked()
    {
        exitPanel.SetActive(true);
    }

    private void PauseButtonClicked()
    {
        pausePanel.SetActive(true);
        EventManager.Instance.TriggerGamePause();
    }

    private void ResumeButtonClicked()
    {
        pausePanel.SetActive(false);
        EventManager.Instance.TriggerGameResume();
    }

    private void OpenPortfolio()
    {
        Application.OpenURL("https://darshansahay2808.wixsite.com/myportfolio");
    }

    private void QuitApplicationButtonClicked()
    {
        Application.Quit();
    }

    private void OnResetButtonClicked()
    {
        UpdateCoinScore(0.ToString());
        UpdateDistanceScore(0f);
        UpdateLivesUI(3);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        designhealthSlider.SetActive(false);
        EventManager.Instance.TriggerGameRestart();
    }

    private void OnQuitButtonClicked()
    {
        int index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

    private void UpdateBGMSlider(float value)
    {
        EventManager.Instance.TriggerOnBGMAudioLevelChanged(value);
    }

    private void UpdateSFXSlider(float value)
    {
        EventManager.Instance.TriggerOnSFXAudioLevelChanged(value);
    }
}
