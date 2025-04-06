using System.IO;
using TMPro;
using UnityEngine;

public class VoiceLoader : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSource;

    [SerializeField] private AudioClip AudioFR;
    [SerializeField] private AudioClip AudioEN;

    private void Start()
    {
        SettingsManager.Instance.onLanguageSwitch += OnLanguageSwitch;
    }

    private void OnEnable()
    {
        OnLanguageSwitch();
    }

    private void OnLanguageSwitch()
    {
        AudioSource.clip = SettingsManager.Instance.IsGameInFrench() ? AudioFR : AudioEN;
    }
}