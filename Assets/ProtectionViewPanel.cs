using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProtectionViewPanel : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private BarController barController;
    [SerializeField] private TMP_Text timeText;

    private float duration;
    private float timeLeft;
    private bool isActive = false;

    public void Init(Sprite spellImage, float spellDuration, string spellDescription)
    {
        previewImage.sprite = spellImage;

        duration = spellDuration;
        timeLeft = duration;
        isActive = true;

        barController.SetValues(timeLeft, true, 0f, duration);
        UpdateTimeText();

        gameObject.SetActive(true);
    }
    public void ExtendDuration(float additionalDuration)
    {
        duration += additionalDuration;
        timeLeft += additionalDuration;
        barController.SetValues(timeLeft, true, 0f, duration);
        UpdateTimeText();
    }

    private void Update()
    {
        if (!isActive) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isActive = false;
            gameObject.SetActive(false);
        }

        barController.SetValues(timeLeft, true, 0f, duration);
        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            timeText.text = Mathf.CeilToInt(timeLeft).ToString() + "s";
        }
    }

}