using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    public static UISoundPlayer Instance { get; private set; }

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;


    [Header("UI & Gameplay Sounds")]
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip openPanel;
    [SerializeField] private AudioClip closePanel;

    [SerializeField] private AudioClip diceRoll;
    [SerializeField] private AudioClip move;

    [Header("Shop Sounds")]
    [SerializeField] private AudioClip shopBuy;
    [SerializeField] private AudioClip noMoney;

    [Header("Fight")]
    [SerializeField] private AudioClip fightStart;

    [Header("Fight Results")]
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;

    [Header("Potion")]
    [SerializeField] private AudioClip healingPotion;


    private AudioSource source;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
    }

    public void PlayClick() => Play(click);
    public void PlayDiceRoll() => Play(diceRoll);
    public void PlayMove() => Play(move);

    public void PlayOpen() => Play(openPanel);
    public void PlayClose() => Play(closePanel);

    public void PlayShopBuy() => Play(shopBuy);
    public void PlayNoMoney() => Play(noMoney);

    public void PlayFightStart() => Play(fightStart);

    public void PlayVictory() => Play(victory);
    public void PlayDefeat() => Play(defeat);

    public void PlayHealing() => Play(healingPotion);

    private void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;
        source.PlayOneShot(clip, sfxVolume);
    }

    private void Start()
    {
        // Load SFX volume
        if (PlayerPrefs.HasKey("sfxVolume"))
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }


}