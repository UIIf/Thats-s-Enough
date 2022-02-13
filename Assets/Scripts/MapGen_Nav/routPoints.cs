using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class routPoints : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] Color routeColor;
    [SerializeField] float size = 0.1f;
    Vector3[] pointsVec;

    private void Awake()
    {
        pointsVec = new Vector3[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            pointsVec[i] = points[i].position;
        }
    }

    private void OnValidate()
    {
        pointsVec = new Vector3[points.Length];

        for(int i = 0; i < points.Length; i++)
        {
            pointsVec[i] = points[i].position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = routeColor;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i].position, size);
        }

        if (points.Length < 1)
            return;

        Gizmos.color = Color.black;

        for (int i = 0; i < points.Length - 1; i++)
        {
            // Gizmos.DrawSphere(points[i].position, size);
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }

        if (points.Length > 1)
        {
            //Gizmos.DrawSphere(points[points.Length - 1].position, size);
            Gizmos.DrawLine(points[0].position, points[points.Length - 1].position);
        }
    }

    public Vector3[] GetPoints()
    {
        
        return pointsVec;
    }
}
