using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class rayWeapon : MonoBehaviour, WeaponInterface
{
    
    [SerializeField] private Transform barrel;

    [Header("Enemy interact")]
    [SerializeField] private bool isPlayerGun = true;
    [SerializeField] private float notifyRadius;
    [SerializeField] private bool showNotifyRadius = true;
    [SerializeField] private Color notifyColor = Color.green;

    [Header("Gun parameters")]
    [SerializeField] private float shootDelay;
    [SerializeField] private int maxAmmo;

    [SerializeField] gunType type;

    [Header("Audio")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip emptySound;

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

    /*private void OnDrawGizmos()
    {
        if (showNotifyRadius)
        {
            Handles.color = notifyColor;
            Handles.DrawWireArc(barrel.position, Vector3.up, Vector3.forward, 360, notifyRadius);
        }
    }*/
    public void Shoot(Vector3 targetPoint)
    {
        if (!canShoot) return;

        if(currentAmmo <= 0)
        {
            _audio.PlayOneShot(emptySound);
            return;
        }

        if (isPlayerGun) EmulateShootSound();

        _audio.PlayOneShot(shotSound);

        Vector3 barrelPos = barrel.position;
        targetPoint.y = barrelPos.y;

        RaycastHit hit;
        Ray shot = new Ray(barrelPos, (targetPoint - barrelPos).normalized);

        if (Physics.Raycast(shot, out hit, Mathf.Infinity))//LayerMask.GetMask("innerWall","outerWall","door", "player", "enemy")
        {
            GameObject shotTarget = hit.transform.gameObject;
            if (hit.transform.gameObject.GetComponent<Humanoid>() == null)
            {
                shotTarget = null;
            }
            Instantiate(rayTray, rayTray.transform.position,rayTray.transform.rotation).GetComponent<BulletInterface>().BulletShootCoroutine(barrel.position, hit.point, shotTarget);
        }

        canShoot = false;
        StartCoroutine(ShootDelayCorutine());

        currentAmmo--;
    }

    private void EmulateShootSound()
    {
        Collider[] col = Physics.OverlapSphere(barrel.position, notifyRadius,LayerMask.GetMask("enemy"));
        for(int i = 0; i < col.Length; i++)
        {
            EnemyMainScript mainScript = col[i].GetComponent<EnemyMainScript>();

            if(mainScript != null)
            {
                mainScript.ReactOnShot(barrel.position);
            }
        }
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

    public gunType GetGunType(){
        return type;
    }
}
