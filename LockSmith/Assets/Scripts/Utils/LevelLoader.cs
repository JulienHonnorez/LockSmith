using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string LevelKey;
    [SerializeField] private bool IsTimerLevel;
    [SerializeField] private float Timer;
    public TextMeshProUGUI TimerText;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(LevelKey);
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void FixedUpdate()
    {
        if (IsTimerLevel)
        {
            if (Timer > 0)
                UpdateTimerDisplay();
            else
                LoadNextLevel();
        }
    }

    void UpdateTimerDisplay()
    {
        Timer -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(Timer / 60f);
        int seconds = Mathf.FloorToInt(Timer % 60f);

        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
