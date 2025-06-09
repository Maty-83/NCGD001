using Assets.Scripts;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIVolumeController : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        float value;
        audioMixer.GetFloat("MasterVolume", out value);
        masterSlider.value = Mathf.Pow(10, value / 20f);

        audioMixer.GetFloat("MusicVolume", out value);
        musicSlider.value = Mathf.Pow(10, value / 20f);

        audioMixer.GetFloat("SFXVolume", out value);
        sfxSlider.value = Mathf.Pow(10, value / 20f);
    }

    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
    }

    private void SetVolume(string parameterName, float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            GameManager.Instance.Resume();
        }
    }
}
