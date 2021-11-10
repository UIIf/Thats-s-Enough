using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunScript : MonoBehaviour
{
    [SerializeField] private int maxAmmoCount;
    [SerializeField] private float shootInterval;
    [SerializeField] private GameObject bullet;
    [SerializeField] private int range;

    GameObject barrel;

    private int ammoCount;
    private float shootTimer;
    void Awake()
    {
        shootTimer = 0;
        ammoCount = maxAmmoCount;
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            if(gameObject.transform.GetChild(i).name == "Barrel")
            {
                barrel = gameObject.transform.GetChild(i).gameObject;
            }
        }
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer < 0)
            shootTimer = 0;
    }

    public void Shoot(Vector3 targetPoint)
    {
        if(shootTimer <= 0 && ammoCount > 0)
        {
            targetPoint.y = barrel.transform.position.y;
            Vector3 target = Quaternion.LookRotation(targetPoint - barrel.transform.position).eulerAngles
                                + Vector3.up * Random.Range(-range,range);

            target.x = 0;
            target.z = 0;

            Instantiate(bullet, barrel.transform.position, Quaternion.Euler(target));
            shootTimer = shootInterval;
            ammoCount--;
        }
        else
        {
            //Play sound of error
        }
    }

    public void DropGun()
    {
        transform.parent = null;
        gameObject.AddComponent<dropWeaponScript>();
    }

    public int GetMaxAmmo()
    {
        return maxAmmoCount;
    }

    public int GetAmmo()
    {
        return ammoCount;
    }

}