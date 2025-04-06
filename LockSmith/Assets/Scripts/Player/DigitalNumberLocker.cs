using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DigitalNumberLocker : Locker
{
    [SerializeField] private List<TextMeshPro> NumberDisplayers;
    [SerializeField] private List<GameObject> NumberFocuses;
    [SerializeField] private List<Light2D> Lights;
    [SerializeField] private List<int> CurrentValues;
    [SerializeField] private List<int> GoalValues;
    [SerializeField] private TextMeshPro NumberOfTryLeftDisplayer;

    [SerializeField] private Color CorrectColor;
    [SerializeField] private Color WrongPlaceColor;
    [SerializeField] private Color WrongColor;

    [SerializeField] private int MinValue;
    [SerializeField] private int MaxValue;
    [SerializeField] private int NumberCount;
    [SerializeField] private int NumberOfTry;

    private int currentIndex;
    private int NumberOfTryLeft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Setup();

        currentIndex = 0;
        NumberFocuses[currentIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Setup()
    {
        CurrentValues = new List<int>();
        GoalValues = new List<int>();
        NumberOfTryLeft = NumberOfTry;

        for (int i = 0; i < NumberCount; i++)
        {
            CurrentValues.Add(1);
            GoalValues.Add(Random.Range(MinValue, MaxValue));
            Lights[i].color = Color.white;
        }

        NumberDisplayers[currentIndex].text = CurrentValues[currentIndex].ToString();
        NumberOfTryLeftDisplayer.text = NumberOfTryLeft.ToString();
    }

    public void Decode()
    {
        if (!Lights.First().gameObject.activeSelf)
            foreach (var light in Lights)
                light.gameObject.SetActive(true);

        for (int i = 0; i < CurrentValues.Count; i++)
        {
            if (CurrentValues[i] == GoalValues[i])
                Lights[i].color = CorrectColor;
            else if (GoalValues.Contains(CurrentValues[i]))
            {
                var goalCount = GoalValues.Count(x => x == CurrentValues[i]);
                var valuesCount = CurrentValues.Count(x => x == CurrentValues[i]);

                if (valuesCount <= goalCount)
                    Lights[i].color = WrongPlaceColor;
                else
                {
                    var goodSimilarValueCount = 0;
                    for (int x = 0; x < CurrentValues.Count; x++)
                        if (CurrentValues[x] == CurrentValues[i] && (CurrentValues[x] == GoalValues[x] || x < i))
                            goodSimilarValueCount++;

                    if (goodSimilarValueCount < goalCount)
                        Lights[i].color = WrongPlaceColor;
                    else
                        Lights[i].color = WrongColor;
                }
            }
            else
                Lights[i].color = WrongColor;
        }

        NumberOfTryLeft--;

        if (NumberOfTryLeft <= 0 && Lights.All(x => x.color == CorrectColor))
        {
            IsUnlock = true;
            UnlockAudioSource.Play();
        }
        else if (NumberOfTryLeft <= 0)
            Setup();
        else
            NumberOfTryLeftDisplayer.text = NumberOfTryLeft.ToString();

    }

    public void OnValidate()
    {
        IsUnlock = Lights.All(x => x.color == CorrectColor);
    }

    public void SwitchIndex(int indexIncrement)
    {
        NumberFocuses[currentIndex].SetActive(false);
        currentIndex += indexIncrement;

        if (currentIndex < 0)
            currentIndex = NumberCount - 1;
        if (currentIndex >= NumberCount)
            currentIndex = 0;

        NumberFocuses[currentIndex].SetActive(true);
    }

    public void SetCurrentValue(int value)
    {
        if (IsUnlock)
            return;

        CurrentValues[currentIndex] = value;

        if (CurrentValues[currentIndex] < MinValue)
            CurrentValues[currentIndex] = MaxValue;
        if (CurrentValues[currentIndex] > MaxValue)
            CurrentValues[currentIndex] = MinValue;

        NumberDisplayers[currentIndex].text = CurrentValues[currentIndex].ToString();
    }
}
