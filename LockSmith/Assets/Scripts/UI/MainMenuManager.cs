using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject NewGameValidationPanel;
    [SerializeField] private GameObject SettingsPanel;

    private int NewGameSlotIndexCache = -1;

    void Start()
    {
        CheckObjectSetup();
        DisplayMainMenuPanel();
    }

    private void CheckObjectSetup()
    {
        if (MainMenuPanel is null)
            ExceptionManager.ThrowException(name, nameof(MainMenuManager), $"L'objet {nameof(MainMenuPanel)} est nul.");
        if (NewGameValidationPanel is null)
            ExceptionManager.ThrowException(name, nameof(MainMenuManager), $"L'objet {nameof(NewGameValidationPanel)} est nul.");
        if (SettingsPanel is null)
            ExceptionManager.ThrowException(name, nameof(MainMenuManager), $"L'objet {nameof(SettingsPanel)} est nul.");
    }

    public void DisplayMainMenuPanel()
    {
        NewGameValidationPanel.SetActive(false);
        SettingsPanel.SetActive(false);

        MainMenuPanel.SetActive(true);
    }

    public void DisplayNewGameValidationPanel(int slotIndex)
    {
        NewGameSlotIndexCache = slotIndex;

        MainMenuPanel.SetActive(false);
        SettingsPanel.SetActive(false);

        NewGameValidationPanel.SetActive(true);
    }

    public void DisplaySettingsPanel()
    {
        MainMenuPanel.SetActive(false);
        NewGameValidationPanel.SetActive(false);

        SettingsPanel.SetActive(true);
    }

    public void LaunchNewGame()
    {
        SceneManager.LoadScene("1");
    }

    public void ContinueGame()
    {
        LoadGame(-1);
    }

    public void LoadGame(int slotIndexToLoad)
    {
        var slotToLoad = GenerateSlotIndex(slotIndexToLoad);
        var gameSlotDatas = MyGameManager.Instance.LoadSpecificGameSlot(slotToLoad);

        if (gameSlotDatas != null)
            SceneManager.LoadScene(gameSlotDatas.LevelKey);
        else
            ExceptionManager.ThrowException(name, nameof(MainMenuManager), "Une erreur est survenue lors du chargement des données de partie.");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private GameSlot GenerateSlotIndex(int index)
    {
        switch (index)
        {
            case 1:
                return GameSlot.SLOT1;
            case 2:
                return GameSlot.SLOT2;
            case 3:
                return GameSlot.SLOT3;
            default:
                return GameSlot.LOADCURRENTSLOT;
        }
    }
}
