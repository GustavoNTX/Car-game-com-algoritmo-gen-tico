using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Header("This is the waypoint we are going towards, not yet reached")]
    public float minDistanceToReachWaypoint = 5;

    public WaypointNode[] nextWaypointNode;
}
