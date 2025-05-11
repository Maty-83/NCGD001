using UnityEngine;

public class PauseCanvasControll : MonoBehaviour
{
    [SerializeField] private PauseMenuController controll;
    [SerializeField] private MagicPanelController magicPanel;

    public bool isPaused = false;
    private bool isShowingPanel = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShowingPanel)
            {
                ShowMagicTree();
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

    public void ShowMagicTree()
    {
        isShowingPanel = !isShowingPanel;
        magicPanel.gameObject.SetActive(isShowingPanel);
        if (isShowingPanel)
            magicPanel.Display();
    }
}
