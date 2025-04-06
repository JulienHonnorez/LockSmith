using UnityEngine;

public class Locker : MonoBehaviour
{
    [SerializeField] public bool IsUnlock = false;
    [SerializeField] public AudioSource UnlockAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsUnlock = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
