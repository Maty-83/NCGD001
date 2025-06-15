using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject LevelSelection;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && LevelSelection.activeSelf)
        {
            LevelSelection.SetActive(false);
        }
    }

    public void OnNewGameClick()
    {
        GameManager.Instance.Resume();
        SceneManager.LoadScene("StoryTelling");
    }

    public void OnQuitClick()
    {
        GameManager.Instance.Quit();
    }

    public void ShowLevelSelection()
    {
        if (!LevelSelection.activeSelf)
        {
            LevelSelection.SetActive(true);
        }
    }
}
