using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    private float musicVolume = 1f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // MASTER volume (affects everything)
        float master = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 0.25f;
        AudioListener.volume = master;

        // MUSIC source (your looping AudioSource on this same object)
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        // MUSIC volume (affects only music)
        musicVolume = PlayerPrefs.HasKey("musicVolume") ? PlayerPrefs.GetFloat("musicVolume") : 1f;
        ApplyMusicVolume();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.Save();
        ApplyMusicVolume();
    }

    public float GetMusicVolume() => musicVolume;

    private void ApplyMusicVolume()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
}
