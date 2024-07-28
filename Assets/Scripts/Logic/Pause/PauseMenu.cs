using Feature.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private bool freezeTime;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button unpauseBtn;
    [SerializeField] private Button returnMenuBtn;

    private void Start()
    {
        pauseBtn.onClick.AddListener(Pause);
        unpauseBtn.onClick.AddListener(Unpause);
        returnMenuBtn.onClick.AddListener(ReturnMenu);

        Unpause();
    }

    public void Unpause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);

        if (freezeTime)
            Time.timeScale = 0;
    }

    private void ReturnMenu()
    {
        if (freezeTime)
            Time.timeScale = 1;

        SceneController.Instance.LoadScene("Menu", "MainMenu");
    }
}