using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

enum botState{
    patrol,
    chase,
    lookAround
}

public class EnemyMainScript : MonoBehaviour, Humanoid
{   
    [Header("Main parametrs")]
    [SerializeField] public EnemyManager curManager;
    [SerializeField] private float _HP;
    [SerializeField] GameObject hand;

    [SerializeField] float shootDelay = 1f;
    [SerializeField ]private bool canShoot;
    [SerializeField]private botState state;
    EnemyMovement eMovement;

    [Header("Visibility")]
    [SerializeField]public float viewDistance = 6f;
    [SerializeField] float closeRange = 1f;
    [Range(0, 360)]
    [SerializeField] float viewAngle = 90f;
    public float shootingRange = 4f;
    [SerializeField] float lookingAroundTime;
    public int startToWatch;
    [SerializeField] bool isLookingAround = false;
    [SerializeField] bool drawOnGizmos = false;
    private WeaponInterface gun;
    private Animator animator;

    //[Header("Target")]
    private Transform target;

    private Color viewColor = Color.yellow;

    public int  DebugCount = 0;

    private void Awake()
    {
        target = curManager.trarget;

        canShoot = true;

        animator = GetComponent<Animator>();
        eMovement = GetComponent<EnemyMovement>();
        eMovement.target = target;
        eMovement.anim = animator;
        eMovement.shootingRange = shootingRange;
        
        animator.SetBool("isWalking", true);
        StartCoroutine("LookingCoroutine");
    }

    IEnumerator LookingCoroutine()
    {
        state = botState.patrol;
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
        state = botState.chase;
        eMovement.changeState(botMoveState.onTarget);
        while (SeeTarget())
        {
            if(canShoot){
                if(Shoot()){
                    StartCoroutine("ShootDelay");
                }
            }
            
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine("LookAround");
        yield break;
    }

    IEnumerator ShootDelay(){
        canShoot = false;
        yield return new WaitForSecondsRealtime(shootDelay);
        canShoot = true;
        yield break;
    }

    IEnumerator LookAround()
    {
        state = botState.lookAround;
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

    private bool Shoot(){
        return gun.Shoot(target.position);
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
        if (drawOnGizmos)
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
        }
    }

    private void Death()
    {
        gun.DropGun(1f);
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
        curManager.ClearIndexFixedCounts(startToWatch);
        StopAllCoroutines();
    }

    //PUBLIC

    public void ReactOnShot(Vector3 point)
    {
        canShoot = true;
        StopAllCoroutines();
        StartCoroutine("LookAround");
        eMovement.checkPoint(point);
        
    }

    public void GetGun(GameObject newgun){
        gun = newgun.GetComponent<rayWeapon>();

        switch(gun.GetGunType()){
            case gunType.oneHanded:
                animator.SetBool("nowOneHanded", true);
                break;

            case gunType.twoHanded:
                animator.SetBool("nowTwoHanded", true);
                break;
        }

        Transform newGunTrans = newgun.transform;
        newGunTrans.parent = hand.transform;
        newGunTrans.localPosition = Vector3.zero;
        newGunTrans.localRotation = Quaternion.Euler(180, 90, 90);
    }
}
