using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponInterface
{
    public void Shoot(Vector3 targetPoint);
    public void DropGun();

    public int GetMaxAmmo();
    public int GetAmmo();
}

public interface Humanoid
{
    public void GetDamage(float dmg);

    public float GetHP();

}