using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnNewGameClick()
    {
        SceneManager.LoadScene("StoryTelling");
    }

    public void OnQuitClick()
    {
        GameManager.Instance.Quit();
    }
}
