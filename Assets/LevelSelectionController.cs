using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionController : MonoBehaviour
{
    [SerializeField] string PortLevel;
    [SerializeField] int LevelNumber;

    public void SelectLevel()
    {
        GameManager.LevelCounter = LevelNumber;
        GameManager.Instance.Resume();
        SceneManager.LoadScene(PortLevel);
    }
}
