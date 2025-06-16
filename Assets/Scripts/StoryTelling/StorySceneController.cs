using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StorySceneController : MonoBehaviour
{
    public StoryPartController[] storyParts;
    public Image maskImage;
    public AudioSource narratorAudio;

    public GameObject midPanel;

    public float fadeDuration = 1f;
    public string LoadedScene = "TutorialScene";

    private int currentPartIndex = 0;
    private bool isTransitioning = false;
    private bool isFading = false;
    private bool fadingToBlack = false;
    private float fadeTimer = 0f;

    private bool autoNextTriggered = false;
    private bool panelShown = false;

    void Start()
    {
        foreach (var part in storyParts)
        {
            part.gameObject.SetActive(false);
        }

        if (midPanel != null)
            midPanel.SetActive(false);

        SetMaskAlpha(1f);

        StartCurrentPart();

        StartFade(false);
    }

    void Update()
    {
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            float alpha = fadingToBlack ? t : ( 1f - t );
            SetMaskAlpha(alpha);

            if (t >= 1f)
            {
                isFading = false;

                if (fadingToBlack)
                {
                    OnFadeToBlackComplete();
                }
            }
        }

        if (!isTransitioning && !isFading && narratorAudio.isPlaying == false && !autoNextTriggered)
        {
            if (storyParts[currentPartIndex].IsTypingFinished())
            {
                autoNextTriggered = true;
                StartFade(true);
                isTransitioning = true;
            }
        }

        if (!panelShown && narratorAudio.isPlaying && narratorAudio.clip != null)
        {
            float progress = narratorAudio.time / narratorAudio.clip.length;
            if (progress >= 0.5f)
            {
                ShowMidPanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSkipPressed();
        }
    }

    void StartCurrentPart()
    {
        if (currentPartIndex >= storyParts.Length)
        {
            return;
        }

        var part = storyParts[currentPartIndex];
        part.gameObject.SetActive(true);
        part.StartPart();

        narratorAudio.clip = part.narratorClip;
        if (narratorAudio.clip != null)
        {
            narratorAudio.Play();
        }

        autoNextTriggered = false;
        panelShown = false;

        if (midPanel != null)
            midPanel.SetActive(false);
    }

    void OnSkipPressed()
    {
        if (narratorAudio.isPlaying)
        {
            narratorAudio.Stop();
            storyParts[currentPartIndex].SkipTyping();
            return;
        }

        if (!isTransitioning && !isFading)
        {
            StartFade(true);
            isTransitioning = true;
            autoNextTriggered = true;
        }
    }

    private void OnFadeToBlackComplete()
    {
        storyParts[currentPartIndex].gameObject.SetActive(false);
        currentPartIndex++;

        if (currentPartIndex < storyParts.Length)
        {
            StartCurrentPart();
            StartFade(false);
            isTransitioning = false;
        }
        else
        {
            SceneManager.LoadScene(LoadedScene);
        }
    }

    private void StartFade(bool toBlack)
    {
        isFading = true;
        fadingToBlack = toBlack;
        fadeTimer = 0f;
    }

    private void SetMaskAlpha(float alpha)
    {
        Color c = maskImage.color;
        c.a = alpha;
        maskImage.color = c;
    }

    private void ShowMidPanel()
    {
        if (midPanel != null)
        {
            midPanel.SetActive(true);
        }

        panelShown = true;
    }
}
