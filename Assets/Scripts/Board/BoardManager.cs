using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public WaypointFollower follower;
    public Waypoint[] allWaypoints;

    void Start()
    {
        string result = PlayerPrefs.GetString("BattleResult", "None");
        int lastIndex = PlayerPrefs.GetInt("LastTileIndex", 0);

        if (lastIndex < 0 || lastIndex >= allWaypoints.Length)
        {
            lastIndex = 0;
        }

        if (result == "Win")
        {
            // J�tka sealt, kus pooleli j�i
            follower.start = allWaypoints[lastIndex];
            follower.transform.position = allWaypoints[lastIndex].transform.position;

            follower.SetCurrentWaypoint(allWaypoints[lastIndex]);

            Debug.Log($"Continuing from tile {lastIndex}");
        }
        else if (result == "Lose")
        {
            // Alusta uuesti algusest
            follower.start = allWaypoints[0];
            follower.transform.position = allWaypoints[0].transform.position;

            follower.SetCurrentWaypoint(allWaypoints[0]); // uuenda ka current
            Debug.Log("Restarting from Start tile");
        }
        else
        {
            // Uus m�ng � kustuta vana m�lu
            PlayerPrefs.DeleteKey("LastTileIndex");
            PlayerPrefs.DeleteKey("BattleResult");
            PlayerPrefs.Save();

            follower.start = allWaypoints[0];
            follower.transform.position = allWaypoints[0].transform.position;
            follower.SetCurrentWaypoint(allWaypoints[0]);
            Debug.Log("Fresh game start � cleared old memory");
        }

        PlayerPrefs.Save();
    }
}
