using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayWeapon : MonoBehaviour, WeaponInterface
{
    [SerializeField] private int maxAmmo;
    [SerializeField] private GameObject barrel;
    [SerializeField] private float shootDelay;
    [SerializeField] private float damage;

    private int currentAmmo;
    private float beforeNextShot;

    void Awake()
    {
        currentAmmo = maxAmmo;
        beforeNextShot = 0;
    }

    void Update()
    {
        if(beforeNextShot >= 0)
        {
            beforeNextShot -= Time.deltaTime;
        }
            
    }

    public void Shoot(Vector3 targetPoint)
    {
        if (beforeNextShot <= 0 && currentAmmo > 0)
        {
            Vector3 barrelPos = barrel.transform.position;
            targetPoint.y = barrelPos.y;


            RaycastHit hit;
            Ray shot = new Ray(barrelPos, (targetPoint - barrelPos).normalized);

            if (Physics.Raycast(shot, out hit,Mathf.Infinity))//LayerMask.GetMask("innerWall","outerWall","door", "player", "enemy")
            {
                if (hit.transform.gameObject.GetComponent<Humanoid>() != null)
                {
                    hit.transform.gameObject.GetComponent<Humanoid>().GetDamage(damage);
                }
            }


            beforeNextShot = shootDelay;
            currentAmmo--;
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
}
