using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    public static UISoundPlayer Instance { get; private set; }

    [Header("UI & Gameplay Sounds")]
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip openPanel;
    [SerializeField] private AudioClip closePanel;

    [SerializeField] private AudioClip diceRoll;
    [SerializeField] private AudioClip move;


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

    private void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;
        source.PlayOneShot(clip);
    }
}