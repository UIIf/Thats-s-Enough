using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeedFix : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool linking;
    public float origSpeed;
    public Vector3 Size = Vector3.zero;

    // just change linkspeed to alter off mesh link traverse speed;
    public float linkSpeed;

    void Start()
    {
        NavMeshLink link = GetComponent<NavMeshLink>();
    }

    void FixedUpdate()
    {
        Collider[] col = Physics.OverlapBox(transform.position, Size, transform.rotation,LayerMask.GetMask("enemy"));
        if (col.Length > 0)
        {
            for(int i = 0; i < col.Length; i++)
            {
                NavMeshAgent agent = col[i].gameObject.transform.parent.gameObject.GetComponent<NavMeshAgent>();
                agent.speed = agent.speed;

                //print(1);
                //print(1);
            }
        }
    }
}
