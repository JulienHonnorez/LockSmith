using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private List<Locker> Lockers;
    [SerializeField] private bool IsOpened = false;
    [SerializeField] private Animator CameraAnimator;
    [SerializeField] private Animator DoorAnimator;

    void Start()
    {
        IsOpened = false;
    }

    void Update()
    {
        if (!IsOpened && Lockers.All(x => x.IsUnlock))
        {
            IsOpened = true;
            DoorAnimator.SetTrigger("Open");
            CameraAnimator.SetTrigger("FadeIn");
        }
    }
}
