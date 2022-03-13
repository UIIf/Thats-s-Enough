using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum gunType{
    oneHanded,
    twoHanded
}
public interface WeaponInterface
{
    public void Shoot(Vector3 targetPoint);
    public void DropGun();

    public int GetMaxAmmo();
    public int GetAmmo();

    public gunType GetGunType();
}

public interface BulletInterface
{
    public void BulletShootCoroutine(Vector3 start, Vector3 end, GameObject target = null);
}

public interface Humanoid
{
    public void GetDamage(float dmg);

    public float GetHP();

}