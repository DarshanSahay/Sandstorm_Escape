using System;
using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{
    public Sound[] sfxSounds;
    public Sound[] bgmSounds;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private readonly string BGM_Volume = "bgmvolume";
    private readonly string SFX_Volume = "sfxvolume";

    private new void Awake()
    {
        base.Awake();

        if(PlayerPrefs.HasKey(BGM_Volume))
        {
            UpdateBGMVolume(PlayerPrefs.GetFloat(BGM_Volume));
        }
        if (PlayerPrefs.HasKey(SFX_Volume))
        {
            UpdateSFXVolume(PlayerPrefs.GetFloat(SFX_Volume));
        }
    }

    private void Start()
    {
        EventManager.Instance.OnBGMAudioLevelChanged += UpdateBGMVolume;
        EventManager.Instance.OnBGMAudioLevelChanged += UpdateSFXVolume;

        EventManager.Instance.OnGameStart += PlayInGameBGM;
        EventManager.Instance.OnGameRestart += PlayInGameBGM;
        EventManager.Instance.OnCoinCollected += (int value) => PlayCoinCollectSFX();
        EventManager.Instance.OnShieldCollected += (float value) => PlayShieldSFX();
        EventManager.Instance.OnScoreBoostCollected += (float value) => PlayScoreBoostSFX();
        EventManager.Instance.OnPlayerJumped += PlayJumpSFX;
        EventManager.Instance.OnPlayerDoubleJumped += PlayDoubleJumpSFX;
        EventManager.Instance.OnPlayerHitObstacle += PlayHitObstacleSFX;
        PlayBGM("BGM1");
    }

    public void PlayBGM(string name)
    {
        Sound s = Array.Find(bgmSounds, x => x.name == name);

        if(s == null)
        {
            Debug.Log("Audio BGM Not Found");
        }
        else
        {
            bgmSource.clip = s.clip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Audio SFX Not Found");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(sfxSource.clip);
        }
    }

    public void PlayCoinCollectSFX()
    {
        PlaySFX("CoinPicked");
    }

    public void PlayHitObstacleSFX()
    {
        PlaySFX("ObstacleHit");
    }

    public void PlayJumpSFX()
    {
        PlaySFX("Jump");
    }

    public void PlayDoubleJumpSFX()
    {
        PlaySFX("DoubleJump");
    }

    public void PlayScoreBoostSFX()
    {
        PlaySFX("ScoreBoostPicked");
    }

    public void PlayShieldSFX()
    {
        PlaySFX("ShieldPicked");
    }

    private void PlayInGameBGM()
    {
        PlayBGM("BGM2");
    }

    public void UpdateBGMVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat(BGM_Volume, bgmSource.volume);
    }

    public void UpdateSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(SFX_Volume, sfxSource.volume);
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public SoundType type;
    public AudioClip clip;
}

public enum SoundType
{
    BGM,
    SFX
}

