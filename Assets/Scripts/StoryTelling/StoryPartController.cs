using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryPartController : MonoBehaviour
{
    public Image textPanelImage;
    public TextMeshProUGUI storyText;
    [TextArea] public string fullText;
    public AudioClip narratorClip;

    public float panelFadeDuration = 1f;
    public float charSpeed = 0.03f;

    private bool isFadingIn = false;
    private float fadeTimer = 0f;

    private bool isTyping = false;
    private float charTimer = 0f;
    private int currentCharIndex = 0;

    private bool typingFinished = false;

    public void StartPart()
    {
        SetPanelAlpha(0f);
        storyText.text = "";

        isFadingIn = true;
        fadeTimer = 0f;

        isTyping = false;
        typingFinished = false;
    }

    void Update()
    {
        if (isFadingIn)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / panelFadeDuration);
            SetPanelAlpha(t);

            if (t >= 1f)
            {
                isFadingIn = false;
                StartTyping();
            }
        }

        if (isTyping)
        {
            charTimer += Time.deltaTime;
            if (charTimer >= charSpeed)
            {
                charTimer = 0f;
                currentCharIndex++;
                if (currentCharIndex <= fullText.Length)
                {
                    storyText.text = fullText.Substring(0, currentCharIndex);
                }
                else
                {
                    isTyping = false;
                    typingFinished = true;
                }
            }
        }
    }

    private void StartTyping()
    {
        isTyping = true;
        charTimer = 0f;
        currentCharIndex = 0;
    }

    private void SetPanelAlpha(float alpha)
    {
        Color c = textPanelImage.color;
        c.a = alpha;
        textPanelImage.color = c;
    }

    public void SkipTyping()
    {
        if (isTyping)
        {
            storyText.text = fullText;
            isTyping = false;
            typingFinished = true;
        }
    }

    public bool IsTypingFinished()
    {
        return typingFinished;
    }
}
