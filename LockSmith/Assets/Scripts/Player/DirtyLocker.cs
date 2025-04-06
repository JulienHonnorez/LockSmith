using UnityEngine;

public class DirtyLocker : Locker
{
    [SerializeField] private GameObject DirtyContainer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsUnlock && DirtyContainer.transform.childCount == 0)
        {
            IsUnlock = true;
            UnlockAudioSource.Play();
        }
    }
}
