using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGamePanelController : MonoBehaviour
{
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private TMP_Text Text;
    [SerializeField] private Button LeftButton;
    [SerializeField] private TMP_Text LeftButtonText;

    public delegate void LeftButtonController();

    public void Invoke(string text, string leftButtonText, LeftButtonController controller)
    {
        if (UIPanel.activeSelf)
            return;

        Time.timeScale = 0;
        UIPanel.SetActive(true);
        Text.text = text;

        LeftButtonText.text = leftButtonText;
        LeftButton.onClick.RemoveAllListeners();
        LeftButton.onClick.AddListener(delegate {
            Time.timeScale = 1;
            controller();
        });
    }

    public void Quit()
    {
        GameManager.LevelCounter = 0;
        GameManager.Instance.Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
