using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiSliderLocker : Locker
{
    [SerializeField] private List<SliderLocker> Lockers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsUnlock && Lockers.All(x => x.IsUnlock))
        {
            IsUnlock = true;
            UnlockAudioSource.Play();
        }
        else if (IsUnlock && Lockers.Any(x => !x.IsUnlock))
        {
            IsUnlock = false;
        }
    }
}
