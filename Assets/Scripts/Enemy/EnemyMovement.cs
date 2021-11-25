using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent agent;
    OffMeshLinkData curLink;

    Vector3[] curPatrolPoints = null;

    bool onTarget = false;

    //Speed
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    //Patroling
    [SerializeField] int chosenPatrolPoint;
    [SerializeField] float distFromPatrolPoint = 0.1f;
    float sqrDistFromPatrolPoint;

    //Link variables
    Vector3 linkEnd;
    Vector3 linkDirection;
    bool linkingBool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        sqrDistFromPatrolPoint = distFromPatrolPoint * distFromPatrolPoint;
        linkingBool = false;
        agent.speed = walkSpeed;
    }

    void OffMeshLinkTraveler()
    {
        if(agent.isOnOffMeshLink && !linkingBool)
        {
            //prevDist = Mathf.Infinity;
            rb.isKinematic = false;
            
            curLink = agent.currentOffMeshLinkData;

            Vector3 linkDirection = curLink.endPos - curLink.startPos;

            linkEnd = curLink.endPos;

            linkDirection = new Vector3(    0,
                                            ((Mathf.Abs(linkDirection.z) > 0.01) ? Mathf.Sign(linkDirection.z) * 90 : 0 ) +
                                                ((Mathf.Abs(linkDirection.x) > 0.01) ? Mathf.Sign(linkDirection.x) * 180 : 0) - 90,
                                            0);

            linkingBool = true;
            
        }

        if (linkingBool)
        {
            rb.velocity = (linkEnd - transform.position).normalized * agent.speed;
            transform.rotation = Quaternion.Euler( Vector3.Lerp(transform.rotation.eulerAngles * Mathf.Rad2Deg, linkDirection, 0.05f) * Mathf.Deg2Rad);

            //For april fools
            //transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles * Mathf.Rad2Deg, linkDirection, 0.2f));

            float curDist = (transform.position - linkEnd).sqrMagnitude;
            print(curDist);
            if (curDist < 0.01)
            {
                linkingBool = false;
                rb.isKinematic = true;
                agent.CompleteOffMeshLink();
            }
        }
    }

    void GetPatrolPoints()
    {
        Collider[] col = Physics.OverlapBox(transform.position, new Vector3(0.01f, 10f, 0.01f), Quaternion.identity, LayerMask.GetMask("floor"));
        switch (col.Length)
        {
            case 0:
                Destroy(gameObject);
                break;
            default:
                curPatrolPoints = col[0].transform.parent.GetChild(4).GetComponent<PatrolPointHolder>().GetPatrolPoints();
                break;
        }
        chosenPatrolPoint = Random.Range(0,curPatrolPoints.Length);
        agent.SetDestination(curPatrolPoints[chosenPatrolPoint]);
    }

    void FixedUpdate()
    {
        if (onTarget)
        {
            rb.velocity = Vector3.zero;
            OffMeshLinkTraveler();
        }
        else
        {
            if(curPatrolPoints == null)
            {
                GetPatrolPoints();
            }
            else
            {
                if ((transform.position - curPatrolPoints[chosenPatrolPoint]).sqrMagnitude < sqrDistFromPatrolPoint)
                {
                    chosenPatrolPoint++;
                    chosenPatrolPoint %= curPatrolPoints.Length;
                    agent.SetDestination(curPatrolPoints[chosenPatrolPoint]);
                }
                else
                {
                    if(agent.isPathStale)
                        agent.SetDestination(curPatrolPoints[chosenPatrolPoint]);
                }
            }
        }
        
    }

}
