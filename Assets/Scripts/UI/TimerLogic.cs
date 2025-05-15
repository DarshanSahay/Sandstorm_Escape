using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerLogic : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpTimerUI
    {
        public Image radialIconImage; // Radial fill
        public Image iconImage;   // Icon to show which powerup
        public GameObject parent;
    }

    [SerializeField] private PowerUpTimerUI shieldTimerUI;
    [SerializeField] private PowerUpTimerUI scoreBoostTimerUI;

    // Optional: In case you want to manage more in future
    private Dictionary<PowerUpType, Coroutine> activeTimers = new Dictionary<PowerUpType, Coroutine>();

    private void Start()
    {
        EventManager.Instance.OnPowerupPicked += OpenPowerUpTimer;
        EventManager.Instance.OnGameRestart += ResetAll;
    }

    private void OpenPowerUpTimer(PowerUpData data)
    {
        // If a timer is already running for this powerup, restart it
        if (activeTimers.ContainsKey(data.type))
        {
            StopCoroutine(activeTimers[data.type]);
            activeTimers.Remove(data.type);
        }

        Coroutine timerCoroutine = StartCoroutine(PowerUpTimerCoroutine(data));
        activeTimers[data.type] = timerCoroutine;
    }

    private IEnumerator PowerUpTimerCoroutine(PowerUpData data)
    {
        PowerUpTimerUI targetUI = GetTimerUI(data.type);
        if (targetUI == null) yield break;

        float duration = data.powerTimer;
        float timer = 0f;

        targetUI.radialIconImage.fillAmount = 1f;
        targetUI.radialIconImage.enabled = (true);
        targetUI.iconImage.enabled = (true);
        targetUI.parent.SetActive(true);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float remaining = Mathf.Clamp01(1f - timer / duration);
            targetUI.radialIconImage.fillAmount = remaining;
            yield return null;
        }

        targetUI.radialIconImage.enabled = (false);
        targetUI.iconImage.enabled = (false);
        targetUI.parent.SetActive(false);

        if (activeTimers.ContainsKey(data.type))
            activeTimers.Remove(data.type);
    }

    private PowerUpTimerUI GetTimerUI(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Shield:
                return shieldTimerUI;
            case PowerUpType.ScoreBooster:
                return scoreBoostTimerUI;
            default:
                return null;
        }
    }

    private void ResetAll()
    {
        StopAllCoroutines();
        shieldTimerUI.parent.SetActive(false);
        scoreBoostTimerUI.parent.SetActive(false);
    }

}
