using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempPlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject hand;
    private CharacterController _controller;
    private Transform _transform;
    private Rigidbody rb;
    private CamScript camScript;

    gunScript HoldedGun = null;
    
    private Vector3 moveDir = Vector3.zero;
    


    //Чтобы не пересоздавать
    
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        camScript = GetComponent<CamScript>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if(HoldedGun != null)
            {
                HoldedGun.Shoot(camScript.targetPoint);
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (HoldedGun != null)
            {
                HoldedGun.DropGun();
                HoldedGun = null;
            }
        }

        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        _controller.Move(moveDir * speed * Time.deltaTime);
        //transform.LookAt(mousePose);
    }
    void FixedUpdate()
    {
        


        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            GameObject newGun = other.gameObject;
            Transform newGunTrans = newGun.transform;
            if (other.gameObject.GetComponent<Rigidbody>())
            {
                Destroy(newGun.GetComponent<Rigidbody>());
                Destroy(newGun.GetComponent<dropWeaponScript>());
            }
            BoxCollider[] boxcol = newGun.GetComponents<BoxCollider>();
            for (int i = 0; i < boxcol.Length; i++)
            {
                boxcol[i].enabled = false;
            }
            //other.gameObject.transform.rotation = Quaternion.Euler(Vector3.up * 90 +_transform.rotation.eulerAngles);
            newGunTrans.parent = hand.transform;
            newGunTrans.localPosition = Vector3.zero;
            newGunTrans.rotation = Quaternion.Euler(Quaternion.LookRotation(camScript.targetPoint - newGunTrans.position).eulerAngles + Vector3.up*90);
            HoldedGun = newGun.GetComponent<gunScript>();
        }
        
    }
}
