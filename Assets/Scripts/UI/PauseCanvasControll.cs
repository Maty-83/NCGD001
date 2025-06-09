using UnityEngine;

public class PauseCanvasControll : MonoBehaviour
{
    [SerializeField] private PauseMenuController controll;
    [SerializeField] private MagicPanelController magicPanel;
    [SerializeField] private UIVolumeController VolumePanel;

    public bool isPaused = false;
    private bool isShowingPanel = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (VolumePanel.gameObject.activeSelf && !isShowingPanel || magicPanel.gameObject.activeSelf)
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
        VolumePanel.gameObject.SetActive(isShowingPanel);
    }
}
