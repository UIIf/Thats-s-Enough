using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float atackDist;
    [SerializeField] Vector3 forward;
    [SerializeField] float rayDist = 6;
    [SerializeField] float angle = 30;
    [SerializeField] int count = 20;

    float sqrAtackDist;

    Rigidbody rb;
    NavMeshAgent agent;

    bool onTarget = false;
    Vector3 targetPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        sqrAtackDist = atackDist * atackDist; //Now it sqrMaxVelosity
        agent.SetDestination(new Vector3(25, 0, 20));
        //NavMeshPath smth;
        //agent.CalculatePath(new Vector3(0, 0, 20), smth);
    }

    

    private void Update()
    {
        if (onTarget)
        {

        }
    }
}
