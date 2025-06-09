using UnityEngine;

public class PauseCanvasControll : MonoBehaviour
{
    [SerializeField] private PauseMenuController controll;
    [SerializeField] private UIVolumeController magicPanel;

    public bool isPaused = false;
    private bool isShowingPanel = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (magicPanel.gameObject.activeSelf && !isShowingPanel)
                return;

            if (isShowingPanel)
            {
                ShowVolume();
                return;
            }

            if (isPaused)
            {
                controll.gameObject.SetActive(false);
                controll.Resume();
            }
            else
            {
                controll.gameObject.SetActive(true);
                controll.Pause();
            }
        }
    }

    public void Resume()
    {
       isPaused = false;
       controll.gameObject.SetActive(false);
    }

    public void ShowVolume()
    {
        isShowingPanel = !isShowingPanel;
        magicPanel.gameObject.SetActive(isShowingPanel);
    }
}
