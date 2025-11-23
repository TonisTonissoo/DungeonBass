using System.Collections;
using UnityEngine;
using TMPro;

public class WaypointFollower : MonoBehaviour
{
    public Waypoint start;             // drag your START waypoint here
    public float moveSpeed = 4f;
    public bool IsMoving { get; private set; }

    [Header("Loop Counter UI (optional)")]
    public TextMeshProUGUI loopText;   // drag your UI text here

    private Waypoint current;
    private int loops = 1;

    void Start()
    {
        current = start;
        if (current != null)
            transform.position = current.transform.position;
        else
            Debug.LogError("WaypointFollower: assign a start waypoint.");

        UpdateLoopText();
    }

    // Call this with how many tiles to move (e.g., a dice roll)
    public void MoveSteps(int steps)
    {
        if (IsMoving || steps <= 0) return;
        StartCoroutine(MoveStepsCo(steps));
    }

    private IEnumerator MoveStepsCo(int steps)
    {
        IsMoving = true;

        while (steps-- > 0)
        {
            Waypoint previous = current;

            // Get next waypoint (wrap to start if needed)
            Waypoint nextWp = current.GetNext();
            if (nextWp == null)
                nextWp = start;

            // MOVE to next tile
            current = nextWp;

            // arrival movement
            Vector3 target = current.transform.position;
            while ((transform.position - target).sqrMagnitude > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = target;

            // CHECK if this tile IS the START tile
            bool currIsStart = current != null &&
                               current.tileEvent != null &&
                               current.tileEvent.tileType == TileType.Start;

            // If we arrived to START → STOP movement early
            if (currIsStart)
            {
                loops++;

                if (PlayerStats.Instance != null)
                    PlayerStats.Instance.currentLoop = loops;

                HUDController.Instance?.UpdateHUD();
                UpdateLoopText();

                Debug.Log("[Loop] Finished loop at START tile → stopping.");
                break;
            }
        }

        // Trigger event of the tile where the movement ended
        if (current != null)
            current.TriggerTileEvent();

        IsMoving = false;
    }









    private void UpdateLoopText()
    {
        if (loopText != null && loops <= 20)
            loopText.text = $"Loop: {loops}/20";
    }

    public void SetCurrentWaypoint(Waypoint wp)
    {
        current = wp;
    }

    public void TeleportTo(Waypoint waypoint)
    {
        current = waypoint;
        transform.position = waypoint.transform.position;
        IsMoving = false;
    }


}
