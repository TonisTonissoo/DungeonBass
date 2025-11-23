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

    void Start()
    {
        bool returnFromBoss = PlayerPrefs.GetInt("ReturnAfterBoss", 0) == 1;

        if (returnFromBoss)
        {
            current = start;
            transform.position = start.transform.position;

            IsMoving = false;

            PlayerPrefs.SetInt("ReturnAfterBoss", 0);
            PlayerPrefs.Save();

            Debug.Log("[Follower] Returned from Boss -> Forced to START waypoint.");
        }
        else
        {
            current = start;
            transform.position = start.transform.position;
        }

        // Always sync loop from PlayerStats
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

            if (currIsStart)
            {
                // Loop increase on reaching START tile
                PlayerStats.Instance.currentLoop++;

                // Update loop for scaling system
                if (GameLoopManager.Instance != null)
                    GameLoopManager.Instance.SetLoop(PlayerStats.Instance.currentLoop);

                // Update UI
                HUDController.Instance?.UpdateHUD();
                UpdateLoopText();

                Debug.Log("[Loop] Finished loop at START tile → new loop = " + PlayerStats.Instance.currentLoop);

                break; // stop movement exactly on START tile
            }


        }

        // Trigger event of the tile where the movement ended
        if (current != null && gameObject.scene.isLoaded)
            current.TriggerTileEvent();

        IsMoving = false;
    }

    public void StopMovementImmediately()
    {
        StopAllCoroutines();
        IsMoving = false;
    }


    private void UpdateLoopText()
    {
        if (loopText != null)
            loopText.text = $"Loop: {PlayerStats.Instance.currentLoop}/20";
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
