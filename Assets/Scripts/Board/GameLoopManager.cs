using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;

    [SerializeField] private int maxLoops = 20;

    public int CurrentLoop { get; private set; } = 1;
    public int MaxLoops => maxLoops;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLoop(int loop)
    {
        CurrentLoop = Mathf.Max(1, loop);
    }

    public bool IsFinalLoop(int loop) => loop >= maxLoops;
    public bool IsFinalLoop() => CurrentLoop >= maxLoops;
}
