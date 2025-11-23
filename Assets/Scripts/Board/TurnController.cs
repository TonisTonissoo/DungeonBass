using UnityEngine;

public class TurnController : MonoBehaviour
{
    [System.Serializable]
    public struct DicePair
    {
        public int a, b;
        public int Sum => a + b;
        public DicePair(int a, int b) { this.a = a; this.b = b; }
    }

    public KeyCode rollKey = KeyCode.Space;
    public WaypointFollower follower;       // your existing mover (must have MoveSteps(int) + IsMoving)
    public DicePanelManager panel;          // hook up in Inspector

    private DicePair[] options = new DicePair[3];
    private bool[] used = new bool[3];

    void Update()
    {
        // blokk telepordi ajal
        if (HorseCarriageUI.Instance != null && HorseCarriageUI.Instance.IsChoosingTile)
            return;

        // SPACE rollimine (ÜKS TÄRING)
        if (Input.GetKeyDown(KeyCode.Space) && !follower.IsMoving)
        {
            Debug.Log("[Dice] Single dice roll triggered.");
            int steps = Random.Range(1, 7);
            follower.MoveSteps(steps);
        }
    }



    bool AllUsed()
    {
        return used[0] && used[1] && used[2];
    }

    int RemainingCount()
    {
        int c = 0;
        for (int i = 0; i < 3; i++) if (!used[i]) c++;
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

    void ShowForFirstPick()
    {
        panel.SetHeader("Choose your first move");
        panel.ShowOptions(options, used, OnOptionClicked);
    }

    void ShowForNextPicks()
    {
        int left = RemainingCount();
        if (left == 2) panel.SetHeader("Choose your next move (2 left)");
        else if (left == 1) panel.SetHeader("Choose your last move (1 left)");
        panel.ShowOptions(options, used, OnOptionClicked);
    }

    void OnOptionClicked(int index)
    {
        if (follower.IsMoving) return;
        if (index < 0 || index > 2) return;
        if (used[index]) return;

        used[index] = true;
        panel.Hide();

        int steps = options[index].Sum;
        follower.MoveSteps(steps);
    }

    // Hook this to your small "Dice" button OnClick in the Inspector
    public void ToggleDicePanel()
    {
        if (follower.IsMoving) return;

        int left = RemainingCount();
        if (left == 0)
        {
            // Nothing left this turn; do nothing (next Space will roll new)
            return;
        }

        if (!panel.IsVisible)
            ShowForNextPicks();
        else
            panel.Hide();
    }
}
