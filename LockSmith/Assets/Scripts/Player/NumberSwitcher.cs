using TMPro;
using UnityEngine;

public class NumberSwitcher : Locker
{
    [SerializeField] private TextMeshPro NumberDisplayer;
    [SerializeField] private int MinValue;
    [SerializeField] private int MaxValue;
    [SerializeField] private GameObject Unlocker;
    [SerializeField] private Animator TigeAnimator;
    [SerializeField] private bool InvertAnimation;
    [SerializeField] private AudioSource AudioSource;

    private int CurrentValue;
    private int GoalValue;
    private bool PreviousState;
    private bool FirstTigeSet = true;

    public bool Solved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Unlocker.SetActive(false);
        CurrentValue = MinValue - 1;
        NumberDisplayer.text = CurrentValue.ToString();

        GoalValue = Random.Range(MinValue, MaxValue);
        AddValue(1, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddValue(int value, bool playSound = true)
    {
        if (Solved)
            return;

        if (playSound)
            AudioSource.Play();

        CurrentValue += value;

        if (CurrentValue < MinValue)
            CurrentValue = MaxValue;
        if (CurrentValue > MaxValue)
            CurrentValue = MinValue;

        NumberDisplayer.text = CurrentValue.ToString();
        IsUnlock = CurrentValue == GoalValue;
        Unlocker.SetActive(IsUnlock);

        if (PreviousState != IsUnlock || FirstTigeSet)
        {
            FirstTigeSet = false;

            if (InvertAnimation)
                TigeAnimator.SetTrigger(!IsUnlock ? "Unlock" : "Lock");
            else
                TigeAnimator.SetTrigger(IsUnlock ? "Unlock" : "Lock");
            PreviousState = IsUnlock;
        }
    }
}
