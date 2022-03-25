using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPlayerScript :MonoBehaviour, Humanoid
{
    [SerializeField] private float _HP;
    [SerializeField] private GameObject hand;
    [SerializeField] float dropGunForce;
    [SerializeField] GameObject HealthBar;
    [SerializeField] Text AmmoUI;
    [SerializeField] float fightDelay;
    [SerializeField] float regenerationMultiplier;

    float currentFightDelay = 0;
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
                currentFightDelay = fightDelay;
                holded_guns[0].Shoot(camScript.targetPoint);
                AmmoUI.text = holded_guns[0].GetAmmo().ToString();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (holded_guns[0] != null)
            {
                AmmoUI.text = "";
                animator.SetBool("nowOneHanded", false);
                animator.SetBool("nowTwoHanded", false);
                holded_guns[0].DropGun(dropGunForce);
                holded_guns[0] = null;
            }
        }

        if(currentFightDelay >= 0)
            currentFightDelay -= Time.deltaTime;

        if (currentFightDelay <= 0 && _HP < 1000) {
            _HP += regenerationMultiplier * Time.deltaTime;
            RefreshHealthBar();
        }

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }

    private void Death()
    {
        //print("You're dead");
    }

    public void GetDamage(float dmg)
    {
        currentFightDelay = fightDelay;
        _HP -= dmg;
        RefreshHealthBar();
        if (_HP <= 0)
        {
            Death();
        }
    }

    public float GetHP()
    {
        return _HP;
    }

    public void RefreshHealthBar()
    {
        HealthBar.transform.localScale = new Vector2((_HP / 1000) > 0 ? _HP / 1000 : 0, 1);
    }

    private void PlaceGun(GameObject gun)
    {
        holded_guns[0] = gun.GetComponent<rayWeapon>();
        AmmoUI.text = holded_guns[0].GetAmmo().ToString();

        switch (holded_guns[0].GetGunType()){
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
        if (newGun.GetComponent<WeaponInterface>().GetAmmo() <= 0) return;
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
