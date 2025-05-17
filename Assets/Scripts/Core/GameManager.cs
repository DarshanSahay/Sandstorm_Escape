using System.IO;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public bool isGameRunning;
    public GameObject player;
    public int currentScore;
    public int highScore;

    private string saveFilePath;

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "SandStormsave.json");
        LoadData();

        Time.timeScale = 1f;
        EventManager.Instance.OnDistanceUpdated += UpdateScore;
        EventManager.Instance.OnGameStart += SetGameStatusOn;
        EventManager.Instance.OnGameOver += OnGameOver;
        EventManager.Instance.OnGamePaused += GamePause;
        EventManager.Instance.OnGameResumed += GameResume;
        EventManager.Instance.OnGameRestart += SetGameStatusOn;
    }

    private void SetGameStatusOn()
    {
        isGameRunning = true;
        player.SetActive(true);
        Time.timeScale = 1f;
    }

    private void SetGameStatusOff()
    {
        isGameRunning = false;
        Time.timeScale = 0f;
        player.SetActive(false);
    }

    private void UpdateScore(float score)
    {
        currentScore = (int)score;
    }

    private void OnGameOver()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveData();
        }

        UpdateScoreUI();
        SetGameStatusOff();
    }

    private void UpdateScoreUI()
    {
        EventManager.Instance.TriggerOnGameOverUIUpdate(currentScore.ToString(), highScore.ToString());
    }

    private void GamePause()
    {
        Time.timeScale = 0f;
    }

    private void GameResume()
    {
        Time.timeScale = 1f;
    }

    private void SaveData()
    {
        SaveData data = new SaveData { highScore = highScore };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            highScore = data.highScore;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public int highScore;
}