using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPointHolder : MonoBehaviour
{

    [SerializeField] GameObject pointParent = null;

    [SerializeField] Vector3[] points = null;

    void Awake()
    {
        Transform _transform = pointParent.transform;
        if(pointParent != null)
        {
            points = new Vector3[_transform.childCount];
            for(int i = 0; i < _transform.childCount; i++)
            {
                points[i] = _transform.GetChild(i).transform.position;
            }
        }
    }

    public Vector3[] GetPatrolPoints()
    {
        return points;
    }

}
