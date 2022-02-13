using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOnGismo : MonoBehaviour
{
    [SerializeField] Color32 color = Color.red*255;
    [SerializeField] float size = 0.1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }
}
