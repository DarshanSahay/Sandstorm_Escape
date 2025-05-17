using System;
using UnityEngine;

public class EventManager : SingletonBase<EventManager>
{
    // Game State Events
    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnGameRestart;
    public event Action OnGamePaused;
    public event Action OnGameResumed;

    // Scoring Events
    public event Action<int> OnCoinCollected;      // param: coins
    public event Action<string> OnCoinScoreChanged;
    public event Action<float> OnScoreBoostCollected;      // param: coins
    public event Action<float> OnShieldCollected;      // param: coins
    public event Action<float> OnDistanceUpdated;  // param: distance
    public event Action<int> OnScoreUpdated;       // param: score
    public event Action<int> OnComboHit;           // param: combo count

    // Player Status Events
    public event Action OnPlayerHitObstacle;
    public event Action OnPlayerDied;
    public event Action<int> OnLifeUpdated;        // param: remaining lives
    public event Action<GameObject> OnObjectReturnToPool;

    public event Action OnPlayerJumped;
    public event Action OnPlayerDoubleJumped;

    //Powerup Events
    public event Action<PowerUpData> OnPowerupPicked;     // param: powerup
    public event Action<PowerUpData> OnPowerupExpired;    // param: powerup

    public event Action<float> OnWorldSpeedUpdate;

    public event Action<float> OnBGMAudioLevelChanged;
    public event Action<float> OnSFXAudioLevelChanged;

    public event Action<string, string> OnGameOverUIUpdate;

    // ==========================
    // == Invoke Methods Below ==
    // ==========================

    // Game State
    public void TriggerGameStart() => OnGameStart?.Invoke();
    public void TriggerGameOver() => OnGameOver?.Invoke();
    public void TriggerGameRestart() => OnGameRestart?.Invoke();
    public void TriggerGamePause() => OnGamePaused?.Invoke();
    public void TriggerGameResume() => OnGameResumed?.Invoke();

    // Scoring
    public void TriggerCoinCollected(int amount) => OnCoinCollected?.Invoke(amount);
    public void TriggerOnCoinScoreChanged(string score) => OnCoinScoreChanged?.Invoke(score);
    public void TriggerOnScoreBoostCollected(float duration) => OnScoreBoostCollected?.Invoke(duration);
    public void TriggerOnShieldCollected(float duration) => OnShieldCollected?.Invoke(duration);
    public void TriggerDistanceUpdated(float distance) => OnDistanceUpdated?.Invoke(distance);
    public void TriggerScoreUpdated(int score) => OnScoreUpdated?.Invoke(score);
    public void TriggerComboHit(int comboCount) => OnComboHit?.Invoke(comboCount);
    public void TriggerOnObjectReturnToPool(GameObject item) => OnObjectReturnToPool?.Invoke(item);

    // Player Status
    public void TriggerPlayerHitObstacle() => OnPlayerHitObstacle?.Invoke();
    public void TriggerPlayerDied() => OnPlayerDied?.Invoke();
    public void TriggerLifeUpdated(int lives) => OnLifeUpdated?.Invoke(lives);

    public void TriggerOnPlayerJumped() => OnPlayerJumped?.Invoke();
    public void TriggerOnPlayerDoubleJumped() => OnPlayerDoubleJumped?.Invoke();
    // Powerups
    public void TriggerPowerupPicked(PowerUpData data) => OnPowerupPicked?.Invoke(data);
    public void TriggerPowerupExpired(PowerUpData data) => OnPowerupExpired?.Invoke(data);

    public void TriggerOnWorldSpeedUpdate(float speed) => OnWorldSpeedUpdate?.Invoke(speed);

    public void TriggerOnBGMAudioLevelChanged(float volume) => OnBGMAudioLevelChanged?.Invoke(volume);
    public void TriggerOnSFXAudioLevelChanged(float volume) => OnSFXAudioLevelChanged?.Invoke(volume);

    public void TriggerOnGameOverUIUpdate(string score, string highScore) => OnGameOverUIUpdate?.Invoke(score, highScore);
}
