using UnityEngine;
using System.Collections.Generic;

public class WaypointVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.positionCount = 0;

        UpdateLine();
    }

    void UpdateLine()
    {
        List<Transform> waypoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("WaypointNode"))
            {
                waypoints.Add(child);
            }
        }

        if (waypoints.Count > 0)
        {
            lineRenderer.positionCount = waypoints.Count;

            for (int i = 0; i < waypoints.Count; i++)
            {
                lineRenderer.SetPosition(i, waypoints[i].position);
            }
        }
        else
        {
            Debug.LogError("Nenhum waypoint com a tag 'WaypointNode' foi encontrado.");
        }
    }
}