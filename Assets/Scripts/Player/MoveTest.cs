using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public WaypointFollower follower;

    void Update()
    {
        if (follower == null) return;

        // Space = random 1..12 steps (dice feel)
        //if (Input.GetKeyDown(KeyCode.Space) && !follower.IsMoving)
        //    follower.MoveSteps(Random.Range(1, 13));

        // Or arrow keys for manual stepping while testing
        if (Input.GetKeyDown(KeyCode.RightArrow) && !follower.IsMoving)
            follower.MoveSteps(1);
    }
}
