using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer AudioMixer;

    private SettingDatas Settings = new SettingDatas();

    // Singleton
    private static SettingsManager instance;
    public static SettingsManager Instance { get { return instance; } }

    // Language event
    public event Action onLanguageSwitch;
    public void LanguageSwitch()
    {
        if (onLanguageSwitch != null)
            onLanguageSwitch();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;

            Load();
            ApplySettings();
        }
    }

    private void Start()
    {
        SetMusicVolume(Settings.MusicVolume);
        SetEffectVolume(Settings.EffectVolume);
    }

    #region System
    public bool IsGameInFrench()
    {
        return Settings.GameIsInFrench;
    }

    public void SetGameLanguage(bool isInFrench)
    {
        Settings.GameIsInFrench = isInFrench;

        LanguageSwitch();
    }
    #endregion

    #region Audio
    public float GetMusicVolume()
    {
        return Settings.MusicVolume;
    }

    public float GetEffectVolume()
    {
        return Settings.EffectVolume;
    }

    public void SetMusicVolume(float volume)
    {
        Settings.MusicVolume = volume;
        AudioMixer.SetFloat(ConstantManager.AudioMixer_MusicVolumeKey, Mathf.Log10(volume) * 20);
    }

    public void SetEffectVolume(float volume)
    {
        Settings.EffectVolume = volume;
        AudioMixer.SetFloat(ConstantManager.AudioMixer_EffectVolumeKey, Mathf.Log10(volume) * 20);
    }
    #endregion

    #region Graphics
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Settings.QualityLevel = qualityIndex;
    }

    public int GetQualityIndex()
    {
        return Settings.QualityLevel;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        Settings.IsFullScreen = isFullScreen;
    }

    public bool GetIsFullScreenValue()
    {
        return Settings.IsFullScreen;
    }

    public void SetResolution(Resolution resolution)
    {
        Settings.ResolutionWith = resolution.width;
        Settings.ResolutionHeight = resolution.height;

        Save();
    }

    public Resolution GetResolution()
    {
        Resolution resolution = new Resolution();
        resolution.width = Settings.ResolutionWith;
        resolution.height = Settings.ResolutionHeight;

        return resolution;
    }
    #endregion

    public void Save()
    {
        string path = $"{Application.persistentDataPath}/{ConstantManager.SettingsFileName}";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, Settings);
        stream.Close();
    }

    private void Load()
    {
        string path = $"{Application.persistentDataPath}/{ConstantManager.SettingsFileName}";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings = formatter.Deserialize(stream) as SettingDatas;
            stream.Close();
        }
        else
        {
            ExceptionManager.ThrowException(name, nameof(SettingsManager), $"Le fichier d'option n'existe pas : {path}.");
        }
    }

    private void ApplySettings()
    {
        AudioMixer.SetFloat(ConstantManager.AudioMixer_MusicVolumeKey, Mathf.Log10(Settings.MusicVolume) * 20);
        AudioMixer.SetFloat(ConstantManager.AudioMixer_EffectVolumeKey, Mathf.Log10(Settings.EffectVolume) * 20);

        QualitySettings.SetQualityLevel(Settings.QualityLevel);
        Screen.fullScreen = !Settings.IsFullScreen;

        if (Settings.ResolutionWith == 0)
        {
            Settings.ResolutionWith = Screen.currentResolution.width;
            Settings.ResolutionHeight = Screen.currentResolution.height;
        }

        try
        {
            Screen.SetResolution(Settings.ResolutionWith, Settings.ResolutionHeight, Screen.fullScreen);
        }
        catch (Exception)
        {
            ExceptionManager.ThrowException(name, nameof(SettingsManager), "Unsupported resolution. Monitor base resolution applied.");

            Settings.ResolutionWith = Screen.currentResolution.width;
            Settings.ResolutionHeight = Screen.currentResolution.height;

            Screen.SetResolution(Settings.ResolutionWith, Settings.ResolutionHeight, Screen.fullScreen);
        }
    }
}

[Serializable]
public class SettingDatas
{
    // System
    public bool GameIsInFrench = false;

    // Audio
    public float MusicVolume = 1f;
    public float EffectVolume = 1f;

    // Graphics
    public int QualityLevel = 1;
    public bool IsFullScreen = true;
    public int ResolutionWith = 0;
    public int ResolutionHeight = 0;
}
