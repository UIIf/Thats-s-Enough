using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum botMoveState
{
    patrol,
    onTarget,
    wait,
    stasis
}
public class EnemyMovement : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent agent;
    OffMeshLinkData curLink;

    public Animator anim;

    //FromMainScript
    public Transform target = null;
    public float shootingRange;

    Vector3[] curPatrolPoints = null;
    [SerializeField]  Vector3 destenationPoint;

    [Header("Speed")]
    [SerializeField] float walkSpeed;

    [Header("Patroling")]
    [SerializeField] int chosenPatrolPoint;
    [SerializeField] float distFromPatrolPoint = 0.1f;
    
    float sqrDistFromPatrolPoint;

    //Link variables
    Vector3 linkEnd;
    Vector3 linkDirection;
    bool linkingBool;
    
    [SerializeField] private botMoveState state = botMoveState.patrol;

    //Debug
    [Header("debug")]



    public bool startWaiting;

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
            rb.isKinematic = false;
            
            curLink = agent.currentOffMeshLinkData;

            linkEnd = curLink.endPos;
            linkEnd.y = transform.position.y;


            //Vector3 linkDirection = curLink.endPos - curLink.startPos;

            //linkDirection = new Vector3(    0,
            //                                ((Mathf.Abs(linkDirection.z) > 0.1f) ? Mathf.Sign(linkDirection.z) * 90 : 0 ) +
            //                                    ((Mathf.Abs(linkDirection.x) > 0.1f) ? Mathf.Sign(linkDirection.x) * 180 : 0) - 90,
            //                                0);

            linkingBool = true;
            transform.localEulerAngles = linkDirection;
        }

        if (linkingBool)
        {
            rb.MovePosition(transform.position + (linkEnd - transform.position) * walkSpeed * Time.deltaTime);
            //TODO: add rotation

            float curDist = (transform.position - linkEnd).sqrMagnitude;
            if (curDist < 0.06f)
            {
                linkingBool = false;
                rb.isKinematic = false;
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
                curPatrolPoints = col[0].transform.parent.GetChild(4).GetComponent<EnemyPointHolder>().GetPatrolPoints();
                break;
        }

        
    }

    //private void RotateToTarget() // ������������ � ������� ���� �� ��������� rotationSpeed
    //{
    //    Vector3 lookVector = Target.position - agentTransform.position;
    //    lookVector.y = 0;
    //    if (lookVector == Vector3.zero) return;
    //    agentTransform.rotation = Quaternion.RotateTowards
    //        (
    //            agentTransform.rotation,
    //            Quaternion.LookRotation(lookVector, Vector3.up),
    //            rotationSpeed * Time.deltaTime
    //        );

    //}

    public void changeState(botMoveState newState)
    {
        agent.isStopped = false;
        curPatrolPoints = null;
        startWaiting = false;
        state = newState;
        if(state == botMoveState.stasis){
            agent.isStopped = true;
        }
    }

    public void checkPoint(Vector3 point)
    {
        destenationPoint = point;
        destenationPoint.y = transform.position.y;
        agent.destination = destenationPoint;
    }
    void Patroling()
    {
        if ((transform.position - destenationPoint).sqrMagnitude < sqrDistFromPatrolPoint || curPatrolPoints == null)
        {
            NextDestenationPoint();
            agent.destination = destenationPoint;
            return;
        }
    }

    void MoveOnTarget()
    {
        transform.LookAt(target);
        if (Vector3.Distance(transform.position, target.position) < shootingRange)
        {
            anim.SetBool("isWalking", false);
            agent.isStopped = true;
            return;
        }
        else
        {
            anim.SetBool("isWalking", true);
            agent.isStopped = false;
        }

        destenationPoint = target.position + Vector3.up * (transform.position.y - target.position.y);
        agent.destination = destenationPoint;
    }

    void MoveOnWait()
    {
        if ((transform.position - destenationPoint).sqrMagnitude < sqrDistFromPatrolPoint)
        {
            startWaiting = true;
            return;
        }
    }

    void NextDestenationPoint()
    {
        if (curPatrolPoints == null)
        {
            GetPatrolPoints();
            chosenPatrolPoint = Random.Range(0, curPatrolPoints.Length);
            destenationPoint = curPatrolPoints[chosenPatrolPoint];
            return;
        }
        else
        {
            chosenPatrolPoint++;
            chosenPatrolPoint %= curPatrolPoints.Length;
            destenationPoint = curPatrolPoints[chosenPatrolPoint];
        }
        destenationPoint.y = transform.position.y;
    }

    void FixedUpdate()
    {
        if (agent.isOnOffMeshLink)
        {
            OffMeshLinkTraveler();
            return;
        }
        switch (state){
            case botMoveState.wait:
                MoveOnWait();
                break;
            case botMoveState.patrol:
                Patroling();
                break;
            case botMoveState.onTarget:
                MoveOnTarget();
                break;
        }
        

    }

}
