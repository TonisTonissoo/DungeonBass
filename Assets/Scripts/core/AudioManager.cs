using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        float volume = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume") : 0.25f;
        AudioListener.volume = volume;
    }
}
