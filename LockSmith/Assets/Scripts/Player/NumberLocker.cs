using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NumberLocker : Locker
{
    [SerializeField] private List<NumberSwitcher> Lockers;

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

            foreach (var locker in Lockers)
                locker.Solved = true;
        }
    }
}
