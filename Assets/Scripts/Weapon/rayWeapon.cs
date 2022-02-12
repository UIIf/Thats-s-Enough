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

    //[Header("Tray")]
    //[SerializeField] private GameObject rayTray;
    //[Tooltip("How much units trail travel at 1 second")]
    //[SerializeField] private float rayTraySpeed;
    [SerializeField] private GameObject rayTray;
    BulletInterface rayTrayInterface;

    AudioSource _audio;

    private int currentAmmo;

    private bool canShoot;
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        rayTrayInterface = rayTray.GetComponent<BulletInterface>();
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
            //StartCoroutine(DrawRay(barrel.transform.position, hit.point));
            Instantiate(rayTray, rayTray.transform).GetComponent<BulletInterface>().BulletShootCoroutine(barrel.transform.position, hit.point);
            
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
}
