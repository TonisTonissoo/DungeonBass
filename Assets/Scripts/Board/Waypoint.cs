using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint Next;
    public TileEvent tileEvent;

    public Waypoint GetNext() => Next;

    public void TriggerTileEvent()
    {
        if (tileEvent != null)
            tileEvent.TriggerEvent();
        else
            Debug.Log("No event assigned to this tile.");
    }

    void OnDrawGizmos()
    {
        if (Next == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Next.transform.position);
    }
}
