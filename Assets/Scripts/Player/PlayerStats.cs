using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int CoinScore { get; private set; }
    public float DistanceScore { get; private set; }

    public int PlayerLives { get; private set; }

    private float totalDistance;
    private bool isShieldActive;
    private float scoreBoosterMultiplier = 1f;
    private Coroutine scoreBoosterCoroutine;
    private Coroutine shieldCoroutine;

    private void Start()
    {
        CoinScore = 0;
        DistanceScore = 0f;
        PlayerLives = 3;

        EventManager.Instance.OnDistanceUpdated += UpdateDistanceScore;
        EventManager.Instance.OnCoinCollected += AddCoins;
        EventManager.Instance.OnPlayerHitObstacle += UpdateLives;
        EventManager.Instance.OnPlayerDied += ResetStats;
        EventManager.Instance.OnGameRestart += ResetStats;
        EventManager.Instance.OnPowerupPicked += UpdatePowerUpStatus;
    }

    public void AddCoins(int amount)
    {
        CoinScore += amount;
        EventManager.Instance.TriggerOnCoinScoreChanged(CoinScore.ToString());
    }

    private void UpdateDistanceScore(float distance)
    {
        totalDistance += distance * scoreBoosterMultiplier;
    }

    private void UpdateLives()
    {
        if (isShieldActive)
            return;

        PlayerLives -= 1;
        EventManager.Instance.TriggerLifeUpdated(PlayerLives);

        if (PlayerLives <= 0)
        {
            EventManager.Instance.TriggerPlayerDied();
            EventManager.Instance.TriggerGameOver();
        }
    }

    private void UpdatePowerUpStatus(PowerUpData data)
    {
        switch (data.type)
        {
            case PowerUpType.ScoreBooster:
                if (scoreBoosterCoroutine != null)
                    StopCoroutine(scoreBoosterCoroutine);

                scoreBoosterCoroutine = StartCoroutine(RunScoreBooster(data));
                break;

            case PowerUpType.Shield:
                if (shieldCoroutine != null)
                    StopCoroutine(shieldCoroutine);

                shieldCoroutine = StartCoroutine(RunShield(data));
                break;
        }
    }

    IEnumerator RunScoreBooster(PowerUpData data)
    {
        EventManager.Instance.TriggerOnScoreBoostCollected(data.powerTimer);
        yield return new WaitForSeconds(data.powerTimer);
        scoreBoosterCoroutine = null;
    }

    IEnumerator RunShield(PowerUpData data)
    {
        isShieldActive = true;
        EventManager.Instance.TriggerOnShieldCollected(data.powerTimer);
        yield return new WaitForSeconds(data.powerTimer);
        isShieldActive = false;
        shieldCoroutine = null;
    }

    public void ResetStats()
    {
        CoinScore = 0;
        DistanceScore = 0f;
        PlayerLives = 3;
        isShieldActive = false;
        scoreBoosterMultiplier = 1f;
        EventManager.Instance.TriggerOnCoinScoreChanged(CoinScore.ToString());
        EventManager.Instance.TriggerDistanceUpdated(DistanceScore);
        StopAllCoroutines();
    }
}
