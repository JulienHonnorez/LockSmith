using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsLoader : MonoBehaviour
{
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider EffectSlider;

    [SerializeField] private TMPro.TMP_Dropdown QualityLevel;
    [SerializeField] private TMPro.TMP_Dropdown ResolutionDropdown;
    [SerializeField] private Toggle WindowedToggle;
    [SerializeField] private Toggle EnglishToggle;
    [SerializeField] private Toggle FrenchToggle;

    private float CacheMusicVolume = -1f;
    private float CacheEffectVolume = -1f;
    private int CacheQualityIndex = -1;
    private int CacheResolutionIndex = -1;
    private bool CacheWindowed = false;
    private bool CacheGameIsInFrench = false;

    private List<Resolution> Resolutions = new List<Resolution>();

    // Start is called before the first frame update
    private void Start()
    {
        CheckObjectSetup();
        SettupResolutionDropdown();

        MusicSlider.value = SettingsManager.Instance.GetMusicVolume();
        CacheMusicVolume = MusicSlider.value;
        EffectSlider.value = SettingsManager.Instance.GetEffectVolume();
        CacheEffectVolume = EffectSlider.value;
        QualityLevel.value = SettingsManager.Instance.GetQualityIndex();
        CacheQualityIndex = QualityLevel.value;
        WindowedToggle.isOn = SettingsManager.Instance.GetIsFullScreenValue();
        CacheWindowed = WindowedToggle.isOn;

        CacheGameIsInFrench = SettingsManager.Instance.IsGameInFrench();
        FrenchToggle.isOn = CacheGameIsInFrench;
        EnglishToggle.isOn = !CacheGameIsInFrench;
    }

    private void Update()
    {
        CheckIfMusicVolumeChange();
        CheckIfEffectVolumeChange();
        CheckIfFullScreenStatusChange();
        CheckIfQualityLevelChange();
        CheckIfResolutionChange();
        CheckIfLanguageChange();
    }

    private void CheckObjectSetup()
    {
        if (MusicSlider is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(MusicSlider)}' est nul.");
        if (EffectSlider is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(EffectSlider)}' est nul.");
        if (QualityLevel is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(QualityLevel)}' est nul.");
        if (ResolutionDropdown is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(ResolutionDropdown)}' est nul.");
        if (WindowedToggle is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(WindowedToggle)}' est nul.");
        if (EnglishToggle is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(EnglishToggle)}' est nul.");
        if (FrenchToggle is null)
            ExceptionManager.ThrowException(name, nameof(SettingsLoader), $"L'objet '{nameof(FrenchToggle)}' est nul.");
    }

    private void CheckIfLanguageChange()
    {
        if (CacheGameIsInFrench != FrenchToggle.isOn)
        {
            CacheGameIsInFrench = FrenchToggle.isOn;

            SettingsManager.Instance.SetGameLanguage(FrenchToggle.isOn);
            SettingsManager.Instance.Save();

            FrenchToggle.isOn = CacheGameIsInFrench;
            EnglishToggle.isOn = !CacheGameIsInFrench;
        }
    }

    private void CheckIfMusicVolumeChange()
    {
        if (MusicSlider.value != CacheMusicVolume)
        {
            CacheMusicVolume = MusicSlider.value;
            SettingsManager.Instance.SetMusicVolume(MusicSlider.value);

            SettingsManager.Instance.Save();
        }
    }

    private void CheckIfEffectVolumeChange()
    {
        if (EffectSlider.value != CacheEffectVolume)
        {
            CacheEffectVolume = EffectSlider.value;
            SettingsManager.Instance.SetEffectVolume(EffectSlider.value);

            SettingsManager.Instance.Save();
        }
    }

    private void CheckIfFullScreenStatusChange()
    {
        if (WindowedToggle.isOn != CacheWindowed)
        {
            CacheWindowed = WindowedToggle.isOn;
            SettingsManager.Instance.SetFullScreen(!WindowedToggle.isOn);

            SettingsManager.Instance.Save();
        }
    }

    private void CheckIfQualityLevelChange()
    {
        if (QualityLevel.value != CacheQualityIndex)
        {
            CacheQualityIndex = QualityLevel.value;
            SettingsManager.Instance.SetQuality(QualityLevel.value);

            SettingsManager.Instance.Save();
        }
    }

    private void CheckIfResolutionChange()
    {
        if (ResolutionDropdown.value != CacheResolutionIndex)
        {
            CacheResolutionIndex = ResolutionDropdown.value;
            SetResolution(ResolutionDropdown.value);

            SettingsManager.Instance.Save();
        }
    }

    private void SettupResolutionDropdown()
    {
        var tmpResolutions = Screen.resolutions;

        ResolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        Resolution currentResolution = SettingsManager.Instance.GetResolution();

        // Get only X first highest resolutions
        for (int i = tmpResolutions.Length - 1; i > 0; i--)
        {
            string option = $"{tmpResolutions[i].width} x {tmpResolutions[i].height}";

            if (!options.Contains(option))
            {
                options.Add(option);
                Resolutions.Add(tmpResolutions[i]);
            }

            if (options.Count == ConstantManager.MaxNumberOfResolutionDisplayed)
                break;
        }

        for (int i = 0; i < Resolutions.Count; i++)
        {
            if (Resolutions[i].width == currentResolution.width && Resolutions[i].height == currentResolution.height)
            {
                CacheResolutionIndex = i;
                break;
            }

            CacheResolutionIndex = Resolutions.Count - 1;
            SetResolution(CacheResolutionIndex);
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = CacheResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        SettingsManager.Instance.SetResolution(resolution);
    }
}
