using TMPro;
using UnityEngine;

public class GameVersionDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Displayer;

    void OnEnable()
    {
        CheckObjectSetup();
        Displayer.text = $"Version {ConstantManager.CurrentProjectVersion}";
    }

    private void CheckObjectSetup()
    {
        if (Displayer is null)
            ExceptionManager.ThrowException(name, nameof(GameVersionDisplayer), $"L'objet '{Displayer}' est nul.");
    }
}
