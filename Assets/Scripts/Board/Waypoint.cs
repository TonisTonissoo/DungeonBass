using Mono.Cecil.Cil;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint Next;
    public Waypoint GetNext() => Next;

    void OnDrawGizmos()
    {
        if (Next == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Next.transform.position);
    }

}
