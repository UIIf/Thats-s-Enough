using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayWeapon : MonoBehaviour, WeaponInterface
{
    
    [SerializeField] private GameObject barrel;

    [Header("Gun params")]
    [SerializeField] private float shootDelay;
    [SerializeField] private int maxAmmo;
    [SerializeField] private float damage;

    [Header("Audio")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip emptySound;

    [Header("Tray")]
    [SerializeField] private GameObject rayTray;
    [Tooltip("How much units trail travel at 1 second")]
    [SerializeField] private float rayTraySpeed;

    AudioSource _audio;

    private int currentAmmo;

    private bool canShoot;
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        currentAmmo = maxAmmo;
        canShoot = true;
    }

    public void Shoot(Vector3 targetPoint)
    {
        if (!canShoot) return;

        if(currentAmmo <= 0)
        {
            _audio.PlayOneShot(emptySound);
            return;
        }

        _audio.PlayOneShot(shotSound);

        Vector3 barrelPos = barrel.transform.position;
        targetPoint.y = barrelPos.y;

        RaycastHit hit;
        Ray shot = new Ray(barrelPos, (targetPoint - barrelPos).normalized);

        if (Physics.Raycast(shot, out hit, Mathf.Infinity))//LayerMask.GetMask("innerWall","outerWall","door", "player", "enemy")
        {
            if (hit.transform.gameObject.GetComponent<Humanoid>() != null)
            {
                hit.transform.gameObject.GetComponent<Humanoid>().GetDamage(damage);
            }
            StartCoroutine(DrawRay(barrel.transform.position, hit.point));
        }

        canShoot = false;
        StartCoroutine(ShootDelayCorutine());

        currentAmmo--;
    }

    public void DropGun()
    {
        transform.parent = null;
        gameObject.AddComponent<dropWeaponScript>();
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
    public int GetAmmo()
    {
        return currentAmmo;
    }

    private IEnumerator ShootDelayCorutine()
    {
        yield return new WaitForSecondsRealtime(shootDelay);
        canShoot = true;
        yield break;
    }

    private IEnumerator DrawRay(Vector3 start, Vector3 end)
    {
        end.y = start.y;

        float x = end.x - start.x;
        float z = end.z - start.z;

        //float sin = x / Mathf.Sqrt(x * x + z * z);
        float Angle = Mathf.Asin(x / Mathf.Sqrt(x * x + z * z)) * Mathf.Rad2Deg;
        if (z < 0)
        {
            Angle = 180 - Angle;
        }
        //Debug.Log(Angle);
        
        Vector3 prevRot = rayTray.transform.eulerAngles;
        prevRot.y = Angle - 180;
        Transform curTr = Instantiate(rayTray, start, Quaternion.Euler(prevRot)).transform;

        float dist = Vector3.Distance(start,end);

        if(dist < 0.1f)
        {
            yield break;
        }

        float trayScalePersent = 1 / dist;
        Vector3 newScale = Vector3.one;

        if(dist * 0.3f < 1)
        {
            trayScalePersent = 0.3f;
        }

        //newScale.y = trayScalePersent * dist;

        float dt = (1 - trayScalePersent)/dist*rayTraySpeed * Time.fixedDeltaTime;
        Debug.Log(dt);


        for(; trayScalePersent <= 1; trayScalePersent += dt)
        {
            newScale.y = trayScalePersent * dist;
            curTr.localScale = newScale;
            yield return new WaitForFixedUpdate();
        }

        newScale.y = dist;
        curTr.localScale = newScale;
        yield return new WaitForFixedUpdate();

        Destroy(curTr.gameObject);

        yield break;
    }
   
}
