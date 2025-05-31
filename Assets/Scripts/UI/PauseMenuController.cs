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
        GameManager.Instance.Pause();
        CanvasControll.isPaused = true;
    }

    public void Resume()
    {
        GameManager.Instance.Resume();
        CanvasControll.Resume();
    }

    public void Reset()
    {
        GameManager.Instance.Reset();
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }
}
