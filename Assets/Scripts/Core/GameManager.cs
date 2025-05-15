using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public bool isGameRunning;
    public GameObject player;

    private void Start()
    {
        Time.timeScale = 1f;
        EventManager.Instance.OnGameStart += SetGameStatusOn;
        EventManager.Instance.OnGameOver += SetGameStatusOff;
        EventManager.Instance.OnGamePaused += GamePause;
        EventManager.Instance.OnGameResumed += GameResume;
        EventManager.Instance.OnGameRestart += GameResume;
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

    private void GamePause()
    {
        Time.timeScale = 0f;
    }

    private void GameResume()
    {
        Time.timeScale = 1f;
    }
}
