using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] PauseCanvasControll CanvasControll;
    [SerializeField] TMP_Text ScoreText;

    private void OnEnable()
    {
        var player = GameManager.Instance.PlayerController;
        if(player != null )
            ScoreText.text = $"XP: {player.Score.ToString()}";
    }

    public void Pause()
    {
        Time.timeScale = 0;
        CanvasControll.isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        CanvasControll.Resume();
    }

    public void Reset()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
