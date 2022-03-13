using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPlayerScript :MonoBehaviour, Humanoid
{
    [SerializeField] private float _HP;
    [SerializeField] private GameObject hand;
    [SerializeField] float dropGunForce;

    private CamScript camScript;
    private Animator animator;

    WeaponInterface[] holded_guns = { null, null };

    void Awake()
    {
        camScript = GetComponent<CamScript>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (holded_guns[0] != null)
            {
                holded_guns[0].Shoot(camScript.targetPoint);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (holded_guns[0] != null)
            {
                animator.SetBool("nowOneHanded", false);
                animator.SetBool("nowTwoHanded", false);
                holded_guns[0].DropGun(dropGunForce);
                holded_guns[0] = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }

    private void Death()
    {
        print("You're dead");
    }

    public void GetDamage(float dmg)
    {
        _HP -= dmg;
        if (_HP <= 0)
        {
            Death();
        }
    }

    public float GetHP()
    {
        return _HP;
    }

    private void PlaceGun(GameObject gun)
    {
        holded_guns[0] = gun.GetComponent<rayWeapon>();

        switch(holded_guns[0].GetGunType()){
            case gunType.oneHanded:
                animator.SetBool("nowOneHanded", true);
                break;

            case gunType.twoHanded:
                animator.SetBool("nowTwoHanded", true);
                break;
        }

        Transform newGunTrans = gun.transform;
        newGunTrans.parent = hand.transform;
        newGunTrans.localPosition = Vector3.zero;
        newGunTrans.localRotation = Quaternion.Euler(180, 90, 90);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Weapon" || other.gameObject.GetComponent<dropWeaponScript>()) return;
        if(holded_guns[0] != null) return;
        GameObject newGun = other.gameObject;
        if (newGun.GetComponent<Rigidbody>())
        {
            Destroy(newGun.GetComponent<Rigidbody>());
            Destroy(newGun.GetComponent<dropWeaponScript>());
        }
        BoxCollider[] boxcol = newGun.GetComponents<BoxCollider>();
        for (int i = 0; i < boxcol.Length; i++)
        {
            boxcol[i].enabled = false;
        }

        
        PlaceGun(newGun);
        
    }
}
