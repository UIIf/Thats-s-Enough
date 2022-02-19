using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyMainScript : MonoBehaviour, Humanoid
{
    [Header("Main parametrs")]
    [SerializeField] private float _HP;
    [SerializeField] GameObject deathParticl;

    [Header("Visibility")]
    [SerializeField]public float viewDistance = 6f;
    [SerializeField] float closeRange = 1f;
    [Range(0, 360)]
    [SerializeField] float viewAngle = 90f;
    [Tooltip("In seconds")]
    [SerializeField] float updatePeriod = 0.3f;

    [Header("Target")]
    [SerializeField] private Transform target;

    private Color viewColor = Color.yellow;

    private void Awake()
    {
        StartCoroutine("LookingCoroutine");
    }

    private void FixedUpdate()
    {
        
    }

    IEnumerator LookingCoroutine()
    {
        while (true)
        {
            if (SeeTarget())
            {
                viewColor = Color.red;
            }

            yield return new WaitForSecondsRealtime(updatePeriod);
        }
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

    private void OnDrawGizmos()
    {
        Handles.color = viewColor;
        Vector3 center = transform.position;

        Handles.DrawWireArc(center, transform.up, -transform.forward, (360 - viewAngle) /2,closeRange);
        Handles.DrawWireArc(center, transform.up, -transform.forward, (viewAngle - 360) / 2, closeRange);

        Vector3 leftFirst = center + Quaternion.Euler(new Vector3(0, -viewAngle / 2f, 0)) * (transform.forward * closeRange);
        Vector3 leftSecond = center + Quaternion.Euler(new Vector3(0, -viewAngle / 2f, 0)) * (transform.forward * viewDistance);

        Vector3 rightFirst = center + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (transform.forward * closeRange);
        Vector3 rightSecond = center + Quaternion.Euler(new Vector3(0, viewAngle / 2f, 0)) * (transform.forward * viewDistance);

        Handles.DrawLine(leftFirst, leftSecond);
        Handles.DrawLine(rightFirst, rightSecond);

        Handles.DrawWireArc(center, transform.up, transform.forward, viewAngle / 2f, viewDistance);
        Handles.DrawWireArc(center, transform.up, transform.forward, -viewAngle / 2f, viewDistance);
    }

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
}
