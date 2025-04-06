using UnityEngine;

public class SliderLocker : Locker
{
    [SerializeField] private GameObject Slider;
    [SerializeField] private GameObject Goal;

    public float proximityThreshold = 5f;

    void Update()
    {
        float distance = Vector3.Distance(Slider.transform.position, Goal.transform.position);

        if (!IsUnlock && distance <= proximityThreshold)
        {
            IsUnlock = true;

            if (UnlockAudioSource is not null)
                UnlockAudioSource.Play();
        }
        else if (IsUnlock && distance > proximityThreshold)
        {
            IsUnlock = false;
        }
    }
}
