using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private bool PlayGradualyOnAwake = false;

    [SerializeField][Min(0)] public float ValueModifier = 1;
    [SerializeField][Min(0)] public float TimeModifier = 0.1f;

    [SerializeField] public List<AudioSource> AudioSources = new List<AudioSource>();
    [SerializeField][Min(0)] public List<float> AudioSourceVolumes = new List<float>();

    private void Start()
    {
        CheckObjectSetup();

        if (PlayGradualyOnAwake)
            StartCoroutine(PlayAllGradualy());
    }

    private void CheckObjectSetup()
    {
        if (AudioSources.Count != AudioSourceVolumes.Count)
            ExceptionManager.ThrowException(name, nameof(AudioManager), "Le nombre de sources audio est différent du nombre de volumes audio.");
    }

    public IEnumerator PlayAllGradualy()
    {
        bool actionCompleted = false;

        while (!actionCompleted)
        {
            actionCompleted = true;

            for (int i = 0; i < AudioSources.Count; i++)
            {
                if (AudioSources[i].volume < AudioSourceVolumes[i])
                {
                    AudioSources[i].volume += ValueModifier;
                    actionCompleted = false;
                }
            }
            yield return new WaitForSeconds(TimeModifier);
        }
    }

    public IEnumerator StopAllGradualy()
    {
        bool actionCompleted = false;

        while (!actionCompleted)
        {
            actionCompleted = true;

            for (int i = 0; i < AudioSources.Count; i++)
            {
                if (AudioSources[i].volume > 0)
                {
                    AudioSources[i].volume -= ValueModifier;
                    actionCompleted = false;
                }
            }
            yield return new WaitForSeconds(TimeModifier);
        }
    }
}
