using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;

    private float lastSoundTime;
    private const float soundCooldown = 0.05f;

    private void Start()
    {
        float savedVolume = PlayerPrefs.HasKey("masterVolume")
            ? PlayerPrefs.GetFloat("masterVolume")
            : 0.25f;

        volumeSlider.value = savedVolume;
        UpdateVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        UpdateVolume(value);

        if (Time.unscaledTime - lastSoundTime > soundCooldown)
        {
            UISoundPlayer.Instance.PlayClick();
            lastSoundTime = Time.unscaledTime;
        }
    }

    private void UpdateVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
        PlayerPrefs.Save();
        volumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
