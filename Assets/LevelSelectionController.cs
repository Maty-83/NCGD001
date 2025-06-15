using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionController : MonoBehaviour
{
    [SerializeField] string PortLevel;

    public void SelectLevel()
    {
        GameManager.Instance.Resume();
        SceneManager.LoadScene(PortLevel);
    }
}
