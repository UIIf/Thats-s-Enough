using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunTable : MonoBehaviour
{
    public UnityAction TrigerUsed;
    private void OnTriggerEnter(Collider other){
        if (other.tag != "Player") return;
        TrigerUsed.Invoke();
        Destroy(gameObject);
    }

    public void PlaceGun(GameObject gun){
        
        gun = Instantiate(gun,transform);
        gun.transform.parent = null;
        gun.GetComponent<WeaponInterface>().DropGun(0);
    }
}
