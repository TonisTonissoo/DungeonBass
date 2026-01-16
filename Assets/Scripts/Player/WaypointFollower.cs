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
        // 1) Boss fightist tagasi tulek
        bool returnFromBoss = PlayerPrefs.GetInt("ReturnAfterBoss", 0) == 1;

        if (returnFromBoss)
        {
            current = start;
            transform.position = start.transform.position;

            PlayerPrefs.SetInt("ReturnAfterBoss", 0);
            PlayerPrefs.Save();

            Debug.Log("[Follower] Returned from Boss → forced to START.");
            UpdateLoopText();
            return;
        }

        // 2) Tavalise COMBATi tagasitulek (kasutame LastTileIndex’i)
        if (PlayerPrefs.HasKey("LastTileIndex"))
        {
            int index = PlayerPrefs.GetInt("LastTileIndex");
            Waypoint[] all = FindObjectsOfType<Waypoint>();

            // Leia waypoint õige indexiga
            foreach (Waypoint w in all)
            {
                if (w.transform.GetSiblingIndex() == index)
                {
                    current = w;
                    transform.position = w.transform.position;

                    Debug.Log("[Follower] Returned from Normal Fight → restored tile: " + index);
                    UpdateLoopText();
                    return;
                }
            }
        }

        // 3) Mängu täiesti uus algus
        current = start;
        transform.position = start.transform.position;

        Debug.Log("[Follower] Fresh start.");
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

            Waypoint nextWp = current.GetNext();
            if (nextWp == null)
                nextWp = start;

            current = nextWp;

            Vector3 target = current.transform.position;
            while ((transform.position - target).sqrMagnitude > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = target;

            // ✅ heli siis kui jõudis tile'i peale
            UISoundPlayer.Instance?.PlayMove();

            bool currIsShop = current != null &&
                              current.tileEvent != null &&
                              current.tileEvent.tileType == TileType.Shop;
            if (currIsShop)
            {
                Debug.Log("[Shop] Stopping movement at shop tile (index " +
                          (current.transform.GetSiblingIndex() + 1) + ").");
                break;
            }

            bool currIsStart = current != null &&
                               current.tileEvent != null &&
                               current.tileEvent.tileType == TileType.Start;

            if (currIsStart)
            {
                PlayerStats.Instance.currentLoop++;

                if (GameLoopManager.Instance != null)
                    GameLoopManager.Instance.SetLoop(PlayerStats.Instance.currentLoop);

                HUDController.Instance?.UpdateHUD();
                UpdateLoopText();

                Debug.Log("[Loop] Finished loop at START tile → new loop = " + PlayerStats.Instance.currentLoop);

                break;
            }
        }

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
