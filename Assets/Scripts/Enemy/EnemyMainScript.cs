using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyMainScript : MonoBehaviour, Humanoid
{
    //Debug
    [Header("debug")]
    [SerializeField] string showCoroutine;
    //
   
    [Header("Main parametrs")]
    [SerializeField] EnemyManager curManager;
    [SerializeField] private float _HP;
    [SerializeField] GameObject deathParticl;
    EnemyMovement eMovement;

    [Header("Visibility")]
    [SerializeField]public float viewDistance = 6f;
    [SerializeField] float closeRange = 1f;
    [Range(0, 360)]
    [SerializeField] float viewAngle = 90f;
    [SerializeField] float shootingRange = 4f;
    [SerializeField] float lookingAroundTime;
    public uint startToWatch;
    [SerializeField] bool isLookingAround = false;


    //[Header("Target")]
    private Transform target;

    private Color viewColor = Color.yellow;

    private void Awake()
    {
        startToWatch = curManager.maxFixedUpdateCount + 1;
        target = curManager.trarget;
        //Debug 
        startToWatch = 20;
        //


        eMovement = GetComponent<EnemyMovement>();
        eMovement.target = target;
        eMovement.closeRange = closeRange;

        StartCoroutine("LookingCoroutine");
    }

    IEnumerator LookingCoroutine()
    {
        showCoroutine = "look";
        eMovement.changeState(botMoveState.patrol);
        while (startToWatch != curManager.fUCounter)
            yield return new WaitForFixedUpdate();

        //Start watch
        while (!SeeTarget())
        {
            viewColor = Color.red;
            for(int i = 0; i < curManager.maxFixedUpdateCount; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        
        StartCoroutine("RunForTarget");
    }

    IEnumerator RunForTarget()
    {
        showCoroutine = "Run";
        eMovement.changeState(botMoveState.onTarget);
        while (SeeTarget())
        {
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine("LookAround");
        yield break;
    }

    IEnumerator LookAround()
    {
        showCoroutine = "Wait";
        eMovement.changeState(botMoveState.wait);
        isLookingAround = true;
        lookingAroundTime = curManager.lookArT;
        

        while (isLookingAround && !SeeTarget())
        {
            if (eMovement.startWaiting)
            {
                StartCoroutine("LookAroundTimer");
            }
            yield return new WaitForFixedUpdate();
        }


        if (isLookingAround)
        {
            StopCoroutine("LookAroundTimer");
            isLookingAround = false;
            StartCoroutine("RunForTarget");
        }
        else
        {
            StartCoroutine("LookingCoroutine");
        }

        yield break;
    }

    IEnumerator LookAroundTimer()
    {
        yield return new WaitForSecondsRealtime(lookingAroundTime);
        isLookingAround = false;
    }

    private bool SeeTarget()
    {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget > viewDistance)
        {
            viewColor = Color.yellow;
            return false;
        }

        float realAngle = Vector3.Angle(transform.forward, target.position - transform.position);
        if (realAngle > viewAngle/2 && distanceToTarget > closeRange)
        {
            viewColor = Color.green;
            return false;
        }

        RaycastHit hit;
        Ray eyeRay = new Ray(transform.position, (target.position - transform.position).normalized);

        if(!Physics.Raycast(eyeRay, out hit, Mathf.Infinity))
        {
            viewColor = Color.black;
            return false;
        }
        viewColor = Color.blue;

        return hit.transform == target;
    }

    /*private void OnDrawGizmos()
    {
        Handles.color = viewColor;
        Vector3 center = transform.position;

        Handles.DrawWireArc(center, transform.up, -transform.forward, (360 - viewAngle) / 2, closeRange);
        Handles.DrawWireArc(center, transform.up, -transform.forward, (viewAngle - 360) / 2, closeRange);

        Vector3 leftFirst = center + Quaternion.Euler(new Vector3(0, -viewAngle / 2f, 0)) * (transform.forward * closeRange);
        Vector3 leftSecond = center + Quaternion.Euler(new Vector3(0, -viewAngle / 2f, 0)) * (transform.forward * viewDistance);

        Vector3 rightFirst = center + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (transform.forward * closeRange);
        Vector3 rightSecond = center + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (transform.forward * viewDistance);

        Handles.DrawLine(leftFirst, leftSecond);
        Handles.DrawLine(rightFirst, rightSecond);

        Handles.DrawWireArc(center, transform.up, transform.forward, viewAngle / 2f, viewDistance);
        Handles.DrawWireArc(center, transform.up, transform.forward, -viewAngle / 2f, viewDistance);

        Handles.color = Color.white;
        Handles.DrawWireArc(center, transform.up, transform.forward, 360, shootingRange);
    }*/

    private void Death()
    {
        Instantiate(deathParticl,transform.position,deathParticl.transform.rotation);
        Destroy(gameObject);
    }

    public void GetDamage(float dmg)
    {
        _HP -= dmg;
        print("Ouch");
        if(_HP <= 0)
        {
            Death();
        }
    }

    public float GetHP()
    {
        return _HP;
    }

    ~EnemyMainScript()
    {
        StopAllCoroutines();
    }

    //PUBLIC

    public void ReactOnShot(Vector3 point)
    {
        StopAllCoroutines();
        StartCoroutine("LookAround");
        eMovement.checkPoint(point);
    }
}
