using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Master")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("Music")]
    public Slider musicSlider;
    public TMP_Text musicText;

    [Header("SFX")]
    public Slider sfxSlider;
    public TMP_Text sfxText;

    private float lastSoundTime;
    private const float soundCooldown = 0.05f;

    private bool isShuttingDown = false;

    private void OnEnable()
    {
        isShuttingDown = false;

        // hook listeners
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        // load + apply
        float savedMaster = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 0.25f;
        float savedMusic = PlayerPrefs.HasKey("musicVolume") ? PlayerPrefs.GetFloat("musicVolume") : 1f;
        float savedSfx = PlayerPrefs.HasKey("sfxVolume") ? PlayerPrefs.GetFloat("sfxVolume") : 1f;

        if (volumeSlider) volumeSlider.SetValueWithoutNotify(savedMaster);
        if (musicSlider) musicSlider.SetValueWithoutNotify(savedMusic);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(savedSfx);

        ApplyMaster(savedMaster);
        ApplyMusic(savedMusic);
        ApplySFX(savedSfx);
    }

    private void OnDisable()
    {
        // UI is going away (scene change / panel destroy) -> don't tick anymore
        isShuttingDown = true;

        if (volumeSlider) volumeSlider.onValueChanged.RemoveListener(OnMasterChanged);
        if (musicSlider) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    private void OnMasterChanged(float value)
    {
        ApplyMaster(value);
        Tick();
    }

    private void OnMusicChanged(float value)
    {
        ApplyMusic(value);
        Tick();
    }

    private void OnSfxChanged(float value)
    {
        ApplySFX(value);
        Tick();
    }

    private void ApplyMaster(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
        PlayerPrefs.Save();
        if (volumeText) volumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void ApplyMusic(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        if (musicText) musicText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void ApplySFX(float value)
    {
        UISoundPlayer.Instance?.SetSFXVolume(value);
        if (sfxText) sfxText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void Tick()
    {
        if (isShuttingDown) return;              // key fix
        if (!isActiveAndEnabled) return;

        if (Time.unscaledTime - lastSoundTime > soundCooldown)
        {
            UISoundPlayer.Instance?.PlayClick();
            lastSoundTime = Time.unscaledTime;
        }
    }
}
