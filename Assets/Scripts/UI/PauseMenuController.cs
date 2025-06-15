using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] PauseCanvasControll CanvasControll;
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text LevelText;
    [SerializeField] TMP_Text MissionDescription;
    [SerializeField] TMP_Text UnbuyedSpells;

    private void OnEnable()
    {
        var player = GameManager.Instance.PlayerController;
        if(player != null )
            ScoreText.text = $"XP: {player.Score.ToString()}";

        LevelText.text = $"Level: {GameManager.LevelCounter.ToString()}";
        MissionDescription.text = GameManager.Instance.ActualMissionDesription;
        UnbuyedSpells.text = $"Locked spells: {GameManager.Instance.GetAvalibleWeapons().Count.ToString()}";
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
        GameManager.LevelCounter = 0;
        GameManager.Instance.Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
