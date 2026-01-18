using UnityEngine;

public class TurnController : MonoBehaviour
{
    public static TurnController Instance { get; private set; }

    [System.Serializable]
    public struct DicePair
    {
        public int a, b;
        public int Sum => a + b;
        public DicePair(int a, int b) { this.a = a; this.b = b; }
    }

    public KeyCode toggleKey = KeyCode.Space;
    public WaypointFollower follower;
    public DicePanelManager panel;

    // PERSIST across scene reloads:
    private static DicePair[] options = new DicePair[3];
    private static bool[] used = new bool[3];
    private static bool hasActiveOptions = false;
    private static bool isRollingVisual = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (panel != null)
            panel.Hide();

        StartCoroutine(InitHint());
    }

    private System.Collections.IEnumerator InitHint()
    {
        yield return null;
        SpaceHintUI.Restore();
        UpdateClosedHint();
    }

    void Update()
    {
        if (PauseManager.IsPaused)
            return;

        if (follower == null)
        {
            follower = FindObjectOfType<WaypointFollower>();
            if (follower == null)
                return;
        }

        if (panel == null)
            panel = FindObjectOfType<DicePanelManager>();

        if (HorseCarriageUI.Instance != null && HorseCarriageUI.Instance.IsChoosingTile)
            return;

        if (!Input.GetKeyDown(toggleKey))
            return;

        HandleToggleKey();
    }

    void HandleToggleKey()
    {
        if (follower != null && follower.IsMoving) return;
        if (isRollingVisual) return;

        // PANEL IS CURRENTLY HIDDEN
        if (!panel.IsVisible)
        {
            // need new roll? -> play GIF then roll
            if (!hasActiveOptions || AllUsed())
            {
                SpaceHintUI.Show("");
                StartCoroutine(RollWithAnimation());
            }
            else
            {
                // Paneel lahti
                UISoundPlayer.Instance.PlayOpen();

                ShowForCurrentState();
                UpdateOpenHint();
            }
        }
        // PANEL IS VISIBLE -> CLOSE IT
        else
        {
            // Paneel kinni
            UISoundPlayer.Instance.PlayClose();

            panel.Hide();
            UpdateClosedHint();
        }
    }

    bool AllUsed() => used[0] && used[1] && used[2];

    int RemainingCount()
    {
        int c = 0;
        for (int i = 0; i < 3; i++)
            if (!used[i]) c++;
        return c;
    }

    void RollThreeOptions()
    {
        for (int i = 0; i < 3; i++)
        {
            int a = Random.Range(1, 7);
            int b = Random.Range(1, 7);
            options[i] = new DicePair(a, b);
            used[i] = false;
        }
    }

    void ShowForCurrentState()
    {
        if (panel == null) return;

        int left = RemainingCount();
        if (!hasActiveOptions || left == 3)
            panel.SetHeader("Choose your first move");
        else if (left == 2)
            panel.SetHeader("Choose your next move (2 left)");
        else if (left == 1)
            panel.SetHeader("Choose your last move (1 left)");
        else
            panel.SetHeader("No moves left");

        panel.ShowOptions(options, used, OnOptionClicked);
    }

    // run gif, then show result pictures
    System.Collections.IEnumerator RollWithAnimation()
    {
        if (panel == null) yield break;

        isRollingVisual = true;

        // Dice roll heli
        UISoundPlayer.Instance.PlayDiceRoll();


        SpaceHintUI.Show("");
        panel.ShowRolling();

        yield return new WaitForSeconds(1.5f);

        RollThreeOptions();
        hasActiveOptions = true;

        ShowForCurrentState();

        isRollingVisual = false;
        UpdateOpenHint();
    }

    void OnOptionClicked(int index)
    {
        if (follower != null && follower.IsMoving) return;
        if (index < 0 || index > 2) return;
        if (used[index]) return;

        // Nupu klikk
        UISoundPlayer.Instance.PlayClick();

        used[index] = true;

        if (panel != null)
        {
            // Paneel kinni peale valikut
            UISoundPlayer.Instance.PlayClose();
            panel.Hide();
        }

        int steps = options[index].Sum;
        if (follower != null)
            follower.MoveSteps(steps);

        UpdateClosedHint();
    }

    // --- Hint helpers ---

    public void UpdateClosedHint()
    {
        if (!hasActiveOptions || AllUsed())
            SpaceHintUI.Show("Press SPACE to roll dice");
        else
            SpaceHintUI.Show("Press SPACE to open dice");
    }

    public void UpdateOpenHint()
    {
        SpaceHintUI.Show("Press SPACE to close dice");
    }

    public static void ResetDiceState()
    {
        // nulli kogu static dice state
        for (int i = 0; i < 3; i++)
        {
            options[i] = new DicePair(0, 0);
            used[i] = false;
        }

        hasActiveOptions = false;
        isRollingVisual = false;

        Debug.Log("[TurnController] ResetDiceState (static) done");
    }

    public void ResetRuntimeUI()
    {
        StopAllCoroutines();

        if (panel == null)
            panel = FindObjectOfType<DicePanelManager>(true);

        if (panel != null)
            panel.ResetDiceUI();

        UpdateClosedHint();
    }


}
