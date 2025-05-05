using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    [SerializeField] float MinValue = 0f;
    [SerializeField] float MaxValue = 1f;
    [SerializeField] float Value = 0.5f;
    [SerializeField] bool IsVisible = true;
    [SerializeField] Slider Slider;
    [SerializeField] GameObject GameObject;

    void Start()
    {
        SetValues(Value, IsVisible, MinValue, MaxValue);
    }

    public void SetValues(float value, bool IsVisible = true)
    {
        if (value < MinValue || value > MaxValue || Slider == null)
        {
            return;
        }

        Value = value;
        this.IsVisible = IsVisible;
        SetSlider(Value, IsVisible, MinValue, MaxValue);
    }

    public void SetValues(float value, bool IsVisible = true, float minValue = 0f, float maxValue = 1f)
    {
        if (value < minValue || value > maxValue || minValue >= maxValue || Slider == null)
        {
            return;
        }

        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
        this.IsVisible = IsVisible;
        SetSlider(Value, IsVisible, minValue, maxValue); 
    }

    private void SetSlider(float value, bool IsVisible, float minValue, float maxValue)
    {
        Slider.minValue = minValue;
        Slider.maxValue = maxValue;
        Slider.value = value;
        GameObject.SetActive(IsVisible);
    }
}
