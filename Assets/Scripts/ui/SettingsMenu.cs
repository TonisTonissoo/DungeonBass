using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;

    private void Start()
    {
        float savedVolume = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 0.25f;
        volumeSlider.value = savedVolume;
        UpdateVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    private void UpdateVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
        PlayerPrefs.Save();
        volumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
