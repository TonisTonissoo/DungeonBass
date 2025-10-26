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

            // advance
            current = (current.GetNext() != null) ? current.GetNext() : start;

            // if we wrapped from last to start, count a loop
            if (current == start && previous != start)
            {
                loops++;
                UpdateLoopText();
            }

            // walk to next waypoint
            Vector3 target = current.transform.position;
            while ((transform.position - target).sqrMagnitude > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = target;
        }

        if (current != null)
        {
            current.TriggerTileEvent();
        }

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

}
